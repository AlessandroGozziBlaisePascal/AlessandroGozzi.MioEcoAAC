using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Mappers;
using AlessandroGozzi.AppAAC.Domain.Entities;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Reorganization.ChangeActionsOrder
{
    public class ChangeActionsOrderCommandHandler: IRequestHandler<ChangeActionsOrderCommand, Result>
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public ChangeActionsOrderCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(ChangeActionsOrderCommand request, CancellationToken cancellationToken)
        {
            var routine = await _routineRepository.GetByIdAsync(request.RoutineId, cancellationToken);
            if (routine == null)
                return Result.Failure(Error.NotFound("ChangeActionsOrder.Routine","Routine not found"));

            var domainActions = new List<RoutineAction>();
            #region DTO => DOMAIN ACTIONS LIST
            foreach (var actionDto in request.NewOrder)
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

            var result = routine.ChangeActionOrderInActivity(request.ActivityId, domainActions);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
