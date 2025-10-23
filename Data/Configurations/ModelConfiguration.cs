using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;

    public class ModelConfiguration : IEntityTypeConfiguration<ModelEntity>
    {
        public void Configure(EntityTypeBuilder<ModelEntity> builder)
        {
            // Set keys
            //builder.HasKey(dataset => dataset.Id).HasName("id");
            builder.HasKey(model => new { model.Id });

            // Uniqueness
            // builder.HasIndex(SubscriptionEntity => SubscriptionEntity.Name).IsUnique();

            // Relations and foreign keys.

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("Model");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}