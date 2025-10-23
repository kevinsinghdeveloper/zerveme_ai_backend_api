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
    public class JobResourceManagerTests
    {
        private ZervemedataDbContext _dbContext = null!;
        private JobResourceManager _jobResourceManager = null!;

        private JobFreqTypeEntity jobFreqType = null!;

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

            jobFreqType = new JobFreqTypeEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Job Frequency",
                Description = "A test job frequency"
            };


            _dbContext.Users.Add(user);
            _dbContext.OrganizationEntities.Add(org);
            _dbContext.OrgUserEntities.Add(orgUser);
            _dbContext.JobFreqTypeEntities.Add(jobFreqType);
            _dbContext.SaveChanges();

            _jobResourceManager = new JobResourceManager(_dbContext);
        }

        [Test]
        public async Task CreateJob_ShouldCreateNewJob_WhenValid()
        {
            var creationMeta = new JobCreationMetaData()
            {
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "test-user-id"
            };

            var result = await _jobResourceManager.Create(creationMeta);

            Assert.That(result, Is.Not.Null);
            if (result.JobFreqType != null) Assert.That(jobFreqType.Id, Is.EqualTo(result.JobFreqType.Id));
        }

        [Test]
        public void CreateJob_ShouldThrow_WhenUserDoesNotExist()
        {
            var creationMeta = new JobCreationMetaData
            {
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "non-existent-user-id"
            };

            Assert.ThrowsAsync<Exception>(async () => await _jobResourceManager.Create(creationMeta));
        }

        [Test]
        public void CreateJob_ShouldThrow_WhenJobFreqTypeDoesNotExist()
        {
            var creationMeta = new JobCreationMetaData
            {
                JobFreqTypeId = Guid.NewGuid(), // Non-existent JobFreqTypeId
                CurrentUser = "test-user-id"
            };

            Assert.ThrowsAsync<Exception>(async () => await _jobResourceManager.Create(creationMeta));
        }

        [Test]
        public async Task UpdateJob_ShouldUpdateJob_WhenValid()
        {
            var creationMeta = new JobCreationMetaData()
            {
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _jobResourceManager.Create(creationMeta);

            var updateMeta = new JobUpdateMetaData
            {
                Id = creationResult.Id,
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "test-user-id"
            };

            var result = await _jobResourceManager.Update(updateMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(creationResult.Id));
            Assert.That(result.JobFreqType, Is.Not.Null);
            Debug.Assert(result.JobFreqType != null, "result.JobFreqType != null");
            Assert.That(result.JobFreqType.Id, Is.EqualTo(jobFreqType.Id));
        }

        [Test]
        public void UpdateJob_ShouldThrow_WhenJobDoesNotExist()
        {
            var updateMeta = new JobUpdateMetaData
            {
                Id = Guid.NewGuid(), // Non-existent Job ID
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "test-user-id"
            };

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _jobResourceManager.Update(updateMeta));
        }

        [Test]
        public void UpdateJob_ShouldThrow_WhenUserDoesNotExist()
        {
            var updateMeta = new JobUpdateMetaData
            {
                Id = Guid.NewGuid(), // Non-existent Job ID
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "non-existent-user-id"
            };

            Assert.ThrowsAsync<Exception>(async () => await _jobResourceManager.Update(updateMeta));
        }

        [Test]
        public void UpdateJob_ShouldThrow_WhenJobFreqTypeDoesNotExist()
        {
            var creationMeta = new JobCreationMetaData()
            {
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = _jobResourceManager.Create(creationMeta).Result;

            var updateMeta = new JobUpdateMetaData
            {
                Id = creationResult.Id,
                JobFreqTypeId = Guid.NewGuid(), // Non-existent JobFreqTypeId
                CurrentUser = "test-user-id"
            };

            Assert.ThrowsAsync<Exception>(async () => await _jobResourceManager.Update(updateMeta));
        }

        // [Test]
        // public async Task DeleteJob_ShouldDelete()
        // {
        //     var creationMeta = new JobCreationMetaData()
        //     {
        //         JobFreqTypeId = jobFreqType.Id,
        //         CurrentUser = "test-user-id"
        //     };
        //
        //     var creationResult = await _jobResourceManager.Create(creationMeta);
        //
        //     var key = new JobKey()
        //     {
        //         Id = creationResult.Id
        //     };
        //     var result = await _jobResourceManager.SoftDelete(key);
        //
        //     Assert.That(result, Is.Not.Null);
        //     Assert.That(result, Is.EqualTo(true));
        // }

        [Test]
        public async Task GetAllJobs_ShouldGetAllJobs()
        {
            var creationMeta = new JobCreationMetaData()
            {
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "test-user-id"
            };

            var _ = _jobResourceManager.Create(creationMeta).Result;

            var key = new JobKey()
            {
                CurrentUser = "test-user-id"
            };

            var result = await _jobResourceManager.GetAll(key);

            var jobInfos = result.ToList();
            Assert.That(jobInfos, Is.Not.Null);
            Assert.That(jobInfos.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetJob_ShouldGetJob()
        {
            var creationMeta = new JobCreationMetaData()
            {
                JobFreqTypeId = jobFreqType.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = _jobResourceManager.Create(creationMeta).Result;

            var key = new JobKey()
            {
                Id = creationResult.Id,
                CurrentUser = "test-user-id"
            };

            var result = await _jobResourceManager.Get(key);

            Assert.That(result, Is.Not.Null);
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.That(result.Id, Is.EqualTo(creationResult.Id));
        }
        
    }
}