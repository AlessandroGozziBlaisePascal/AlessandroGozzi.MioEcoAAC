using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.SharedKernel;

namespace AlessandroGozzi.AppAAC.Domain.ValueObjects
{
    public record SpeechText
    {
        public string Text { get; init; }

        private SpeechText() { }// For EF Core

        private SpeechText(string text)
        {
            Text = text;
        }

        public static Result<SpeechText> Create(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Result.Failure<SpeechText>(Error.Validation("SpeechText.Invalid", "The speech text cannot be null or empty."));
            return Result.Success(new SpeechText(text));
        }
    }
}
