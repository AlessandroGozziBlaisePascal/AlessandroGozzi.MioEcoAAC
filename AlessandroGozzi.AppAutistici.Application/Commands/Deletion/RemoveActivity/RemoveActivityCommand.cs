using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Deletion.RemoveActivity
{
    public record RemoveActivityCommand(
        Guid RoutineId,
        Guid ActivityId)
    :IRequest<Result>;
}
