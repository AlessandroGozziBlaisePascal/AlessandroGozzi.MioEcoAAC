using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Enums;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Routines.UpdateRoutine
{
    public record UpdateRoutineCommand(
        Guid RoutineId,
        string? Name,
        string? SpeechText)
    :IRequest<Result>;
}
