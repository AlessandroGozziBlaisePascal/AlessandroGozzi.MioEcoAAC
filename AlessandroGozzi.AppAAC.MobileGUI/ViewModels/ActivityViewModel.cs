using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Interfaces;
using AlessandroGozzi.AppAAC.Domain.Models;
using AlessandroGozzi.AppAAC.MobileGUI.Pages;
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

        // Card segnaposto per il "+" all'interno della griglia
        private readonly Activity _addPlaceholderActivity = new()
        {
            Id = Guid.Empty,
            Name = "Aggiungi attività",
            IsAddButton = true
        };

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

        public static bool NeedsRefresh { get; set; } = false;

        private ObservableCollection<Activity> _activities = new();
        public ObservableCollection<Activity> Activities
        {
            get => _activities;
            set => SetProperty(ref _activities, value);
        }

        private Activity? _selectedActivity;
        public Activity? SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                if (SetProperty(ref _selectedActivity, value))
                {
                    ConfermaAttivitaCommand.NotifyCanExecuteChanged();

                    if (value != null && !value.IsAddButton && !string.IsNullOrWhiteSpace(value.SpeechText))
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _ttsService.SpeakItalianAsync(value.SpeechText);
                            }
                            catch
                            {
                                // Fallback TTS silenzioso
                            }
                        });
                    }
                }
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditButtonText))]
        private bool _isEditMode;

        public string EditButtonText => IsEditMode ? "Fine" : "Modifica";

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ActivityViewModel(IRoutineRepository routineRepository, ITextToSpeechService ttsService)
        {
            _routineRepository = routineRepository;
            _ttsService = ttsService;
        }

        [RelayCommand]
        public async Task LoadActivitiesAsync(Guid routineGuid)
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var items = await _routineRepository.GetActivitiesByRoutineIdAsync(routineGuid);

                Activities.Clear();
                foreach (var item in items.Where(a => !a.IsDeleted).OrderBy(a => a.Order))
                {
                    item.IsSelected = false;
                    Activities.Add(item);
                }

                if (IsEditMode && !Activities.Contains(_addPlaceholderActivity))
                {
                    Activities.Add(_addPlaceholderActivity);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;

            if (IsEditMode)
            {
                if (!Activities.Contains(_addPlaceholderActivity))
                {
                    Activities.Add(_addPlaceholderActivity);
                }
            }
            else
            {
                if (Activities.Contains(_addPlaceholderActivity))
                {
                    Activities.Remove(_addPlaceholderActivity);
                }

                foreach (var act in Activities)
                {
                    act.IsSelected = false;
                }

                SelectedActivity = null;
            }
        }

        [RelayCommand]
        public void SelectActivity(Activity? activity)
        {
            if (activity is null || activity.IsAddButton) return;

            foreach (var act in Activities)
            {
                act.IsSelected = false;
            }

            activity.IsSelected = true;
            SelectedActivity = activity;
        }

        [RelayCommand(CanExecute = nameof(CanConfermaAttivita))]
        private async Task ConfermaAttivitaAsync()
        {
            if (SelectedActivity == null || SelectedActivity.IsAddButton) return;

            await Shell.Current.GoToAsync("ActionsPage", new Dictionary<string, object>
        {
            { "ActivityId", SelectedActivity.Id.ToString() },
            { "ActivityTitle", SelectedActivity.Name }
        });
        }

        private bool CanConfermaAttivita() => SelectedActivity != null && !SelectedActivity.IsAddButton;

        [RelayCommand]
        public async Task EliminaAttivitaAsync(Activity? activity)
        {
            var target = activity ?? SelectedActivity;

            if (target is null || target.IsAddButton)
            {
                await Shell.Current.DisplayAlert("Attenzione", "Seleziona prima l'attività da eliminare toccando la relativa card.", "OK");
                return;
            }

            bool conferma = await Shell.Current.DisplayAlert(
                "Elimina Attività",
                $"Vuoi davvero eliminare '{target.Name}'?",
                "Elimina",
                "Annulla");

            if (conferma)
            {
                try
                {
                    target.IsDeleted = true;

                    if (Guid.TryParse(RoutineId, out var rId))
                    {
                        await _routineRepository.SaveActivityAsync(rId, target);
                    }

                    Activities.Remove(target);
                    SelectedActivity = null;
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Errore", $"Impossibile eliminare l'attività: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        public async Task AggiungiAttivitaAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddActivityPage), new Dictionary<string, object>
        {
            { "RoutineId", RoutineId }
        });
        }

        [RelayCommand]
        private async Task TornaIndietroAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
