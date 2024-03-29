using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IProvinceRepository : IRepository<Province>
    {
        Task<Province> Update(Province entity);

        IQueryable<Province> GetAllQueryable();
    }
}

