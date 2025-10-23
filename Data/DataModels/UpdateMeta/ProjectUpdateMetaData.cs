namespace zervemedata.Data.DataModels.UpdateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ProjectUpdateMetaData : IUpdateMetaData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}