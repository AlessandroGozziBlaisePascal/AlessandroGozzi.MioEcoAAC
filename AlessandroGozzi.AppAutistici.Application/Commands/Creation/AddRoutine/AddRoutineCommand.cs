using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppAutistici.Application.Dtos;
using MediatR;

namespace AlessandroGozzi.AppAutistici.Application.Commands.Creation.AddRoutine
{
    public record AddRoutineCommand(
        string Name, 
        string Text, 
        string ImageUrl, 
        IReadOnlyList<ActivityDto> Activities)
    :IRequest<Result>;
}
