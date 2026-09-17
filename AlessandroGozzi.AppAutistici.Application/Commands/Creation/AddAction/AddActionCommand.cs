using AlessandroGozzi.AppACC.SharedKernel;
using MediatR;
using AlessandroGozzi.AppACC.Application.Dtos;

namespace AlessandroGozzi.AppAAC.Application.Commands.Creation.AddAction
{
    public record AddActionCommand(
        Guid RoutineId, 
        Guid ActivityId,
        string Name, 
        string ImageUrl, 
        string Text, 
        IReadOnlyList<RoutineActionDto> Actions)
    : IRequest<Result>;
}
