using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AlessandroGozzi.AppAAC.Domain.Interfaces;
using AlessandroGozzi.AppAAC.Domain.Models;
using AlessandroGozzi.AppAAC.MobileGUI.Pages;
using AlessandroGozzi.AppAAC.MobileGUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AlessandroGozzi.AppAAC.MobileGUI.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly ITextToSpeechService _ttsService;

        private readonly Routine _addPlaceholderRoutine = new()
        {
            Id = Guid.Empty,
            Name = "Aggiungi routine",
            IsAddButton = true
        };

        private ObservableCollection<Routine> _routines = new();
        public ObservableCollection<Routine> Routines
        {
            get => _routines;
            set => SetProperty(ref _routines, value);
        }

        public static bool NeedsRefresh { get; set; } = false;

        private Routine? _selectedRoutine;
        public Routine? SelectedRoutine
        {
            get => _selectedRoutine;
            set
            {
                if (SetProperty(ref _selectedRoutine, value) && value != null)
                {
                    // Non avviare la sintesi vocale se viene toccata la card speciale di aggiunta
                    if (value.IsAddButton) return;

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _ttsService.SpeakItalianAsync(value.Name);
                        }
                        catch
                        {
                            // Fallback silenzioso per TextToSpeech
                        }
                    });
                }
            }
        }

        [ObservableProperty] private bool _isParentalGateOpen;
        [ObservableProperty] private double _holdProgress;
        [ObservableProperty] private string _holdButtonText = "Tieni premuto per 3s";

        [RelayCommand] public void OpenParentalGate() => IsParentalGateOpen = true;
        [RelayCommand] public void CloseParentalGate() { IsParentalGateOpen = false; HoldProgress = 0; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditButtonText))]
        [NotifyPropertyChangedFor(nameof(SelectedCardStroke))]
        [NotifyPropertyChangedFor(nameof(SelectedCardBackground))]
        private bool _isEditMode;

        public string EditButtonText => IsEditMode ? "✓ Fine" : "Modifica";

        // Colori sincronizzati per la card selezionata
        public Color SelectedCardStroke => IsEditMode ? Color.FromArgb("#EF4444") : Color.FromArgb("#0284C7");
        public Color SelectedCardBackground => IsEditMode ? Color.FromArgb("#FEE2E2") : Color.FromArgb("#E0F2FE");

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public HomeViewModel(IRoutineRepository routineRepository, ITextToSpeechService service)
        {
            _routineRepository = routineRepository;
            _ttsService = service;
        }

        [RelayCommand]
        public void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;

            if (IsEditMode)
            {
                if (!Routines.Contains(_addPlaceholderRoutine))
                {
                    Routines.Add(_addPlaceholderRoutine);
                }
            }
            else
            {
                if (Routines.Contains(_addPlaceholderRoutine))
                {
                    Routines.Remove(_addPlaceholderRoutine);
                }

                foreach (var routine in Routines)
                {
                    routine.IsSelected = false;
                }

                SelectedRoutine = null;
            }
        }

        [RelayCommand]
        public async Task LoadRoutinesAsync()
        {
            if (_isLoading) return;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var items = await _routineRepository.GetAllAvailableAsync();

                _routines.Clear();
                foreach (var item in items)
                {
                    _routines.Add(item);
                }

                // Se ricarica mentre la modalità modifica è attiva, riaccoda il "+"
                if (IsEditMode && !_routines.Contains(_addPlaceholderRoutine))
                {
                    _routines.Add(_addPlaceholderRoutine);
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
        public async Task LoadRoutineActionsAsync()
        {
            if (_selectedRoutine == null || _selectedRoutine.IsAddButton) return;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                await Shell.Current.GoToAsync(nameof(ActivityPage), new Dictionary<string, object>
            {
                { "RoutineId", _selectedRoutine.Id.ToString() },
                { "RoutineTitle", _selectedRoutine.Name }
            });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Errore nel caricamento delle attività: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task EliminaRoutineAsync(Routine? routine)
        {
            var target = routine ?? SelectedRoutine;

            if (target is null || target.IsAddButton)
            {
                await Shell.Current.DisplayAlert("Attenzione", "Seleziona prima la routine da eliminare toccando la relativa card.", "OK");
                return;
            }

            if (target.IsSystem || target.Name == "Giornata normale" || target.Name == "Fine settimana")
            {
                await Shell.Current.DisplayAlert("Protetta", "Questa routine è di sistema e non può essere eliminata.", "OK");
                return;
            }

            bool conferma = await Shell.Current.DisplayAlert(
                "Elimina Routine",
                $"Vuoi davvero eliminare '{target.Name}'?",
                "Elimina",
                "Annulla");

            if (conferma)
            {
                try
                {
                    target.IsDeleted = true;
                    await _routineRepository.DeleteRoutineAsync(target.Id);
                    Routines.Remove(target);
                    SelectedRoutine = null;
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Errore", $"Impossibile eliminare: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        public async Task AggiungiRoutineAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddRoutinePage));
        }

        [RelayCommand]
        public void SelectRoutine(Routine? routine)
        {
            if (routine is null || routine.IsAddButton) return;

            // Deseleziona tutte le altre
            foreach (var r in Routines)
            {
                r.IsSelected = false;
            }

            // Seleziona quella cliccata
            routine.IsSelected = true;
            SelectedRoutine = routine;
        }
    }
}

