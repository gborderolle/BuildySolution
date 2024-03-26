using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IOwnerDSRepository : IRepository<OwnerDS>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<OwnerDS> Update(OwnerDS entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<OwnerDS> GetAllQueryable();
    }
}

