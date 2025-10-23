using System.Data;
using Microsoft.AspNetCore.Authorization;
using zervemedata.Data.DataModels.UpdateMeta;
using zervemedata.Data.Entities;
using zervemedata.Data.Enumerations;

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
    using IProjectResourceManager = zervemedata.Core.Contracts.Abstractions.IProjectResourceManager<
        zervemedata.Data.DataModels.Data.ProjectInfo,
        zervemedata.Data.Entities.Keys.ProjectKey,
        zervemedata.Data.DataModels.CreateMeta.ProjectCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ProjectUpdateMetaData>;
    
    public class ProjectResourceManager(
        ZervemedataDbContext dbContext) : IProjectResourceManager
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
        
        public async Task<ProjectInfo> Create(ProjectCreationMetaData creationMetaData)
        {
            var (_, _, organization) = await ValidateUserAndOrg(creationMetaData.CurrentUser);

            var organizationId = creationMetaData.OrganizationId ?? organization.Id;

            // Validate input
            if (creationMetaData == null)
            {
                throw new ArgumentNullException(nameof(creationMetaData), "Creation metadata cannot be null.");
            }

            // Check if project name exists for the organization
            var projectExists = await dbContext.ProjectEntities
                .AnyAsync(p => p.Name == creationMetaData.Name && p.OrganizationId == organizationId);

            if (projectExists)
            {
                throw new DuplicateNameException(
                    $"A project with name '{creationMetaData.Name}' already exists in the organization.");
            }

            // Get user
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == creationMetaData.CurrentUser);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {creationMetaData.CurrentUser} not found.");
            }

            // Create project
            var project = new ProjectEntity
            {
                Name = creationMetaData.Name,
                Description = creationMetaData.Description,
                OrganizationId = organization.Id,
                CreatedUser = user
            };

            dbContext.ProjectEntities.Add(project);
            await dbContext.SaveChangesAsync();

            return project.ToDataModel();
        }

        public async Task<ProjectInfo> Update(ProjectUpdateMetaData updateMetaData)
        {
            var (user, _, organization) = await ValidateUserAndOrg(updateMetaData.CurrentUser);

            var project = await dbContext.ProjectEntities
                .FirstOrDefaultAsync(p => p.Id == updateMetaData.Id
                                          && p.OrganizationId == organization.Id);

            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID {updateMetaData.Id} not found.");
            }

            project.Updated = DateTime.Now;
            project.Name = updateMetaData.Name;
            project.Description = updateMetaData.Description;
            project.UpdatedUser = user;

            dbContext.ProjectEntities.Update(project);

            await dbContext.SaveChangesAsync();

            return project.ToDataModel();
        }

        public Task<IEnumerable<ProjectInfo>> GetAllUserProjects(ProjectKey key)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectInfo>> GetAllOrgProjects(ProjectKey key)
        {
            var (_, _, organization) = await ValidateUserAndOrg(key.CurrentUser);

            var orgProjects = await dbContext.ProjectEntities
                .Include(p => p.ReportEntities.Where(r => r.Deleted == null))  // Only include non-deleted reports
                .ThenInclude(r => r.ModelEntity)
                .Where(p => p.OrganizationId == organization.Id && p.Deleted == null)  // Only non-deleted projects
                .ToListAsync();
    
            if (orgProjects == null)
            {
                throw new KeyNotFoundException($"Organization with ID {organization.Id} not found.");
            }

            // limit to users ord
            return orgProjects
                .Select(p => p.ToDataModel())
                .ToList();
        }

        public async Task<ProjectInfo?> Get(ProjectKey key)
        {
            var (_, _, organization) = await ValidateUserAndOrg(key.CurrentUser);

            var project = await dbContext.ProjectEntities
                .Include(p => p.ReportEntities.Where(r => r.Deleted == null))  // Only include non-deleted reports
                .FirstOrDefaultAsync(p => p.Id == key.Id
                                          && p.OrganizationId == organization.Id
                                          && p.Deleted == null);  // Only non-deleted project

            return project?.ToDataModel();
        }

        public async Task<bool> SoftDelete(ProjectKey key)
        {
            var (user, _, organization) = await ValidateUserAndOrg(key.CurrentUser);

            var project = await dbContext.ProjectEntities.FirstOrDefaultAsync(p => p.Id == key.Id
                && p.OrganizationId == organization.Id);

            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID {key.Id} not found.");
            }

            // soft delete children as well
            // Report -> Job -> JobSchedule
            var reports = await dbContext.ReportEntities
                .Where(r => r.ProjectId == project.Id)
                .ToListAsync();

            foreach (var report in reports)
            {
                var job = report.JobEntity;

                if (job == null)
                {
                    continue;
                }

                job.Deleted = DateTime.Now;
                job.UpdatedUser = user;
                dbContext.JobEntities.Update(job);

                report.Deleted = DateTime.Now;
                dbContext.ReportEntities.Update(report);
            }
                
            
            project.Deleted = DateTime.Now;
            project.UpdatedUser = user;
            dbContext.ProjectEntities.Update(project);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}