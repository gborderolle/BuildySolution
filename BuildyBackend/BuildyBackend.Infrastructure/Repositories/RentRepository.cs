using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class RentRepository : Repository<Rent>, IRentRepository
    {
        private readonly ContextDB _dbContext;

        public RentRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Rent> Update(Rent entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Rent> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

    }
}