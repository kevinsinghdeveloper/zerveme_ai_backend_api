using System.ComponentModel.DataAnnotations;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;

    public class ReportCreationMetaData : ICreationMetaData
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public Guid ProjectId { get; set; }

        public Guid ReportTypeId { get; set; }

        public Guid JobId { get; set; }

        public Guid ModelId { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}