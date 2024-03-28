using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IReportRepository : IRepository<Report>
    {
        Task<Report> Update(Report entity);

        IQueryable<Report> GetAllQueryable();
    }
}

