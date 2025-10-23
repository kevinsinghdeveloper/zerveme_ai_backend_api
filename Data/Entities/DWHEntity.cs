using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;

namespace zervemedata.Data.Entities
{
    using zervemedata.Data.Enumerations;

    public class DWHEntity : IBaseEntity
    {
        public Guid Id { get; set;}

        [MaxLength(100)] public string? Name { get; set; } = null!;

        [MaxLength(100)] public string PrimarySchema { get; set; } = null!;

        [MaxLength(100)] public string? SecondarySchema { get; set; }

        [Required] [MaxLength(255)] public string ConnectionString { get; set; } = null!;

        public DWHLoadingStatus DwhLoadingStatus { get; set; }
        public DWH? Dwh { get; set; }
        
        [ForeignKey("UpdatedUser")] public string? UpdatedUserId { get; set; }

        public virtual List<DatasetEntity> DatasetEntities { get; set; } = new List<DatasetEntity>();

        [ForeignKey("OrganizationEntity")]
        public Guid? OrganizationId { get; set; }

        public virtual OrganizationEntity? OrganizationEntity { get; set; }

        public ApplicationUser? UpdatedUser { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}