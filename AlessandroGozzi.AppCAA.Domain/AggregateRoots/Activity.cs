using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppCAA.Domain.Entities;
using AlessandroGozzi.AppCAA.Domain.ValueObjects;

namespace AlessandroGozzi.AppCAA.Domain.AggregateRoots
{
    public class Activity: AggregateRoot
    {
        private readonly List<RoutineAction> _actions = new();
        public List<RoutineAction> Actions => [.. _actions.AsReadOnly()];
        public string Name { get; private set; }
        public bool IsFinished => _actions.All(a => a.IsFinished);
        public SpeechText Text { get; private set; }

        private Activity() { } // For EF Core

        public Activity(string name, SpeechText text)
        {
            Name = name;
            Text = text;
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
            var action = _actions.FirstOrDefault(a => a.Id == actionId);
            if (action == null)
                return Result.Failure(Error.NotFound("Activity.ActionNotFound", "The routine action was not found in the activity."));
            _actions.Remove(action);
            return Result.Success();
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
    }
}
