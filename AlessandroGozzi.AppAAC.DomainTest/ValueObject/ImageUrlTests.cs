using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.AppAAC.DomainTest.ValueObject
{
    public class ImageUrlTests
    {
        [Fact]
        public void Create_WithValidUrl_ShouldReturnSuccess()
        {
            var validUrl = "https://example.com/image.png";

            var result = ImageUrl.Create(validUrl);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Value.Should().Be(validUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidUrl_ShouldReturnValidationError(string? invalidUrl)
        {
            var result = ImageUrl.Create(invalidUrl!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("ImageUrl.InvalidURL");
        }

        [Fact]
        public void ToString_ShouldReturnContainedValue()
        {
            var url = "https://example.com/image.png";
            var imageUrl = ImageUrl.Create(url).Value;

            var stringValue = imageUrl.ToString();

            stringValue.Should().Be(url);
        }

        [Fact]
        public void Equals_WithSameValue_ShouldBeEqual()
        {
            var url = "https://example.com/image.png";
            var first = ImageUrl.Create(url).Value;
            var second = ImageUrl.Create(url).Value;

            first.Should().Be(second);
        }
    }
}
