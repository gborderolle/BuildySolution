using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Core.DTO;
using BuildyBackend.Core.Helpers;
using BuildyBackend.Infrastructure.DbContext;
using BuildyBackend.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ContextDB _dbContext;
        private readonly ILogService _logService;
        internal DbSet<T> dbSet;

        public Repository(ContextDB db)
        {
            _dbContext = db;
            dbSet = _dbContext.Set<T>();
        }

        public async Task Create(T entity)
        {
            await dbSet.AddAsync(entity);
            await Save();
        }

        public async Task<T> Update(T entity)
        {
            // Generic update logic here. You can override it in specific repositories if needed.
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
            await Save();
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<T>> GetAll(
            Expression<Func<T, bool>>? where = null,
            Expression<Func<T, object>>? orderBy = null,
            IEnumerable<IncludePropertyConfiguration<T>> includes = null,
            IEnumerable<ThenIncludePropertyConfiguration<T>> thenIncludes = null,
            PaginationDTO paginationDTO = null,
            HttpContext httpContext = null,
            bool tracked = false,
            bool ascendingOrder = true)
        {
            IQueryable<T> query = dbSet;

            if (includes != null)
            {
                foreach (var includeConfig in includes)
                {
                    query = query.Include(includeConfig.IncludeExpression);
                }
            }

            if (thenIncludes != null)
            {
                foreach (var thenIncludeConfig in thenIncludes)
                {
                    if (thenIncludeConfig.IncludeExpression != null)
                    {
                        var queryWithInclude = query.Include(thenIncludeConfig.IncludeExpression);
                        if (thenIncludeConfig.ThenIncludeExpression != null)
                        {
                            query = queryWithInclude.ThenInclude(thenIncludeConfig.ThenIncludeExpression);
                        }
                    }
                }
            }

            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderBy != null)
            {
                query = ascendingOrder ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (httpContext != null && paginationDTO != null)
            {
                await httpContext.InsertParamPaginationHeader(query);
                query = query.DoPagination(paginationDTO);
            }

            return await query.ToListAsync();
        }

        public async Task<T> Get(
            Expression<Func<T, bool>>? filter = null,
            IEnumerable<IncludePropertyConfiguration<T>> includes = null,
            IEnumerable<ThenIncludePropertyConfiguration<T>> thenIncludes = null,
            bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (includes != null)
            {
                foreach (var includeConfig in includes)
                {
                    query = query.Include(includeConfig.IncludeExpression);
                }
            }

            if (thenIncludes != null)
            {
                foreach (var thenIncludeConfig in thenIncludes)
                {
                    if (thenIncludeConfig.IncludeExpression != null)
                    {
                        var queryWithInclude = query.Include(thenIncludeConfig.IncludeExpression);
                        if (thenIncludeConfig.ThenIncludeExpression != null)
                        {
                            query = queryWithInclude.ThenInclude(thenIncludeConfig.ThenIncludeExpression);
                        }
                    }
                }
            }

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}
