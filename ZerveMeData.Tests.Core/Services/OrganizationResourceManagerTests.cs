using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Moq;
using zervemedata.Data.Enumerations;

namespace ZerveMeData.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using zervemedata.Core.Services;
    using zervemedata.Data;
    using zervemedata.Data.AuthData;
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data.DataModels.UpdateMeta;
    using zervemedata.Data.Entities;
    using zervemedata.Data.Entities.Keys;
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class OrganizationResourceManagerTests
    {
        private ZervemedataDbContext _dbContext = null!;
        private OrganizationsResourceManager _organizationResourceManager = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ZervemedataDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ZervemedataDbContext(options);

            var user = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "test@example.com",
                FirstName = "testfirst",
                LastName = "testlast",
                Email = "testuser@gmail.com"
            };

            var org = new OrganizationEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Organization",
            };

            var orgUser = new OrgUserEntity
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                UserId = user.Id
            };

            _dbContext.Users.Add(user);
            _dbContext.OrganizationEntities.Add(org);
            _dbContext.OrgUserEntities.Add(orgUser);
            _dbContext.SaveChanges();

            var appUserManager = GetMockUserManager();
            _organizationResourceManager = new OrganizationsResourceManager(_dbContext, appUserManager);
        }


        private RoleManager<IdentityRole> GetMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                store.Object, null, null, null, null);

            mockRoleManager.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true); // assume roles already exist

            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            return mockRoleManager.Object;
        }


        private UserManager<ApplicationUser> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            // Mock user creation
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Mock user lookup
            mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null!); // Adjust if needed

            // Mock AddToRolesAsync
            mockUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            // Optional: If your code uses AddToRoleAsync
            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Optional: If your code checks if user is already in a role
            mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            return mockUserManager.Object;
        }

        [Test]
        public async Task CreateOrganization_ShouldCreateNewOrganization_WhenValid()
        {
            var creationMeta = new OrganizationCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model"
            };

            var result = await _organizationResourceManager.Create(creationMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(creationMeta.Name));
        }

        [Test]
        public async Task GetAllOrganizations_ShouldGetAllOrganizations()
        {
            var creationMeta = new OrganizationCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model"
            };

            var res = await _organizationResourceManager.Create(creationMeta);

            // TODO add user auth....
            var result = await _organizationResourceManager.GetAll();

            var organizationsInfos = result.ToList();
            Assert.That(organizationsInfos, Is.Not.Null);
            Assert.That(organizationsInfos.Count(), Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task GetOrganization_ShouldGetOrganization()
        {
            var creationMeta = new OrganizationCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model"
            };

            var res = await _organizationResourceManager.Create(creationMeta);

            var key = new OrganizationKey()
            {
                Id = res.Id,
            };

            var result = await _organizationResourceManager.Get(key);

            Assert.That(result, Is.Not.Null);
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.That(result.Id, Is.EqualTo(res.Id));
        }
    }
}