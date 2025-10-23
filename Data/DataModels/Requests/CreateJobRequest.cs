namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class CreateJobRequest
    {
        public Guid JobFreqTypeId { get; set; }
    }
}