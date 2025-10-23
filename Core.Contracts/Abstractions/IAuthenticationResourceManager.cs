namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IAuthenticationResourceManager <in TAuthMetaData, TResponseMetaData>
        where TAuthMetaData : IAuthMetaData
        where TResponseMetaData : IResponseMetaData
    {
        Task<TResponseMetaData?> Authenticate(TAuthMetaData authMetaData);
        
    } 
}