using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class DatasetCreationMetaData : ICreationMetaData
    {
        public string Name { get; set; }

        public required DomainData DomainData { get; set; }

        public string? VizResponseData { get; set; }
        
        public DateTime? Updated { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime? Deleted {get; set; }
        
        public Guid? DWHId { get; set; }

        public required Guid ReportId { get; set; } // TODO make it required

        public string CurrentUser { get; set; } = null!;

        // public DatasetEtlConfiguration? DatasetEtlConfiguration { get; set; }
    }

}