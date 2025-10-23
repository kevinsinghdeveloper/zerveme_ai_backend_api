using System.Diagnostics;
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
    public class ModelsResourceManagerTests
    {
        private ZervemedataDbContext _dbContext = null!;
        private ModelsResourceManager _modelsResourceManager = null!;
        private ModelTypeEntity _modelType = null!;
        private ModelTypeEntity _updatedModelType = null!;

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

            _modelType = new ModelTypeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Job Frequency",
                Description = "A test job frequency"
            };

            _updatedModelType = new ModelTypeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Updated Job Frequency",
                Description = "An updated test job frequency"
            };

            _dbContext.Users.Add(user);
            _dbContext.OrganizationEntities.Add(org);
            _dbContext.OrgUserEntities.Add(orgUser);
            _dbContext.ModelTypeEntities.Add(_modelType);
            _dbContext.ModelTypeEntities.Add(_updatedModelType);
            _dbContext.SaveChanges();

            _modelsResourceManager = new ModelsResourceManager(_dbContext);
        }

        [Test]
        public async Task CreateModel_ShouldCreateNewModel_WhenValid()
        {
            var creationMeta = new ModelsCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model",
                ModelConfig = "{}",
                ModelTypeId = _modelType.Id,
            };

            var result = await _modelsResourceManager.Create(creationMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(creationMeta.Name));
        }

        [Test]
        public void CreateModel_ShouldThrow_WhenUserDoesNotExist()
        {
            var creationMeta = new ModelsCreationMetaData
            {
                CurrentUser = "test-user-id2",
                Name = "Test Model",
                ModelConfig = "{}",
                ModelTypeId = _modelType.Id,
            };
            Assert.ThrowsAsync<Exception>(async () => await _modelsResourceManager.Create(creationMeta));
        }

        [Test]
        public void CreateModel_ShouldThrow_WhenModelTypeDoesNotExist()
        {
            var creationMeta = new ModelsCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model",
                ModelConfig = "{}",
                ModelTypeId = Guid.NewGuid(),
            };
            Assert.ThrowsAsync<Exception>(async () => await _modelsResourceManager.Create(creationMeta));
        }

        [Test]
        public async Task UpdateModel_ShouldUpdateModel_WhenValid()
        {
            var creationMeta = new ModelsCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model",
                ModelConfig = "{}",
                ModelTypeId = _modelType.Id,
            };

            var creationResult = await _modelsResourceManager.Create(creationMeta);

            var updateMeta = new ModelsUpdateMetaData()
            {
                Id = creationResult.Id,
                Name = "Updated Model",
                ModelConfig = "{343}",
                Description = "Updated description",
                CurrentUser = "test-user-id",
                ModelTypeId = _updatedModelType.Id
            };

            var result = await _modelsResourceManager.Update(updateMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(creationResult.Id));
            Assert.That(result.Name, Is.EqualTo(updateMeta.Name));
            Assert.That(result.ModelConfig, Is.EqualTo(updateMeta.ModelConfig));
            Assert.That(result.Description, Is.EqualTo(updateMeta.Description));
            Assert.That(result.ModelType, Is.Not.Null);
            if (result.ModelType != null)
                Assert.That(result.ModelTypeId, Is.EqualTo(updateMeta.ModelTypeId));
        }

        [Test]
        public async Task UpdateModel_ShouldThrow_WhenUserDoesNotExist()
        {
            var creationMeta = new ModelsCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model",
                ModelConfig = "{}",
                ModelTypeId = _modelType.Id,
            };

            var creationResult = await _modelsResourceManager.Create(creationMeta);

            var updateMeta = new ModelsUpdateMetaData()
            {
                Id = creationResult.Id,
                Name = "Updated Model",
                ModelConfig = "{343}",
                Description = "Updated description",
                CurrentUser = "test-user-id2",
                ModelTypeId = _updatedModelType.Id,
            };

            Assert.ThrowsAsync<Exception>(async () => await _modelsResourceManager.Update(updateMeta));
        }

        [Test]
        public async Task GetAllModels_ShouldGetAllModels()
        {
            var creationMeta = new ModelsCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model",
                ModelConfig = "{}",
                ModelTypeId = _modelType.Id,
            };

            var _ = await _modelsResourceManager.Create(creationMeta);

            var key = new ModelsKey()
            {
                CurrentUser = "test-user-id"
            };

            var result = await _modelsResourceManager.GetAll(key);

            var modelsInfos = result.ToList();
            Assert.That(modelsInfos, Is.Not.Null);
            Assert.That(modelsInfos.Count(), Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task GetModel_ShouldGetModel()
        {
            var creationMeta = new ModelsCreationMetaData
            {
                CurrentUser = "test-user-id",
                Name = "Test Model",
                ModelConfig = "{}",
                ModelTypeId = _modelType.Id,
            };

            var creationResult = await _modelsResourceManager.Create(creationMeta);

            var key = new ModelsKey()
            {
                Id = creationResult.Id,
                CurrentUser = "test-user-id"
            };

            var result = await _modelsResourceManager.Get(key);

            Assert.That(result, Is.Not.Null);
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.That(result.Id, Is.EqualTo(creationResult.Id));
        }

    }
}