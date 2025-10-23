namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class UpdateJobScheduleRequest
    {
        public Guid JobId { get; set; }

        public string? StatusLog { get; set; }
    }
}