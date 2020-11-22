using Metricaly.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.Data.Configurations
{
    public class MetricConfiguration : IEntityTypeConfiguration<Metric>
    {
        public void Configure(EntityTypeBuilder<Metric> builder)
        {
            builder.Property(m => m.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Name)
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(m => m.Namespace)
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(m => m.CreatedDate)
                .IsRequired();

            builder.HasIndex(p => new { p.ApplicationId, p.Namespace, p.Name })
            .IsUnique(true);
        }
    }
}
