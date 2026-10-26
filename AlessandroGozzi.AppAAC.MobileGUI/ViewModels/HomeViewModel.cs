using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AlessandroGozzi.AppAAC.Domain.Models;
using AlessandroGozzi.AppAAC.Domain.Repositories;
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

        private ObservableCollection<Routine> _routines = new();
        public ObservableCollection<Routine> Routines
        {
            get => _routines;
            set => SetProperty(ref _routines, value);
        }

        private Routine? _selectedRoutine;
        public Routine? SelectedRoutine
        {
            get => _selectedRoutine;
            set
            {
                if (SetProperty(ref _selectedRoutine, value) && value != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _ttsService.SpeakItalianAsync(value.Name);
                        }
                        catch
                        {
                            // Fallback per TextToSpeech
                        }
                    });
                }
            }
        }

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
        public void SelectRoutine(Routine? routine)
        {
            if (routine is null) return;
            SelectedRoutine = routine;
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
            if (_selectedRoutine == null) return;

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
    }
}
