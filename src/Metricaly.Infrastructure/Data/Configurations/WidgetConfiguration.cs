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
    public class WidgetConfiguration : IEntityTypeConfiguration<Widget>
    {
        public void Configure(EntityTypeBuilder<Widget> builder)
        {
            builder.Property(m => m.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(m => m.Type)
                .IsRequired();


            builder.Property(m => m.CreatedDate)
                .IsRequired();

            builder.Property(b => b.Data)
                .IsRequired();
            //.HasColumnType("jsonb");
        }
    }
}
