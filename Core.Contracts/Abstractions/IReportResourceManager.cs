namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IReportResourceManager<TResource, TResource2, TResource3, in TResourceKey, in TCreationMetaData,
        in TUpdateMetaData>
        where TResource : IResource
        where TResource2 : IResource
        where TResource3 : IResource
        where TResourceKey : IResourceKey
        where TCreationMetaData : ICreationMetaData
        where TUpdateMetaData : IUpdateMetaData
    {
        Task<TResource> Create(TCreationMetaData creationMetaData);

        Task<TResource> Update(TUpdateMetaData updateMetaData);

        Task<IEnumerable<TResource>> GetAll(TResourceKey key);

        Task<IEnumerable<TResource>> GetAllReportsFromProject(TResourceKey key);

        Task<IEnumerable<TResource2>> GetAllReportTypes(TResourceKey key);

        Task<TResource?> Get(TResourceKey key);

        //Task<IEnumerable<IResource>> GetAllRoles();

        Task<TResource3> GetReportDomainOptions(TResourceKey key);

        Task<bool> SoftDelete(TResourceKey key);
    }
}