using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Moq;
using zervemedata.Data.Entities;
using zervemedata.Data.Entities.Keys;
using zervemedata.Data.Enumerations;

namespace ZerveMeData.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using zervemedata.Core.Services;
    using zervemedata.Data;
    using zervemedata.Data.AuthData;
    using zervemedata.Data.DataModels.CreateMeta;
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class UserResourceManagerTests
    {
        private ZervemedataDbContext _dbContext = null!;
        private UsersResourceManager _usersResourceManager = null!;
        private string _userId = Guid.NewGuid().ToString();
        private ApplicationUser _testUser = null!;        

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ZervemedataDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ZervemedataDbContext(options);

            _testUser = new ApplicationUser
            {
                Id = _userId,
                UserName = "test@example.com",
                FirstName = "testfirst",
                LastName = "testlast",
                Email = "testuser@gmail.com",
                ContactEntity = new ContactEntity()
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
                UserId = _testUser.Id
            };

            var appUserManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            await appUserManager.CreateAsync(_testUser);
            await _dbContext.AddAsync(org);
            await _dbContext.AddAsync(orgUser);
            await _dbContext.SaveChangesAsync();

            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                string roleName = role.ToString();

                // Ensure role exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            _usersResourceManager = new UsersResourceManager(_dbContext, appUserManager);

            _dbContext.Users.Add(_testUser);
            await _dbContext.SaveChangesAsync();
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
        public async Task CreateUser_ShouldCreateNewUser_WhenValid()
        {
            var creationMeta = new UserCreationMetaData()
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Password = "TestPassword123",
                EmailAddress = "test@gmail.com",
                Roles = { Role.User, Role.Admin },
                CurrentUser = _userId
            };

            var result = await _usersResourceManager.Create(creationMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo(creationMeta.UserName));
        }
        
        [Test]
        public async Task GetAllUsers_ShouldReturnAllUsers_WhenValid()
        {
            var creationMeta = new UserCreationMetaData()
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Password = "TestPassword123",
                EmailAddress = "test@gmail.com",
                Roles = { Role.User, Role.Admin },
                CurrentUser = _userId
            };

            var result = await _usersResourceManager.Create(creationMeta);

            var key = new UserKey()
            {
                CurrentUser = _userId
            };

            var allUsers = await _usersResourceManager.GetAll(key);

            var userInfos = allUsers.ToList();
            Assert.That(userInfos, Is.Not.Null);
            Assert.That(userInfos.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetUser_ShouldReturnUser_WhenValid()
        {
            var creationMeta = new UserCreationMetaData()
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Password = "TestPassword123",
                EmailAddress = "test@gmail.com",
                Roles = { Role.User, Role.Admin },
                CurrentUser = _userId
            };

            var result = await _usersResourceManager.Create(creationMeta);

            
            var key = new UserKey()
            {
                Id = _userId,
                CurrentUser = _userId
            };

            var user = await _usersResourceManager.Get(key);

            Assert.That(user, Is.Not.Null);
            Debug.Assert(user != null, nameof(user) + " != null");
            Assert.That(user.UserName, Is.EqualTo(_testUser.UserName));
        }
        // TODO Implement authenication tests
    }
}