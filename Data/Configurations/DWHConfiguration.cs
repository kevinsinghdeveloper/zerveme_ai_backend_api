using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    
    public class DWHConfiguration : IEntityTypeConfiguration<DWHEntity>
    {
        public void Configure(EntityTypeBuilder<DWHEntity> builder)
        {
            // Set keys
            builder.HasKey(dwh => dwh.Id); //.HasName("id");
            
            // Uniqueness
            builder.HasIndex(dwh => dwh.Name).IsUnique();
            
            // Relations and foreign keys.
            //builder.HasMany(dwh => dwh.DatasetEntities)
            //    .WithOne(dataset => dataset.DWHEntity)
            //    .HasForeignKey(dataset => dataset.DWHId)
            //    .OnDelete(DeleteBehavior.NoAction);

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("DWH");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}