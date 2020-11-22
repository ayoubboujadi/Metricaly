using Metricaly.Core.Entities;
using Metricaly.Core.Widgets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.Data.Configurations
{
    public class DashboardWidgetConfiguration : IEntityTypeConfiguration<DashboardWidget>
    {
        public void Configure(EntityTypeBuilder<DashboardWidget> builder)
        {
            builder.Property(m => m.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Height)
                .IsRequired();

            builder.Property(m => m.Width)
                .IsRequired();

            builder.Property(m => m.X)
                .IsRequired();

            builder.Property(m => m.Y)
                .IsRequired();

            builder.Property(m => m.CreatedDate)
                .IsRequired();
        }
    }
}
