using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;

namespace zervemedata.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class ProjectEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [MaxLength(150)] 
        public string Name { get; set; } = null!;

        [MaxLength(1000)] public string? Description { get; set; }

        [ForeignKey("OrganizationEntity")]
        public Guid OrganizationId { get; set; }

        public virtual OrganizationEntity OrganizationEntity { get; set; } = null!;

        public virtual List<ReportEntity> ReportEntities { get; set; } = new List<ReportEntity>();

        [ForeignKey("UpdatedUser")] public string? UpdatedUserId { get; set; }
        [ForeignKey("CreatedUser")] public string? CreatedUserId { get; set; }

        public ApplicationUser? UpdatedUser { get; set; } = null!;
        public ApplicationUser? CreatedUser { get; set; } = null!; // owner?
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }= DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}