using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;

    public class ProjectConfiguration : IEntityTypeConfiguration<ProjectEntity>
    {
        public void Configure(EntityTypeBuilder<ProjectEntity> builder)
        {
            // Set keys
            //builder.HasKey(dataset => dataset.Id).HasName("id");
            builder.HasKey(portfolio => new { portfolio.Id });

            // Uniqueness
            builder.HasIndex(portfolio => portfolio.Name).IsUnique();

            // Relations and foreign keys.
            builder.HasMany(project => project.ReportEntities)
                .WithOne(report => report.ProjectEntity)
                .HasForeignKey(report => report.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("Project");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}