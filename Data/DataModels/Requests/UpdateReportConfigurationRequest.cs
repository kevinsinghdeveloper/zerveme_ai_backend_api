using zervemedata.Data.JsonDataModels;

namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class UpdateReportConfigurationRequest
    {
        public Guid Id { get; set; }
        public Guid? ReportTypeId { get; set; }
        public ReportConfigObject? ReportConfig { get; set; }

        public string? VizTemplate { get; set; }
    }
}