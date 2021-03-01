using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace temsAPI.Contracts
{
    interface IGenericRepository<T> where T : class
    {
        // Interface which defines CRUD
        // Every domain class should offer CRUD operations

        Task<IList<T>> FindAll(
            Expression<Func<T, bool>> expression = null, // q => q.Id == 2
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, // q => q.OrderBy(q => q.Id)
            List<string> includes = null
            );

        Task<T> Find(
           Expression<Func<T, bool>> expression,
           List<string> includes = null
           );

        Task<bool> isExists(Expression<Func<T, bool>> expression = null);
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
