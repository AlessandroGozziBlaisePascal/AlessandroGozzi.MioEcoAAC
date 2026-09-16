using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppAutistici.Application.Dtos;
using MediatR;

namespace AlessandroGozzi.AppAutistici.Application.Commands.Creation.AddAction
{
    public record AddActionCommand(Guid RoutineId, 
        Guid ActivityId,
        string Name, 
        string ImageUrl, 
        string Text, 
        IReadOnlyList<RoutineActionDto> Actions)
    : IRequest<Result>;
}
