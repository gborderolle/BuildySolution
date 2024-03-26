using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using BuildyBackend.Infrastructure.DbContext;
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

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public async Task<CityDS> Update(CityDS entity)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
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