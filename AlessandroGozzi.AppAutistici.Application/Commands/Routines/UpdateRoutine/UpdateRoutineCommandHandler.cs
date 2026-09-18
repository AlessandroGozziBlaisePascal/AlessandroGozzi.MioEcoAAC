using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Routines.UpdateRoutine
{
    public class UpdateRoutineCommandHandler: IRequestHandler<UpdateRoutineCommand, Result>
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public UpdateRoutineCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(UpdateRoutineCommand request, CancellationToken cancellationToken)
        {
            var routine = await _routineRepository.GetByIdAsync(request.RoutineId, cancellationToken);
            if (routine == null)
                return Result.Failure(Error.NotFound("Routine.NotFound", "The specified routine was not found."));

            SpeechText? speechText = null;
            if (request.SpeechText is not null)
            {
                var speechResult = SpeechText.Create(request.SpeechText);
                if (speechResult.IsFailure)
                    return Result.Failure(speechResult.Error);
                speechText = speechResult.Value;
            }

            var updateResult = routine.Update(request.Name, speechText);
            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Error);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
