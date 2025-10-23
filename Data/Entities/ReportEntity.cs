using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;

namespace zervemedata.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class ReportEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)] public string? Description { get; set; }

        [ForeignKey("ProjectEntity")]
        public Guid ProjectId { get; set; }

        [ForeignKey("ReportTypeEntity")] public Guid ReportTypeId { get; set; }

        [ForeignKey("ModelEntity")] public Guid? ModelId { get; set; }
        public virtual ModelEntity? ModelEntity { get; set; } = null!;

        public virtual ProjectEntity ProjectEntity { get; set; } = null!;
        public virtual ReportTypeEntity ReportTypeEntity { get; set; } = null!;
        public virtual ReportDatasetEntity? ReportDatasetEntity { get; set; } = null!;

        public virtual DatasetEntity? DatasetEntity { get; set; } = null!;

        [ForeignKey("UpdatedUser")] public string? UpdatedUserId { get; set; }
        [ForeignKey("CreatedUser")] public string? CreatedUserId { get; set; }

        [ForeignKey("JobEntity")] public Guid? JobEntityId { get; set; }

        public virtual JobEntity? JobEntity { get; set; } = null!;   

        public ApplicationUser? UpdatedUser { get; set; } = null!;
        public ApplicationUser? CreatedUser { get; set; } = null!;
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}