using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;

    public class ReportConfiguration : IEntityTypeConfiguration<ReportEntity>
    {
        public void Configure(EntityTypeBuilder<ReportEntity> builder)
        {
            // Set keys
            //builder.HasKey(dataset => dataset.Id).HasName("id");
            builder.HasKey(report => new { report.Id });

            // Uniqueness
            // builder.HasIndex(project => project.Name).IsUnique();

            // Relations and foreign keys.
            builder.HasOne(r => r.ReportDatasetEntity)
                .WithOne(j => j.ReportEntity)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.DatasetEntity)
                .WithOne(d => d.ReportEntity)
                .OnDelete(DeleteBehavior.NoAction);

            // Unmapped columns.

            builder
                .Navigation(r => r.JobEntity)
                .AutoInclude();

            builder
                .Navigation(r => r.ReportTypeEntity)
                .AutoInclude();

            builder.Navigation(r => r.ReportDatasetEntity).AutoInclude();

            builder.Navigation(r => r.DatasetEntity).AutoInclude();
            
            // Set the table name.
            builder.ToTable("Report");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}