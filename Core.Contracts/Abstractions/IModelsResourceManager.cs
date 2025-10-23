namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IModelsResourceManager<TResource, TResource2, in TResourceKey, in TCreationMetaData,
        in TUpdateMetaData>
        where TResource : IResource
        where TResource2 : IResource
        where TResourceKey : IResourceKey
        where TCreationMetaData : ICreationMetaData
        where TUpdateMetaData : IUpdateMetaData
    {
        Task<TResource> Create(TCreationMetaData creationMetaData);

        Task<TResource> Update(TUpdateMetaData updateMetaData);

        Task<IEnumerable<TResource>> GetAll(TResourceKey key);

        Task<IEnumerable<TResource2>> GetAllModelTypes(TResourceKey key);

        Task<TResource> Get(TResourceKey key);

        Task<bool> SoftDelete(TResourceKey key);
    }
}