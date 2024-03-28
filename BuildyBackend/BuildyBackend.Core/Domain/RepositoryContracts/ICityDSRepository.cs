using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface ICityDSRepository : IRepository<CityDS>
    {
        Task<CityDS> Update(CityDS entity);

        IQueryable<CityDS> GetAllQueryable();
    }
}

