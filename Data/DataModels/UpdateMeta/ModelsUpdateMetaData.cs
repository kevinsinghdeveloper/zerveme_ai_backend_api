namespace zervemedata.Data.DataModels.UpdateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ModelsUpdateMetaData : IUpdateMetaData
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ModelConfig { get; set; }
        public Guid? ModelTypeId { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}