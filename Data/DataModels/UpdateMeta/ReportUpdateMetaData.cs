namespace zervemedata.Data.DataModels.UpdateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ReportUpdateMetaData : IUpdateMetaData
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ProjectId { get; set; }

        public Guid? ReportTypeId { get; set; }

        public Guid? JobFreqTypeId { get; set; }

        public Guid? ModelId { get; set; }

        public string? DatasetConfig { get; set; } // This will be the JSON string from the UI input

        public string CurrentUser { get; set; } = null!;
    }
}