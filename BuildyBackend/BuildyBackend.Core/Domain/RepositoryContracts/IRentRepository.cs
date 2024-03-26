using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IRentRepository : IRepository<Rent>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<Rent> Update(Rent entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<Rent> GetAllQueryable();
    }
}

