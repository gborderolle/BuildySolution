using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IWorkerRepository : IRepository<Worker>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<Worker> Update(Worker entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<Worker> GetAllQueryable();
        Task<List<Worker>> FindWorkersByJobId(int jobId);
    }
}

