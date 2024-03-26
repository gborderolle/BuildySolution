using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface ICountryDSRepository : IRepository<CountryDS>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<CountryDS> Update(CountryDS entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<CountryDS> GetAllQueryable();
    }
}

