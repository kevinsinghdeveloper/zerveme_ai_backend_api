namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class DatasetRunQueryTrackerConfiguration : IEntityTypeConfiguration<DatasetRunQueryTrackerEntity>
    {
        public void Configure(EntityTypeBuilder<DatasetRunQueryTrackerEntity> builder)
        {
            // Set keys
            // builder.HasKey(dt => new { dt.Id });
            // builder.HasKey(dt => dt.Id).HasName("id");
            builder.HasKey(dt => dt.Id);
            
            // Uniqueness
            // builder.HasIndex(dt => dt.).IsUnique();

            // Relations and foreign keys.
            // builder.HasOne(dt => dt.DatasetEntity)
            //     .WithMany(d => d.DatasetRunQueryTrackerEntities)
            //     .HasForeignKey(d => new {d.DatasetId, d.DWHId})
            //     .OnDelete(DeleteBehavior.NoAction);

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("DatasetRunQueryTracker");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}