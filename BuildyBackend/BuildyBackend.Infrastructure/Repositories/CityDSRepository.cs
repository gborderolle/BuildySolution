using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class CityDSRepository : Repository<CityDS>, ICityDSRepository
    {
        private readonly ContextDB _dbContext;

        public CityDSRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CityDS> Update(CityDS entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<CityDS> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}