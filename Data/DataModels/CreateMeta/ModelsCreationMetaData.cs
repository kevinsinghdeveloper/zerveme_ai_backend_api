using System.ComponentModel.DataAnnotations;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;

    public class ModelsCreationMetaData : ICreationMetaData
    {
        public string CurrentUser { get; set; } = null!;
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string ModelConfig { get; set; }
        public required Guid ModelTypeId { get; set; }
    }
}