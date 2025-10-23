namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using zervemedata.Data.AuthData;

    public class ReportTypeConfiguration : IEntityTypeConfiguration<ReportTypeEntity>
    {
        public void Configure(EntityTypeBuilder<ReportTypeEntity> builder)
        {
            // Set keys
            builder.HasKey(rt => new { rt.Id });

            // Uniqueness
            //builder.HasIndex(dataset => dataset.Name).IsUnique();

            // Relations and foreign keys.
            builder.HasOne(rt => rt.ReportConfigurationEntity)
                .WithOne(rc => rc.ReportType)
                .OnDelete(DeleteBehavior.NoAction);

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("ReportType");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}