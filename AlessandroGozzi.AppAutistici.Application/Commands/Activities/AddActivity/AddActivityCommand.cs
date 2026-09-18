using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Dtos;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Activities.AddActivity
{
    public record AddActivityCommand(
        Guid RoutineId, 
        string Name, 
        string ImageUrl, 
        string Text, 
        IReadOnlyList<RoutineActionDto> Actions)
    :IRequest<Result>;
}
