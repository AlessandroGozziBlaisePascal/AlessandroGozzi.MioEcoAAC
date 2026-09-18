using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Actions.UpdateAction
{
    public record UpdateActionCommand(
        Guid RoutineId,
        Guid ActivityId,
        Guid ActionId,
        string? Name,
        string? SpeechText,
        string? ImageUrl)
    :IRequest<Result>;

}
