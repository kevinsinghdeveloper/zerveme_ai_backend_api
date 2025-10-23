using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    public class UpdateDatasetRequest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public DomainData? DomainData { get; set; }

        public string? VizResponseData { get; set; }

        public DateTime? Updated { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Deleted { get; set; }

        public DateTime? DataRefreshedDate { get; set; }
        
        public Guid? DWHId { get; set; }

        public Guid? ReportId { get; set; }

        // public DatasetEtlConfiguration? DatasetEtlConfiguration { get; set; }
    }
}