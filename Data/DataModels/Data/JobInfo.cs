using zervemedata.Data.Entities;
using zervemedata.Data.Enumerations;

namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class JobInfo : IResource
    {
        public Guid Id { get; set; }
        public JobFreqTypeInfo? JobFreqType { get; set; }

        // public Guid? ReportId { get; set; }
        public Guid? JobScheduleId { get; set; }
        public string? UpdatedUserId { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Deleted { get; set; }

        public DateTime? LastRunDate { get; set; }

        public ReportEntity ReportEntity { get; set; } = null!;
        public JobStatusType? JobStatusType { get; set; }
    }
}