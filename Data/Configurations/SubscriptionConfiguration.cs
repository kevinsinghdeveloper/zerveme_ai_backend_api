using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;

    public class SubscriptionConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
    {
        public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
        {
            // Set keys
            //builder.HasKey(dataset => dataset.Id).HasName("id");
            builder.HasKey(project => new { project.Id });

            // Uniqueness
            // builder.HasIndex(SubscriptionEntity => SubscriptionEntity.Name).IsUnique();

            // Relations and foreign keys.

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("Subscription");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}