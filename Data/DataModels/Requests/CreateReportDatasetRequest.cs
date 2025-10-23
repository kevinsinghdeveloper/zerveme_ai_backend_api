namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class CreateReportDatasetRequest
    {
        public required Guid ReportConfigurationId { get; set; }
        public Guid ReportId { get; set; }
        public required string DatasetConfig { get; set; } // This will be the JSON string from the UI input
    }
}