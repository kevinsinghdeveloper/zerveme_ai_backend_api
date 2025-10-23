using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;
using zervemedata.Data.Enumerations;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.Entities
{
    public class ReportDatasetEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [ForeignKey("ReportConfigurationEntity")]
        public Guid ReportConfigurationId { get; set; }

        public required ReportConfigurationEntity ReportConfigurationEntity { get; set; }

        // [ForeignKey("JobEntity")] public Guid? JobId { get; set; }
        //
        // public JobEntity JobEntity { get; set; } = null!;
        [ForeignKey("ReportEntity")] public Guid ReportId { get; set; }

        public required ReportEntity ReportEntity { get; set; } = null!;

        // this needs to be standardized
        public required string DatasetConfig { get; set; } // This will be the JSON string from the UI input

        [ForeignKey("UpdatedUser")] public string? UpdatedUserId { get; set; }
        [ForeignKey("CreatedUser")] public string? CreatedUserId { get; set; }

        public ApplicationUser? UpdatedUser { get; set; } = null!;
        public ApplicationUser? CreatedUser { get; set; } = null!;

        // Base entity properties
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }

        // Any additional job-specific properties
    }
}