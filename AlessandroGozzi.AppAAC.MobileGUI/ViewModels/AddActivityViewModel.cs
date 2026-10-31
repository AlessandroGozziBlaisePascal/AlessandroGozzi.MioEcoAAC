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
    [QueryProperty(nameof(RoutineId), "RoutineId")]
    public partial class AddActivityViewModel : ObservableObject
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly IArasaacService _arasaacService;

        [ObservableProperty]
        private string _routineId = string.Empty;

        [ObservableProperty]
        private string _activityName = string.Empty;

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

        public AddActivityViewModel(IRoutineRepository routineRepository, IArasaacService arasaacService)
        {
            _routineRepository = routineRepository;
            _arasaacService = arasaacService;
        }

        [RelayCommand]
        public async Task SearchSymbolsAsync()
        {
            // Se non viene specificata una query, usa il nome dell'attività
            var term = string.IsNullOrWhiteSpace(SearchQuery) ? ActivityName : SearchQuery;

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

                if (results.Count == 0)
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
        public async Task SaveActivityAsync()
        {
            if (string.IsNullOrWhiteSpace(ActivityName))
            {
                await Shell.Current.DisplayAlert("Attenzione", "Inserisci il nome dell'attività.", "OK");
                return;
            }

            if (SelectedPictogram == null)
            {
                await Shell.Current.DisplayAlert("Attenzione", "Seleziona un simbolo per l'attività.", "OK");
                return;
            }

            if (!Guid.TryParse(RoutineId, out var routineGuid))
            {
                await Shell.Current.DisplayAlert("Errore", "Identificativo della routine non valido.", "OK");
                return;
            }

            try
            {
                IsSaving = true;

                // Calcola l'indice Order sequenziale accodando in fondo
                var existingActivities = await _routineRepository.GetActivitiesByRoutineIdAsync(routineGuid);
                int nextOrder = existingActivities.Count > 0 ? existingActivities.Max(a => a.Order) + 1 : 0;

                var newActivity = new Activity
                {
                    Id = Guid.NewGuid(),
                    RoutineId = routineGuid,
                    Name = ActivityName.Trim(),
                    SpeechText = ActivityName.Trim(),
                    ImageUrl = SelectedPictogram.ImageUrl,
                    Order = nextOrder,
                    IsDeleted = false
                };

                await _routineRepository.SaveActivityAsync(routineGuid, newActivity);

                ActivityViewModel.NeedsRefresh = true;

                await Shell.Current.DisplayAlert("Fatto!", "Attività aggiunta con successo.", "OK");
                await Shell.Current.GoToAsync(".."); // Ritorna alla pagina ActivityPage
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", $"Impossibile salvare l'attività: {ex.Message}", "OK");
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
