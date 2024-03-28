using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class CountryDSRepository : Repository<CountryDS>, ICountryDSRepository
    {
        private readonly ContextDB _dbContext;

        public CountryDSRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CountryDS> Update(CountryDS entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<CountryDS> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}