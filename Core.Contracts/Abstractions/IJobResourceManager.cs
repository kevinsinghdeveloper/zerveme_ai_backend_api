namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IJobResourceManager<TResource, TResource2, in TResourceKey, in TCreationMetaData,
        in TUpdateMetaDataForUpdateJob,
        in TUpdateMetaDataForJobScheduleUpdate>
        where TResource : IResource
        where TResource2 : IResource
        where TResourceKey : IResourceKey
        where TCreationMetaData : ICreationMetaData
        where TUpdateMetaDataForUpdateJob : IUpdateMetaData
        where TUpdateMetaDataForJobScheduleUpdate : IUpdateMetaData
    {
        Task<TResource> Create(TCreationMetaData creationMetaData);

        Task<TResource> Update(TUpdateMetaDataForUpdateJob updateMetaData);

        Task<IEnumerable<TResource>> GetAll(TResourceKey key);

        Task<IEnumerable<TResource2>> GetAllJobFreqTypes(TResourceKey key);

        Task<TResource?> Get(TResourceKey key);

        Task<bool> SoftDelete(TResourceKey key);

        Task<bool> StartJob(TResourceKey key);
        Task<bool> CompleteJob(TResourceKey key);
        Task<bool> QueueJob(TResourceKey key);
        Task<bool> CancelJob(TResourceKey key);
        

        // Add  other job specific methods
    }
}