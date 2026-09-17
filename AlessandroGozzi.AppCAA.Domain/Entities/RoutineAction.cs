using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppCAA.Domain.ValueObjects;

namespace AlessandroGozzi.AppCAA.Domain.Entities
{
    public class RoutineAction: Entity  
    {
        public ImageUrl Image { get; init; }
        public string Description { get; init; }
        public SpeechText Audio { get; init; }
        public bool IsFinished { get; private set; } = false;
        public bool IsDeleted { get; private set; } = false; //For soft delete
        public Guid ActivityId { get; init; } = Guid.Empty;

        private RoutineAction() { } // For EF Core

        private RoutineAction(ImageUrl image, SpeechText audio, string description)
        {
            Image = image;
            Audio = audio;
            Description = description;
        }
        public static Result<RoutineAction> Create(ImageUrl image, SpeechText audio, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidDescription", "The description cannot be null or empty."));
            if(image == null)
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidImage", "The image cannot be null."));
            if(audio == null)
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidAudio", "The audio cannot be null."));
            var routineAction = new RoutineAction(image, audio, description);
            return Result.Success(routineAction);
        }

        public Result MarkAsFinished()
        {
            if (IsFinished)
                return Result.Failure(Error.Failure("RoutineAction.AlreadyFinished", "The routine action is already marked as finished."));
            IsFinished = true;
            return Result.Success();
        }

        public Result Remove()
        {
            if (IsDeleted)
                return Result.Failure(Error.Failure("RoutineAction.AlreadyDeleted", "The routine action is already marked as deleted."));
            IsDeleted = true;
            return Result.Success();
        }
    }
}
