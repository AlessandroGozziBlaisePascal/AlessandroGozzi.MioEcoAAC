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
        public Guid ActivityId { get; init; }

        public RoutineAction() { } // For EF Core

        public RoutineAction(ImageUrl image, SpeechText audio, Guid activityId, string description)
        {
            Image = image;
            Audio = audio;
            ActivityId = activityId;
            Description = description;
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
