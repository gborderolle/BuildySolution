using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IProvinceDSRepository : IRepository<ProvinceDS>
    {
        Task<ProvinceDS> Update(ProvinceDS entity);

        IQueryable<ProvinceDS> GetAllQueryable();
    }
}

