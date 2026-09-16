using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace AlessandroGozzi.AppACC.SharedKernel
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOnUtc { get; }
    }

    public abstract record DomainEvent : IDomainEvent
    {
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}
