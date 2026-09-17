using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.SharedKernel;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Deletion.RemoveActivity
{
    public class RemoveActivityCommandHandler:IRequestHandler<RemoveActivityCommand, Result>
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public RemoveActivityCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(RemoveActivityCommand request, CancellationToken cancellationToken)
        {
            var routine = await _routineRepository.GetByIdAsync(request.RoutineId, cancellationToken);
            if (routine == null)
                return Result.Failure(Error.NotFound("RemoveActivity.Routine","Routine not found"));

            var result = routine.RemoveActivity(request.ActivityId);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
