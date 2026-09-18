using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Activity = AlessandroGozzi.AppAAC.Domain.Entities.Activity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;

namespace AlessandroGozzi.AppAAC.Infrastructure.Persistance.Configurations
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");

            // Chiave primaria ereditata da Entity
            builder.HasKey(a => a.Id);

            // Name
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Value Object SpeechText -> convertito a string su SQLite
            builder.Property(a => a.Text)
                .HasConversion(
                    vo => vo.Text,
                    val => SpeechText.Create(val).Value)
                .IsRequired();

            // Value Object ImageUrl -> convertito a string su SQLite
            builder.Property(a => a.ImageUrl)
                .HasConversion(
                    vo => vo.Value,
                    val => ImageUrl.Create(val).Value)
                .IsRequired();

            // IsDeleted flag per il Soft Delete
            builder.Property(a => a.IsDeleted)
                .IsRequired();

            // Filtro globale per escludere record cancellati
            builder.HasQueryFilter(a => !a.IsDeleted);

            // Mappatura della lista privata _actions e della navigation property Actions
            builder.HasMany(a => a.Actions)
                .WithOne()
                .HasForeignKey("ActivityId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Forza EF Core a scrivere direttamente nel backing field privato _actions
            builder.Navigation(a => a.Actions)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
