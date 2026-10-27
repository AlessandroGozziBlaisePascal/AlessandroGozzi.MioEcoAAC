using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlessandroGozzi.AppAAC.MobileGUI.ViewModels
{
    public partial class ActionStepItem : ObservableObject
    {
        public RoutineAction Action { get; }

        [ObservableProperty]
        private bool _isUnlocked;

        [ObservableProperty]
        private bool _isCompleted;

        public ActionStepItem(RoutineAction action, bool isUnlocked = false)
        {
            Action = action;
            _isUnlocked = isUnlocked; 
            _isCompleted = false;
        }
    }
}
