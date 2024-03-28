using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class EstateRepository : Repository<Estate>, IEstateRepository
    {
        private readonly ContextDB _dbContext;

        public EstateRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Estate> Update(Estate entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Estate> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}