using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Application.Commands.Creation.AddRoutine;
using AlessandroGozzi.AppAAC.Application.Mappers;
using AlessandroGozzi.AppAAC.Domain.AggregateRoots;
using AlessandroGozzi.AppAAC.Domain.Enums;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.SharedKernel;
using MediatR;

namespace AlessandroGozzi.AppAutistici.Application.Commands.Creation.AddRoutine
{
    public class AddRoutineCommandHandler: IRequestHandler<AddRoutineCommand, Result>
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IRoutineRepository _routineRepository;

        public AddRoutineCommandHandler(IUnitOfWork unitOfWork, IRoutineRepository routineRepository)
        {
            _unitOfWork = unitOfWork;
            _routineRepository = routineRepository;
        }

        public async Task<Result> Handle(AddRoutineCommand request, CancellationToken cancellationToken)
        {
            var textResult = request.Text.ToSpeechTextDomain();
            if(textResult.IsFailure)
            {
                return Result.Failure(textResult.Error);
            }

            var routineResult = Routine.Create(request.Name, textResult.Value, RoutineType.Custom);
            if(routineResult.IsFailure)
            {
                return Result.Failure(routineResult.Error);
            }

            _routineRepository.Add(routineResult.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
