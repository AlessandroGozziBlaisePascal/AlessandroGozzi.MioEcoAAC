using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.AggregateRoots;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlessandroGozzi.AppAAC.Infrastructure.Persistance.Configurations
{
    public class RoutineConfiguration : IEntityTypeConfiguration<Routine>
    {
        public void Configure(EntityTypeBuilder<Routine> builder)
        {
            builder.ToTable("Routines");

            // Chiave primaria ereditata da AggregateRoot
            builder.HasKey(r => r.Id);

            // Name
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Value Object SpeechText -> convertito a string su SQLite
            builder.Property(r => r.Text)
                .HasConversion(
                    vo => vo.Text,
                    val => SpeechText.Create(val).Value)
                .IsRequired();

            // Enum RoutineType -> salvato come stringa o int
            builder.Property(r => r.Type)
                .IsRequired()
                .HasConversion<string>();

            // Soft delete flag
            builder.Property(r => r.IsDeleted)
                .IsRequired();

            // Esclude automaticamente i record rimossi
            builder.HasQueryFilter(r => !r.IsDeleted);

            // Relazione uno-a-molti incapsulata: punta alla navigation 'Activities' 
            // e mappa direttamente sul backing field '_activities'
            builder.HasMany(r => r.Activities)
                .WithOne()
                .HasForeignKey("RoutineId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(r => r.Activities)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
