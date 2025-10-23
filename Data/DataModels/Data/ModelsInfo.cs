namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class ModelsInfo : IResource
    {
        public Guid Id { get; set; }

        public Guid? OrganizationId { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public string? ModelConfig { get; set; }

        public string? ModelType { get; set; }

        public Guid? ModelTypeId { get; set; }
    }
}
