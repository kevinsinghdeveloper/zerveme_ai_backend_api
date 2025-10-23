using zervemedata.Data.Entities;

namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class ReportDatasetInfo : IResource
    {
        public Guid Id { get; set; }

        public required ReportInfo ReportInfo { get; set; }

        public required string DatasetConfig { get; set; }
    }
}