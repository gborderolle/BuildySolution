using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class ProvinceDSRepository : Repository<Province>, IProvinceRepository
    {
        private readonly ContextDB _dbContext;

        public ProvinceDSRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Province> Update(Province entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Province> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}