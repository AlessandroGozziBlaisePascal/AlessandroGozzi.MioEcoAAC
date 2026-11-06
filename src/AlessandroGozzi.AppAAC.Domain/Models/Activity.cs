using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace AlessandroGozzi.AppAAC.Domain.Models
{
    public partial class Activity: ObservableObject
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Indexed]
        public Guid RoutineId { get; set; }
        [ObservableProperty]
        [property: Ignore]
        private bool _isSelected;
        public bool IsAddButton { get; set; }
        public int Order { get; set; }

        public string Name { get; set; } = string.Empty;
        public string SpeechText { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        [Ignore]
        public ObservableCollection<RoutineAction> Actions { get; set; } = new();

        [Ignore]
        public bool IsFinished => Actions.Count > 0 && Actions.All(a => a.IsFinished);

        public Activity() { }

        public Activity(Guid id, Guid routineId, string name, string text, string url)
        {
            Id = id;
            RoutineId = routineId;
            Name = name;
            SpeechText = text;
            ImageUrl = url;
        }

        public Activity(Guid routineId, string name, string text, string url)
            : this(Guid.NewGuid(), routineId, name, text, url) { }
    }
}
