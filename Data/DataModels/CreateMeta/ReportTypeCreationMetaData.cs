using System.ComponentModel.DataAnnotations;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;

    public class ReportTypeCreationMetaData : ICreationMetaData
    {
        public required ReportConfigObject ReportConfig { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? VizTemplate { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}