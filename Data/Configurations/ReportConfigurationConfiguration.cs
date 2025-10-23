using System.Text.Json;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using zervemedata.Data.AuthData;

    public class ReportConfigurationConfiguration : IEntityTypeConfiguration<ReportConfigurationEntity>
    {
        public void Configure(EntityTypeBuilder<ReportConfigurationEntity> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder
                .Property(e => e.ReportConfig)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<ReportConfigObject>(v, (JsonSerializerOptions?)null))
                .HasColumnType("nvarchar(max)");

            builder.ToTable("ReportConfiguration");
        }
    }
}