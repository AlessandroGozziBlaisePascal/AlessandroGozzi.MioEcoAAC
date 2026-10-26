using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Models;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.AppAAC.DomainTest.Entities
{
    public class RoutineActionTests
    {
        private readonly ImageUrl _defaultImageUrl;
        private readonly SpeechText _defaultSpeechText;

        public RoutineActionTests()
        {
            _defaultImageUrl = ImageUrl.Create("image.jpg").Value;
            _defaultSpeechText = SpeechText.Create("Testo audio azione").Value;
        }

        private RoutineAction CreateValidAction(string description = "Lavarsi le mani")
        {
            return RoutineAction.Create(_defaultImageUrl, _defaultSpeechText, description).Value;
        }

        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccess()
        {
            var result = RoutineAction.Create(_defaultImageUrl, _defaultSpeechText, "Prendi lo spazzolino");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Image.Should().Be(_defaultImageUrl);
            result.Value.Audio.Should().Be(_defaultSpeechText);
            result.Value.Name.Should().Be("Prendi lo spazzolino");
            result.Value.IsFinished.Should().BeFalse();
            result.Value.IsDeleted.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidDescription_ShouldReturnValidationError(string? invalidDescription)
        {
            var result = RoutineAction.Create(_defaultImageUrl, _defaultSpeechText, invalidDescription!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.InvalidDescription");
        }

        [Fact]
        public void Create_WithNullImage_ShouldReturnValidationError()
        {
            var result = RoutineAction.Create(null!, _defaultSpeechText, "Descrizione valida");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.InvalidImage");
        }

        [Fact]
        public void Create_WithNullAudio_ShouldReturnValidationError()
        {
            var result = RoutineAction.Create(_defaultImageUrl, null!, "Descrizione valida");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.InvalidAudio");
        }

        [Fact]
        public void MarkAsFinished_WhenNotFinished_ShouldSetIsFinishedToTrue()
        {
            var action = CreateValidAction();

            var result = action.MarkAsFinished();

            result.IsSuccess.Should().BeTrue();
            action.IsFinished.Should().BeTrue();
        }

        [Fact]
        public void MarkAsFinished_WhenAlreadyFinished_ShouldReturnFailure()
        {
            var action = CreateValidAction();
            action.MarkAsFinished();

            var result = action.MarkAsFinished();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.AlreadyFinished");
        }

        [Fact]
        public void Remove_WhenNotDeleted_ShouldSetIsDeletedToTrue()
        {
            var action = CreateValidAction();

            var result = action.Remove();

            result.IsSuccess.Should().BeTrue();
            action.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Remove_WhenAlreadyDeleted_ShouldReturnFailure()
        {
            var action = CreateValidAction();
            action.Remove();

            var result = action.Remove();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.AlreadyDeleted");
        }

        [Fact]
        public void Update_WhenActionIsDeleted_ShouldReturnFailure()
        {
            var action = CreateValidAction();
            action.Remove();

            var result = action.Update("Nuovo Nome", null, null);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.Deleted");
        }

        [Fact]
        public void Update_WithValidPartialValues_ShouldUpdateFields()
        {
            var action = CreateValidAction("Nome Iniziale");
            var newAudio = SpeechText.Create("Nuovo audio").Value;
            var newImage = ImageUrl.Create("https://example.com/new.png").Value;

            var result = action.Update("Nome Modificato", newAudio, newImage);

            result.IsSuccess.Should().BeTrue();
            action.Name.Should().Be("Nome Modificato");
            action.Audio.Should().Be(newAudio);
            action.Image.Should().Be(newImage);
        }

        [Fact]
        public void Update_WithNullValues_ShouldPreserveExistingFields()
        {
            var action = CreateValidAction("Nome Iniziale");
            var originalAudio = action.Audio;
            var originalImage = action.Image;

            var result = action.Update(null, null, null);

            result.IsSuccess.Should().BeTrue();
            action.Name.Should().Be("Nome Iniziale");
            action.Audio.Should().Be(originalAudio);
            action.Image.Should().Be(originalImage);
        }
    }
}
