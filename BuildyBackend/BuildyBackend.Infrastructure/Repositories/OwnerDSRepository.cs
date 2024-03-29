using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class OwnerRepository : Repository<Owner>, IOwnerRepository
    {
        private readonly ContextDB _dbContext;

        public OwnerRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Owner> Update(Owner entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Owner> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}