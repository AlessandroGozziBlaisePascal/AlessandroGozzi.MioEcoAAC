using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAutistici.Application.Commands.Deletion.RemoveRoutine
{
    public record RemoveRoutineCommand(
        Guid RoutineId)
    :IRequest<Result>;
}
