using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Models;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.MobileGUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlessandroGozzi.AppAAC.MobileGUI.ViewModels
{
    [QueryProperty(nameof(RoutineId), "RoutineId")]
    [QueryProperty(nameof(RoutineTitle), "RoutineTitle")]
    public partial class ActivityViewModel : ObservableObject
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly ITextToSpeechService _ttsService;

        public ActivityViewModel(IRoutineRepository routineRepository, ITextToSpeechService ttsService)
        {
            _routineRepository = routineRepository;
            _ttsService = ttsService;
        }

        private string _routineTitle = string.Empty;
        public string RoutineTitle
        {
            get => _routineTitle;
            set => SetProperty(ref _routineTitle, value);
        }

        private string _routineId = string.Empty;
        public string RoutineId
        {
            get => _routineId;
            set
            {
                if (SetProperty(ref _routineId, value) && Guid.TryParse(value, out var guid))
                {
                    _ = LoadActivitiesAsync(guid);
                }
            }
        }

        private ObservableCollection<Activity> _activities = new();
        public ObservableCollection<Activity> Activities
        {
            get => _activities;
            set => SetProperty(ref _activities, value);
        }

        // Attività selezionata dall'utente: pronuncia immediatamente lo SpeechText al tocco
        private Activity? _selectedActivity;
        public Activity? SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                if (SetProperty(ref _selectedActivity, value))
                {
                    ConfermaAttivitaCommand?.NotifyCanExecuteChanged();

                    if (value != null && !string.IsNullOrWhiteSpace(value.SpeechText))
                    {
                        _ = _ttsService.SpeakItalianAsync(value.SpeechText);
                    }
                }
            }
        }

        private async Task LoadActivitiesAsync(Guid routineId)
        {
            var list = await _routineRepository.GetActivitiesByRoutineIdAsync(routineId);
            Activities = new ObservableCollection<Activity>(list);
        }

        // Naviga solo quando l'utente preme il pulsante inferiore
        [RelayCommand(CanExecute = nameof(CanConfermaAttivita))]
        private async Task ConfermaAttivitaAsync()
        {
            if (SelectedActivity == null) return;

            await Shell.Current.GoToAsync("ActionsPage", new Dictionary<string, object>
        {
            { "ActivityId", SelectedActivity.Id.ToString() }
        });
        }

        private bool CanConfermaAttivita() => SelectedActivity != null;

        [RelayCommand]
        private async Task TornaIndietroAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
