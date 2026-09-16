using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppCAA.Domain.ValueObjects;

namespace AlessandroGozzi.AppAutistici.Application.Mappers
{
    public static class SpeechTextMapper
    {
        public static Result<SpeechText> ToSpeechTextDomain(this string speechText) => SpeechText.Create(speechText);
    }
}
