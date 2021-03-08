using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace temsAPI.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        // Interface which defines CRUD
        // Every domain class should offer CRUD operations

        Task<IList<TType>> FindAll<TType>(
            Expression<Func<T, bool>> where = null,
            Expression<Func<T, TType>> select = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            List<string> includes = null
            );

        Task<IList<TType>> Find<TType>(
           Expression<Func<T, bool>> where = null,
           Expression<Func<T, TType>> select = null,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
           List<string> includes = null
           );

        Task<bool> isExists(Expression<Func<T, bool>> expression = null);
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
