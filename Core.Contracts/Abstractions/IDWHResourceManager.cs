namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IDwResourceManager<in TCreationMetaData, in TUpdateMetaData, TResource>
        where TCreationMetaData: ICreationMetaData
        where TUpdateMetaData : IUpdateMetaData
        where TResource : IResource
    {
        Task<bool> Create(TCreationMetaData creationMetaData);
        
        Task<TResource> Update(TUpdateMetaData updateMetaData);
        
        Task<TResource> Get(Guid Id);
        
        Task<IEnumerable<TResource>> GetAll();
        
        Task<TResource> SwapSchemas(Guid Id);
    }
}