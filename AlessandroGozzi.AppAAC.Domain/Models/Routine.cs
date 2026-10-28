using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Enums;
using SQLite;

namespace AlessandroGozzi.AppAAC.Domain.Models
{
    public class Routine
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string SpeechText { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public RoutineType Type { get; set; }
        public bool IsDeleted { get; set; } = false;

        [Ignore]
        public ObservableCollection<Activity> Activities { get; set; } = new();

        public Routine() { }

        public Routine(Guid id, string name, string text, RoutineType type)
        {
            Id = id;
            Name = name;
            SpeechText = text;
            Type = type;
        }

        public Routine(string name, string text, RoutineType type)
            : this(Guid.NewGuid(), name, text, type) { }
    }
}
