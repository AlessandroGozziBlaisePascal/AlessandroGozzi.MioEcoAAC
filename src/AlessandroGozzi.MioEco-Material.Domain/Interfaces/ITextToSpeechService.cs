using System;
using System.Collections.Generic;
using System.Text;

namespace AlessandroGozzi.MioEco_Material.Domain.Interfaces
{
    public interface ITextToSpeechService
    {
        Task SpeakItalianAsync(string text);
    }
}
