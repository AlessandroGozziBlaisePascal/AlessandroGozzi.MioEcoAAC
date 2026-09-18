using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Routines.RemoveRoutine
{
    public record RemoveRoutineCommand(
        Guid RoutineId)
    :IRequest<Result>;
}
