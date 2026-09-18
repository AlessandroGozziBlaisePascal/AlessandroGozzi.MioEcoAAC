using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Actions.RemoveAction
{
    public class RemoveActionCommandHandler: IRequestHandler<RemoveActionCommand, Result>
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public RemoveActionCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(RemoveActionCommand request, CancellationToken cancellationToken)
        {
            var routine = await _routineRepository.GetByIdAsync(request.RoutineId, cancellationToken);
            if (routine == null)
                return Result.Failure(Error.NotFound("RemoveAction.Routine","Routine not found"));

            var result = routine.RemoveActionFromActivity(request.ActivityId, request.ActionId);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
