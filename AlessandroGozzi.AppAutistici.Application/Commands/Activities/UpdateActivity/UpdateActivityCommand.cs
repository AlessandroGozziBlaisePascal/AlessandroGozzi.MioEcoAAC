using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Activities.UpdateActivity
{
    public record UpdateActivityCommand(
        Guid RoutineId,
        Guid ActivityId,
        string? Name,
        string? SpeechText,
        string? ImageUrl)
    :IRequest<Result>;
}
