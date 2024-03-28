using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IEstateRepository : IRepository<Estate>
    {
        Task<Estate> Update(Estate entity);

        IQueryable<Estate> GetAllQueryable();
    }
}

