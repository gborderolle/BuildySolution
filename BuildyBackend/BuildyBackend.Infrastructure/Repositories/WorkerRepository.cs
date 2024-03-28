using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class WorkerRepository : Repository<Worker>, IWorkerRepository
    {
        private readonly ContextDB _dbContext;

        public WorkerRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Worker> Update(Worker entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Worker> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

        public async Task<List<Worker>> FindWorkersByJobId(int jobId)
        {
            return await _dbContext.Set<Worker>() // Use Set<Worker>() instead of Workers
                                   .Where(worker => worker.JobId == jobId)
                                   .ToListAsync();
        }

    }
}