using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using Core.Contracts.ContractModels;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Http;
using Moq;
using zervemedata.Core.Services.DataWarehouseManagers;
using zervemedata.Data.Enumerations;
using zervemedata.Data.JsonDataModels;
using DotNet.Testcontainers.Containers;
using Google.Apis.Bigquery.v2.Data;
using Npgsql;
using Testcontainers.PostgreSql;
using zervemedata.Data.DataModels.Querying;

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
    public class DatasetResourceManagerTests
    {
        private ZervemedataDbContext _dbContext = null!;

        // private ReportResourceManager _reportResourceManager = null!;
        // private ReportDatasetResourceManager _reportDatasetManager = null!;
        private DatasetsResourceManager _datasetsResourceManager = null!;

        private JobFreqTypeEntity _jobFreqType = null!;
        private ProjectEntity _project = null!;
        private ReportTypeEntity _reportType = null!;
        private JobEntity _job = null!;
        private ModelEntity _model = null!;
        private JobFreqTypeEntity _updateJobFreqType = null!;
        private OrganizationEntity _org = null!;
        private ReportEntity _report = null!;
        private DWHEntity _dWH = null!;

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

            // _dWH = new DWHEntity()
            // {
            //     Id = Guid.NewGuid(),
            //     Name = "Test DWH",
            //     PrimarySchema = "testSchema",
            //     ConnectionString = "Server=test;Database=testdb;User Id=testuser;Password=testpassword;",
            //     DwhLoadingStatus = DWHLoadingStatus.Ready,
            //     OrganizationId = _org.Id,
            //     Dwh = DWH.Postgres
            // };
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
            _dbContext.ReportEntities.Add(_report);
            _dbContext.DwhEntities.Add(_dWH);
            await _dbContext.SaveChangesAsync();

            var dwhService = new DatawarehouseHandlerService();
            var httpContextAccessor = new HttpContextAccessor();
            _datasetsResourceManager = new DatasetsResourceManager(_dbContext, dwhService, httpContextAccessor);

            await CreateTestTable();
        }

        [Test]
        public async Task CreateDataset_ShouldCreateNewDataset_WhenValid()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var result = await _datasetsResourceManager.Create(creationMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(creationMeta.Name));
            Debug.Assert(result.DomainData != null, "result.DomainData != null");
            Assert.That(result.DomainData.TableName, Is.EqualTo(creationMeta.DomainData.TableName));
            Assert.That(result.DomainData.KpiCols, Is.Not.Null);
            Assert.That(result.DomainData.KpiCols, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task UpdateDataset_ShouldUpdateDataset_WhenValid()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResults = await _datasetsResourceManager.Create(creationMeta);

            var updateMeta = new DatasetUpdateMetaData()
            {
                Id = creationResults.Id,
                Name = "Updated Dataset",
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
            };

            var result = await _datasetsResourceManager.Update(updateMeta);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(updateMeta.Name));
            Debug.Assert(result.DomainData != null, "result.DomainData != null");
            Assert.That(result.DomainData.TableName, Is.EqualTo(updateMeta.DomainData.TableName));
            Assert.That(result.DomainData.KpiCols, Is.Not.Null);
            Assert.That(result.DomainData.KpiCols, Has.Count.EqualTo(1));
        }
        
        // test get and get all
        [Test]
        public async Task GetDataset_ShouldReturnDataset_WhenValidId()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };
        
            var creationResults = await _datasetsResourceManager.Create(creationMeta);
        
            var key = new DatasetKey()
            {
                Id = creationResults.Id,
                CurrentUser = "test-user-id"
            };
        
        
            var result = await _datasetsResourceManager.Get(key);
        
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(creationMeta.Name));
            Debug.Assert(result.DomainData != null, "result.DomainData != null");
            Assert.That(result.DomainData.TableName, Is.EqualTo(creationMeta.DomainData.TableName));
            Assert.That(result.DomainData.KpiCols, Is.Not.Null);
            Assert.That(result.DomainData.KpiCols, Has.Count.EqualTo(1));
        }
        
        [Test]
        public async Task GetAllDatasets_ShouldReturnAllDatasets_WhenValid()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };
        
            var creationResult = await _datasetsResourceManager.Create(creationMeta);
        
            var key = new DatasetKey()
            {
                CurrentUser = "test-user-id",
                Id = creationResult.Id
            };
        
            var result =
                await _datasetsResourceManager.GetAll(key); // TODO we should limit to what the organization has
        
            var datasetDatas = result.ToList();
            Assert.That(datasetDatas, Is.Not.Null);
            Assert.That(datasetDatas, Has.Count.EqualTo(1));
            Assert.That(datasetDatas[0].Name, Is.EqualTo(creationMeta.Name));
            Debug.Assert(datasetDatas[0].DomainData != null, "datasetDatas[0].DomainData != null");
        }

        [Test]
        public async Task GetData_ShouldReturnDataForUserRequest_WhenValid()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };
        
            var creationResult = await _datasetsResourceManager.Create(creationMeta);
            // PeriodStart = DateTime.Parse("01-01-2023"),
            // PeriodEnd = DateTime.Parse("01-01-2024"),
            var queryParams = new DatasetDWHQueryingParameters()
            {
                DatasetId = creationResult.Id,
                Attributes = new List<string>()
                {
                    "Attr1"
                },
                KPIs = new List<string>()
                {
                    "KPI1"
                },
                RowLimit = 100,
                ReportId = _report.Id,
                Filters = new List<FilterCondition>()
                {
                    new FilterCondition()
                    {
                        Col = "date",
                        Op = "between",
                        Val = ["2023-01-01", "2023-01-02"]
                    }
                }
            };
        
            var dataResponse = await _datasetsResourceManager.GetData(queryParams, "test-user-id");
            var dataResult = dataResponse.Data;
            Assert.That(dataResponse, Is.Not.Null);
            Assert.That(dataResponse.RowLimit, Is.EqualTo(100));
        
            var jsonDoc = dataResult as System.Text.Json.JsonDocument;
            Assert.That(jsonDoc, Is.Not.Null, "Data should be a JsonDocument");
            Debug.Assert(jsonDoc != null, nameof(jsonDoc) + " != null");
            Assert.That(jsonDoc.RootElement.ValueKind != JsonValueKind.Undefined);
            Assert.That(jsonDoc.RootElement.ValueKind != JsonValueKind.Null);
        
            switch (jsonDoc.RootElement.ValueKind)
            {
                case JsonValueKind.Object:
                    Assert.That(jsonDoc.RootElement.EnumerateObject().Any(), "JSON object should not be empty");
                    break;
                case JsonValueKind.Array:
                    Assert.That(jsonDoc.RootElement.GetArrayLength() > 0, "JSON array should not be empty");
                    break;
            }
        }

        [Test]
        public async Task GetDataWithReportId_ShouldReturnDataForUserRequest_WhenValid()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _datasetsResourceManager.Create(creationMeta);

            var queryParams = new DatasetDWHQueryingParameters()
            {
                DatasetId = creationResult.Id,
                Attributes = new List<string>()
                {
                    "Attr1"
                },
                KPIs = new List<string>()
                {
                    "KPI1"
                },
                RowLimit = 100,
                ReportId = _report.Id,
                Filters = new List<FilterCondition>()
                {
                    new FilterCondition()
                    {
                        Col = "date",
                        Op = "between",
                        Val = ["2023-01-01", "2023-01-02"]
                    }
                }
            };

            var dataResponse = await _datasetsResourceManager.GetData(queryParams, "test-user-id");
            var dataResult = dataResponse.Data;
            Assert.That(dataResponse, Is.Not.Null);
            Assert.That(dataResponse.RowLimit, Is.EqualTo(100));

            var jsonDoc = dataResult as System.Text.Json.JsonDocument;
            Assert.That(jsonDoc, Is.Not.Null, "Data should be a JsonDocument");
            Debug.Assert(jsonDoc != null, nameof(jsonDoc) + " != null");
            Assert.That(jsonDoc.RootElement.ValueKind != JsonValueKind.Undefined);
            Assert.That(jsonDoc.RootElement.ValueKind != JsonValueKind.Null);

            switch (jsonDoc.RootElement.ValueKind)
            {
                case JsonValueKind.Object:
                    Assert.That(jsonDoc.RootElement.EnumerateObject().Any(), "JSON object should not be empty");
                    break;
                case JsonValueKind.Array:
                    Assert.That(jsonDoc.RootElement.GetArrayLength() > 0, "JSON array should not be empty");
                    break;
            }
        }

        [Test]
        public async Task GetData_ShouldNotFail_WhenNoData()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _datasetsResourceManager.Create(creationMeta);

            var queryParams = new DatasetDWHQueryingParameters()
            {
                DatasetId = creationResult.Id,
                Attributes = new List<string>()
                {
                    "Attr1"
                },
                KPIs = new List<string>()
                {
                    "KPI1"
                },
                RowLimit = 100,
                Filters = new List<FilterCondition>()
                {
                    new FilterCondition()
                    {
                        Col = "date",
                        Op = "between",
                        Val = ["01-01-2077", "01-01-2078"]
                    }
                },
                ReportId = _report.Id
            };

            var dataResponse = await _datasetsResourceManager.GetData(queryParams, "test-user-id");
            Assert.That(dataResponse, Is.Not.Null);
            Assert.That(dataResponse.Data, Is.Null);
        }

        [Test]
        public async Task GetDataWithReportId_ShouldNotFail_WhenNoData()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _datasetsResourceManager.Create(creationMeta);

            var queryParams = new DatasetDWHQueryingParameters()
            {
                Attributes = new List<string>()
                {
                    "Attr1"
                },
                KPIs = new List<string>()
                {
                    "KPI1"
                },
                RowLimit = 100,
                Filters = new List<FilterCondition>()
                {
                    new FilterCondition()
                    {
                        Col = "date",
                        Op = "between",
                        Val = ["01-01-2077", "01-01-2078"]
                    }
                },
                ReportId = _report.Id
            };

            var dataResponse = await _datasetsResourceManager.GetData(queryParams, "test-user-id");
            Assert.That(dataResponse, Is.Not.Null);
            Assert.That(dataResponse.Data, Is.Null);
        }

        [Test]
        public async Task GetData_ShouldFail_WhenColDoesNotExist()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _datasetsResourceManager.Create(creationMeta);

            var queryParams = new DatasetDWHQueryingParameters()
            {
                DatasetId = creationResult.Id,
                Attributes = new List<string>()
                {
                    "Attr2"
                },
                KPIs = new List<string>()
                {
                    "KPI1"
                },
                RowLimit = 100,
                ReportId = _report.Id,
                Filters = new List<FilterCondition>()
                {
                    new FilterCondition()
                    {
                        Col = "date",
                        Op = "between",
                        Val = ["2023-01-01", "2023-01-02"]
                    }
                }
            };

            Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
            {
                await _datasetsResourceManager.GetData(queryParams, "test-user-id");
            });
        }

        [Test]
        public async Task GetDataWithReportId_ShouldFail_WhenColDoesNotExist()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _datasetsResourceManager.Create(creationMeta);

            var queryParams = new DatasetDWHQueryingParameters()
            {
                Attributes = new List<string>()
                {
                    "Attr2"
                },
                KPIs = new List<string>()
                {
                    "KPI1"
                },
                RowLimit = 100,
                ReportId = _report.Id,
                Filters = new List<FilterCondition>()
                {
                    new FilterCondition()
                    {
                        Col = "date",
                        Op = "between",
                        Val = ["2023-01-01", "2023-01-02"]
                    }
                }
            };

            Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
            {
                await _datasetsResourceManager.GetData(queryParams, "test-user-id");
            });
        }

        [Test]
        public async Task GetData_ShouldCreateTrackerRecord_WhenRequestIsProcessed()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _datasetsResourceManager.Create(creationMeta);

            var queryParams = new DatasetDWHQueryingParameters()
            {
                DatasetId = creationResult.Id,
                Attributes = new List<string>()
                {
                    "Attr1"
                },
                KPIs = new List<string>()
                {
                    "KPI1"
                },
                RowLimit = 100,
                ReportId = _report.Id,
                Filters = new List<FilterCondition>()
                {
                    new FilterCondition()
                    {
                        Col = "date",
                        Op = "between",
                        Val = ["2023-01-01", "2023-01-02"]
                    }
                }
            };

            var dataResponse = await _datasetsResourceManager.GetData(queryParams, "test-user-id");

            var tracker =
                await _dbContext.DatasetRunQueryTrackerEntities.FirstOrDefaultAsync(i =>
                    i.DatasetId == dataResponse.Id);

            Assert.That(tracker, Is.Not.Null);
        }

        // Test case for sample data
        [Test]
        public async Task GetDataSample_ShouldReturnDataForDataSampleUserRequest_WhenValid()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _datasetsResourceManager.Create(creationMeta);
            // PeriodStart = DateTime.Parse("01-01-2023"),
            // PeriodEnd = DateTime.Parse("01-01-2024"),
            var queryParams = new DatasetDWHQueryingParameters()
            {
                DatasetId = creationResult.Id,
                RowLimit = 100,
            };

            var dataResponse = await _datasetsResourceManager.GetData(queryParams, "test-user-id");
            var dataResult = dataResponse.Data;
            Assert.That(dataResponse, Is.Not.Null);
            Assert.That(dataResponse.RowLimit, Is.EqualTo(100));

            var jsonDoc = dataResult as System.Text.Json.JsonDocument;
            Assert.That(jsonDoc, Is.Not.Null, "Data should be a JsonDocument");
            Debug.Assert(jsonDoc != null, nameof(jsonDoc) + " != null");
            Assert.That(jsonDoc.RootElement.ValueKind != JsonValueKind.Undefined);
            Assert.That(jsonDoc.RootElement.ValueKind != JsonValueKind.Null);

            switch (jsonDoc.RootElement.ValueKind)
            {
                case JsonValueKind.Object:
                    Assert.That(jsonDoc.RootElement.EnumerateObject().Any(), "JSON object should not be empty");
                    break;
                case JsonValueKind.Array:
                    Assert.That(jsonDoc.RootElement.GetArrayLength() > 0, "JSON array should not be empty");
                    break;
            }
        }
        
        //     // Test getData with filtering
        //     
        //
        //     // TODO test with ONLY reportId
        //     
        //     


        // TODO add a test for GetDomainOptions
        [Test]
        public async Task GetDomainOptions_ShouldReturnDataOptions_WhenValid()
        {
            var creationMeta = new DatasetCreationMetaData()
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
                ReportId = _report.Id,
                DWHId = _dWH.Id,
                CurrentUser = "test-user-id"
            };

            var creationResult = await _datasetsResourceManager.Create(creationMeta);

            var key = new DatasetKey()
            {
                Id = creationResult.Id,
                CurrentUser = "test-user-id"
            };

            var result = await _datasetsResourceManager.GetDomainOptions(key);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DomainData, Is.Not.Null);
        }
    }
}