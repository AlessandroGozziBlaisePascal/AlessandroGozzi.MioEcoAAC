using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace AlessandroGozzi.AppAAC.Domain.Models
{
    public class RoutineAction
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Indexed]
        public Guid ActivityId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string SpeechText { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public bool IsFinished { get; set; } = false;

        public RoutineAction() { }

        public RoutineAction(Guid id, Guid activityId, string name, string url, string text)
        {
            Id = id;
            ActivityId = activityId;
            Name = name;
            ImageUrl = url;
            SpeechText = text;
        }

        public RoutineAction(Guid activityId, string name, string url, string text)
            : this(Guid.NewGuid(), activityId, name, url, text) { }
    }
}
