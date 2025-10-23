namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class CreateReportRequest
    {
        [MaxLength(150)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A name must be specified")]
        public string Name { get; set; } = null!;

        [MaxLength(1000)] public string? Description { get; set; }

        public Guid ProjectId { get; set; }

        public Guid ReportTypeId { get; set; }

        public Guid JobFreqTypeId { get; set; }

        public Guid ModelId { get; set; }

        public required string DatasetConfig { get; set; } // This will be the JSON string from the UI input
    }
}