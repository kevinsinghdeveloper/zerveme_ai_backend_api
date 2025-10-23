namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using zervemedata.Data.AuthData;

    public class JobFreqTypeConfiguration : IEntityTypeConfiguration<JobFreqTypeEntity>
    {
        public void Configure(EntityTypeBuilder<JobFreqTypeEntity> builder)
        {
            // Set keys
            builder.HasKey(rt => new { rt.Id });

            // Uniqueness
            //builder.HasIndex(dataset => dataset.Name).IsUnique();

            // Relations and foreign keys.

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("JobFreqType");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}