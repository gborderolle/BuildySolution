using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IReportRepository : IRepository<Report>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Task<Report> Update(Report entity);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        IQueryable<Report> GetAllQueryable();
    }
}

