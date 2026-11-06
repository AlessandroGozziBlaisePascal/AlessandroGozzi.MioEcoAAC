using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Dtos;
using AlessandroGozzi.AppAAC.Domain.Interfaces;
using AlessandroGozzi.AppAAC.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlessandroGozzi.AppAAC.MobileGUI.ViewModels
{
    [QueryProperty(nameof(ActivityId), "ActivityId")]
    public partial class AddActionViewModel : ObservableObject
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly IArasaacService _arasaacService;

        [ObservableProperty]
        private string _activityId = string.Empty;

        [ObservableProperty]
        private string _actionName = string.Empty;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private ArasaacPictogramDto? _selectedPictogram;

        [ObservableProperty]
        private bool _isSearching;

        [ObservableProperty]
        private bool _isSaving;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ObservableCollection<ArasaacPictogramDto> SearchResults { get; } = new();

        public AddActionViewModel(IRoutineRepository routineRepository, IArasaacService arasaacService)
        {
            _routineRepository = routineRepository;
            _arasaacService = arasaacService;
        }

        [RelayCommand]
        public async Task SearchSymbolsAsync()
        {
            var term = string.IsNullOrWhiteSpace(SearchQuery) ? ActionName : SearchQuery;

            if (string.IsNullOrWhiteSpace(term))
            {
                ErrorMessage = "Inserisci un nome o una parola chiave per cercare un simbolo.";
                return;
            }

            try
            {
                IsSearching = true;
                ErrorMessage = string.Empty;
                SearchResults.Clear();

                var results = await _arasaacService.SearchPictogramsAsync(term);

                if (results == null || results.Count == 0)
                {
                    ErrorMessage = $"Nessun pittogramma trovato per '{term}'. Prova con un sinonimo.";
                }
                else
                {
                    foreach (var pic in results)
                    {
                        SearchResults.Add(pic);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Errore di connessione: {ex.Message}";
            }
            finally
            {
                IsSearching = false;
            }
        }

        [RelayCommand]
        public void SelectPictogram(ArasaacPictogramDto? pictogram)
        {
            if (pictogram is null) return;
            SelectedPictogram = pictogram;
        }

        [RelayCommand]
        public async Task SaveActionAsync()
        {
            if (string.IsNullOrWhiteSpace(ActionName))
            {
                await Shell.Current.DisplayAlert("Attenzione", "Inserisci il nome del passaggio/azione.", "OK");
                return;
            }

            if (SelectedPictogram == null)
            {
                await Shell.Current.DisplayAlert("Attenzione", "Seleziona un simbolo per l'azione.", "OK");
                return;
            }

            if (!Guid.TryParse(ActivityId, out var activityGuid))
            {
                await Shell.Current.DisplayAlert("Errore", "Identificativo dell'attività non valido.", "OK");
                return;
            }

            try
            {
                IsSaving = true;

                // Calcola l'indice sequenziale in fondo alla sequenza
                var existingActions = await _routineRepository.GetActionsByActivityIdAsync(activityGuid);
                int nextOrder = existingActions.Count > 0 ? existingActions.Max(a => a.Order) + 1 : 0;

                var newAction = new RoutineAction
                {
                    Id = Guid.NewGuid(),
                    ActivityId = activityGuid,
                    Name = ActionName.Trim(),
                    SpeechText = ActionName.Trim(),
                    ImageUrl = SelectedPictogram.ImageUrl,
                    Order = nextOrder,
                    IsFinished = false,
                    IsDeleted = false,
                    IsAddButton = false
                };

                await _routineRepository.SaveActionAsync(activityGuid, newAction);

                ActionsViewModel.NeedsRefresh = true;

                await Shell.Current.DisplayAlert("Fatto!", "Azione aggiunta con successo.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", $"Impossibile salvare l'azione: {ex.Message}", "OK");
            }
            finally
            {
                IsSaving = false;
            }
        }

        [RelayCommand]
        public async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
