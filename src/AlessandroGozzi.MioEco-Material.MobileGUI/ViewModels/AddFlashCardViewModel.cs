using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AlessandroGozzi.MioEco_Material.Domain;
using AlessandroGozzi.MioEco_Material.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlessandroGozzi.MioEco_Material.MobileGUI.ViewModels
{
    public partial class AddFlashCardViewModel : ObservableObject
    {
        private readonly IFlashCardRepository _repository;
        private readonly IArasaacService _arasaacService;

        [ObservableProperty]
        private string _cardName = string.Empty;

        [ObservableProperty]
        private string _speechText = string.Empty;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private Category _selectedCategory = Category.Bisogni;

        [ObservableProperty]
        private ArasaacPictogramDto? _selectedPictogram;

        // Percorso foto da galleria locale
        [ObservableProperty]
        private string? _customImagePath;

        [ObservableProperty]
        private bool _isSearching;

        [ObservableProperty]
        private bool _isSaving;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ObservableCollection<ArasaacPictogramDto> SearchResults { get; } = new();
        public List<Category> CategoriesList { get; } = Enum.GetValues(typeof(Category)).Cast<Category>().ToList();

        public AddFlashCardViewModel(IFlashCardRepository repository, IArasaacService arasaacService)
        {
            _repository = repository;
            _arasaacService = arasaacService;
        }

        [RelayCommand]
        public async Task PickPhotoFromGalleryAsync()
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Shell.Current.DisplayAlert("Avviso", "Accesso alle foto non supportato su questo dispositivo.", "OK");
                    return;
                }

                var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Seleziona una foto per la tessera"
                });

                if (photo == null) return;

                // Copia l'immagine nella cartella locale dell'app per renderla permanente
                string localDirectory = FileSystem.AppDataDirectory;
                string extension = Path.GetExtension(photo.FileName);
                string newFileName = $"{Guid.NewGuid()}{extension}";
                string destinationPath = Path.Combine(localDirectory, newFileName);

                using (var sourceStream = await photo.OpenReadAsync())
                using (var destinationStream = File.Create(destinationPath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                // Imposta l'immagine personalizzata e azzera il pittogramma ARASAAC
                CustomImagePath = destinationPath;
                SelectedPictogram = null;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", $"Impossibile caricare la foto: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task SearchSymbolsAsync()
        {
            var term = string.IsNullOrWhiteSpace(SearchQuery) ? CardName : SearchQuery;
            if (string.IsNullOrWhiteSpace(term))
            {
                ErrorMessage = "Inserisci un nome o una parola chiave.";
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
                    ErrorMessage = $"Nessun pittogramma trovato per '{term}'.";
                }
                else
                {
                    foreach (var pic in results)
                        SearchResults.Add(pic);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsSearching = false;
            }
        }

        [RelayCommand]
        public void SelectPictogram(ArasaacPictogramDto? pictogram)
        {
            if (pictogram == null) return;
            SelectedPictogram = pictogram;
            CustomImagePath = null; // Rimuove la foto da galleria se si sceglie un pittogramma
        }

        [RelayCommand]
        public async Task SaveCardAsync()
        {
            if (string.IsNullOrWhiteSpace(CardName))
            {
                await Shell.Current.DisplayAlert("Attenzione", "Inserisci il nome della tessera.", "OK");
                return;
            }

            // Determina la sorgente immagine (foto da galleria o pittogramma ARASAAC)
            string? chosenImagePath = CustomImagePath ?? SelectedPictogram?.ImageUrl;

            if (string.IsNullOrWhiteSpace(chosenImagePath))
            {
                await Shell.Current.DisplayAlert("Attenzione", "Scegli un simbolo ARASAAC o carica una foto dalla galleria.", "OK");
                return;
            }

            try
            {
                IsSaving = true;

                var speech = string.IsNullOrWhiteSpace(SpeechText) ? CardName.Trim() : SpeechText.Trim();

                var newCard = new FlashCard
                {
                    Id = Guid.NewGuid(),
                    Name = CardName.Trim(),
                    SpeechText = speech,
                    ImagePath = chosenImagePath,
                    Category = SelectedCategory,
                    IsDeleted = false,
                    UsageCount = 0
                };

                await _repository.AddAsync(newCard);

                await Shell.Current.DisplayAlert("Fatto!", "Tessera creata con successo.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", $"Impossibile salvare la tessera: {ex.Message}", "OK");
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
