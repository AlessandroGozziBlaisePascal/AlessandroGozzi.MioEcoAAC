using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Mappers;
using AlessandroGozzi.AppAAC.SharedKernel;
using AlessandroGozzi.AppAAC.Domain.Entities;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.AppAAC.Application.Commands.Creation.AddAction
{
    public class AddActionCommandHandler: IRequestHandler<AddActionCommand, Result>
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public AddActionCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(AddActionCommand request, CancellationToken cancellationToken)
        {
            var routine = await _routineRepository.GetByIdAsync(request.RoutineId, cancellationToken);
            if (routine is null)
            {
                return Result.Failure(Error.NotFound("AddAction.Routine", "Routine not found"));
            }

            var imUrlResult = request.ImageUrl.ToImageUrlDomain();
            if(imUrlResult.IsFailure)
            {
                return Result.Failure(imUrlResult.Error);
            }

            var speechTextResult = request.Text.ToSpeechTextDomain();
            if(speechTextResult.IsFailure)
            {
                return Result.Failure(speechTextResult.Error);
            }

            var actionResult = RoutineAction.Create(imUrlResult.Value, speechTextResult.Value, request.Name);
            if(actionResult.IsFailure)
            {
                return Result.Failure(actionResult.Error);
            }

            var result = routine.AddActionToActivity(request.ActivityId, actionResult.Value);
            if(result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
