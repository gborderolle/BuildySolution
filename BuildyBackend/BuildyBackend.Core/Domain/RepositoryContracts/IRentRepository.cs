using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IRentRepository : IRepository<Rent>
    {
        Task<Rent> Update(Rent entity);

        IQueryable<Rent> GetAllQueryable();
    }
}

