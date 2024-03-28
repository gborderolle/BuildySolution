using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IJobRepository : IRepository<Job>
    {

        Task<Job> Update(Job entity);

        IQueryable<Job> GetAllQueryable();
    }
}

