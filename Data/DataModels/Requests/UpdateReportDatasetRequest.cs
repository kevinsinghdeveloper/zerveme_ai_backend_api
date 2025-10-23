namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class UpdateReportDatasetRequest
    {
        public Guid Id { get; set; }
        public Guid? ReportConfigurationId { get; set; }
        public Guid? ReportId { get; set; }
        public string? DatasetConfig { get; set; } // This will be the JSON string from the UI input
    }
}