namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IReportDatasetResourceManager<TResource, in TResourceKey, in TCreationMetaData,
        in TUpdateMetaData>
        where TResource : IResource
        where TResourceKey : IResourceKey
        where TCreationMetaData : ICreationMetaData
        where TUpdateMetaData : IUpdateMetaData
    {
        Task<TResource> Create(TCreationMetaData creationMetaData);

        Task<TResource> Update(TUpdateMetaData updateMetaData);

        Task<IEnumerable<TResource>> GetAll(TResourceKey key);

        Task<TResource?> Get(TResourceKey key);

        Task<bool> SoftDelete(TResourceKey key);
    }
}