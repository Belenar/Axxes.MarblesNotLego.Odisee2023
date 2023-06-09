﻿using Axxes.ToyCollector.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Axxes.ToyCollector.DataAccess.EF.EntityMapping
{
    public class ToyMapping : IEntityTypeConfiguration<Toy>
    {
        public void Configure(EntityTypeBuilder<Toy> builder)
        {
            builder
                .Property(t => t.Description)
                .HasMaxLength(250)
                .HasColumnType("varchar(250)");

            builder
                .Property(t => t.AcquiredDate)
                .HasColumnType("date");
        }
    }
}