namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class OrganizationsInfo : IResource
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}