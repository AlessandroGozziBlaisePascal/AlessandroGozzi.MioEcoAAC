using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AlessandroGozzi.MioEco_Material.Domain;
using AlessandroGozzi.MioEco_Material.Domain.Interfaces;
using AlessandroGozzi.MioEco_Material.MobileGUI.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace AlessandroGozzi.MioEco_Material.MobileGUI.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IFlashCardRepository _repository;
        private readonly ITextToSpeechService _ttsService;

        private List<FlashCard> _allCards = new();

        [ObservableProperty]
        private ObservableCollection<FlashCard> _filteredCards = new();

        [ObservableProperty]
        private Category? _selectedCategory;

        // Modalità Modifica
        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private FlashCard? _cardToDelete;

        // Card mostrata nel popup overlay a tutto schermo
        [ObservableProperty]
        private FlashCard? _selectedCardForDetail;

        [ObservableProperty]
        private bool _isCardDetailVisible;

        // Filtri Categorie
        [ObservableProperty]
        private bool _isBevandeSelected;
        [ObservableProperty]
        private bool _isCiboSelected;
        [ObservableProperty]
        private bool _isBisogniSelected;
        [ObservableProperty]
        private bool _isAttivitaSelected;
        [ObservableProperty]
        private bool _isPersoneSelected;
        [ObservableProperty]
        private bool _isEmozioniSelected;

        public HomeViewModel(IFlashCardRepository repository, ITextToSpeechService ttsService)
        {
            _repository = repository;
            _ttsService = ttsService;
        }

        [RelayCommand]
        public async Task LoadCardsAsync()
        {
            try
            {
                _allCards = await _repository.GetAllAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore caricamento: {ex.Message}");
            }
        }

        [RelayCommand]
        public void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;

            if (!IsEditMode && CardToDelete != null)
            {
                CardToDelete.IsSelected = false;
                CardToDelete = null;
            }
        }

        [RelayCommand]
        public async Task AddFlashCardAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddFlashCardPage));
        }

        [RelayCommand]
        public async Task ChangeCardImageAsync(FlashCard? card)
        {
            if (card == null) return;

            var navParams = new Dictionary<string, object>
            {
                { "Card", card }
            };
            await Shell.Current.GoToAsync(nameof(ChangeImagePage), navParams);
        }

        [RelayCommand]
        public async Task DeleteSelectedCardAsync()
        {
            if (CardToDelete == null)
            {
                await Shell.Current.DisplayAlert("Avviso", "Seleziona prima una tessera da eliminare.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert("Conferma", $"Vuoi eliminare '{CardToDelete.Name}'?", "Elimina", "Annulla");
            if (confirm)
            {
                await _repository.DeleteAsync(CardToDelete.Id);
                _allCards.Remove(CardToDelete);
                CardToDelete = null;
                ApplyFilter();
            }
        }

        [RelayCommand]
        public void ToggleCategory(string categoryName)
        {
            Category target = categoryName switch
            {
                "Bevande" => Category.Bevande,
                "Cibo" => Category.Cibo,
                "Bisogni" => Category.Bisogni,
                "Attivita" => Category.Attivita,
                "Persone" => Category.Persone,
                "Emozioni" => Category.Emozioni,
                _ => Category.Bevande
            };

            if (SelectedCategory == target)
            {
                SelectedCategory = null;
                ResetAllSelectionFlags();
            }
            else
            {
                SelectedCategory = target;
                ResetAllSelectionFlags();

                switch (target)
                {
                    case Category.Bevande: IsBevandeSelected = true; break;
                    case Category.Cibo: IsCiboSelected = true; break;
                    case Category.Bisogni: IsBisogniSelected = true; break;
                    case Category.Attivita: IsAttivitaSelected = true; break;
                    case Category.Persone: IsPersoneSelected = true; break;
                    case Category.Emozioni: IsEmozioniSelected = true; break;
                }
            }

            ApplyFilter();
        }

        private void ResetAllSelectionFlags()
        {
            IsBevandeSelected = false;
            IsCiboSelected = false;
            IsBisogniSelected = false;
            IsAttivitaSelected = false;
            IsPersoneSelected = false;
            IsEmozioniSelected = false;
        }

        [RelayCommand]
        public async Task SelectCardAsync(FlashCard? card)
        {
            if (card == null) return;

            // In modalità Modifica: seleziona per eliminazione
            if (IsEditMode)
            {
                if (CardToDelete == card)
                {
                    card.IsSelected = false;
                    CardToDelete = null;
                }
                else
                {
                    if (CardToDelete != null)
                        CardToDelete.IsSelected = false;

                    card.IsSelected = true;
                    CardToDelete = card;
                }
                return;
            }

            // In modalità CAA: sintesi vocale e apertura popup nella Home
            if (!string.IsNullOrWhiteSpace(card.SpeechText))
            {
                await _ttsService.SpeakItalianAsync(card.SpeechText);
            }

            await _repository.IncrementUsageAsync(card.Id);
            card.IncrementUsageCount();

            // Mostra il popup overlay della tessera
            SelectedCardForDetail = card;
            IsCardDetailVisible = true;
        }

        [RelayCommand]
        public void CloseCardDetail()
        {
            IsCardDetailVisible = false;
            SelectedCardForDetail = null;
        }

        [RelayCommand]
        public async Task SpeakCurrentDetailAsync()
        {
            if (SelectedCardForDetail != null && !string.IsNullOrWhiteSpace(SelectedCardForDetail.SpeechText))
            {
                await _ttsService.SpeakItalianAsync(SelectedCardForDetail.SpeechText);
            }
        }

        private void ApplyFilter()
        {
            FilteredCards.Clear();

            var query = SelectedCategory.HasValue
                ? _allCards.Where(c => c.Category == SelectedCategory.Value)
                : _allCards;

            foreach (var card in query)
            {
                FilteredCards.Add(card);
            }
        }
    }
}
