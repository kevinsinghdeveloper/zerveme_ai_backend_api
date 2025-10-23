namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using zervemedata.Data.AuthData;

    public class ReportDatasetConfiguration : IEntityTypeConfiguration<ReportDatasetEntity>
    {
        public void Configure(EntityTypeBuilder<ReportDatasetEntity> builder)
        {
            // Set keys
            builder.HasKey(rt => new { rt.Id });

            // Uniqueness
            //builder.HasIndex(dataset => dataset.Name).IsUnique();

            // Relations and foreign keys.

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("ReportDataset");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}