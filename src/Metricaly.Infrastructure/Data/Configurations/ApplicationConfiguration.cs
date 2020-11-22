using Metricaly.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metricaly.Infrastructure.Data.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.Property(m => m.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Name)
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(m => m.ApiKey)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(m => m.UserId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(m => m.CreatedDate)
                .IsRequired();

            builder.HasIndex(m => m.ApiKey)
                .IsUnique(true);
        }
    }
}
