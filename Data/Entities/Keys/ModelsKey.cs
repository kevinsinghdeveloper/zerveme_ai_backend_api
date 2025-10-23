namespace zervemedata.Data.Entities.Keys
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ModelsKey : IResourceKey
    {
        public Guid? Id { get; set; } = null;

        public string CurrentUser { get; set; } = null!;
    }
}