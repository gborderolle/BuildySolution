using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IWorkerRepository : IRepository<Worker>
    {
        Task<Worker> Update(Worker entity);

        IQueryable<Worker> GetAllQueryable();
        Task<List<Worker>> FindWorkersByJobId(int jobId);
    }
}

