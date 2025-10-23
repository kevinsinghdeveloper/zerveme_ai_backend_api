namespace zervemedata.Data.Entities.Keys
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ReportConfigurationKey : IResourceKey
    {
        public Guid? Id { get; set; }

        public string CurrentUser { get; set; } = null!;
    }
}