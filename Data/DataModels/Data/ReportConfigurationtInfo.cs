using zervemedata.Data.Entities;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class ReportConfigurationInfo : IResource
    {
        public Guid Id { get; set; }

        public required ReportConfigObject ReportConfig { get; set; }

        public string? VizTemplate { get; set; }

        public required Guid ReportTypeId { get; set; }
    }
}