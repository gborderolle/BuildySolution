using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class OwnerDSRepository : Repository<OwnerDS>, IOwnerDSRepository
    {
        private readonly ContextDB _dbContext;

        public OwnerDSRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OwnerDS> Update(OwnerDS entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<OwnerDS> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}