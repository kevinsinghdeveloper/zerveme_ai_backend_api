using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class CreateReportTypeRequest
    {
        public required ReportConfigObject ReportConfig { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? VizTemplate { get; set; }
    }
}