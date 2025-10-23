namespace zervemedata.Data.DataModels.UpdateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ReportDatasetUpdateMetaData : IUpdateMetaData
    {
        public Guid Id { get; set; }

        public Guid? ReportConfigurationId { get; set; }
        public Guid? ReportId { get; set; } // TODO change to reportId
        public string? DatasetConfig { get; set; } // This will be the JSON string from the UI input

        public string CurrentUser { get; set; } = null!;
    }
}