using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppCAA.Domain.Enums;
using AlessandroGozzi.AppCAA.Domain.ValueObjects;

namespace AlessandroGozzi.AppCAA.Domain.AggregateRoots
{
    public class Routine : AggregateRoot
    {
        private readonly List<Guid> _activitiesIds = new();
        public IReadOnlyCollection<Guid> ActivitiesIds => [.._activitiesIds.AsReadOnly()];
        public string Name { get; private set; }
        public SpeechText Text { get; private set; }
        public RoutineType Type { get; private set; }

        public Routine() { } // For EF Core

        public Routine(string name, SpeechText text, RoutineType type)
        {
            Name = name;
            Text = text;
            Type = type;
        }

        public Result AddActivity(Guid activityId)
        {
            if (_activitiesIds.Contains(activityId))
                return Result.Failure(Error.Validation("Routine.ActivityAlreadyExists", "The activity already exists in the routine."));
            _activitiesIds.Add(activityId);
            return Result.Success();
        }

        public Result RemoveActivity(Guid activityId)
        {
            if (!_activitiesIds.Contains(activityId))
                return Result.Failure(Error.NotFound("Routine.ActivityNotFound", "The activity was not found in the routine."));
            _activitiesIds.Remove(activityId);
            return Result.Success();
        }
    }
}
