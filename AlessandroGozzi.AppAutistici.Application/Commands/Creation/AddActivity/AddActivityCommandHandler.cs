using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.SharedKernel;
using AlessandroGozzi.AppAAC.Domain.Entities;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using MediatR;
using AlessandroGozzi.AppAAC.Application.Mappers;

namespace AlessandroGozzi.AppAAC.Application.Commands.Creation.AddActivity
{
    public class AddActivityCommandHandler: IRequestHandler<AddActivityCommand, Result> 
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public AddActivityCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(AddActivityCommand request, CancellationToken cancellationToken)
        {
            var routine = await _routineRepository.GetByIdAsync(request.RoutineId, cancellationToken);
            if (routine is null)
            {
                return Result.Failure(Error.NotFound("AddActivity.Routine", "Routine not found"));
            }

            var imResult = request.ImageUrl.ToImageUrlDomain();
            if (imResult.IsFailure)
            {
                return Result.Failure(imResult.Error);
            }

            var textResult = request.Text.ToSpeechTextDomain();
            if (textResult.IsFailure)
            {
                return Result.Failure(textResult.Error);
            }

            var domainActions = new List<RoutineAction>();

            foreach (var actionDto in request.Actions)
            {
                var actionImageResult = actionDto.Image.ToImageUrlDomain();
                if (actionImageResult.IsFailure)
                {
                    return Result.Failure(actionImageResult.Error);
                }

                var actionAudioResult = actionDto.SpeechText.ToSpeechTextDomain();
                if (actionAudioResult.IsFailure)
                {
                    return Result.Failure(actionAudioResult.Error);
                }

                var actionResult = RoutineAction.Create(
                    actionImageResult.Value,
                    actionAudioResult.Value,
                    actionDto.Name
                );
                if (actionResult.IsFailure)
                {
                    return Result.Failure(actionResult.Error);
                }

                domainActions.Add(actionResult.Value);
            }

            var activity = Activity.Create(request.Name, textResult.Value, imResult.Value, domainActions);
            if (activity.IsFailure)
            {
                return Result.Failure(activity.Error);
            }

            var activityResult = routine.AddActivity(activity.Value);
            if(activityResult.IsFailure)
            {
                return Result.Failure(activityResult.Error);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
