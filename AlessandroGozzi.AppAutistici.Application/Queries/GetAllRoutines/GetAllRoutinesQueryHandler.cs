using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Dtos;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Queries.GetAllRoutines
{
    public class GetAllRoutinesQueryHandler : IRequestHandler<GetAllRoutinesQuery, Result<List<RoutineSummaryDto>>>
    {
        private readonly IRoutineRepository _routineRepository;

        public GetAllRoutinesQueryHandler(IRoutineRepository routineRepository)
        {
            _routineRepository = routineRepository;
        }

        public async Task<Result<List<RoutineSummaryDto>>> Handle(
            GetAllRoutinesQuery request,
            CancellationToken cancellationToken)
        {
            var routines = await _routineRepository.GetAllAvailable(cancellationToken);

            if (routines is null || routines.Count == 0)
            {
                return Result.Failure<List<RoutineSummaryDto>>(Error.NotFound("Routines.NotFound", "Nessuna routine trovata nel database."));
            }

            var routineDtos = routines
                .Select(r => new RoutineSummaryDto(
                    r.Id,
                    r.Name,
                    r.Text.Text,
                    r.Type.ToString()
                ))
                .ToList();

            return Result.Success(routineDtos);
        }
    }
}
