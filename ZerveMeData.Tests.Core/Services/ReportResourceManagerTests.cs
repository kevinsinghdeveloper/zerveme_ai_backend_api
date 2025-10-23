using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Npgsql;
using Testcontainers.PostgreSql;
using zervemedata.Core.Services.DataWarehouseManagers;
using zervemedata.Data.DataModels.Querying;
using zervemedata.Data.Enumerations;
using zervemedata.Data.JsonDataModels;

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
    public class ReportResourceManagerTests
    {
        private ZervemedataDbContext _dbContext = null!;
        private ReportResourceManager _reportResourceManager = null!;
        private ReportDatasetResourceManager _reportDatasetManager = null!;
        private DatasetsResourceManager _datasetsResourceManager = null!;

        private JobFreqTypeEntity _jobFreqType = null!;
        private ProjectEntity _project = null!;
        private ReportTypeEntity _reportType = null!;
        private JobEntity _job = null!;
        private ModelEntity _model = null!;
        private JobFreqTypeEntity _updateJobFreqType = null!;
        private OrganizationEntity _org = null!;
        private DWHEntity _dWH = null!;
        private ReportEntity _report = null!;
        private DatasetEntity _dataset = null!;
        private DatasetDWHQueryingParameters _queryParams = null!;

        private PostgreSqlContainer _postgresContainer;

        private async Task CreateTestTable()
        {
            // Connect to the PostgreSQL database
            using var connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
            await connection.OpenAsync();

            // Create a test table with the specified structure
            string createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""testtable"" (
                id SERIAL PRIMARY KEY,
                ""date"" DATE NOT NULL,
                ""kpi1"" INTEGER,
                ""attr1"" TEXT
            );";

            using (var command = new NpgsqlCommand(createTableSql, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Insert sample data for the period 01-01-2023 to 01-01-2024
            DateTime startDate = DateTime.Parse("01-01-2023");
            DateTime endDate = DateTime.Parse("01-01-2024");

            // Generate daily data for the period
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                string insertSql = @"
            INSERT INTO ""testtable"" (""date"", ""kpi1"", ""attr1"")
            VALUES (@Date, @KPI1, @Attr1);";

                // Create random data
                int kpiValue = new Random().Next(1, 1000);
                string[] categories = { "Category A", "Category B", "Category C" };
                string attrValue = categories[new Random().Next(categories.Length)];

                using (var command = new NpgsqlCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("Date", date);
                    command.Parameters.AddWithValue("kpi1", kpiValue);
                    command.Parameters.AddWithValue("attr1", attrValue);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithDatabase("testdb")
                .WithUsername("testuser")
                .WithPassword("testpassword")
                .WithImage("postgres:15")
                .WithCleanUp(true)
                .Build();

            await _postgresContainer.StartAsync();
        }
        

        [SetUp]
        public async Task Setup()
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

            _org = new OrganizationEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Organization",
            };

            var orgUser = new OrgUserEntity
            {
                Id = Guid.NewGuid(),
                OrganizationId = _org.Id,
                UserId = user.Id
            };

            _reportType = new ReportTypeEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Report Type",
                Description = "A test report type"
            };

            var reportConfigObj = new ReportConfigObject()
            {
                Fields = new List<Field>()
            };

            var reportConfiguration = new ReportConfigurationEntity()
            {
                Id = Guid.NewGuid(),
                ReportConfig = reportConfigObj,
                ReportType = _reportType,
                CreatedUser = user
            };

            _project = new ProjectEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Project",
                Description = "A test project",
                OrganizationId = _org.Id
            };


            _jobFreqType = new JobFreqTypeEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Job Frequency",
                Description = "A test job frequency"
            };

            _updateJobFreqType = new JobFreqTypeEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Job Frequency updated",
                Description = "A test job frequency updated"
            };

            _job = new JobEntity()
            {
                Id = Guid.NewGuid(),
                JobFreqTypeId = _jobFreqType.Id
            };

            var modelType = new ModelTypeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Model Type",
                Description = "A test model type"
            };

            _model = new ModelEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Model",
                Description = "A test model",
                OrganizationId = _org.Id,
                ModelTypeEntity = modelType,
                ModelConfig = "{}"
            };

            _dWH = new DWHEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test DWH",
                PrimarySchema = "public",
                ConnectionString = _postgresContainer.GetConnectionString(), // ← actual connection string
                DwhLoadingStatus = DWHLoadingStatus.Ready,
                OrganizationId = _org.Id,
                Dwh = DWH.Postgres
            };

            _report = new ReportEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Report",
                Description = "A test report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                ModelId = _model.Id,
                CreatedUserId = user.Id
            };

            _dataset = new DatasetEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Dataset",
                DomainData = new DomainData()
                {
                    TableName = "TestTable",
                    KpiCols =
                    [
                        new ColumnObject()
                        {
                            ColumnName = "KPI1",
                            ColumnType = ColumnType.Numeric
                        }
                    ],
                    AttrCols =
                    [
                        new ColumnObject()
                        {
                            ColumnName = "Attr1",
                            ColumnType = ColumnType.Text
                        },
                        new ColumnObject()
                        {
                            ColumnName = "date",
                            ColumnType = ColumnType.Date
                        }
                    ]
                },
                ReportEntity = _report,
                DWHId = _dWH.Id,
                CreatedUserId = user.Id
            };

            _queryParams = new DatasetDWHQueryingParameters()
            {
                DatasetId = _dataset.Id,
                Attributes = new List<string>()
                {
                    "Attr1"
                },
                KPIs = new List<string>()
                {
                    "KPI1"
                },
                RowLimit = 100,
                ReportId = _report.Id
            };

            _dbContext.Users.Add(user);
            _dbContext.OrganizationEntities.Add(_org);
            _dbContext.OrgUserEntities.Add(orgUser);
            _dbContext.ProjectEntities.Add(_project);
            _dbContext.JobFreqTypeEntities.Add(_jobFreqType);
            _dbContext.ReportTypeEntities.Add(_reportType);
            _dbContext.JobEntities.Add(_job);
            _dbContext.ModelTypeEntities.Add(modelType);
            _dbContext.ModelEntities.Add(_model);
            _dbContext.JobFreqTypeEntities.Add(_updateJobFreqType);
            _dbContext.ReportConfigurationEntities.Add(reportConfiguration);
            _dbContext.DwhEntities.Add(_dWH);
            _dbContext.ReportEntities.Add(_report);
            _dbContext.DatasetEntities.Add(_dataset);
            _dbContext.SaveChanges();

            _reportResourceManager = new ReportResourceManager(_dbContext);
            _reportDatasetManager = new ReportDatasetResourceManager(_dbContext);

            var dwhService = new DatawarehouseHandlerService();
            var httpContextAccessor = new HttpContextAccessor();
            _datasetsResourceManager = new DatasetsResourceManager(_dbContext, dwhService, httpContextAccessor);

            await CreateTestTable();
        }

        [Test]
        public async Task CreateReport_ShouldCreateNewReport_WhenValid()
        {
            var creationMeta = new ReportCreationMetaData()
            {
                CurrentUser = "test-user-id",
                Name = "Test Report",
                Description = "A test report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                JobId = _job.Id,
                ModelId = _model.Id
            };

            var result = await _reportResourceManager.Create(creationMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(creationMeta.Name));
        }
        //
        [Test]
        public async Task UpdateReport_ShouldUpdateReport_WhenValid()
        {
            var creationMeta = new ReportCreationMetaData()
            {
                CurrentUser = "test-user-id",
                Name = "Test Report",
                Description = "A test report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                JobId = _job.Id,
                ModelId = _model.Id
            };

            var creationResult = await _reportResourceManager.Create(creationMeta);

            var reportDatasetCreationMeta = new ReportDatasetCreationMetaData()
            {
                ReportId = creationResult.Id,
                ReportConfigurationId = creationResult.ReportConfigurationId,
                DatasetConfig = creationResult.DatasetConfig ?? "",
                CurrentUser = "test-user-id"
            };

            var datasetResult = await _reportDatasetManager.Create(reportDatasetCreationMeta);

            Assert.That(datasetResult, Is.Not.Null);

            var updateMeta = new ReportUpdateMetaData()
            {
                Id = creationResult.Id,
                CurrentUser = "test-user-id",
                Name = "Updated Report",
                Description = "An updated report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                ModelId = _model.Id,
                JobFreqTypeId = _updateJobFreqType.Id,
                DatasetConfig = "{}"
            };

            var result = await _reportResourceManager.Update(updateMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(updateMeta.Name));
            Assert.That(result.Description, Is.EqualTo(updateMeta.Description));
            Debug.Assert(result.Job != null, "result.Job != null");
            Debug.Assert(result.Job.JobFreqType != null, "result.Job.JobFreqType != null");
            Assert.That(result.Job.JobFreqType.Id, Is.EqualTo(_updateJobFreqType.Id));
        }

        [Test]
        public void UpdateReport_ShouldThrowException_WhenReportNotFound()
        {
            var updateMeta = new ReportUpdateMetaData()
            {
                Id = Guid.NewGuid(),
                CurrentUser = "test-user-id",
                Name = "Updated Report",
                Description = "An updated report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                ModelId = _model.Id,
                JobFreqTypeId = _updateJobFreqType.Id,
                DatasetConfig = "{}"
            };

            Assert.ThrowsAsync<Exception>(async () => { await _reportResourceManager.Update(updateMeta); });
        }
        
        // // check getters
        [Test]
        public async Task GetAllReports_ShouldReturnAllReports_WhenValid()
        {
            var creationMeta = new ReportCreationMetaData()
            {
                CurrentUser = "test-user-id",
                Name = "Test Report",
                Description = "A test report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                JobId = _job.Id,
                ModelId = _model.Id
            };

            var creationResult = await _reportResourceManager.Create(creationMeta);

            var reportDatasetCreationMeta = new ReportDatasetCreationMetaData()
            {
                ReportId = creationResult.Id,
                ReportConfigurationId = creationResult.ReportConfigurationId,
                DatasetConfig = creationResult.DatasetConfig ?? "",
                CurrentUser = "test-user-id"
            };

            var key = new ReportKey()
            {
                CurrentUser = "test-user-id",
            };

            var datasetResult = await _reportDatasetManager.Create(reportDatasetCreationMeta);

            Assert.That(datasetResult, Is.Not.Null);

            var result = await _reportResourceManager.GetAll(key);


            var reportInfos = result.ToList();
            Assert.That(reportInfos, Is.Not.Null);
            Assert.That(reportInfos.Count(), Is.GreaterThanOrEqualTo(1));
        }
        
        // // TODO GetAllReports_ShouldReturnAllReports_WhenValid --> for current org
        //
        // get report
        [Test]
        public async Task GetReport_ShouldReturnRequestedReport_WhenInvalid()
        {
            var creationMeta = new ReportCreationMetaData()
            {
                CurrentUser = "test-user-id",
                Name = "Test Report",
                Description = "A test report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                JobId = _job.Id,
                ModelId = _model.Id
            };

            var creationResult = await _reportResourceManager.Create(creationMeta);

            var key = new ReportKey()
            {
                Id = creationResult.Id,
                CurrentUser = "test-user-id",
            };

            var result = await _reportResourceManager.Get(key);

            Assert.That(result, Is.Not.Null);
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.That(result.Name, Is.EqualTo(creationMeta.Name));
        }
        
        // // delete
        [Test]
        public async Task DeleteReport_ShouldSoftDeleteReport_WhenValid()
        {
            var creationMeta = new ReportCreationMetaData()
            {
                CurrentUser = "test-user-id",
                Name = "Test Report",
                Description = "A test report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                JobId = _job.Id,
                ModelId = _model.Id
            };

            var creationResult = await _reportResourceManager.Create(creationMeta);

            var key = new ReportKey()
            {
                Id = creationResult.Id,
                CurrentUser = "test-user-id",
            };

            var result = await _reportResourceManager.SoftDelete(key);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public async Task GetReportDomainOptions_ShouldReturnRequestedDomainOptions()
        {
            var creationMeta = new ReportCreationMetaData()
            {
                CurrentUser = "test-user-id",
                Name = "Test Report",
                Description = "A test report",
                ProjectId = _project.Id,
                ReportTypeId = _reportType.Id,
                JobId = _job.Id,
                ModelId = _model.Id
            };

            var creationResult = await _reportResourceManager.Create(creationMeta);

            var key = new ReportKey()
            {
                Id = creationResult.Id,
                CurrentUser = "test-user-id",
            };

            var result = await _reportResourceManager.Get(key);

            Assert.That(result, Is.Not.Null);


            var creationDatasetMeta = new DatasetCreationMetaData()
            {
                Name = "Test Dataset",
                DomainData = new DomainData()
                {
                    TableName = "TestTable",
                    KpiCols = new List<ColumnObject>()
                    {
                        new ColumnObject()
                        {
                            ColumnName = "KPI1",
                            ColumnType = ColumnType.Numeric
                        }
                    },
                    AttrCols = new List<ColumnObject>()
                    {
                        new ColumnObject()
                        {
                            ColumnName = "Attr1",
                            ColumnType = ColumnType.Text
                        },
                        new ColumnObject()
                        {
                            ColumnName = "date",
                            ColumnType = ColumnType.Date
                        }
                    }
                },
                ReportId = result.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationDatasetResult = await _datasetsResourceManager.Create(creationDatasetMeta);

            Assert.That(creationDatasetResult, Is.Not.Null);
            Assert.That(creationDatasetResult.Name, Is.EqualTo(creationDatasetMeta.Name));
            // TODO check domain data
            Assert.That(creationDatasetResult.DomainData, Is.Not.Null);
            Assert.That(creationDatasetResult.DomainData?.TableName,
                Is.EqualTo(creationDatasetMeta.DomainData.TableName));
            Assert.That(creationDatasetResult.DomainData?.KpiCols, Is.Not.Null);
        }

        [Test]
        public async Task GetData_ShouldGetDataIfValid()
        {
            var key = new ReportKey()
            {
                Id = _report.Id,
                CurrentUser = "test-user-id",
            };

            var report = await _reportResourceManager.Get(key);

            if (report == null)
            {
                Assert.Fail();
            }

            var response = await _datasetsResourceManager.GetData(_queryParams, "test-user-id");

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Data, Is.Not.Null);
        }
    }
}