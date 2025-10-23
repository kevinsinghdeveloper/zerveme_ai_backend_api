namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.JsonDataModels;

    public class DatasetData : IResource
    {
        public Guid Id { get; set;}

        public string Name { get; set; }

        public DomainData? DomainData { get; set; }

        public string? VizResponseData { get; set; }
        
        public DateTime? Updated { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime? Deleted {get; set; }

        public DateTime? DataRefreshedDate { get; set; }
        
        public Guid? DWHId { get; set; }
    }
}