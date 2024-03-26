using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using BuildyBackend.Infrastructure.DbContext;


namespace BuildyBackend.Infrastructure.Repositories
{
    public class PhotoRepository : Repository<Photo>, IPhotoRepository
    {
        private readonly ContextDB _dbContext;

        public PhotoRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public async Task<Photo> Update(Photo entity)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Photo> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

        public async Task<List<Photo>> FindPhotosByJobId(int jobId)
        {
            return await _dbContext.Set<Photo>() // Use Set<Worker>() instead of Workers
                                   .Where(photo => photo.JobId == jobId)
                                   .ToListAsync();
        }

        public async Task<List<Photo>> FindPhotosByReportId(int reportId)
        {
            return await _dbContext.Set<Photo>() // Use Set<Worker>() instead of Workers
                                   .Where(photo => photo.ReportId == reportId)
                                   .ToListAsync();
        }

    }
}