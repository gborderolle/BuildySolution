using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface ICityDSRepository : IRepository<CityDS>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<CityDS> Update(CityDS entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<CityDS> GetAllQueryable();
    }
}

