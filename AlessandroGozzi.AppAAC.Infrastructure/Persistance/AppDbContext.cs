using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.AggregateRoots;
using AlessandroGozzi.AppAAC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ActivityEntity = AlessandroGozzi.AppAAC.Domain.Entities.Activity;

namespace AlessandroGozzi.AppAAC.Infrastructure.Persistance
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Routine> Routines => Set<Routine>();
        public DbSet<ActivityEntity> Activities => Set<ActivityEntity>();
        public DbSet<RoutineAction> RoutineActions => Set<RoutineAction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Routine>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Activity>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<RoutineAction>().HasQueryFilter(act => !act.IsDeleted);
            // Applica in automatico tutte le classi IEntityTypeConfiguration dello stesso assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Deleted)
                {
                    var isDeletedProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "IsDeleted");
                    if (isDeletedProp != null)
                    {
                        entry.State = EntityState.Modified;
                        isDeletedProp.CurrentValue = true;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
