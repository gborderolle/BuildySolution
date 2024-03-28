using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class ProvinceDSRepository : Repository<ProvinceDS>, IProvinceDSRepository
    {
        private readonly ContextDB _dbContext;

        public ProvinceDSRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProvinceDS> Update(ProvinceDS entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<ProvinceDS> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}