using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.AppAAC.DomainTest.ValueObject
{
    public class SpeechTextTests
    {
        [Fact]
        public void Create_WithValidText_ShouldReturnSuccess()
        {
            var validText = "Testo da riprodurre vocalmente";

            var result = SpeechText.Create(validText);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Text.Should().Be(validText);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidText_ShouldReturnValidationError(string? invalidText)
        {
            var result = SpeechText.Create(invalidText!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("SpeechText.Invalid");
        }

        [Fact]
        public void Equals_WithSameText_ShouldBeEqual()
        {
            var text = "Ciao mondo";
            var first = SpeechText.Create(text).Value;
            var second = SpeechText.Create(text).Value;

            first.Should().Be(second);
        }
    }
}
