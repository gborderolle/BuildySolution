using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IOwnerDSRepository : IRepository<OwnerDS>
    {
        Task<OwnerDS> Update(OwnerDS entity);

        IQueryable<OwnerDS> GetAllQueryable();
    }
}

