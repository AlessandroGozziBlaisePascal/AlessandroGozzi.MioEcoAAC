using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;
using AlessandroGozzi.AppAAC.Application.Dtos;

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
