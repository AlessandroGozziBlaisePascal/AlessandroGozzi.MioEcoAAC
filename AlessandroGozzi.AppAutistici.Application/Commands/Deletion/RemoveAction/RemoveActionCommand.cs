using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAutistici.Application.Commands.Deletion.RemoveAction
{
    public record RemoveActionCommand(
        Guid RoutineId, 
        Guid ActivityId,
        Guid ActionId)
    :IRequest<Result>;
}
