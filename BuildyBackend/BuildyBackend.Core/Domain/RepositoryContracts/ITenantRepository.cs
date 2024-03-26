using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface ITenantRepository : IRepository<Tenant>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<Tenant> Update(Tenant entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<Tenant> GetAllQueryable();
        Task<List<Tenant>> FindTenantsByRentId(int rentId);
    }
}

