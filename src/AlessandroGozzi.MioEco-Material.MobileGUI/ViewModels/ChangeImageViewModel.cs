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
    [QueryProperty(nameof(Card), "Card")]
    public partial class ChangeImageViewModel : ObservableObject
    {
        private readonly IFlashCardRepository _repository;
        private readonly IArasaacService _arasaacService;

        [ObservableProperty]
        private FlashCard? _card;

        [ObservableProperty]
        private string _cardName = string.Empty;

        [ObservableProperty]
        private string _previewImagePath = string.Empty;

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

        public ChangeImageViewModel(IFlashCardRepository repository, IArasaacService arasaacService)
        {
            _repository = repository;
            _arasaacService = arasaacService;
        }

        partial void OnCardChanged(FlashCard? value)
        {
            if (value != null)
            {
                CardName = value.Name;
                PreviewImagePath = value.ImagePath;
                SearchQuery = value.Name;
            }
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

                string localDirectory = FileSystem.AppDataDirectory;
                string extension = Path.GetExtension(photo.FileName);
                string destinationPath = Path.Combine(localDirectory, $"{Guid.NewGuid()}{extension}");

                using (var sourceStream = await photo.OpenReadAsync())
                using (var destinationStream = File.Create(destinationPath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                PreviewImagePath = destinationPath;
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
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                ErrorMessage = "Inserisci una parola per cercare il simbolo.";
                return;
            }

            try
            {
                IsSearching = true;
                ErrorMessage = string.Empty;
                SearchResults.Clear();

                var results = await _arasaacService.SearchPictogramsAsync(SearchQuery);
                if (results.Count == 0)
                {
                    ErrorMessage = $"Nessun pittogramma trovato per '{SearchQuery}'.";
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
            PreviewImagePath = pictogram.ImageUrl;
        }

        [RelayCommand]
        public async Task SaveImageAsync()
        {
            if (Card == null) return;

            if (string.IsNullOrWhiteSpace(PreviewImagePath))
            {
                await Shell.Current.DisplayAlert("Attenzione", "Seleziona prima una nuova immagine.", "OK");
                return;
            }

            try
            {
                IsSaving = true;
                Card.ImagePath = PreviewImagePath;
                await _repository.UpdateAsync(Card);

                await Shell.Current.DisplayAlert("Fatto!", "Immagine aggiornata con successo.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", $"Impossibile salvare l'immagine: {ex.Message}", "OK");
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
