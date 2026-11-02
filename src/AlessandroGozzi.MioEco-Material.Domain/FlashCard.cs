using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using SQLite;
using NotNullAttribute = SQLite.NotNullAttribute;
using TableAttribute = SQLite.TableAttribute;

namespace AlessandroGozzi.MioEco_Material.Domain
{
    [Table("FlashCards")]
    public class FlashCard : INotifyPropertyChanged
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        public string SpeechText { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public int UsageCount { get; set; }

        public Category Category { get; set; }

        public string Emoji { get; set; } = string.Empty;

        [Ignore]
        public string DisplayText => string.IsNullOrWhiteSpace(Emoji) ? Name : $"{Emoji} {Name}";

        private bool _isSelected;

        [Ignore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public FlashCard()
        {
            Id = Guid.NewGuid();
        }

        public FlashCard(string name, string speechText, string imagePath, Category category, string emoji = "")
        {
            Id = Guid.NewGuid();
            Name = name;
            SpeechText = speechText;
            ImagePath = imagePath;
            Category = category;
            Emoji = emoji;
            UsageCount = 0;
            IsDeleted = false;
        }

        public void IncrementUsageCount()
        {
            UsageCount++;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum Category
    {
        Cibo,
        Bevande,
        Bisogni,
        Attivita,
        Persone,
        Emozioni,
        Luoghi,
        Altro
    }
}

    

