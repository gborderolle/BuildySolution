using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IJobRepository : IRepository<Job>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<Job> Update(Job entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<Job> GetAllQueryable();
    }
}

