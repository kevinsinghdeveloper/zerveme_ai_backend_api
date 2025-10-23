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
    public class ProjectResourceManagerTests
    {
        private ZervemedataDbContext _dbContext = null!;
        private ProjectResourceManager _projectResourceManager = null!;

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

            _projectResourceManager = new ProjectResourceManager(_dbContext);
        }

        [Test]
        public async Task CreateProject_ShouldCreateNewProject_WhenValid()
        {
            var creationMeta = new ProjectCreationMetaData
            {
                Name = "Test Project",
                Description = "A test project",
                CurrentUser = "test-user-id"
                // OrganizationId = null, will default to user's organization
            };

            var result = await _projectResourceManager.Create(creationMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That("Test Project", Is.EqualTo(result.Name));
            Assert.That("A test project", Is.EqualTo(result.Description));
        }

        [Test]
        public void CreateProject_ShouldThrow_WhenUserDoesNotExist()
        {
            var creationMeta = new ProjectCreationMetaData
            {
                Name = "Invalid",
                Description = "Doesn't matter",
                CurrentUser = "non-existent-user"
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _projectResourceManager.Create(creationMeta));
            if (ex != null) Assert.That(ex.Message, Is.EqualTo("User not found."));
        }

        [Test]
        public async Task GetAllProjects_ShouldReturnAllProjects_WhenValid()
        {
            var creationMeta = new ProjectCreationMetaData
            {
                Name = "Test Project",
                Description = "A test project",
                CurrentUser = "test-user-id"
            };

            _projectResourceManager.Create(creationMeta).Wait();

            var key = new ProjectKey()
            {
                CurrentUser = "test-user-id"
            };


            var result = await _projectResourceManager.GetAllOrgProjects(key);

            var projectInfos = result.ToList();

            Assert.That(projectInfos, Is.Not.Null);
            Assert.That(projectInfos.Count(), Is.EqualTo(1));
            Assert.That(projectInfos.First().Name, Is.EqualTo("Test Project"));
        }

        [Test]
        public async Task UpdateProject_ShouldUpdateProject_WhenValid()
        {
            var creationMeta = new ProjectCreationMetaData
            {
                Name = "Test Project",
                Description = "A test project",
                CurrentUser = "test-user-id"
            };

            var createdProject = await _projectResourceManager.Create(creationMeta);

            var updateMeta = new ProjectUpdateMetaData
            {
                Id = createdProject.Id,
                Name = "Updated Project",
                Description = "Updated description",
                CurrentUser = "test-user-id"
            };

            var result = await _projectResourceManager.Update(updateMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Updated Project"));
            Assert.That(result.Description, Is.EqualTo("Updated description"));
        }

        [Test]
        public void UpdateProject_ShouldThrow_WhenProjectDoesNotExist()
        {
            var updateMeta = new ProjectUpdateMetaData
            {
                Id = Guid.NewGuid(),
                Name = "Updated Project",
                Description = "Updated description",
                CurrentUser = "test-user-id"
            };

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _projectResourceManager.Update(updateMeta));
            if (ex != null) Assert.That(ex.Message, Is.EqualTo($"Project with ID {updateMeta.Id} not found."));
        }

        [Test]
        public void UpdateProject_ShouldThrow_WhenUserDoesNotExist()
        {
            var updateMeta = new ProjectUpdateMetaData
            {
                Id = Guid.NewGuid(),
                Name = "Updated Project",
                Description = "Updated description",
                CurrentUser = "non-existent-user"
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _projectResourceManager.Update(updateMeta));
            if (ex != null) Assert.That(ex.Message, Is.EqualTo("User not found."));
        }

        [Test]
        public async Task GetProject_ShouldThrow_WhenGetProjectWhenExist()
        {
            var creationMeta = new ProjectCreationMetaData
            {
                Name = "Test Project",
                Description = "A test project",
                CurrentUser = "test-user-id"
            };

            var createdProject = await _projectResourceManager.Create(creationMeta);

            var key = new ProjectKey()
            {
                CurrentUser = "test-user-id",
                Id = createdProject.Id
            };

            var result = await _projectResourceManager.Get(key);

            Assert.That(result, Is.Not.Null);
            if (result != null) Assert.That(result.Name, Is.EqualTo("Test Project"));
        }

        [Test]
        public async Task DeleteProject_ShouldDeleteProject_WhenValid()
        {
            var creationMeta = new ProjectCreationMetaData
            {
                Name = "Test Project",
                Description = "A test project",
                CurrentUser = "test-user-id"
            };

            var createdProject = await _projectResourceManager.Create(creationMeta);

            var key = new ProjectKey()
            {
                CurrentUser = "test-user-id",
                Id = createdProject.Id
            };

            var result = await _projectResourceManager.SoftDelete(key);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));

            // get project should be empty as well
            var getCreatedProject = await _projectResourceManager.Get(key);
            Assert.That(getCreatedProject, Is.Null);
        }
    }
}
