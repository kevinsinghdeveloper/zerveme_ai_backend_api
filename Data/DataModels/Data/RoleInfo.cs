namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;

    public class RoleInfo : IResource
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}