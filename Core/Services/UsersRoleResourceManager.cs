namespace zervemedata.Core.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using zervemedata.Data;
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Core.Extensions;
    using zervemedata.Data.DataModels.Data;

    
    using IUserRolesResourceManager = zervemedata.Core.Contracts.Abstractions.IUserRolesResourceManager<
        zervemedata.Data.DataModels.Data.RoleInfo>;

    public class UsersRolesResourceManager(
        RoleManager<IdentityRole> roleManager) : IUserRolesResourceManager
    {
        public async Task<IEnumerable<RoleInfo>> GetAll()
        {
            var roles = await roleManager.Roles.ToListAsync();
            
            return roles.Select(role => role.ToDataModel());
        }
       
    }
}