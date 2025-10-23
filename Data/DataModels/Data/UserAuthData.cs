namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;
    public class UserAuthData: IResource
    {
        public string Token { get; set; }
        
        public DateTime Expiration { get; set; }
    }
}