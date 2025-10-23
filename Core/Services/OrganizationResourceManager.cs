using zervemedata.Core.Contracts.Abstractions;
using zervemedata.Data.DataModels.UpdateMeta;
using zervemedata.Data.Entities;

namespace zervemedata.Core.Services
{
    using zervemedata.Data.DataModels.CreateMeta;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using zervemedata.Data;
    using zervemedata.Data.AuthData;
    using Google.Apis.Util;
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Core.Extensions;
    using zervemedata.Data.DataModels.Data;
    using zervemedata.Data.Entities.Keys;
    using IOrganizationResourceManager = zervemedata.Core.Contracts.Abstractions.IOrganizationResourceManager<
        zervemedata.Data.DataModels.Data.OrganizationsInfo,
        zervemedata.Data.DataModels.CreateMeta.OrganizationCreationMetaData,
        zervemedata.Data.Entities.Keys.OrganizationKey,
        zervemedata.Data.DataModels.UpdateMeta.OrganizationUpdateMetaData>;

    public class OrganizationsResourceManager(
        ZervemedataDbContext dbContext,
        UserManager<ApplicationUser> userManager) : IOrganizationResourceManager
    {
        public async Task<OrganizationsInfo> Create(OrganizationCreationMetaData creationMetaData)
        {
            var creatorUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == creationMetaData.CurrentUser);

            if (creatorUser == null)
            {
                throw new Exception("User not found");
            }

            // create org
            var organization = new OrganizationEntity()
            {
                Name = creationMetaData.Name,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Owner = creatorUser
            };

            organization.ContactEntity = new ContactEntity()
            {
                Address1 = creationMetaData.Address1,
                Address2 = creationMetaData.Address2,
                Address3 = creationMetaData.Address3,
                City = creationMetaData.City,
                State = creationMetaData.State,
                Country = creationMetaData.Country,
                ZipCode = creationMetaData.ZipCode,
                PrimaryPhone = creationMetaData.PrimaryPhone,
                SecondaryPhone = creationMetaData.SecondaryPhone,
                NotificationEmail = creationMetaData.NotificationEmail,
                Created = DateTime.Now,
                Updated = DateTime.Now,
            };

            var orgUser = new OrgUserEntity()
            {
                User = creatorUser,
                OrganizationEntity = organization
            };

            await dbContext.OrganizationEntities.AddAsync(organization);
            await dbContext.OrgUserEntities.AddAsync(orgUser);
            await dbContext.SaveChangesAsync();

            return organization.ToDataModel();
        }

        public async Task<OrganizationsInfo?> Get(OrganizationKey key)
        {
            var organization = await dbContext.OrganizationEntities.FirstOrDefaultAsync(o => o.Id == key.Id);

            if (organization == null) return null;

            return organization?.ToDataModel();
        }

        public async Task<OrganizationsInfo> Update(OrganizationUpdateMetaData updateMetaData)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrganizationsInfo>> GetAll()
        {
            var organizations = await dbContext.OrganizationEntities.ToListAsync();

            return organizations.Select(o => o.ToDataModel());
        }

        public async Task<IEnumerable<OrganizationsInfo>> GetAllUsers()
        {
            throw new NotImplementedException();
        }
    }
}