using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Activities.UpdateActivity
{
    public class UpdateActivityCommandHandler: IRequestHandler<UpdateActivityCommand, Result>
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public UpdateActivityCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            var routine = await _routineRepository.GetByIdAsync(request.RoutineId, cancellationToken);
            if (routine == null)
                return Result.Failure(Error.NotFound("Routine.NotFound", "The routine containing the specified activity was not found."));

            var activity = routine.Activities.FirstOrDefault(a => a.Id == request.ActivityId);
            if (activity == null)
                return Result.Failure(Error.NotFound("Activity.NotFound", "The specified activity was not found in the routine."));

            ImageUrl? imageUrl = null;
            if (request.ImageUrl is not null)
            {
                var imageResult = ImageUrl.Create(request.ImageUrl);
                if (imageResult.IsFailure)
                    return Result.Failure(imageResult.Error);
                imageUrl = imageResult.Value;
            }

            SpeechText? speechText = null;
            if (request.SpeechText is not null)
            {
                var speechResult = SpeechText.Create(request.SpeechText);
                if (speechResult.IsFailure)
                    return Result.Failure(speechResult.Error);
                speechText = speechResult.Value;
            }

            var updateResult = activity.Update(request.Name, speechText, imageUrl);
            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
