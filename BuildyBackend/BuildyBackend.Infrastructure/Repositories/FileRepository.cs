using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using BuildyBackend.Infrastructure.DbContext;


namespace BuildyBackend.Infrastructure.Repositories
{
    public class FileRepository : Repository<File1>, IFileRepository
    {
        private readonly ContextDB _dbContext;

        public FileRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public async Task<File1> Update(File1 entity)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<File1> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

        public async Task<List<File1>> FindFilesByRentId(int rentId)
        {
            return await _dbContext.Set<File1>() // Use Set<Worker>() instead of Workers
                                   .Where(File1 => File1.RentId == rentId)
                                   .ToListAsync();
        }

    }
}