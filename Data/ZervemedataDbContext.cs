using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace zervemedata.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using zervemedata.Data.AuthData;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using zervemedata.Data.Configurations;
    using zervemedata.Data.Entities;

    public class ZervemedataDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<DatasetEntity> DatasetEntities { get; set; }

        public virtual DbSet<DWHEntity> DwhEntities { get; set; }

        public virtual DbSet<DatasetRunQueryTrackerEntity> DatasetRunQueryTrackerEntities { get; set; }

        public virtual DbSet<OrganizationEntity> OrganizationEntities { get; set; }

        public virtual DbSet<OrgUserEntity> OrgUserEntities { get; set; }

        public virtual DbSet<ProjectEntity> ProjectEntities { get; set; }

        public virtual DbSet<ReportEntity> ReportEntities { get; set; }

        public virtual DbSet<ReportTypeEntity> ReportTypeEntities { get; set; }

        public virtual DbSet<SubscriptionEntity> SubscriptionEntities { get; set; }

        public virtual DbSet<ContactEntity> ContactEntities { get; set; }

        public virtual DbSet<JobEntity> JobEntities { get; set; }

        public virtual DbSet<JobFreqTypeEntity> JobFreqTypeEntities { get; set; }

        public virtual DbSet<ReportConfigurationEntity> ReportConfigurationEntities { get; set; }

        public virtual DbSet<ReportDatasetEntity> ReportDatasetEntities { get; set; }

        public virtual DbSet<ModelEntity> ModelEntities { get; set; }

        public virtual DbSet<ModelTypeEntity> ModelTypeEntities { get; set; }
        
        public ZervemedataDbContext(DbContextOptions<ZervemedataDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUserLogin<string>>()
                .HasKey(userLogin => new { userLogin.UserId,  userLogin.ProviderKey });
            builder.Entity<IdentityUserRole<string>>()
                .HasKey(userRole => new { userRole.UserId,  userRole.RoleId });
            builder.Entity<IdentityUserToken<string>>()
                .HasKey(userToken => new { userToken.UserId });
            
            /*
            builder.Entity<ApplicationUser>().HasOne(user => user.ContactEntity)
                .WithOne(contact => contact.ApplicationUser)
                .HasForeignKey<ApplicationUser>(u => u.ContactId);
            */
            builder.ApplyConfiguration(
                new DatasetConfiguration()
            );
            builder.ApplyConfiguration(
                new DWHConfiguration()
            );
            builder.ApplyConfiguration(
                new DatasetRunQueryTrackerConfiguration()
            );
            builder.ApplyConfiguration(
                new ContactConfiguration()
            );
            builder.ApplyConfiguration(
                new OrganizationConfiguration()
            );
            builder.ApplyConfiguration(
                new ProjectConfiguration()
            );
            builder.ApplyConfiguration(
                new ReportConfiguration()
            );
            builder.ApplyConfiguration(
                new OrgUserConfiguration()
            );
            builder.ApplyConfiguration(
                new SubscriptionConfiguration()
            );
            builder.ApplyConfiguration(
                new ReportTypeConfiguration()
            );
            builder.ApplyConfiguration(
                new JobConfiguration()
            );
            builder.ApplyConfiguration(
                new JobFreqTypeConfiguration()
            );
            builder.ApplyConfiguration(
                new ReportConfigurationConfiguration()
            );
            builder.ApplyConfiguration(
                new ReportDatasetConfiguration()
            );
            builder.ApplyConfiguration(
                new ModelConfiguration()
            );
            builder.ApplyConfiguration(
                new ModelTypeConfiguration()
            );
        }
    }
}