using Core.Contracts.ContractModels;

namespace zervemedata.Data.DataModels.Querying
{
    using zervemedata.Core.Contracts.Abstractions;
    
    public class DatasetDWHQueryingParameters : IQueryingParameters
    {
        public Guid? ReportId { get; set; }
        public Guid? DatasetId { get; set; } 

        public List<string>? Attributes { get; set; }

        public List<string>? KPIs { get; set; }

        public int? RowLimit { get; set; }

        public List<FilterCondition>? Filters { get; set; }
    }
}