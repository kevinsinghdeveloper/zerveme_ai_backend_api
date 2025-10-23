namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IOrganizationResourceManager<TResource, in TCreationMetaData, in TResourceKey, in TUpdateMetaData>
        where TCreationMetaData : ICreationMetaData
        where TResourceKey : IResourceKey
        where TUpdateMetaData : IUpdateMetaData
        where TResource : IResource
    {
        Task<TResource> Create(TCreationMetaData creationMetaData);

        Task<TResource> Update(TUpdateMetaData updateMetaData);

        Task<TResource?> Get(TResourceKey key);

        Task<IEnumerable<TResource>> GetAll();

        Task<IEnumerable<TResource>> GetAllUsers();
    }
}