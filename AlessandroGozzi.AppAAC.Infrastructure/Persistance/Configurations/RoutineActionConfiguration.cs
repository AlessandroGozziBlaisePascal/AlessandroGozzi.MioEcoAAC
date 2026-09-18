using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Entities;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlessandroGozzi.AppAAC.Infrastructure.Persistance.Configurations
{
    public class RoutineActionConfiguration : IEntityTypeConfiguration<RoutineAction>
    {
        public void Configure(EntityTypeBuilder<RoutineAction> builder)
        {
            builder.ToTable("RoutineActions");

            builder.HasKey(ra => ra.Id);

            builder.Property(ra => ra.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ra => ra.Audio)
                .HasConversion(
                    vo => vo.Text,
                    val => SpeechText.Create(val).Value)
                .IsRequired();

            builder.Property(ra => ra.Image)
                .HasConversion(
                    vo => vo.Value,
                    val => ImageUrl.Create(val).Value)
                .IsRequired();

            builder.Property(ra => ra.IsFinished)
                .IsRequired();

            builder.Property(ra => ra.IsDeleted)
                .IsRequired();

            builder.HasQueryFilter(ra => !ra.IsDeleted);
        }
    }
}
