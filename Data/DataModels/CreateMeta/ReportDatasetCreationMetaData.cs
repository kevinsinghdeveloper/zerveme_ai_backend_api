using System.ComponentModel.DataAnnotations;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;

    public class ReportDatasetCreationMetaData : ICreationMetaData
    {
        public required Guid ReportConfigurationId { get; set; }
        public required Guid ReportId { get; set; } // TODO change to reportId
        public required string DatasetConfig { get; set; } // This will be the JSON string from the UI input
        public string CurrentUser { get; set; } = null!;
    }
}