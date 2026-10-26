using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Enums;
using AlessandroGozzi.AppAAC.Domain.Models;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.AppAAC.DomainTest.AggregateRoots
{
    public class RoutineTests
    {
        private readonly SpeechText _defaultText;
        private readonly RoutineType _defaultType = RoutineType.NormalDay;

        public RoutineTests()
        {
            _defaultText = SpeechText.Create("Testo di default").Value;
        }

        private Routine CreateValidRoutine(string name = "Routine Test")
        {
            return Routine.Create(name, _defaultText, _defaultType).Value;
        }

        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccess()
        {
            var result = Routine.Create("Routine Mattutina", _defaultText, _defaultType);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Routine Mattutina");
            result.Value.Text.Should().Be(_defaultText);
            result.Value.Type.Should().Be(_defaultType);
            result.Value.IsDeleted.Should().BeFalse();
            result.Value.Activities.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ShouldReturnValidationError(string? invalidName)
        {
            var result = Routine.Create(invalidName!, _defaultText, _defaultType);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.NameRequired");
        }

        [Fact]
        public void Create_WithNullSpeechText_ShouldReturnValidationError()
        {
            var result = Routine.Create("Routine Test", null!, _defaultType);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.TextRequired");
        }

        [Fact]
        public void Remove_WhenNotDeleted_ShouldSetIsDeletedToTrue()
        {
            var routine = CreateValidRoutine();

            var result = routine.Remove();

            result.IsSuccess.Should().BeTrue();
            routine.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Remove_WhenAlreadyDeleted_ShouldReturnFailure()
        {
            var routine = CreateValidRoutine();
            routine.Remove();

            var result = routine.Remove();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.AlreadyDeleted");
        }

        [Fact]
        public void AddActivity_WhenActivityIsNew_ShouldAddSuccessfully()
        {
            var routine = CreateValidRoutine();
            var activity = CreateFakeActivity();

            var result = routine.AddActivity(activity);

            result.IsSuccess.Should().BeTrue();
            routine.Activities.Should().ContainSingle().Which.Should().Be(activity);
        }

        [Fact]
        public void AddActivity_WhenActivityAlreadyPresent_ShouldReturnValidationError()
        {
            var routine = CreateValidRoutine();
            var activity = CreateFakeActivity();
            routine.AddActivity(activity);

            var result = routine.AddActivity(activity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.ActivityAlreadyExists");
        }

        [Fact]
        public void RemoveActivity_WhenActivityNotFound_ShouldReturnNotFound()
        {
            var routine = CreateValidRoutine();

            var result = routine.RemoveActivity(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.ActivityNotFound");
        }

        [Fact]
        public void ChangeActivityOrder_WithValidNewOrder_ShouldUpdateOrder()
        {
            var routine = CreateValidRoutine();
            var act1 = CreateFakeActivity("Attività 1");
            var act2 = CreateFakeActivity("Attività 2");
            routine.AddActivity(act1);
            routine.AddActivity(act2);

            var newOrder = new List<Activity> { act2, act1 };

            var result = routine.ChangeActivityOrder(newOrder);

            result.IsSuccess.Should().BeTrue();
            routine.Activities.Should().ContainInOrder(act2, act1);
        }

        [Fact]
        public void ChangeActivityOrder_WithDifferentItems_ShouldReturnValidationError()
        {
            var routine = CreateValidRoutine();
            var act1 = CreateFakeActivity("Attività 1");
            routine.AddActivity(act1);

            var unknownActivity = CreateFakeActivity("Sconosciuta");
            var newOrder = new List<Activity> { unknownActivity };

            var result = routine.ChangeActivityOrder(newOrder);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.InvalidActivityOrder");
        }

        [Fact]
        public void Update_WithValidPartialValues_ShouldUpdateFields()
        {
            var routine = CreateValidRoutine("Vecchio Nome");
            var newText = SpeechText.Create("Nuovo testo vocale").Value;

            var result = routine.Update("Nuovo Nome", newText);

            result.IsSuccess.Should().BeTrue();
            routine.Name.Should().Be("Nuovo Nome");
            routine.Text.Should().Be(newText);
        }

        [Fact]
        public void Update_WithNullValues_ShouldPreserveExistingFields()
        {
            var routine = CreateValidRoutine("Nome Originale");
            var originalText = routine.Text;

            var result = routine.Update(null, null);

            result.IsSuccess.Should().BeTrue();
            routine.Name.Should().Be("Nome Originale");
            routine.Text.Should().Be(originalText);
        }

        [Fact]
        public void Update_WhenRoutineIsDeleted_ShouldReturnFailure()
        {
            var routine = CreateValidRoutine();
            routine.Remove();

            var result = routine.Update("Nuovo Nome", null);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Routine.Deleted");
        }

        private Activity CreateFakeActivity(string name = "Attività Default")
        {
            var url = ImageUrl.Create("example.jpg").Value;
            var text = SpeechText.Create("Testo attività").Value;
            var action = RoutineAction.Create(
                url,
                text,
                "Descrizione"
            ).Value;

            return Activity.Create(name, text, url, new List<RoutineAction> { action }).Value;
        }
    }
}
