using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.SharedKernel;

namespace AlessandroGozzi.AppAAC.Domain.ValueObjects
{
    public record ImageUrl
    {
        public string Value { get; init; }

        private ImageUrl() { } // For EF Core
        private ImageUrl(string value)
        {
            Value = value;
        }

        public static Result<ImageUrl> Create(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<ImageUrl>(Error.Validation("ImageUrl.InvalidURL", "The image URL cannot be null or empty."));
            }

            return Result.Success(new ImageUrl(value));
        }

        public override string ToString() => Value;
    }
}
