using System;
using System.Collections.Generic;
using System.Text;
using AlessandroGozzi.MioEco_Material.Domain.Interfaces;

namespace AlessandroGozzi.MioEco_Material.MobileGUI.Services
{
    public class TextToSpeechService : ITextToSpeechService
    {
        private Locale? _cachedItalianLocale;

        public async Task SpeakItalianAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            try
            {
                if (_cachedItalianLocale is null)
                {
                    var locales = await TextToSpeech.Default.GetLocalesAsync();
                    _cachedItalianLocale = locales.FirstOrDefault(l =>
                        l.Language.Equals("it", StringComparison.OrdinalIgnoreCase) ||
                        l.Country.Equals("IT", StringComparison.OrdinalIgnoreCase));
                }

                var options = new SpeechOptions
                {
                    Locale = _cachedItalianLocale,
                    Pitch = 1.0f,
                    Volume = 1.0f
                };

                await TextToSpeech.Default.SpeakAsync(text, options);
            }
            catch
            {
                // Fallback sulla sintesi standard se il locale non è disponibile
                await TextToSpeech.Default.SpeakAsync(text);
            }
        }
    }
}
