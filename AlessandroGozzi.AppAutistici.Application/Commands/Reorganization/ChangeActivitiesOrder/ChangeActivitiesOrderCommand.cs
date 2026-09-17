using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppAutistici.Application.Dtos;
using MediatR;

namespace AlessandroGozzi.AppAutistici.Application.Commands.Reorganization.ChangeActivitiesOrder
{
    public record ChangeActivitiesOrderCommand(
        Guid RoutineId,
        IReadOnlyCollection<ActivityDto> NewOrder)
    : IRequest<Result>;
}
