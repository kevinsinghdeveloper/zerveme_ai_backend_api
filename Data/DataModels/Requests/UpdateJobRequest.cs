namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class UpdateJobRequest
    {
        public Guid Id { get; set; }
        public Guid? JobFreqTypeId { get; set; }
    }
}