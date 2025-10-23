using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    public class CreateDatasetRequest
    {
        public string Name { get; set; }

        public required DomainData DomainData { get; set; }

        public string? VizResponseData { get; set; }

        public Guid? DwhId { get; set; }
        
        public required Guid ReportId { get; set; } // TODO make it required

        // public DatasetEtlConfiguration? DatasetEtlConfiguration { get; set; }
    }
}