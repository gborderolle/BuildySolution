using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IOwnerRepository : IRepository<Owner>
    {
        Task<Owner> Update(Owner entity);

        IQueryable<Owner> GetAllQueryable();
    }
}

