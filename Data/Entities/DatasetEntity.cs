using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class DatasetEntity : IBaseEntity
    {
        public Guid Id { get; set;}

        public string Name { get; set; }

        public required DomainData DomainData { get; set; }
        
        public DateTime? DataRefreshedDate { get; set; }

        [ForeignKey("Dwh")]
        public Guid? DWHId { get; set; }

        public DWHEntity? Dwh { get; set; }

        public List<DatasetRunQueryTrackerEntity> DatasetRunQueryTrackerEntities { get; set; } = null!;

        [ForeignKey("ReportEntity")] public Guid? ReportId { get; set; }

        public ReportEntity ReportEntity { get; set; } = null!;

        public string? VizResponseData { get; set; }

        [ForeignKey("CreatedUser")] public string? CreatedUserId { get; set; }

        [ForeignKey("UpdatedUser")] public string? UpdatedUserId { get; set; }

        public ApplicationUser? CreatedUser { get; set; }

        public ApplicationUser? UpdatedUser { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }

        public OrganizationEntity? OrganizationEntity { get; set; }
        [ForeignKey("OrganizationEntity")] public Guid? OrganizationId { get; set; }
        
    }
}