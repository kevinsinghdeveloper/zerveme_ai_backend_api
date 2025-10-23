namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    public class UserResponseMetaData : IAuthMetaData
    {
        public string userNameOrEmail { get; set; }
        
        public string password { get; set; }
    }

}