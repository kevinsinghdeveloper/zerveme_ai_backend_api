namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IUserRolesResourceManager <TResource>//, IResourceKey, ICreationMetaData>
        where TResource : IResource
    {
        //Task<bool> Create(ICreationMetaData creationMetaData);
        
        //Task<bool> Update(IUpdateMetaData updateMetaData);

        Task<IEnumerable<TResource>> GetAll();
        
        //Task<IEnumerable<IResource>> GetAllRoles();
        
        //bool SoftDeleteHouse(IResourceKey key);
    } 
}