using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;

    public class OrgUserConfiguration : IEntityTypeConfiguration<OrgUserEntity>
    {
        public void Configure(EntityTypeBuilder<OrgUserEntity> builder)
        {
            // Set keys
            //builder.HasKey(dataset => dataset.Id).HasName("id");
            builder.HasKey(user => new { user.Id });

            // Uniqueness
            // builder.HasIndex(SubscriptionEntity => SubscriptionEntity.Name).IsUnique();

            // Relations and foreign keys.

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("OrgUser");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}