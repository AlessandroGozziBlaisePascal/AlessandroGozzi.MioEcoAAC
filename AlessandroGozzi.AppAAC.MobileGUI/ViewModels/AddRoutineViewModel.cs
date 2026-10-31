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
    public partial class AddRoutineViewModel : ObservableObject
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly IArasaacService _arasaacService;

        [ObservableProperty]
        private string _routineName = string.Empty;

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

        public AddRoutineViewModel(IRoutineRepository routineRepository, IArasaacService arasaacService)
        {
            _routineRepository = routineRepository;
            _arasaacService = arasaacService;
        }

        [RelayCommand]
        public async Task SearchSymbolsAsync()
        {
            // Se l'utente non ha specificato una query di ricerca esplicita, usa il nome della routine
            var term = string.IsNullOrWhiteSpace(SearchQuery) ? RoutineName : SearchQuery;

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
        public async Task SaveRoutineAsync()
        {
            if (string.IsNullOrWhiteSpace(RoutineName))
            {
                await Shell.Current.DisplayAlert("Attenzione", "Inserisci il nome della routine.", "OK");
                return;
            }

            if (SelectedPictogram == null)
            {
                await Shell.Current.DisplayAlert("Attenzione", "Seleziona un simbolo per la routine.", "OK");
                return;
            }

            try
            {
                IsSaving = true;

                var newRoutine = new Routine
                {
                    Id = Guid.NewGuid(),
                    Name = RoutineName.Trim(),
                    SpeechText = RoutineName.Trim(),
                    ImageUrl = SelectedPictogram.ImageUrl,
                    IsSystem = false,
                    IsDeleted = false
                };

                await _routineRepository.SaveRoutineAsync(newRoutine); // O InsertAsync a seconda dell'interfaccia

                HomeViewModel.NeedsRefresh = false;

                await Shell.Current.DisplayAlert("Fatto!", "Routine creata con successo.", "OK");
                await Shell.Current.GoToAsync(".."); // Ritorna alla HomePage
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", $"Impossibile salvare la routine: {ex.Message}", "OK");
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
