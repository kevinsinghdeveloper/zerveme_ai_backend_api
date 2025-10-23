using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class CreateReportConfigurationRequest
    {
        public required Guid ReportTypeId { get; set; }
        public required ReportConfigObject ReportConfig { get; set; }

        public string? VizTemplate { get; set; }
    }
}