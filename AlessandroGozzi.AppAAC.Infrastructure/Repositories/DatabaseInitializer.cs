using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Interfaces;
using AlessandroGozzi.AppAAC.Infrastructure.Persistance;

namespace AlessandroGozzi.AppAAC.Infrastructure.Repositories
{
    internal class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly AppDbContext _context;

        public DatabaseInitializer(AppDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAndSeedAsync()
        {
            await DataSeeder.SeedAsync(_context);
        }
    }
}
