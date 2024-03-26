using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
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

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public async Task<ProvinceDS> Update(ProvinceDS entity)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
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