using zervemedata.Data.Entities;

namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class ReportInfo : IResource
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public Guid ProjectId { get; set; }

        public ReportTypeInfo ReportType { get; set; } = null!;
        
        public JobInfo? Job { get; set; } = null!;

        public Guid ReportConfigurationId { get; set; }

        public string? DatasetConfig { get; set; }

        public Object? Model { get; set; } = null!;

        public DatasetData? DatasetData { get; set; } = null!;
    }
}