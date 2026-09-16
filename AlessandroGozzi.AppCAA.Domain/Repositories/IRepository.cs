using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;

namespace AlessandroGozzi.AppCAA.Domain.Repositories
{
    public interface IRepository<T> where T : AggregateRoot
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>?> GetAllAsync(CancellationToken cancellationToken = default);
        void Remove(T entity);
        void Add(T entity); 
    }
}
