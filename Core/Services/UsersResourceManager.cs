using Microsoft.IdentityModel.Tokens;
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
    
    using IUserResourceManager = zervemedata.Core.Contracts.Abstractions.IUserResourceManager<
        zervemedata.Data.DataModels.Data.UserInfo,
        zervemedata.Data.Entities.Keys.UserKey,
        zervemedata.Data.DataModels.CreateMeta.UserCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.UserUpdateMetaData>;

    public class UsersResourceManager(
        ZervemedataDbContext dbContext,
        UserManager<ApplicationUser> userManager) : IUserResourceManager
    {
        private async Task<(ApplicationUser user, OrgUserEntity orgUser, OrganizationEntity organization)>
            ValidateUserAndOrg(string userId)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found.");

            var orgUser = await dbContext.OrgUserEntities.FirstOrDefaultAsync(ou => ou.UserId == user.Id);
            if (orgUser == null) throw new Exception("User not found in organization.");

            var organization =
                await dbContext.OrganizationEntities.FirstOrDefaultAsync(o => o.Id == orgUser.OrganizationId);
            if (organization == null) throw new Exception("Organization not found.");

            return (user, orgUser, organization);
        }
        
        public async Task<UserInfo> Create(UserCreationMetaData creationMetaData)
        {
            var (_, _, organization) = await ValidateUserAndOrg(creationMetaData.CurrentUser);

            var user = new ApplicationUser
            {
                Email = creationMetaData.EmailAddress,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = creationMetaData.UserName,
                FirstName = creationMetaData.FirstName,
                LastName = creationMetaData.LastName,
                Created = DateTime.Now,
                // Set the contact entity for the user
                ContactEntity = new ContactEntity()
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
                    NotificationEmail = creationMetaData.NotificationEmail
                }
            };

            // check roles
            if (creationMetaData.Roles == null || !creationMetaData.Roles.Any())
            {
                throw new Exception("User must have at least one role.");
            }

            // Create the user in the user manager
            var result = await userManager.CreateAsync(user, creationMetaData.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create the user. Errors: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Assign roles to the user
            var roleResult = await userManager.AddToRolesAsync(user, creationMetaData.Roles.Select(r => r.ToString()));
            if (!roleResult.Succeeded)
            {
                throw new Exception("Failed to add roles to user. Errors: " +
                                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // Create the organization-user relation
            var orgUser = new OrgUserEntity()
            {
                OrganizationId = organization.Id,
                User = user
            };

            await dbContext.OrgUserEntities.AddAsync(orgUser);
            await dbContext.SaveChangesAsync();

            // Return the user data model
            return user.ToDataModel(organization);
        }


        // TODO add org user updates here as well
        public async Task<UserInfo> Update(UserUpdateMetaData updateMetaData)
        {
            var (_, _, organization) = await ValidateUserAndOrg(updateMetaData.CurrentUser);
            
            // Fetch the user by ID, including the ContactEntity
            var user = await userManager.Users
                .Include(u => u.ContactEntity)
                .FirstOrDefaultAsync(u => u.Id == updateMetaData.Id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Check users organization
            var orgUser = await dbContext.OrgUserEntities
                .FirstOrDefaultAsync(ou => ou.User == user && ou.OrganizationId == organization.Id);

            if (orgUser == null)
            {
                throw new Exception("User does not belong to the specified organization.");
            }

            // Update user details
            user.FirstName = updateMetaData.firstName ?? user.FirstName;
            user.LastName = updateMetaData.lastName ?? user.LastName;
            user.Email = updateMetaData.emailAddress ?? user.Email;
            user.UserName = updateMetaData.userName ?? user.UserName;

            // Ensure the ContactEntity exists
            user.ContactEntity ??= new ContactEntity();

            // Update contact details
            user.ContactEntity.Address1 = updateMetaData.Address1 ?? user.ContactEntity.Address1;
            user.ContactEntity.Address2 = updateMetaData.Address2 ?? user.ContactEntity.Address2;
            user.ContactEntity.Address3 = updateMetaData.Address3 ?? user.ContactEntity.Address3;
            user.ContactEntity.City = updateMetaData.City ?? user.ContactEntity.City;
            user.ContactEntity.State = updateMetaData.State ?? user.ContactEntity.State;
            user.ContactEntity.Country = updateMetaData.Country ?? user.ContactEntity.Country;
            user.ContactEntity.ZipCode = updateMetaData.ZipCode ?? user.ContactEntity.ZipCode;
            user.ContactEntity.PrimaryPhone = updateMetaData.PrimaryPhone ?? user.ContactEntity.PrimaryPhone;
            user.ContactEntity.SecondaryPhone = updateMetaData.SecondaryPhone ?? user.ContactEntity.SecondaryPhone;
            user.ContactEntity.NotificationEmail =
                updateMetaData.NotificationEmail ?? user.ContactEntity.NotificationEmail;

            // Attempt to update the user
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to update the user. Errors: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Return the updated user data model with organization info
            return user.ToDataModel(organization);
        }

        public async Task<IEnumerable<UserInfo>> GetAll(UserKey key)
        {
            var (currentUser, currOrgUser, currOrganization) = await ValidateUserAndOrg(key.CurrentUser);

            // Include related entities to ensure complete data retrieval
            var orgUsers = await dbContext.OrgUserEntities
                .Include(ou => ou.User) // Include the ApplicationUser navigation property
                .Where(ou => ou.OrganizationId == currOrganization.Id)
                .ToListAsync();

            var users = orgUsers.Select(o => o.User).ToList();

            // Map users with organization info
            return users.Select(user => user.ToDataModel(currOrganization));
        }
        public async Task<bool> SoftDeleteUser(UserKey key)
        {
            // Validate current user
            var (_, _, organization) = await ValidateUserAndOrg(key.CurrentUser);
            
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == key.Id);

            // Check if user in question is part of the same org
            var orgUser = await dbContext.OrgUserEntities
                .FirstOrDefaultAsync(ou => ou.User == user && ou.OrganizationId == organization.Id);

            if (orgUser == null)
            {
                throw new Exception("User not found in current organization.");
            }
            
            if (user == null) return false;
            
            user.Deleted = DateTimeOffset.UtcNow.DateTime;
                
            await userManager.UpdateAsync(user);

            return true;
        }
        
        public async Task<UserInfo?> Get(UserKey key)
        {
            // check current user
            var (currentUser, _, currOrganization) = await ValidateUserAndOrg(key.CurrentUser);

            var currentUserId = key.Id ?? currentUser.Id;

            var orgUsers = await dbContext.OrgUserEntities
                .Include(ou => ou.User) // Include the ApplicationUser navigation property
                .Where(ou => ou.OrganizationId == currOrganization.Id && ou.UserId == currentUserId)
                .FirstOrDefaultAsync();

            if (orgUsers == null)
            {
                throw new Exception("User not found in organization.");
            }

            var user = orgUsers.User;

            // return result
            return user.ToDataModel(currOrganization);
        }
    }
}
