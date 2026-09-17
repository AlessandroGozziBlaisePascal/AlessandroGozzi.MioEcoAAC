using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.AppAAC.DomainTest.ValueObjects
{
    public class ValueObjectsTests
    {
        #region IMAGE URL TESTS
        public class ImageUrlTests
        {
            [Fact]
            public void Create_WithValidUrl_ShouldReturnSuccessAndSetValue()
            {
                var validUrl = "example.jpg";

                var result = ImageUrl.Create(validUrl);

                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
                result.Value.Value.Should().Be(validUrl);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_WithNullOrWhitespace_ShouldReturnInvalidUrlError(string? invalidUrl)
            {
                var result = ImageUrl.Create(invalidUrl!);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("ImageUrl.InvalidURL");
            }

            [Fact]
            public void ToString_ShouldReturnValue()
            {
                var validUrl = "example.jpg";
                var imageUrl = ImageUrl.Create(validUrl).Value;

                var result = imageUrl.ToString();

                result.Should().Be(validUrl);
            }

            [Fact]
            public void Equals_WithSameValue_ShouldBeTrue()
            {
                var url1 = ImageUrl.Create("example.jpg").Value;
                var url2 = ImageUrl.Create("example.jpg").Value;

                (url1 == url2).Should().BeTrue();
                url1.Equals(url2).Should().BeTrue();
            }
        }
        #endregion
        #region SPEECH TEXT TESTS
        public class SpeechTextTests
        {
            [Fact]
            public void Create_WithValidText_ShouldReturnSuccessAndSetText()
            {
                var validText = "Istruzione vocale";

                var result = SpeechText.Create(validText);

                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
                result.Value.Text.Should().Be(validText);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_WithNullOrWhitespace_ShouldReturnInvalidError(string? invalidText)
            {
                var result = SpeechText.Create(invalidText!);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("SpeechText.Invalid");
            }

            [Fact]
            public void Equals_WithSameText_ShouldBeTrue()
            {
                var speech1 = SpeechText.Create("Ciao").Value;
                var speech2 = SpeechText.Create("Ciao").Value;

                (speech1 == speech2).Should().BeTrue();
                speech1.Equals(speech2).Should().BeTrue();
            }
        }
        #endregion
    }
}
