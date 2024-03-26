using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IFileRepository : IRepository<File1>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<File1> Update(File1 entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<File1> GetAllQueryable();
        Task<List<File1>> FindFilesByRentId(int rentId);

    }
}

