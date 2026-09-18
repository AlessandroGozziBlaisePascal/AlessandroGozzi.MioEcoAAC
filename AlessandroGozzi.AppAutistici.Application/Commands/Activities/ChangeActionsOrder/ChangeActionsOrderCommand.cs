using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Dtos;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Activities.ChangeActionsOrder
{
    public record ChangeActionsOrderCommand(
        Guid RoutineId,
        Guid ActivityId,
        IReadOnlyCollection<RoutineActionDto> NewOrder)
    :IRequest<Result>;
}
