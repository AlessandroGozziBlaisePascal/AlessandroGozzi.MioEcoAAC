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
    public class RoutineActionTests
    {
        private static ImageUrl CreateValidImage() =>
            ImageUrl.Create("https://example.com/action.png").Value;

        private static SpeechText CreateValidAudio() =>
            SpeechText.Create("Istruzione vocale").Value;

        private static RoutineAction CreateValidRoutineAction(Guid? activityId = null) =>
            RoutineAction.Create(
                activityId ?? Guid.NewGuid(),
                CreateValidImage(),
                CreateValidAudio(),
                "Descrizione valida"
            ).Value;

        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccessAndSetProperties()
        {
            var activityId = Guid.NewGuid();
            var image = CreateValidImage();
            var audio = CreateValidAudio();
            var description = "Lava le mani";

            var result = RoutineAction.Create(activityId, image, audio, description);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ActivityId.Should().Be(activityId);
            result.Value.Image.Should().Be(image);
            result.Value.Audio.Should().Be(audio);
            result.Value.Description.Should().Be(description);
            result.Value.IsFinished.Should().BeFalse();
            result.Value.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void Create_WithEmptyActivityId_ShouldReturnInvalidActivityIdError()
        {
            var result = RoutineAction.Create(
                Guid.Empty,
                CreateValidImage(),
                CreateValidAudio(),
                "Descrizione"
            );

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.InvalidActivityId");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidDescription_ShouldReturnInvalidDescriptionError(string? invalidDescription)
        {
            var result = RoutineAction.Create(
                Guid.NewGuid(),
                CreateValidImage(),
                CreateValidAudio(),
                invalidDescription!
            );

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.InvalidDescription");
        }

        [Fact]
        public void Create_WithNullImage_ShouldReturnInvalidImageError()
        {
            var result = RoutineAction.Create(
                Guid.NewGuid(),
                null!,
                CreateValidAudio(),
                "Descrizione"
            );

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.InvalidImage");
        }

        [Fact]
        public void Create_WithNullAudio_ShouldReturnInvalidAudioError()
        {
            var result = RoutineAction.Create(
                Guid.NewGuid(),
                CreateValidImage(),
                null!,
                "Descrizione"
            );

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.InvalidAudio");
        }

        [Fact]
        public void MarkAsFinished_WhenNotFinished_ShouldSetIsFinishedToTrue()
        {
            var action = CreateValidRoutineAction();

            var result = action.MarkAsFinished();

            result.IsSuccess.Should().BeTrue();
            action.IsFinished.Should().BeTrue();
        }

        [Fact]
        public void MarkAsFinished_WhenAlreadyFinished_ShouldReturnAlreadyFinishedError()
        {
            var action = CreateValidRoutineAction();
            action.MarkAsFinished();

            var result = action.MarkAsFinished();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.AlreadyFinished");
        }

        [Fact]
        public void Remove_WhenNotDeleted_ShouldSetIsDeletedToTrue()
        {
            var action = CreateValidRoutineAction();

            var result = action.Remove();

            result.IsSuccess.Should().BeTrue();
            action.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Remove_WhenAlreadyDeleted_ShouldReturnAlreadyDeletedError()
        {
            var action = CreateValidRoutineAction();
            action.Remove();

            var result = action.Remove();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("RoutineAction.AlreadyDeleted");
        }
    }
}
