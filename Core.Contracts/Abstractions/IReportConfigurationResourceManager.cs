namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IReportConfigurationResourceManager<TResource, TResource2, in TResourceKey, in TResourceKey2,
        in TCreationMetaData,
        in TCreationMetaData2,
        in TUpdateMetaData>
        where TResource : IResource
        where TResource2 : IResource
        where TResourceKey : IResourceKey
        where TResourceKey2 : IResourceKey
        where TCreationMetaData : ICreationMetaData
        where TUpdateMetaData : IUpdateMetaData
    {
        Task<TResource> Create(TCreationMetaData creationMetaData);

        Task<TResource2> CreateReportType(TCreationMetaData2 creationMetaData);

        Task<TResource> Update(TUpdateMetaData updateMetaData);

        Task<IEnumerable<TResource>> GetAll(TResourceKey key);

        Task<TResource?> Get(TResourceKey key);

        Task<IEnumerable<TResource2>> GetAllReportTypes(TResourceKey2 key);

        Task<bool> SoftDelete(TResourceKey key);
    }
}