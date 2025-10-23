using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.Enumerations;

namespace zervemedata.Data.Entities
{
    using zervemedata.Data.AuthData;

    public class DatasetRunQueryTrackerEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [ForeignKey("DatasetEntity")]
        public Guid DatasetId { get; set; }

        [ForeignKey("Dwh")] public Guid DwhId { get; set; }

        [ForeignKey("CreatedUser")] public string? CreatedUserId { get; set; }

        [MaxLength(5000)]
        public string? MetaData { get; set; }

        [MaxLength(5000)]
        public string? Query { get; set; }

        public int QueryRunTimeElapsedSecs { get; set; }

        public int RowLimit { get; set; }

        public int RowsReturned { get; set; }

        public ApplicationUser? CreatedUser { get; set; }
        
        public virtual DatasetEntity? DatasetEntity { get; set; }

        public DWH? Dwh { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}