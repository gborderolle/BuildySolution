using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using BuildyBackend.Infrastructure.DbContext;


namespace BuildyBackend.Infrastructure.Repositories
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        private readonly ContextDB _dbContext;

        public JobRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public async Task<Job> Update(Job entity)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Job> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}