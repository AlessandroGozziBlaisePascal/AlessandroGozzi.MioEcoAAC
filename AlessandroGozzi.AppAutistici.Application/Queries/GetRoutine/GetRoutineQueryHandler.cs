using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Dtos;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.SharedKernel;

namespace AlessandroGozzi.AppAAC.Application.Queries.GetRoutine
{
    public class GetRoutineQueryHandler
    {
        private readonly IRoutineRepository _routineRepository;

        public GetRoutineQueryHandler(IRoutineRepository routineRepository)
        {
            _routineRepository = routineRepository;
        }

        public async Task<Result<RoutineDto>> HandleAsync(
            GetRoutineQuery query,
            CancellationToken cancellationToken = default)
        {
            var routine = await _routineRepository.GetByIdAsync(query.RoutineId);

            if (routine is null || routine.IsDeleted)
            {
                return Result.Failure<RoutineDto>( Error.NotFound("Routine.NotFound", "Nessuna routine trovata con l'ID specificato."));
            }

            var activityDtos = routine.Activities
            .Where(a => !a.IsDeleted)
            .Select(a => new ActivityDto(
                a.Id,
                a.Name,
                [.. a.Actions
                    .Where(act => !act.IsDeleted)
                    .Select(act => new RoutineActionDto(
                        act.Id,
                        act.Name,
                        act.Image.ToString(),
                        act.Audio.Text,
                        act.IsFinished
                    ))],
                a.ImageUrl.ToString(),
                a.IsFinished,
                a.Text.Text
            )).ToList();

            var routineDto = new RoutineDto(
                routine.Id,
                routine.Name,
                routine.Text.Text,
                activityDtos,
                routine.Activities.All(act => act.IsFinished),
                routine.Type.ToString()
            );

            return Result.Success(routineDto);
        }
    }
}
