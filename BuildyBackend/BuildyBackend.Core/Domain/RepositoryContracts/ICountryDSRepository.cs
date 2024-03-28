using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface ICountryDSRepository : IRepository<CountryDS>
    {
        Task<CountryDS> Update(CountryDS entity);

        IQueryable<CountryDS> GetAllQueryable();
    }
}

