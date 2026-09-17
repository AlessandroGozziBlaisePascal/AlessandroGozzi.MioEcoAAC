using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppCAA.Domain.AggregateRoots;

namespace AlessandroGozzi.AppCAA.Domain.Repositories
{
    public interface IRoutineRepository : IRepository<Routine>
    {
        Task<List<Routine>> GetAllAvailable(CancellationToken cancellationToken = default);
    }
}
