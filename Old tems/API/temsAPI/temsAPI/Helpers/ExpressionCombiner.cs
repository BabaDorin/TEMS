using System;
using System.Linq;
using System.Linq.Expressions;

namespace temsAPI.Helpers
{
    public static class ExpressionCombiner
    {
        private enum CombineOperation
        {
            And, Or
        };

        public static Expression<Func<T, bool>> ConcatAnd<T>(
            this Expression<Func<T, bool>> baseExpression,
            Expression<Func<T, bool>> otherExpression)
        {
            return And(baseExpression, otherExpression);
        }

        public static Expression<Func<T, bool>> ConcatOr<T>(
            this Expression<Func<T, bool>> baseExpression,
            Expression<Func<T, bool>> otherExpression)
        {
            return Or(baseExpression, otherExpression);
        }

        public static Expression<Func<T, TType>> And<T, TType>(params Expression<Func<T, TType>>[] expressions)
        {
            Expression<Func<T, TType>> finalExpression = expressions[0];
            
            if (expressions.Count() == 1)
                return expressions[0];

            for (int i = 1; i < expressions.Count(); i++)
                finalExpression = CombineTwo(finalExpression, expressions[i], CombineOperation.And);

            return finalExpression;
        }

        public static Expression<Func<T, TType>> Or<T, TType>(params Expression<Func<T, TType>>[] expressions)
        {
            Expression<Func<T, TType>> finalExpression = expressions[0];

            if (expressions.Count() == 1)
                return expressions[0];

            for (int i = 1; i < expressions.Count(); i++)
                finalExpression = CombineTwo(finalExpression, expressions[i], CombineOperation.Or);

            return finalExpression;
        }

        private static Expression<Func<T, TType>> CombineTwo<T, TType>(this Expression<Func<T, TType>> exp, Expression<Func<T, TType>> newExp, CombineOperation operation)
        {
            if (exp == null && newExp == null)
                return null;

            if (exp == null)
                return newExp;

            if (newExp == null)
                return exp;
            
            var visitor = new ParameterUpdateVisitor(newExp.Parameters.First(), exp.Parameters.First());
            newExp = visitor.Visit(newExp) as Expression<Func<T, TType>>;

            var binExp = operation == CombineOperation.And ? Expression.And(exp.Body, newExp.Body) : Expression.Or(exp.Body, newExp.Body);
            return Expression.Lambda<Func<T, TType>>(binExp, newExp.Parameters);
        }

        class ParameterUpdateVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (object.ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }
    }
}
