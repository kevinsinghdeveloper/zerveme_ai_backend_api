using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using zervemedata.Data;
using zervemedata.Data.Entities;
using zervemedata.Data.Enumerations;

namespace zervemedata.data.IdentityAuth
{
    using Microsoft.AspNetCore.Identity;
    using zervemedata.Data.AuthData;
    
    public class IdentityDataInitializer
    {
        public static void SeedData (UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ZervemedataDbContext context)
        {

            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedReportTypes(context);
            SeedJobFreqTypes(context);
            SeedModelTypes(context);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByNameAsync("Admin").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "ksingh";
                user.Email = "kevinsingh.developer@gmail.com";

                user.FirstName = "Kevin";
                user.LastName = "Singh";

                IdentityResult result = userManager.CreateAsync
                (user, "password123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, RoleConstants.User).Wait();
                    userManager.AddToRoleAsync(user, RoleConstants.Admin).Wait();
                    userManager.AddToRoleAsync(user, RoleConstants.SuperAdmin).Wait();
                }
            }
            
        }

        public static void SeedRoles (RoleManager<IdentityRole> roleManager)
        {

            /*
            foreach (var rolename in GlobalVariables.Database.GLB_ROLES)
            {
                if (!roleManager.RoleExistsAsync(rolename).Result)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = rolename;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }
            */
            
            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                if (!roleManager.RoleExistsAsync(role.ToString()).Result)
                {
                    var identityRole = new IdentityRole { Name = role.ToString() };
                    IdentityResult roleResult = roleManager.CreateAsync(identityRole).Result;
                }
            }
            
        }

        private static void SeedJobFreqTypes(ZervemedataDbContext dbContext)
        {
            var jobFreqTypes = new List<JobFreqTypeEntity>
            {
                new JobFreqTypeEntity
                    { Name = "OneOff", Description = "One-time job frequency", ScheduleType = ScheduleType.OneOff },
                new JobFreqTypeEntity
                    { Name = "Daily", Description = "Occurs every day", ScheduleType = ScheduleType.Daily },
                new JobFreqTypeEntity
                    { Name = "Weekly", Description = "Occurs every week", ScheduleType = ScheduleType.Weekly },
                new JobFreqTypeEntity
                    { Name = "Monthly", Description = "Occurs every month", ScheduleType = ScheduleType.Monthly }
            };

            foreach (var freqType in jobFreqTypes)
            {
                if (!dbContext.JobFreqTypeEntities.Any(j => j.Name == freqType.Name))
                {
                    dbContext.JobFreqTypeEntities.Add(freqType);
                }
            }

            dbContext.SaveChanges();
        }
        private static void SeedReportTypes(ZervemedataDbContext dbContext)
        {
            if (!dbContext.ReportTypeEntities.Any(rt => rt.Name == "Brand Power Optimization"))
            {
                var brandPowerOptimization = new ReportTypeEntity()
                {
                    Name = "Brand Power Optimization",
                    Description = "Brand Power Optimization Report"
                };

                dbContext.Add(brandPowerOptimization);
            }

            if (!dbContext.ReportTypeEntities.Any(rt => rt.Name == "Dev Test Report"))
            {
                var devTestReport = new ReportTypeEntity()
                {
                    Name = "Dev Test Report",
                    Description = "Report used for APP testing"
                };
                dbContext.Add(devTestReport);
            }

            dbContext.SaveChanges();
        }

        private static void SeedModelTypes(ZervemedataDbContext dbContext)
        {
            if (!dbContext.ModelTypeEntities.Any(m => m.Name == "Open AI 4o"))
            {
                var openAi = new ModelTypeEntity()
                {
                    Name = "Open AI 4o",
                    Description = "version 4o of Open AI"
                };

                dbContext.Add(openAi);
                dbContext.SaveChanges();
            }
        }
    }
}
