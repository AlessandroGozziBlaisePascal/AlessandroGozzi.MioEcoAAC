using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.Infrastructure.Persistance;
using AlessandroGozzi.AppAAC.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.AppAAC.Infrastructure.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : AggregateRoot
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> DbSet;

        protected Repository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>?> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}
