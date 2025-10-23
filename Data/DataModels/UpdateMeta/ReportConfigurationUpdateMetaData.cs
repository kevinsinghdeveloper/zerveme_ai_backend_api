using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.UpdateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ReportConfigurationUpdateMetaData : IUpdateMetaData
    {
        public Guid Id { get; set; }
        public Guid? ReportTypeId { get; set; }
        public string? VizTemplate { get; set; }
        public ReportConfigObject? ReportConfig { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}