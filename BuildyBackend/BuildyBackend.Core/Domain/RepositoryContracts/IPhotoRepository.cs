using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Core.Domain.RepositoryContracts
{
    public interface IPhotoRepository : IRepository<Photo>
    {
        Task<Photo> Update(Photo entity);

        IQueryable<Photo> GetAllQueryable();
        Task<List<Photo>> FindPhotosByJobId(int jobId);
        Task<List<Photo>> FindPhotosByReportId(int reportId);

    }
}

