using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data;

namespace temsAPI.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task Create(T entity)
        {
            await _db.AddAsync(entity);
        }

        public void DeleteRange(List<T> range)
        {
            _db.RemoveRange(range);
        }

        public void Delete(T entity)
        {
            _db.Remove(entity);
        }

        public async Task<IList<TType>> Find<TType>(
            Expression<Func<T, bool>> where = null,
            Expression<Func<T, TType>> select = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            List<string> includes = null)
        {
            IQueryable<T> query = _db;

            if (where != null)
            {
                query = query.Where(where);
            }

            if (includes != null)
            {
                foreach (var table in includes)
                {
                    query = query.Include(table);
                }
            }

            if (include != null)
            {
                query = include(query);
            }

            //if (select != null)
            //{
            //    query = (IQueryable<T>)query.Select(select);
            //}

            return
                (select != null)
                ? await query.Select(select).ToListAsync()
                : (IList<TType>)await query.ToListAsync();
        }

        public async Task<IList<TType>> FindAll<TType>(
            Expression<Func<T, bool>> where = null,
            Expression<Func<T, TType>> select = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            List<string> includes = null,
            int? skip = null,
            int? take = null)
        {
            IQueryable<T> query = _db;

            if (where != null)
                query = query.Where(where);

            if (includes != null)
                foreach (var table in includes)
                    query = query.Include(table);

            if (include != null)
                query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            if(skip != null)
                query = query.Skip((int)skip);

            if(take != null)
                query = query.Take((int)take);

            return
                (select != null)
                ? await query.Select(select).ToListAsync()
                : (IList<TType>)await query.ToListAsync();
        }

        public async Task<bool> isExists(Expression<Func<T, bool>> expression = null)
        {
            IQueryable<T> query = _db;
            return await query.AnyAsync(expression);
        }

        public void Update(T entity)
        {
            _db.Update(entity);
        }
    }
}
