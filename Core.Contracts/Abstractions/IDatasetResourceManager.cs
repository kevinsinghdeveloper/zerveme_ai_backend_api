namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IDatasetResourceManager<in TCreationMetaData, in TResourceKey, in TUpdateMetaData, TResource,
        in TQueryingParameters, TQueriedResponse, TQueriedResponse2>
        where TCreationMetaData : ICreationMetaData
        where TResourceKey : IResourceKey
        where TUpdateMetaData : IUpdateMetaData
        where TResource : IResource
        where TQueryingParameters : IQueryingParameters
        where TQueriedResponse : IDatasetQueriedResponse
        where TQueriedResponse2 : IDatasetQueriedResponse
    {
        Task<TResource> Create(TCreationMetaData creationMetaData);
        
        Task<TResource> Update(TUpdateMetaData updateMetaData);

        Task<TResource> Get(TResourceKey key);

        Task<TResource> GetDatasetWithEtlConfig(TResourceKey key);

        Task<IEnumerable<TResource>> GetAll(TResourceKey key);

        Task<TQueriedResponse> GetData(TQueryingParameters queryingParameters, string currentUser);

        Task<TQueriedResponse2> GetVizData(TQueryingParameters queryingParameters, string currentUser);

        Task<IEnumerable<TResource>> GetAllDatasetNames(TResourceKey key); // working on ...

        Task<TResource> GetDomainOptions(TResourceKey key); 
    }
}