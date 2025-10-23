using System.ComponentModel.DataAnnotations;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;

    public class ProjectCreationMetaData : ICreationMetaData
    {
        [MaxLength(150)] public string Name { get; set; } = null!;

        [MaxLength(1000)] public string? Description { get; set; }

        public Guid? OrganizationId { get; set; }

        public string CurrentUser { get; set; } = null!;

    }
}