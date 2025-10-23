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
    using IModelsResourceManager = zervemedata.Core.Contracts.Abstractions.IModelsResourceManager<
        zervemedata.Data.DataModels.Data.ModelsInfo,
        zervemedata.Data.DataModels.Data.ModelTypeInfo,
        zervemedata.Data.Entities.Keys.ModelsKey,
        zervemedata.Data.DataModels.CreateMeta.ModelsCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ModelsUpdateMetaData>;

    public class ModelsResourceManager(ZervemedataDbContext dbContext) : IModelsResourceManager
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

        public async Task<ModelsInfo> Create(ModelsCreationMetaData creationMetaData)
        {
            var (user, _, organization) = await ValidateUserAndOrg(creationMetaData.CurrentUser);

            // get model type entity
            var modelType =
                await dbContext.ModelTypeEntities.FirstOrDefaultAsync(mt => mt.Id == creationMetaData.ModelTypeId);

            if (modelType == null)
            {
                throw new Exception("Model type not found.");
            }

            // create model entity
            var modelEntity = new ModelEntity
            {
                Name = creationMetaData.Name,
                Description = creationMetaData.Description,
                ModelConfig = creationMetaData.ModelConfig,
                ModelTypeEntity = modelType,
                OrganizationEntity = organization,
                CreatedUser = user,
                Created = DateTime.Now
            };

            // add model entity to db context
            await dbContext.ModelEntities.AddAsync(modelEntity);
            await dbContext.SaveChangesAsync();
            return modelEntity.ToDataModel();
        }


        // TODO add org user updates here as well
        public async Task<ModelsInfo> Update(ModelsUpdateMetaData updateMetaData)
        {
            var (user, _, organization) = await ValidateUserAndOrg(updateMetaData.CurrentUser);

            var modelEntity = await dbContext.ModelEntities
                .Include(m => m.ModelTypeEntity)
                .Include(m => m.OrganizationEntity)
                .FirstOrDefaultAsync(m => m.Id == updateMetaData.Id && m.OrganizationId == organization.Id &&
                                          m.Deleted == null);

            if (modelEntity == null)
            {
                throw new Exception("Model not found.");
            }

            modelEntity.Name = updateMetaData.Name;
            modelEntity.Description = updateMetaData.Description;
            modelEntity.ModelConfig = updateMetaData.ModelConfig;
            modelEntity.UpdatedUser = user;
            modelEntity.Updated = DateTime.Now;

            if (updateMetaData.ModelTypeId != null)
            {
                var modelType =
                    await dbContext.ModelTypeEntities.FirstOrDefaultAsync(mt => mt.Id == updateMetaData.ModelTypeId);
                if (modelType == null)
                {
                    throw new Exception("Model type not found.");
                }

                modelEntity.ModelTypeEntity = modelType;
            }

            await dbContext.SaveChangesAsync();
            return modelEntity.ToDataModel();
        }

        public async Task<IEnumerable<ModelsInfo>> GetAll(ModelsKey key)
        {
            var (_, _, organization) = await ValidateUserAndOrg(key.CurrentUser);

            var models = await dbContext.ModelEntities
                .Include(m => m.ModelTypeEntity)
                .Include(m => m.OrganizationEntity)
                .Where(m => m.OrganizationId == organization.Id &&
                            m.Deleted == null)
                .ToListAsync();

            if (models == null)
            {
                throw new Exception("No Models were found.");
            }

            return models.Select(m => m.ToDataModel());
        }

        public async Task<IEnumerable<ModelTypeInfo>> GetAllModelTypes(ModelsKey key)
        {
            var (_, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            var modelTypes = await dbContext.ModelTypeEntities.Where(m =>
                    m.Deleted == null)
                .ToListAsync();

            if (modelTypes == null)
            {
                throw new Exception("No Model types were found.");
            }

            return modelTypes.Select(mt => mt.ToDataModel());
        }

        public async Task<ModelsInfo> Get(ModelsKey key)
        {
            var (_, _, organization) = await ValidateUserAndOrg(key.CurrentUser);

            if (key.Id == null)
            {
                throw new Exception("Id not found.");
            }

            var modelEntity = await dbContext.ModelEntities
                .Include(m => m.ModelTypeEntity)
                .Include(m => m.OrganizationEntity)
                .FirstOrDefaultAsync(m => m.Id == key.Id && m.OrganizationId == organization.Id &&
                                          m.Deleted == null);

            if (modelEntity == null)
            {
                throw new Exception("Model not found.");
            }

            return modelEntity.ToDataModel();
        }

        public async Task<bool> SoftDelete(ModelsKey key)
        {
            var (_, _, organization) = await ValidateUserAndOrg(key.CurrentUser);

            if (key.Id == null)
            {
                throw new Exception("Id not found.");
            }

            var modelEntity = await dbContext.ModelEntities
                .Include(m => m.ModelTypeEntity)
                .Include(m => m.OrganizationEntity)
                .FirstOrDefaultAsync(m => m.Id == key.Id && m.OrganizationId == organization.Id &&
                                          m.Deleted == null);

            if (modelEntity == null)
            {
                throw new Exception("Model not found.");
            }

            modelEntity.Deleted = DateTime.Now;
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}