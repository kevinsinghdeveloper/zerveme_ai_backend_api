namespace zervemedata.Data.DataModels.Responses
{
    using zervemedata.Core.Contracts.Abstractions;
    
    public class AuthUserResponse : IResponseMetaData
    {
        public string Token { get; set; }
        
        public DateTime Expiration { get; set; }

        public string UserName { get; set; }
    }
}

