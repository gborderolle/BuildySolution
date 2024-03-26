using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IEstateRepository : IRepository<Estate>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<Estate> Update(Estate entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<Estate> GetAllQueryable();
    }
}

