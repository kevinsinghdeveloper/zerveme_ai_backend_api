using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;

namespace zervemedata.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class ModelEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [MaxLength(255)] public string Name { get; set; } = null!;

        [MaxLength(1000)] public string? Description { get; set; }

        [MaxLength(255)] public string? ModelConfig { get; set; } // create an object with standard format??

        [ForeignKey("ModelTypeEntity")] public Guid ModelTypeId { get; set; }
        public required ModelTypeEntity ModelTypeEntity { get; set; } = null!;
        [ForeignKey("OrganizationEntity")] public Guid OrganizationId { get; set; }
        public virtual OrganizationEntity OrganizationEntity { get; set; } = null!;

        [ForeignKey("UpdatedUser")] public string? UpdatedUserId { get; set; }
        [ForeignKey("CreatedUser")] public string? CreatedUserId { get; set; }
        public ApplicationUser? UpdatedUser { get; set; } = null!;
        public ApplicationUser? CreatedUser { get; set; } = null!;
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}