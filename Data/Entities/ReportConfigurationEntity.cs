using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;
using zervemedata.Data.Enumerations;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.Entities
{
    public class ReportConfigurationEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [ForeignKey("ReportType")] public Guid ReportTypeId { get; set; }
        public required ReportTypeEntity ReportType { get; set; }

        public required ReportConfigObject ReportConfig { get; set; }

        public string? VizTemplate { get; set; }

        [ForeignKey("UpdatedUser")] public string? UpdatedUserId { get; set; }
        [ForeignKey("CreatedUser")] public string? CreatedUserId { get; set; }

        public ApplicationUser? UpdatedUser { get; set; }
        public required ApplicationUser CreatedUser { get; set; }

        // Base entity properties
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }

        // Any additional job-specific properties

        // TODO -- table fields
        // Target LLMs
        //
        
    }
}