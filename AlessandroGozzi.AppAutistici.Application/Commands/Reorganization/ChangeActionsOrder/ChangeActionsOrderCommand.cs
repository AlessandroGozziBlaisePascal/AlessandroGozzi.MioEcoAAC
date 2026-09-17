using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppAutistici.Application.Dtos;
using MediatR;

namespace AlessandroGozzi.AppAutistici.Application.Commands.Reorganization.ChangeActionsOrder
{
    public record ChangeActionsOrderCommand(
        Guid RoutineId,
        Guid ActivityId,
        IReadOnlyCollection<RoutineActionDto> NewOrder)
    :IRequest<Result>;
}
