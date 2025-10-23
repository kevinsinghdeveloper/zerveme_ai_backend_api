using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    
    public class DatasetConfiguration : IEntityTypeConfiguration<DatasetEntity>
    {
        public void Configure(EntityTypeBuilder<DatasetEntity> builder)
        {
            // Set keys
            //builder.HasKey(dataset => dataset.Id).HasName("id");
            builder.HasKey(dataset => new { dataset.Id });
                
            // Uniqueness
            builder.HasIndex(dataset => dataset.Name).IsUnique();
            
            // Relations and foreign keys.
            builder.HasOne(dataset => dataset.Dwh)
                .WithMany(dwh => dwh.DatasetEntities) 
                .HasForeignKey(dataset => dataset.DWHId)
                .OnDelete(DeleteBehavior.NoAction);

            // builder.HasOne(dataset => dataset.OrganizationEntity)
            //     .WithMany(organization => organization.DatasetEntities)
            //     .HasForeignKey(dataset => dataset.OrganizationId)
            //     .OnDelete(DeleteBehavior.NoAction);

            // builder.HasMany(dataset => dataset.ReportEntities)
            //     .WithOne(report => report.DatasetEntity)
            //     .HasForeignKey(report => report.DatasetId)
            //     .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(dataset => dataset.DatasetRunQueryTrackerEntities)
                .WithOne(query => query.DatasetEntity)
                .HasForeignKey(query => query.DatasetId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .Property(e => e.DomainData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<DomainData>(v, (JsonSerializerOptions?)null))
                .HasColumnType("nvarchar(max)");


            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("Dataset");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}