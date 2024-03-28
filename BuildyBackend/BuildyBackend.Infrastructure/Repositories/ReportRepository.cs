using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        private readonly ContextDB _dbContext;

        public ReportRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Report> Update(Report entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Report> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}