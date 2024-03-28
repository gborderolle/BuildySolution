using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IFileRepository : IRepository<File1>
    {
        Task<File1> Update(File1 entity);

        IQueryable<File1> GetAllQueryable();
        Task<List<File1>> FindFilesByRentId(int rentId);

    }
}

