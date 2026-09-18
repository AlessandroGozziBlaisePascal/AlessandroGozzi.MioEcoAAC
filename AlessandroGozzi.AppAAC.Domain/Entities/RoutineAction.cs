using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using AlessandroGozzi.AppAAC.SharedKernel;

namespace AlessandroGozzi.AppAAC.Domain.Entities
{
    public class RoutineAction: Entity  
    {
        public ImageUrl Image { get; private set; }
        public string Name { get; private set; }
        public SpeechText Audio { get; private set; }
        public bool IsFinished { get; private set; } = false;
        public bool IsDeleted { get; private set; } = false; //For soft delete

        private RoutineAction() { } // For EF Core

        private RoutineAction(Guid activityId, ImageUrl image, SpeechText audio, string description)
        {
            ActivityId = activityId;
            Image = image;
            Audio = audio;
            Name = description;
        }
        public static Result<RoutineAction> Create(Guid activityId, ImageUrl image, SpeechText audio, string description)
        {
            if (activityId == Guid.Empty)
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidActivityId", "The activity ID cannot be empty."));
            if (string.IsNullOrWhiteSpace(description))
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidDescription", "The description cannot be null or empty."));
            if(image == null)
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidImage", "The image cannot be null."));
            if(audio == null)
                return Result.Failure<RoutineAction>(Error.Validation("RoutineAction.InvalidAudio", "The audio cannot be null."));
            var routineAction = new RoutineAction(activityId, image, audio, description);
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

        public Result Update(string? name, SpeechText? audio, ImageUrl? image)
        {
            if (IsDeleted)
                return Result.Failure(Error.Failure("RoutineAction.Deleted", "Cannot update a deleted routine action."));
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;
            if (audio != null)
                Audio = audio;
            if (image != null)
                Image = image;
            return Result.Success();
        }
    }
}
