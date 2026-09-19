using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AlessandroGozzi.AppAAC.Application.Dtos;
using AlessandroGozzi.AppAAC.Application.Queries.GetAllRoutines;
using AlessandroGozzi.AppAAC.Application.Queries.GetRoutine;
using AlessandroGozzi.AppAAC.Domain.AggregateRoots;
using MediatR;

namespace AlessandroGozzi.AppAAC.MobileGUI.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly ISender _sender;

        private bool _isLoading;
        private string _errorMessage = string.Empty;

        public ObservableCollection<RoutineSummaryDto> Routines { get; } = new();

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand SelectRoutineCommand { get; }

        public HomeViewModel(ISender sender)
        {
            _sender = sender;
            SelectRoutineCommand = new Command<RoutineSummaryDto>(async routine => await OnRoutineSelectedAsync(routine));
        }

        public async Task LoadRoutinesAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _sender.Send(new GetAllRoutinesQuery());

                if (result.IsFailure)
                {
                    ErrorMessage = result.Error.Description;
                    Routines.Clear();
                    return;
                }

                Routines.Clear();
                foreach (var routine in result.Value)
                {
                    Routines.Add(routine);
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

        private async Task OnRoutineSelectedAsync(RoutineSummaryDto? routine)
        {
            if (routine is null) return;

            try
            {
                await TextToSpeech.Default.SpeakAsync(routine.Name);
            }
            catch
            {
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
        #endregion
    }
}
