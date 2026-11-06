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
    [QueryProperty(nameof(ActivityId), "ActivityId")]
    [QueryProperty(nameof(ActivityTitle), "ActivityTitle")]
    public partial class ActionsViewModel : ObservableObject
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly ITextToSpeechService _ttsService;

        // Card segnaposto per il "+"
        private readonly ActionStepItem _addPlaceholderItem = new(new RoutineAction
        {
            Id = Guid.Empty,
            Name = "Aggiungi azione"
        })
        {
            IsAddButton = true,
            IsUnlocked = true
        };

        public static bool NeedsRefresh { get; set; } = false;

        [ObservableProperty]
        private string _activityId = string.Empty;

        [ObservableProperty]
        private string _activityTitle = "Azioni da fare";

        [ObservableProperty]
        private ObservableCollection<ActionStepItem> _actionItems = new();

        [ObservableProperty]
        private ActionStepItem? _activeModalItem;

        [ObservableProperty]
        private bool _isModalOpen;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditButtonText))]
        private bool _isEditMode;

        public string EditButtonText => IsEditMode ? "Fine" : "Modifica";

        [ObservableProperty]
        private ActionStepItem? _selectedActionItem;

        public ActionsViewModel(IRoutineRepository routineRepository, ITextToSpeechService ttsService)
        {
            _routineRepository = routineRepository;
            _ttsService = ttsService;
        }

        [RelayCommand]
        public async Task CaricaAzioniAsync(Guid activityId)
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var actions = await _routineRepository.GetActionsByActivityIdAsync(activityId);

                ActionItems.Clear();

                if (actions != null && actions.Count > 0)
                {
                    var nonDeleted = actions.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                    for (int i = 0; i < nonDeleted.Count; i++)
                    {
                        // La prima azione è sbloccata, le successive bloccate
                        ActionItems.Add(new ActionStepItem(nonDeleted[i], isUnlocked: i == 0));
                    }
                }

                if (IsEditMode && !ActionItems.Contains(_addPlaceholderItem))
                {
                    ActionItems.Add(_addPlaceholderItem);
                }
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
                if (!ActionItems.Contains(_addPlaceholderItem))
                {
                    ActionItems.Add(_addPlaceholderItem);
                }
            }
            else
            {
                if (ActionItems.Contains(_addPlaceholderItem))
                {
                    ActionItems.Remove(_addPlaceholderItem);
                }

                foreach (var item in ActionItems)
                {
                    item.IsSelected = false;
                }

                SelectedActionItem = null;
            }
        }

        // Gestione tocco card: se siamo in Edit seleziona, altrimenti apre il popup d'esecuzione
        [RelayCommand]
        public async Task AzioneToccataAsync(ActionStepItem? item)
        {
            if (item == null || item.IsAddButton) return;

            if (IsEditMode)
            {
                foreach (var i in ActionItems)
                {
                    i.IsSelected = false;
                }

                item.IsSelected = true;
                SelectedActionItem = item;
                return;
            }

            // Modalità normale: esecuzione step
            if (!item.IsUnlocked) return;

            ActiveModalItem = item;
            IsModalOpen = true;

            if (!string.IsNullOrWhiteSpace(item.Action.SpeechText))
            {
                try
                {
                    await _ttsService.SpeakItalianAsync(item.Action.SpeechText);
                }
                catch
                {
                    // Fallback TTS silenzioso
                }
            }
        }

        [RelayCommand]
        public void ChiudiSchermoGrande()
        {
            IsModalOpen = false;
            ActiveModalItem = null;
        }

        [RelayCommand]
        public async Task CompletaAzioneCorrenteAsync()
        {
            if (ActiveModalItem == null) return;

            ActiveModalItem.IsCompleted = true;
            IsModalOpen = false;

            // Escludiamo il placeholder dal conteggio sequenziale
            var itemsReali = ActionItems.Where(a => !a.IsAddButton).ToList();
            int currentIndex = itemsReali.IndexOf(ActiveModalItem);

            if (currentIndex + 1 < itemsReali.Count)
            {
                itemsReali[currentIndex + 1].IsUnlocked = true;
            }
            else if (itemsReali.All(a => a.IsCompleted))
            {
                await Shell.Current.GoToAsync(nameof(CongratulationsPage));
            }

            ActiveModalItem = null;
        }

        [RelayCommand]
        public async Task AggiungiAzioneAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddActionPage), new Dictionary<string, object>
        {
            { "ActivityId", ActivityId }
        });
        }

        [RelayCommand]
        public async Task EliminaAzioneAsync(ActionStepItem? item)
        {
            var target = item ?? SelectedActionItem;

            if (target == null || target.IsAddButton)
            {
                await Shell.Current.DisplayAlert("Attenzione", "Seleziona prima l'azione da eliminare toccando la card.", "OK");
                return;
            }

            bool conferma = await Shell.Current.DisplayAlert(
                "Elimina Azione",
                $"Vuoi davvero eliminare l'azione '{target.Action.Name}'?",
                "Elimina",
                "Annulla");

            if (conferma)
            {
                try
                {
                    target.Action.IsDeleted = true;

                    if (Guid.TryParse(ActivityId, out var actGuid))
                    {
                        await _routineRepository.SaveActionAsync(actGuid, target.Action);
                    }

                    ActionItems.Remove(target);
                    SelectedActionItem = null;
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Errore", $"Impossibile eliminare l'azione: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        public async Task TornaIndietroAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
