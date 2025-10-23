using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace zervemedata.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;

    public class OrganizationConfiguration : IEntityTypeConfiguration<OrganizationEntity>
    {
        public void Configure(EntityTypeBuilder<OrganizationEntity> builder)
        {
            // Set keys
            //builder.HasKey(dataset => dataset.Id).HasName("id");
            builder.HasKey(organization => new { organization.Id });

            // Uniqueness
            builder.HasIndex(organization => organization.Name).IsUnique();

            // Relations and foreign keys.
            builder.HasMany(organization => organization.ProjectEntities)
                .WithOne(portfolio => portfolio.OrganizationEntity)
                .HasForeignKey(portfolio => portfolio.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);
            //
            builder.HasMany(organization => organization.DatasetEntities)
                .WithOne(dataset => dataset.OrganizationEntity)
                .HasForeignKey(dataset => dataset.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(organization => organization.OrgUserEntities)
                .WithOne(orguser => orguser.OrganizationEntity)
                .HasForeignKey(orguser => orguser.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(organization => organization.ModelEntities)
                .WithOne(model => model.OrganizationEntity)
                .HasForeignKey(model => model.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);
                
            
            // Mapped columns.

            // Unmapped columns.

            // Set the table name.
            builder.ToTable("Organization");
            //builder.ToTable(SchemaConstants.WorkplacesTableName, SchemaConstants.SchemaVersionV1);
        }
    }
}