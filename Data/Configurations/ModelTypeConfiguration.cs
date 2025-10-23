namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using zervemedata.Data.AuthData;

    public class ModelTypeConfiguration : IEntityTypeConfiguration<ModelTypeEntity>
    {
        public void Configure(EntityTypeBuilder<ModelTypeEntity> builder)
        {
            // Set keys
            builder.HasKey(m => new { m.Id });

            // Uniqueness
            //builder.HasIndex(dataset => dataset.Name).IsUnique();

            // Relations and foreign keys.

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("ModelType");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}