using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Repositories;
using AlessandroGozzi.AppAAC.MobileGUI.Pages;
using AlessandroGozzi.AppAAC.MobileGUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlessandroGozzi.AppAAC.MobileGUI.ViewModels
{
    [QueryProperty(nameof(ActivityId), "ActivityId")]
    public partial class ActionsViewModel : ObservableObject
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly ITextToSpeechService _ttsService;

        public ActionsViewModel(IRoutineRepository routineRepository, ITextToSpeechService ttsService)
        {
            _routineRepository = routineRepository;
            _ttsService = ttsService;
        }

        private string _activityId = string.Empty;
        public string ActivityId
        {
            get => _activityId;
            set
            {
                if (SetProperty(ref _activityId, value) && Guid.TryParse(value, out var guid))
                {
                    _ = CaricaAzioniAsync(guid);
                }
            }
        }

        private ObservableCollection<ActionStepItem> _actionItems = new();
        public ObservableCollection<ActionStepItem> ActionItems
        {
            get => _actionItems;
            set => SetProperty(ref _actionItems, value);
        }

        // Azione attualmente aperta in modalità grande schermo
        private ActionStepItem? _activeModalItem;
        public ActionStepItem? ActiveModalItem
        {
            get => _activeModalItem;
            set => SetProperty(ref _activeModalItem, value);
        }

        // Controlla la visibilità del popup/overlay a tutto schermo
        private bool _isModalOpen;
        public bool IsModalOpen
        {
            get => _isModalOpen;
            set => SetProperty(ref _isModalOpen, value);
        }

        private async Task CaricaAzioniAsync(Guid activityId)
        {
            // Chiama il metodo che recupera l'attività e le sue relative azioni
            var actions = await _routineRepository.GetActionsByActivityIdAsync(activityId);

            if (actions == null || actions.Count == 0)
                return;

            var lista = new ObservableCollection<ActionStepItem>();
            for (int i = 0; i < actions.Count; i++)
            {
                // La prima azione è sbloccata (indice 0), le altre bloccate/grigie
                lista.Add(new ActionStepItem(actions[i], isUnlocked: i == 0));
            }

            ActionItems = lista;
        }

        // Clic su una card della lista: apre a schermo intero solo se sbloccata
        [RelayCommand]
        private async Task ApriAzioneAsync(ActionStepItem item)
        {
            if (item == null || !item.IsUnlocked) return;

            ActiveModalItem = item;
            IsModalOpen = true;

            if (!string.IsNullOrWhiteSpace(item.Action.SpeechText))
            {
                await _ttsService.SpeakItalianAsync(item.Action.SpeechText);
            }
        }

        // Chiude il grande schermo senza completare
        [RelayCommand]
        private void ChiudiSchermoGrande()
        {
            IsModalOpen = false;
            ActiveModalItem = null;
        }

        // Bottone spunta (completata): segna completata, sblocca la successiva o termina
        [RelayCommand]
        private async Task CompletaAzioneCorrenteAsync()
        {
            if (ActiveModalItem == null) return;

            ActiveModalItem.IsCompleted = true;
            IsModalOpen = false;

            int currentIndex = ActionItems.IndexOf(ActiveModalItem);

            if (currentIndex + 1 < ActionItems.Count)
            {
                ActionItems[currentIndex + 1].IsUnlocked = true;
            }

            else if (ActionItems.All(a => a.IsCompleted))
            {
                await Shell.Current.GoToAsync(nameof(CongratulationsPage));
            }

            ActiveModalItem = null;
        }

        [RelayCommand]
        private async Task TornaIndietroAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
