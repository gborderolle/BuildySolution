using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant> Update(Tenant entity);

        IQueryable<Tenant> GetAllQueryable();
        Task<List<Tenant>> FindTenantsByRentId(int rentId);
    }
}

