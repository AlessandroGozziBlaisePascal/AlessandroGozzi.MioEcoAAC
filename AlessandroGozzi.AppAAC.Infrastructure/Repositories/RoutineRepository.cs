using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.AggregateRoots;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.AppAAC.Infrastructure.Repositories
{
    public class RoutineRepository : Repository<Routine>, IRoutineRepository
    {
        public RoutineRepository(AppDbContext context) : base(context) { }

        public async Task<List<Routine>> GetAllAvailable(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(r => r.Activities)
                    .ThenInclude(a => a.Actions)
                .ToListAsync(cancellationToken);
        }
    }
}
