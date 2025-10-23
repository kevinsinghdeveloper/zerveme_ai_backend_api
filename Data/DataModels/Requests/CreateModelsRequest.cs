namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class CreateModelsRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string ModelConfig { get; set; }
        public required Guid ModelTypeId { get; set; }
    }
}