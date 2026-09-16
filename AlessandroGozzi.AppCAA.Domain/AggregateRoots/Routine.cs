using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppCAA.Domain.Entities;
using AlessandroGozzi.AppCAA.Domain.Enums;
using AlessandroGozzi.AppCAA.Domain.ValueObjects;

namespace AlessandroGozzi.AppCAA.Domain.AggregateRoots
{
    public class Routine : AggregateRoot
    {
        private readonly List<Activity> _activities = new();
        public IReadOnlyCollection<Activity> Activities => [.._activities.AsReadOnly()];
        public string Name { get; private set; }
        public SpeechText Text { get; private set; }
        public RoutineType Type { get; private set; }
        public bool IsDeleted { get; private set; } = false; //For soft delete

        public Routine() { } // For EF Core

        public Routine(string name, SpeechText text, RoutineType type)
        {
            Name = name;
            Text = text;
            Type = type;
        }

        public Result AddActivity(Activity activity)
        {
            if (_activities.Contains(activity))
                return Result.Failure(Error.Validation("Routine.ActivityAlreadyExists", "The activity already exists in the routine."));
            _activities.Add(activity);
            return Result.Success();
        }

        public Result RemoveActivity(Activity activity)
        {
            if (!_activities.Contains(activity))
                return Result.Failure(Error.NotFound("Routine.ActivityNotFound", "The activity was not found in the routine."));
            _activities.Remove(activity);
            return Result.Success();
        }

        public Result AddActionToActivity(Guid activityId, RoutineAction action)
        {
            var activity = _activities.FirstOrDefault(a => a.Id == activityId);
            if (activity == null)
                return Result.Failure(Error.NotFound("Routine.ActivityNotFound", "The activity was not found in the routine."));
            return activity.AddAction(action);
        }
    }
}
