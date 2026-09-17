using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using AlessandroGozzi.AppAAC.SharedKernel;

namespace AlessandroGozzi.AppAAC.Application.Mappers
{
    public static class SpeechTextMapper
    {
        public static Result<SpeechText> ToSpeechTextDomain(this string speechText) => SpeechText.Create(speechText);
    }
}
