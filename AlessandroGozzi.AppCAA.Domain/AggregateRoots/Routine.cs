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

        private Routine() { } // For EF Core

        private Routine(string name, SpeechText text, RoutineType type)
        {
            Name = name;
            Text = text;
            Type = type;
        }

        public static Result<Routine> Create(string name, SpeechText text, RoutineType type)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Routine>(Error.Validation("Routine.NameRequired", "The routine name is required."));
            if (text == null || string.IsNullOrWhiteSpace(text.Text))
                return Result.Failure<Routine>(Error.Validation("Routine.TextRequired", "The routine text is required."));
            var routine = new Routine(name, text, type);
            return Result.Success(routine);
        }

        public Result Remove()
        {
            if (IsDeleted)
                return Result.Failure(Error.Failure("Routine.AlreadyDeleted", "The routine is already marked as deleted."));
            IsDeleted = true;
            return Result.Success();
        }

        public Result AddActivity(Activity activity)
        {
            if (_activities.Contains(activity))
                return Result.Failure(Error.Validation("Routine.ActivityAlreadyExists", "The activity already exists in the routine."));
            _activities.Add(activity);
            return Result.Success();
        }

        public Result RemoveActivity(Guid activityId)
        {
            var act = _activities.FirstOrDefault(a => a.Id == activityId);
            if (act == null)
                return Result.Failure(Error.NotFound("Routine.ActivityNotFound", "The activity was not found in the routine."));
            act.SoftDelete();
            return Result.Success();
        }

        public Result AddActionToActivity(Guid activityId, RoutineAction action)
        {
            var activity = _activities.FirstOrDefault(a => a.Id == activityId);
            if (activity == null)
                return Result.Failure(Error.NotFound("Routine.ActivityNotFound", "The activity was not found in the routine."));
            return activity.AddAction(action);
        }

        public Result RemoveActionFromActivity(Guid activityId, Guid actionId)
        {
            var activity = _activities.FirstOrDefault(a => a.Id == activityId);
            if (activity == null)
                return Result.Failure(Error.NotFound("Routine.ActivityNotFound", "The activity was not found in the routine."));
            return activity.RemoveAction(actionId);
        }

        public Result ChangeActivityOrder(List<Activity> newOrder)
        {
            if (newOrder.Count != _activities.Count || newOrder.Except(_activities).Any())
                return Result.Failure(Error.Validation("Routine.InvalidActivityOrder", "The new order of activities is invalid."));

            var newOrderIds = new HashSet<Guid>(newOrder.Select(a => a.Id));
            var currentIds = new HashSet<Guid>(_activities.Select(a => a.Id));
            if (!newOrderIds.SetEquals(currentIds))
                return Result.Failure(Error.Validation("Routine.InvalidNewOrder", "The new order must contain the same activities."));

            _activities.Clear();
            _activities.AddRange(newOrder);

            return Result.Success();
        }

        public Result ChangeActionOrderInActivity(Guid activityId, List<RoutineAction> newOrder)
        {
            var activity = _activities.FirstOrDefault(a => a.Id == activityId);
            if (activity == null)
                return Result.Failure(Error.NotFound("Routine.ActivityNotFound", "The activity was not found in the routine."));

            return activity.ChangeOrder(newOrder);
        }
    }
}
