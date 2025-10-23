using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;

    public class JobConfiguration : IEntityTypeConfiguration<JobEntity>
    {
        public void Configure(EntityTypeBuilder<JobEntity> builder)
        {
            // Set keys
            //builder.HasKey(dataset => dataset.Id).HasName("id");
            builder.HasKey(job => new { job.Id });

            // Uniqueness

            // Relations and foreign keys.
            // builder.HasMany(job => job.JobScheduleEntities)
            //     .WithOne(js => js.JobEntity)
            //     .HasForeignKey(js => js.JobId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // Mapped columns.

            // Unmapped columns.

            builder
                .Navigation(r => r.JobFreqType)
                .AutoInclude();

            // Set the table name.
            builder.ToTable("Job");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}