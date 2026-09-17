using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppAutistici.Application.Mappers;
using AlessandroGozzi.AppCAA.Domain.Entities;
using AlessandroGozzi.AppCAA.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.AppAutistici.Application.Commands.Reorganization.ChangeActivitiesOrder
{
    public class ChangeActivitiesOrderCommandHandler: IRequestHandler<ChangeActivitiesOrderCommand, Result>
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public ChangeActivitiesOrderCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(ChangeActivitiesOrderCommand request, CancellationToken cancellationToken)
        {
            var routine = await _routineRepository.GetByIdAsync(request.RoutineId, cancellationToken);
            if (routine == null)
                return Result.Failure(Error.NotFound("ChangeActivitiesOrder.Routine","Routine not found"));

            var domainActivities = new List<Activity>();
            #region DTO => DOMAIN ACTIVITIES LIST
            foreach (var activityDto in request.NewOrder)
            {
                var activityImageResult = activityDto.Image.ToImageUrlDomain();
                if (activityImageResult.IsFailure)
                {
                    return Result.Failure(activityImageResult.Error);
                }

                var activityAudioResult = activityDto.SpeechText.ToSpeechTextDomain();
                if (activityAudioResult.IsFailure)
                {
                    return Result.Failure(activityAudioResult.Error);
                }

                var domainActions = new List<RoutineAction>();
                #region DTO => DOMAIN ACTIONS LIST
                foreach (var actionDto in activityDto.Actions)
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
                #endregion

                var activityResult = Activity.Create(
                    activityDto.Name,
                    activityAudioResult.Value,
                    activityImageResult.Value,
                    domainActions
                );

                if (activityResult.IsFailure)
                {
                    return Result.Failure(activityResult.Error);
                }

                domainActivities.Add(activityResult.Value);
            }
            #endregion

            var result = routine.ChangeActivityOrder(domainActivities);
            if (result.IsFailure)
                return Result.Failure(result.Error);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
