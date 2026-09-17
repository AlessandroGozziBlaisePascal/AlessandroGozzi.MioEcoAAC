using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Entities;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.AppAAC.DomainTest.Entities
{
    public class ActivityTests
    {
        private static SpeechText CreateValidSpeechText() =>
            SpeechText.Create("Descrizione test").Value;

        private static ImageUrl CreateValidImageUrl() =>
            ImageUrl.Create("example.jpg").Value;

        private static RoutineAction CreateValidAction(Guid ActivityId, string title = "Azione Test") =>
            RoutineAction.Create(ActivityId, CreateValidImageUrl(), CreateValidSpeechText(), title).Value;

        private static Activity CreateValidActivity(List<RoutineAction>? actions = null)
        {
            var actionList = actions ?? new List<RoutineAction> { CreateValidAction(Guid.NewGuid()), CreateValidAction(Guid.NewGuid(), "Lava denti") };
            return Activity.Create("Attività Mattina", CreateValidSpeechText(), CreateValidImageUrl(), actionList).Value;
        }

        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccessAndSetProperties()
        {
            var name = "Prepararsi";
            var text = CreateValidSpeechText();
            var imageUrl = CreateValidImageUrl();
            var actions = new List<RoutineAction> { CreateValidAction(Guid.NewGuid()) };

            var result = Activity.Create(name, text, imageUrl, actions);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(name);
            result.Value.Text.Should().Be(text);
            result.Value.ImageUrl.Should().Be(imageUrl);
            result.Value.Actions.Should().HaveCount(1);
            result.Value.IsDeleted.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ShouldReturnNameRequiredError(string? invalidName)
        {
            var result = Activity.Create(
                invalidName!,
                CreateValidSpeechText(),
                CreateValidImageUrl(),
                new List<RoutineAction> { CreateValidAction(Guid.NewGuid()) });

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.NameRequired");
        }

        [Fact]
        public void Create_WithNullText_ShouldReturnTextRequiredError()
        {
            var result = Activity.Create(
                "Attività",
                null!,
                CreateValidImageUrl(),
                new List<RoutineAction> { CreateValidAction(Guid.NewGuid()) });

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.TextRequired");
        }

        [Fact]
        public void Create_WithNullImageUrl_ShouldReturnImageUrlRequiredError()
        {
            var result = Activity.Create(
                "Attività",
                CreateValidSpeechText(),
                null!,
                new List<RoutineAction> { CreateValidAction(Guid.NewGuid()) });

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ImageUrlRequired");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Create_WithNullOrEmptyActions_ShouldReturnActionsRequiredError(bool isNull)
        {
            var actions = isNull ? null : new List<RoutineAction>();

            var result = Activity.Create(
                "Attività",
                CreateValidSpeechText(),
                CreateValidImageUrl(),
                actions!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionsRequired");
        }

        [Fact]
        public void AddAction_WithMatchingActivityId_ShouldAddActionToList()
        {
            var activity = CreateValidActivity();
            var newAction = CreateValidAction(activity.Id, "Seconda Azione");

            var result = activity.AddAction(newAction);

            result.IsSuccess.Should().BeTrue();
            activity.Actions.Should().Contain(newAction);
        }

        [Fact]
        public void AddAction_WhenActionAlreadyExists_ShouldReturnActionAlreadyExistsError()
        {
            var activity = CreateValidActivity();
            var existingAction = activity.Actions.First();

            var result = activity.AddAction(existingAction);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionAlreadyExists");
        }

        [Fact]
        public void AddAction_WithMismatchedActivityId_ShouldReturnActionActivityMismatchError()
        {
            var activity = CreateValidActivity();
            var mismatchAction = CreateValidAction(Guid.NewGuid(), "Azione esterna");

            var result = activity.AddAction(mismatchAction);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionActivityMismatch");
        }

        [Fact]
        public void RemoveAction_WhenActionExists_ShouldCallRemoveOnAction()
        {
            var activity = CreateValidActivity();
            var action = activity.Actions.First();

            var result = activity.RemoveAction(action.Id);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void RemoveAction_WhenActionNotFound_ShouldReturnActionNotFoundError()
        {
            var activity = CreateValidActivity();

            var result = activity.RemoveAction(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionNotFound");
        }

        [Fact]
        public void MarkActionAsFinished_WhenActionExists_ShouldMarkActionAsFinished()
        {
            var activity = CreateValidActivity();
            var action = activity.Actions.First();

            var result = activity.MarkActionAsFinished(action.Id);

            result.IsSuccess.Should().BeTrue();
            action.IsFinished.Should().BeTrue();
        }

        [Fact]
        public void MarkActionAsFinished_WhenActionNotFound_ShouldReturnActionNotFoundError()
        {
            var activity = CreateValidActivity();

            var result = activity.MarkActionAsFinished(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionNotFound");
        }

        [Fact]
        public void IsFinished_WhenAllActionsAreFinished_ShouldBeTrue()
        {
            var activity = CreateValidActivity();

            foreach (var action in activity.Actions)
            {
                activity.MarkActionAsFinished(action.Id);
            }

            activity.IsFinished.Should().BeTrue();
        }

        [Fact]
        public void IsFinished_WhenSomeActionsAreNotFinished_ShouldBeFalse()
        {
            var activity = CreateValidActivity();

            activity.IsFinished.Should().BeFalse();
        }

        [Fact]
        public void SoftDelete_WhenNotDeleted_ShouldSetIsDeletedToTrue()
        {
            var activity = CreateValidActivity();

            var result = activity.SoftDelete();

            result.IsSuccess.Should().BeTrue();
            activity.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void SoftDelete_WhenAlreadyDeleted_ShouldReturnAlreadyDeletedError()
        {
            var activity = CreateValidActivity();
            activity.SoftDelete();

            var result = activity.SoftDelete();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.AlreadyDeleted");
        }

        [Fact]
        public void ChangeOrder_WithValidReorderedActions_ShouldUpdateOrder()
        {
            var action1 = CreateValidAction(Guid.NewGuid(), "Azione 1");
            var action2 = CreateValidAction(Guid.NewGuid(), "Azione 2");

            var activity = Activity.Create("Attività", CreateValidSpeechText(), CreateValidImageUrl(), new List<RoutineAction>
            {
                action1,
                action2
            }).Value;

            var reordered = new List<RoutineAction> { action2, action1 };
            var result = activity.ChangeOrder(reordered);

            result.IsSuccess.Should().BeTrue();
            activity.Actions.Should().ContainInConsecutiveOrder(action2, action1);
        }

        [Fact]
        public void ChangeOrder_WithDifferentElementsOrCount_ShouldReturnInvalidNewOrderError()
        {
            var activity = CreateValidActivity();
            var foreignAction = CreateValidAction(activity.Id, "Sconosciuta");

            var result = activity.ChangeOrder([foreignAction]);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.InvalidNewOrder");
        }
    }
}
