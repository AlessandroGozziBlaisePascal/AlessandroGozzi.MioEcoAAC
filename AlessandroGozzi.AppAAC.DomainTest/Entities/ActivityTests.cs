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
        private readonly SpeechText _defaultText;
        private readonly ImageUrl _defaultImageUrl;

        public ActivityTests()
        {
            _defaultText = SpeechText.Create("Testo di default").Value;
            _defaultImageUrl = ImageUrl.Create("example.jpg").Value;
        }

        private RoutineAction CreateFakeAction(string description = "Azione default")
        {
            return RoutineAction.Create(
                _defaultImageUrl,
                _defaultText,
                description
            ).Value;
        }

        private Activity CreateValidActivity(string name = "Attività Test", List<RoutineAction>? actions = null)
        {
            var actionList = actions ?? new List<RoutineAction> { CreateFakeAction() };
            return Activity.Create(name, _defaultText, _defaultImageUrl, actionList).Value;
        }

        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccess()
        {
            var action = CreateFakeAction();
            var actions = new List<RoutineAction> { action };

            var result = Activity.Create("Lavarsi i denti", _defaultText, _defaultImageUrl, actions);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Lavarsi i denti");
            result.Value.Text.Should().Be(_defaultText);
            result.Value.ImageUrl.Should().Be(_defaultImageUrl);
            result.Value.IsDeleted.Should().BeFalse();
            result.Value.Actions.Should().ContainSingle().Which.Should().Be(action);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ShouldReturnValidationError(string? invalidName)
        {
            var actions = new List<RoutineAction> { CreateFakeAction() };

            var result = Activity.Create(invalidName!, _defaultText, _defaultImageUrl, actions);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.NameRequired");
        }

        [Fact]
        public void Create_WithNullSpeechText_ShouldReturnValidationError()
        {
            var actions = new List<RoutineAction> { CreateFakeAction() };

            var result = Activity.Create("Attività", null!, _defaultImageUrl, actions);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.TextRequired");
        }

        [Fact]
        public void Create_WithNullImageUrl_ShouldReturnValidationError()
        {
            var actions = new List<RoutineAction> { CreateFakeAction() };

            var result = Activity.Create("Attività", _defaultText, null!, actions);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ImageUrlRequired");
        }

        [Fact]
        public void Create_WithNullActionsList_ShouldReturnValidationError()
        {
            var result = Activity.Create("Attività", _defaultText, _defaultImageUrl, null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionsRequired");
        }

        [Fact]
        public void Create_WithEmptyActionsList_ShouldReturnValidationError()
        {
            var emptyActions = new List<RoutineAction>();

            var result = Activity.Create("Attività", _defaultText, _defaultImageUrl, emptyActions);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionsRequired");
        }

        [Fact]
        public void AddAction_WhenActionIsNew_ShouldAddSuccessfully()
        {
            var activity = CreateValidActivity();
            var newAction = CreateFakeAction("Nuova Azione");

            var result = activity.AddAction(newAction);

            result.IsSuccess.Should().BeTrue();
            activity.Actions.Should().Contain(newAction);
        }

        [Fact]
        public void AddAction_WhenActionAlreadyExists_ShouldReturnValidationError()
        {
            var action = CreateFakeAction();
            var activity = CreateValidActivity("Attività", new List<RoutineAction> { action });

            var result = activity.AddAction(action);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionAlreadyExists");
        }

        [Fact]
        public void RemoveAction_WhenActionNotFound_ShouldReturnNotFound()
        {
            var activity = CreateValidActivity();

            var result = activity.RemoveAction(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionNotFound");
        }

        [Fact]
        public void MarkActionAsFinished_WhenActionNotFound_ShouldReturnNotFound()
        {
            var activity = CreateValidActivity();

            var result = activity.MarkActionAsFinished(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionNotFound");
        }

        [Fact]
        public void MarkActivityAsFinished_WhenNotAllActionsAreFinished_ShouldReturnValidationError()
        {
            var activity = CreateValidActivity();

            var result = activity.MarkActivityAsFinished();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.NotAllActionsFinished");
        }

        [Fact]
        public void MarkActivityAsFinished_WhenAlreadyFinished_ShouldReturnValidationError()
        {
            var action = CreateFakeAction();
            action.MarkAsFinished();
            var activity = CreateValidActivity("Attività", new List<RoutineAction> { action });

            var result = activity.MarkActivityAsFinished();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.AlreadyFinished");
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
        public void SoftDelete_WhenAlreadyDeleted_ShouldReturnValidationError()
        {
            var activity = CreateValidActivity();
            activity.SoftDelete();

            var result = activity.SoftDelete();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.AlreadyDeleted");
        }

        [Fact]
        public void ChangeOrder_WithValidNewOrder_ShouldUpdateActionsOrder()
        {
            var action1 = CreateFakeAction("Azione 1");
            var action2 = CreateFakeAction("Azione 2");
            var activity = CreateValidActivity("Attività", new List<RoutineAction> { action1, action2 });

            var newOrder = new List<RoutineAction> { action2, action1 };

            var result = activity.ChangeOrder(newOrder);

            result.IsSuccess.Should().BeTrue();
            activity.Actions.Should().ContainInOrder(action2, action1);
        }

        [Fact]
        public void ChangeOrder_WithCountMismatch_ShouldReturnValidationError()
        {
            var action1 = CreateFakeAction("Azione 1");
            var action2 = CreateFakeAction("Azione 2");
            var activity = CreateValidActivity("Attività", new List<RoutineAction> { action1, action2 });

            var incompleteOrder = new List<RoutineAction> { action1 };

            var result = activity.ChangeOrder(incompleteOrder);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.InvalidNewOrder");
        }

        [Fact]
        public void ChangeOrder_WithDifferentActions_ShouldReturnValidationError()
        {
            var action1 = CreateFakeAction("Azione 1");
            var activity = CreateValidActivity("Attività", new List<RoutineAction> { action1 });

            var differentAction = CreateFakeAction("Azione Esterna");
            var newOrder = new List<RoutineAction> { differentAction };

            var result = activity.ChangeOrder(newOrder);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.InvalidNewOrder");
        }

        [Fact]
        public void Update_WhenActivityIsDeleted_ShouldReturnValidationError()
        {
            var activity = CreateValidActivity();
            activity.SoftDelete();

            var result = activity.Update("Nuovo Nome", null, null);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.Deleted");
        }

        [Fact]
        public void Update_WithValidPartialValues_ShouldUpdateFields()
        {
            var activity = CreateValidActivity("Vecchio Nome");
            var newText = SpeechText.Create("Nuovo Testo").Value;
            var newUrl = ImageUrl.Create("https://example.com/new.png").Value;

            var result = activity.Update("Nuovo Nome", newText, newUrl);

            result.IsSuccess.Should().BeTrue();
            activity.Name.Should().Be("Nuovo Nome");
            activity.Text.Should().Be(newText);
            activity.ImageUrl.Should().Be(newUrl);
        }

        [Fact]
        public void Update_WithNullValues_ShouldPreserveExistingFields()
        {
            var activity = CreateValidActivity("Nome Originale");
            var originalText = activity.Text;
            var originalUrl = activity.ImageUrl;

            var result = activity.Update(null, null, null);

            result.IsSuccess.Should().BeTrue();
            activity.Name.Should().Be("Nome Originale");
            activity.Text.Should().Be(originalText);
            activity.ImageUrl.Should().Be(originalUrl);
        }

        [Fact]
        public void UpdateAction_WhenActionNotFound_ShouldReturnNotFound()
        {
            var activity = CreateValidActivity();

            var result = activity.UpdateAction(Guid.NewGuid(), "Nome", null, null);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Activity.ActionNotFound");
        }
    }
}
