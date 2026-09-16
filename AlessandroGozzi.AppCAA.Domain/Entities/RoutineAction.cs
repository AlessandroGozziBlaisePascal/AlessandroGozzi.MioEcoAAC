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
        public Guid ActivityId { get; init; }

        public RoutineAction() { } // For EF Core

        private RoutineAction(ImageUrl image, SpeechText audio, Guid activityId, string description)
        {
            Image = image;
            Audio = audio;
            ActivityId = activityId;
            Description = description;
        }
        public static Result<RoutineAction> Create(ImageUrl image, SpeechText audio, Guid activityId, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidDescription", "The description cannot be null or empty."));
            if(image == null)
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidImage", "The image cannot be null."));
            if(audio == null)
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidAudio", "The audio cannot be null."));
            if(activityId == Guid.Empty)
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidActivityId", "The activity ID cannot be empty."));
            var routineAction = new RoutineAction(image, audio, activityId, description);
            return Result.Success(routineAction);
        }

        public Result MarkAsFinished()
        {
            if (IsFinished)
                return Result.Failure(Error.Failure("RoutineAction.AlreadyFinished", "The routine action is already marked as finished."));
            IsFinished = true;
            return Result.Success();
        }
    }
}
