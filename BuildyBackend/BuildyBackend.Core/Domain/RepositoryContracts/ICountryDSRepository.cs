using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<Country> Update(Country entity);

        IQueryable<Country> GetAllQueryable();
    }
}

