using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.AggregateRoots;
using AlessandroGozzi.AppAAC.Domain.Entities;
using AlessandroGozzi.AppAAC.Domain.Enums;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.AppAAC.DomainTest.AggregateRoots
{
    public class RoutineTests
    {
        private static Routine CreateValidRoutine()
        {
            var text = SpeechText.Create("Test speech text").Value;
            return Routine.Create("Mattina", text, RoutineType.NormalDay).Value;
        }

        private static RoutineAction CreateValidAction(Guid activityId, string name = "Sample Action")  =>
            RoutineAction.Create(activityId, CreateValidImageUrl(), CreateValidSpeechText(), name).Value;

        private static SpeechText CreateValidSpeechText() =>
            SpeechText.Create("Sample text").Value;

        private static ImageUrl CreateValidImageUrl() =>
            ImageUrl.Create("example.png").Value;

        private static Activity CreateValidActivity(Guid Id, string name = "Sample Activity") =>
            Activity.Create(name, CreateValidSpeechText(), CreateValidImageUrl(), 
                new List<RoutineAction> { CreateValidAction(Id), CreateValidAction(Id, "Lava denti"), CreateValidAction(Id, "Mangia") }).Value;

        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccessAndSetProperties()
        {
            var name = "Routine Sera";
            var text = CreateValidSpeechText();
            var type = RoutineType.NormalDay;

            var result = Routine.Create(name, text, type);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(name);
            result.Value.Text.Should().Be(text);
            result.Value.Type.Should().Be(type);
            result.Value.IsDeleted.Should().BeFalse();
            result.Value.Activities.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithEmptyOrWhitespaceName_ShouldReturnNameRequiredError(string? invalidName)
        {
            var text = CreateValidSpeechText();

            var result = Routine.Create(invalidName!, text, RoutineType.NormalDay);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.NameRequired");
        }

        [Fact]
        public void Create_WithNullSpeechText_ShouldReturnTextRequiredError()
        {
            var result = Routine.Create("Routine Pomeriggio", null!, RoutineType.NormalDay);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.TextRequired");
        }

        [Fact]
        public void Remove_WhenActive_ShouldSetIsDeletedToTrue()
        {
            var routine = CreateValidRoutine();

            var result = routine.Remove();

            result.IsSuccess.Should().BeTrue();
            routine.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Remove_WhenAlreadyDeleted_ShouldReturnAlreadyDeletedError()
        {
            var routine = CreateValidRoutine();
            routine.Remove();

            var result = routine.Remove();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.AlreadyDeleted");
        }

        [Fact]
        public void AddActivity_WhenNew_ShouldAddActivityToList()
        {
            var routine = CreateValidRoutine();
            var activity = CreateValidActivity(Guid.NewGuid());

            var result = routine.AddActivity(activity);

            result.IsSuccess.Should().BeTrue();
            routine.Activities.Should().ContainSingle()
                .Which.Should().Be(activity);
        }

        [Fact]
        public void AddActivity_WhenAlreadyPresent_ShouldReturnActivityAlreadyExistsError()
        {
            var routine = CreateValidRoutine();
            var activity = CreateValidActivity(Guid.NewGuid());
            routine.AddActivity(activity);

            var result = routine.AddActivity(activity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.ActivityAlreadyExists");
        }

        [Fact]
        public void RemoveActivity_WhenActivityExists_ShouldReturnSuccess()
        {
            var routine = CreateValidRoutine();
            var activity = CreateValidActivity(Guid.NewGuid());
            routine.AddActivity(activity);

            var result = routine.RemoveActivity(activity.Id);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void RemoveActivity_WhenActivityNotFound_ShouldReturnActivityNotFoundError()
        {
            var routine = CreateValidRoutine();

            var result = routine.RemoveActivity(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.ActivityNotFound");
        }

        [Fact]
        public void ChangeActivityOrder_WithValidReorderedList_ShouldUpdateOrder()
        {
            var routine = CreateValidRoutine();
            var act1 = CreateValidActivity(Guid.NewGuid());
            var act2 = CreateValidActivity(Guid.NewGuid(), "Dormire");
            routine.AddActivity(act1);
            routine.AddActivity(act2);

            var newOrder = new List<Activity> { act2, act1 };

            var result = routine.ChangeActivityOrder(newOrder);

            result.IsSuccess.Should().BeTrue();
            routine.Activities.Should().ContainInConsecutiveOrder(act2, act1);
        }

        [Fact]
        public void ChangeActivityOrder_WithDifferentCountOrElements_ShouldReturnInvalidOrderError()
        {
            var routine = CreateValidRoutine();
            var act1 = CreateValidActivity(Guid.NewGuid(), "Attività 1");
            routine.AddActivity(act1);

            var foreignActivity = CreateValidActivity(Guid.NewGuid(), "Attività sconosciuta");
            var invalidOrder = new List<Activity> { foreignActivity };

            var result = routine.ChangeActivityOrder(invalidOrder);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().BeOneOf("Routine.InvalidActivityOrder", "Routine.InvalidNewOrder");
        }

        [Fact]
        public void ChangeActionOrderInActivity_WhenActivityNotFound_ShouldReturnNotFoundError()
        {
            var routine = CreateValidRoutine();

            var result = routine.ChangeActionOrderInActivity(Guid.NewGuid(), new List<RoutineAction>());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.ActivityNotFound");
        }
    }
}
