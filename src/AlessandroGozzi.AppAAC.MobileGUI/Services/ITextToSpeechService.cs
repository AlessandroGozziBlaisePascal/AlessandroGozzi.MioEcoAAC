using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAAC.MobileGUI.Services
{
    public interface ITextToSpeechService
    {
        Task SpeakItalianAsync(string text);
    }
}
