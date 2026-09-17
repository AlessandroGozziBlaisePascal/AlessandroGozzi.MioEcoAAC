using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Dtos;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Reorganization.ChangeActivitiesOrder
{
    public record ChangeActivitiesOrderCommand(
        Guid RoutineId,
        IReadOnlyCollection<ActivityDto> NewOrder)
    : IRequest<Result>;
}
