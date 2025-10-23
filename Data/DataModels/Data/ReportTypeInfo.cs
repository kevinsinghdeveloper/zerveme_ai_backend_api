using zervemedata.Data.Entities;
using zervemedata.Data.Enumerations;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class ReportTypeInfo : IResource
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public ReportConfigObject? ReportConfig { get; set; }
    }
}