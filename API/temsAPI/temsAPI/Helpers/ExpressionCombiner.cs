using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace temsAPI.Helpers
{
    public static class ExpressionCombiner
    {
        public static Expression<Func<T, bool>> And<T>(params Expression<Func<T, bool>>[] expressions)
        {
            Expression<Func<T, bool>> finalExpression = expressions[0];

            if (expressions.Count() == 1)
                return expressions[0];
            else
                for(int i = 1; i < expressions.Count(); i++)
                {
                    finalExpression = CombineTwo(finalExpression, expressions[i]);
                }

            return finalExpression;
        }

        private static Expression<Func<T, bool>> CombineTwo<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> newExp)
        {
            if (exp == null && newExp == null)
                return null;

            if (exp == null)
                return newExp;

            if (newExp == null)
                return exp;
            
            // get the visitor
            var visitor = new ParameterUpdateVisitor(newExp.Parameters.First(), exp.Parameters.First());
            // replace the parameter in the expression just created
            newExp = visitor.Visit(newExp) as Expression<Func<T, bool>>;

            // now you can and together the two expressions
            var binExp = Expression.And(exp.Body, newExp.Body);
            // and return a new lambda, that will do what you want. NOTE that the binExp has reference only to te newExp.Parameters[0] (there is only 1) parameter, and no other
            return Expression.Lambda<Func<T, bool>>(binExp, newExp.Parameters);
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
