using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppCAA.Domain.AggregateRoots;
using Activity = AlessandroGozzi.AppCAA.Domain.AggregateRoots.Activity;

namespace AlessandroGozzi.AppCAA.Domain.Repositories
{
    public interface IActivityRepository : IRepository<Activity>
    {
        public Task<IReadOnlyList<Activity>> GetByIds(List<Guid> ids, CancellationToken cancellationToken = default);
    }
}
