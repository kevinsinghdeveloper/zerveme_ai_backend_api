using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class DatasetUpdateMetaData : IUpdateMetaData
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public DomainData? DomainData { get; set; }

        public string? VizResponseData { get; set; }
        
        public Guid? DWHId { get; set; }

        public DateTime? DataRefreshedDate { get; set; }
        
        public DatasetEtlConfiguration? DatasetEtlConfigObject { get; set; }

        public Guid? ReportId { get; set; }
    }

}