using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface ICityRepository : IRepository<City>
    {
        Task<City> Update(City entity);

        IQueryable<City> GetAllQueryable();
    }
}

