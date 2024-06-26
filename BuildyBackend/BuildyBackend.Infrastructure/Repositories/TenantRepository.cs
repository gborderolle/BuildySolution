﻿using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Repositories
{
    public class TenantRepository : Repository<Tenant>, ITenantRepository
    {
        private readonly ContextDB _dbContext;

        public TenantRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tenant> Update(Tenant entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await Save();
            return entity;
        }

        public IQueryable<Tenant> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }

        public async Task<List<Tenant>> FindTenantsByRentId(int rentId)
        {
            return await _dbContext.Set<Tenant>() // Use Set<Worker>() instead of Workers
                                   .Where(tenant => tenant.RentId == rentId)
                                   .ToListAsync();
        }
    }
}