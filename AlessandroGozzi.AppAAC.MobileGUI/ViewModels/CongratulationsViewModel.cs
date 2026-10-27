using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.MobileGUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlessandroGozzi.AppAAC.MobileGUI.ViewModels
{
    public partial class CongratulationsViewModel : ObservableObject
    {
        private readonly ITextToSpeechService _ttsService;

        public CongratulationsViewModel(ITextToSpeechService ttsService)
        {
            _ttsService = ttsService;
        }

        public async Task PronunciaMessaggioAsync()
        {
            await _ttsService.SpeakItalianAsync("Complimenti! Hai finito l'attività!");
        }

        [RelayCommand]
        private async Task ChiudiSchermataAsync()
        {
            await Shell.Current.GoToAsync("../..");
        }
    }
}
