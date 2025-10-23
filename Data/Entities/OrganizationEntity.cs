using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;

namespace zervemedata.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class OrganizationEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [ForeignKey("Owner")] public string? OwnerId { get; set; }

        public virtual ApplicationUser? Owner { get; set; }

        [ForeignKey("ContactEntity")] public Guid? ContactId { get; set; }
        public virtual ContactEntity? ContactEntity { get; set; }
        
        public virtual List<ProjectEntity> ProjectEntities { get; set; } = new List<ProjectEntity>();
        public virtual List<DatasetEntity> DatasetEntities { get; set; } = new List<DatasetEntity>();

        public virtual List<OrgUserEntity> OrgUserEntities { get; set; } = new List<OrgUserEntity>();

        public virtual List<ModelEntity> ModelEntities { get; set; } = new List<ModelEntity>();
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}