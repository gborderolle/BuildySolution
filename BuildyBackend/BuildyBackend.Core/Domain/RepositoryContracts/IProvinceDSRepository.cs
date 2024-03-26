using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IProvinceDSRepository : IRepository<ProvinceDS>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<ProvinceDS> Update(ProvinceDS entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<ProvinceDS> GetAllQueryable();
    }
}

