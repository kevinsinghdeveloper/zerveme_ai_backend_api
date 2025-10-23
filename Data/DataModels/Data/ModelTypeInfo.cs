namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class ModelTypeInfo : IResource
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}