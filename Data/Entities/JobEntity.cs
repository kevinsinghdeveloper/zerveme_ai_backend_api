using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;
using zervemedata.Data.Enumerations;

namespace zervemedata.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class JobEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [ForeignKey("UpdatedUser")] public string? UpdatedUserId { get; set; }
        [ForeignKey("CreatedUser")] public string? CreatedUserId { get; set; }

        [ForeignKey("JobFreqType")] public Guid? JobFreqTypeId { get; set; }

        public ApplicationUser? UpdatedUser { get; set; } = null!;
        public ApplicationUser? CreatedUser { get; set; } = null!;
        public JobFreqTypeEntity? JobFreqType { get; set; }

        // Merged properties from JobScheduleEntity
        // start of run
        public DateTime? RecentScheduledRunDateTime { get; set; }

        // next time this will run
        public DateTime? NextScheduledRunDateTime { get; set; }

        // last time this will EVER run
        public DateTime? LastScheduledRunDateTime { get; set; }

        [MaxLength(1000)] public string? StatusLog { get; set; }

        public JobStatusType JobStatusType { get; set; }

        // Base entity properties
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }

        // Any additional job-specific properties
    }
}