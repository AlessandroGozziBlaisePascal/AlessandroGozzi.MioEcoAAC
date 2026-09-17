using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using AlessandroGozzi.AppAAC.SharedKernel;

namespace AlessandroGozzi.AppAAC.Domain.Entities
{
    public class Activity: Entity
    {
        private readonly List<RoutineAction> _actions = new();
        public List<RoutineAction> Actions => [.. _actions.AsReadOnly()];
        public string Name { get; private set; }
        public ImageUrl ImageUrl { get; private set; }
        public bool IsFinished => _actions.All(a => a.IsFinished);
        public bool IsDeleted { get; private set; } = false; //For soft delete
        public SpeechText Text { get; private set; }

        private Activity() { } // For EF Core

        private Activity(string name, SpeechText text, ImageUrl imageUrl, List<RoutineAction> actions)
        {
            Name = name;
            Text = text;
            ImageUrl = imageUrl;
            _actions = actions;
        }

        public static Result<Activity> Create(string name, SpeechText text, ImageUrl imageUrl, List<RoutineAction> actions)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Activity>(Error.Validation("Activity.NameRequired", "The activity name is required."));
            if (text == null)
                return Result.Failure<Activity>(Error.Validation("Activity.TextRequired", "The activity text is required."));
            if (imageUrl == null)
                return Result.Failure<Activity>(Error.Validation("Activity.ImageUrlRequired", "The activity image URL is required."));
            if(actions == null || actions.Count == 0)
                return Result.Failure<Activity>(Error.Validation("Activity.ActionsRequired", "The activity actions are required."));
            var activity = new Activity(name, text, imageUrl, actions);
            return Result.Success(activity);
        }

        public Result AddAction(RoutineAction action)
        {
            if (_actions.Any(a => a.Id == action.Id))
                return Result.Failure(Error.Validation("Activity.ActionAlreadyExists", "The routine action already exists in the activity."));
            if(action.ActivityId != Id)
                return Result.Failure(Error.Validation("Activity.ActionActivityMismatch", "The routine action does not belong to this activity."));
            _actions.Add(action);
            return Result.Success();
        }

        public Result RemoveAction(Guid actionId)
        {
            var act = _actions.FirstOrDefault(a => a.Id == actionId);
            if (act == null)
                return Result.Failure(Error.NotFound("Activity.ActionNotFound", "The routine action was not found in the activity."));
            return act.Remove();
        }

        public Result MarkActionAsFinished(Guid actionId)
        {
            var action = _actions.FirstOrDefault(a => a.Id == actionId);
            if (action == null)
                return Result.Failure(Error.NotFound("Activity.ActionNotFound", "The routine action was not found in the activity."));
            return action.MarkAsFinished();
        }

        public Result MarkActivityAsFinished()
        {
            if (IsFinished)
                return Result.Failure(Error.Validation("Activity.AlreadyFinished", "The activity is already finished"));
            if(_actions.Any(a => !a.IsFinished))
                return Result.Failure(Error.Validation("Activity.NotAllActionsFinished", "Not all routine actions are finished"));
            return Result.Success();
        }

        public Result SoftDelete()
        {
            if (IsDeleted)
                return Result.Failure(Error.Validation("Activity.AlreadyDeleted", "The activity is already deleted."));
            IsDeleted = true;
            return Result.Success();
        }

        public Result ChangeOrder(List<RoutineAction> newOrder)
        {
            if (newOrder == null || newOrder.Count != _actions.Count)
                return Result.Failure(Error.Validation("Activity.InvalidNewOrder", "The new order must contain the same number of actions."));

            var newOrderIds = new HashSet<Guid>(newOrder.Select(a => a.Id));
            var currentIds = new HashSet<Guid>(_actions.Select(a => a.Id));
            if (!newOrderIds.SetEquals(currentIds))
                return Result.Failure(Error.Validation("Activity.InvalidNewOrder", "The new order must contain the same actions."));

            _actions.Clear();
            _actions.AddRange(newOrder);

            return Result.Success();
        }
    }
}
