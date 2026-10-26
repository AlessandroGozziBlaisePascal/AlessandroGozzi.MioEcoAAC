using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Dtos;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Queries.GetActionsById
{
    public class GetActionsByRoutineIdQueryHandler
    : IRequestHandler<GetActionsByRoutineIdQuery, List<ActivityDto>>
    {
        private readonly IRoutineRepository _routineRepository;

        public GetActionsByRoutineIdQueryHandler(IRoutineRepository routineRepository)
        {
            _routineRepository = routineRepository;
        }

        public async Task<List<ActivityDto>> Handle(GetActionsByRoutineIdQuery request, CancellationToken cancellationToken)
        {
            var activities = await _routineRepository.GetActivitiesByRoutineIdAsync(request.RoutineId, cancellationToken);

            return [.. activities
                .Select(a => new ActivityDto(
                    a.Id,
                    a.ImageUrl.ToString(),
                    [.. a.Actions.Select(action => new RoutineActionDto(

                    action.Id,
                    action.Image.ToString(),
                    action.Name,
                    action.Audio.Text,
                    action.IsFinished
                ))],

                    a.Name,
                    a.IsFinished,
                    a.Text.Text
                ))];
        }
    }
}
