using zervemedata.Core.Contracts.Abstractions;
using zervemedata.Core.Services.BackgroundWorkers;

namespace zervemedata.api
{
    // dotnet add package Newtonsoft.Json --version 13.0.3 -s https://api.nuget.org/v3/index.json

    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using zervemedata.Data;
    using zervemedata.data.IdentityAuth;
    using zervemedata.Core.Services;
    using zervemedata.Core.Services.DataWarehouseManagers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using zervemedata.Data.AuthData;
    using zervemedata.Data.Enumerations;
    
    using IUserResourceManager = zervemedata.Core.Contracts.Abstractions.IUserResourceManager<
        zervemedata.Data.DataModels.Data.UserInfo,
        zervemedata.Data.Entities.Keys.UserKey,
        zervemedata.Data.DataModels.CreateMeta.UserCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.UserUpdateMetaData>;
    
    using IUserRolesResourceManager = zervemedata.Core.Contracts.Abstractions.IUserRolesResourceManager<
        zervemedata.Data.DataModels.Data.RoleInfo>;
    
    using IAuthenticationResourceManager = zervemedata.Core.Contracts.Abstractions.IAuthenticationResourceManager<
        zervemedata.Data.DataModels.CreateMeta.UserResponseMetaData, zervemedata.Data.DataModels.Responses.AuthUserResponse >;
    
    using IDatasetResourceManager = zervemedata.Core.Contracts.Abstractions.IDatasetResourceManager<
        zervemedata.Data.DataModels.CreateMeta.DatasetCreationMetaData,
        zervemedata.Data.Entities.Keys.DatasetKey,
        zervemedata.Data.DataModels.CreateMeta.DatasetUpdateMetaData,
        zervemedata.Data.DataModels.Data.DatasetData, zervemedata.Data.DataModels.Querying.DatasetDWHQueryingParameters,
        zervemedata.Data.DataModels.Responses.DatasetQueriedData,
        zervemedata.Data.DataModels.Responses.DatasetTemplateQueryData>;
    
    using IDwResourceManager = zervemedata.Core.Contracts.Abstractions.IDwResourceManager<
        zervemedata.Data.DataModels.CreateMeta.DWHCreationMetaData, zervemedata.Data.DataModels.CreateMeta.DWHUpdateMetaData,
        zervemedata.Data.DataModels.Data.DWHData>;
    using IDataWarehouseManager = zervemedata.Core.Contracts.Abstractions.IDataWarehouseManager<
        zervemedata.Data.DataModels.Querying.SelectQueryResponse
    >;
    using IOrganizationResourceManager = zervemedata.Core.Contracts.Abstractions.IOrganizationResourceManager<
        zervemedata.Data.DataModels.Data.OrganizationsInfo,
        zervemedata.Data.DataModels.CreateMeta.OrganizationCreationMetaData,
        zervemedata.Data.Entities.Keys.OrganizationKey,
        zervemedata.Data.DataModels.UpdateMeta.OrganizationUpdateMetaData>;
    
    using IProjectResourceManager = zervemedata.Core.Contracts.Abstractions.IProjectResourceManager<
        zervemedata.Data.DataModels.Data.ProjectInfo,
        zervemedata.Data.Entities.Keys.ProjectKey,
        zervemedata.Data.DataModels.CreateMeta.ProjectCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ProjectUpdateMetaData>;
    
    using IReportResourceManager = zervemedata.Core.Contracts.Abstractions.IReportResourceManager<
        zervemedata.Data.DataModels.Data.ReportInfo,
        zervemedata.Data.DataModels.Data.ReportTypeInfo,
        zervemedata.Data.DataModels.Data.DatasetData,
        zervemedata.Data.Entities.Keys.ReportKey,
        zervemedata.Data.DataModels.CreateMeta.ReportCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ReportUpdateMetaData>;

    using IJobResourceManager = zervemedata.Core.Contracts.Abstractions.IJobResourceManager<
        zervemedata.Data.DataModels.Data.JobInfo,
        zervemedata.Data.DataModels.Data.JobFreqTypeInfo,
        zervemedata.Data.Entities.Keys.JobKey,
        zervemedata.Data.DataModels.CreateMeta.JobCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.JobUpdateMetaData,
        zervemedata.Data.DataModels.UpdateMeta.JobScheduleUpdateMetaData
    >;
    using IReportDatasetResourceManager = zervemedata.Core.Contracts.Abstractions.IReportDatasetResourceManager<
        zervemedata.Data.DataModels.Data.ReportDatasetInfo,
        zervemedata.Data.Entities.Keys.ReportDatasetKey,
        zervemedata.Data.DataModels.CreateMeta.ReportDatasetCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ReportDatasetUpdateMetaData>;
    using IReportConfigurationResourceManager =
        zervemedata.Core.Contracts.Abstractions.IReportConfigurationResourceManager<
            zervemedata.Data.DataModels.Data.ReportConfigurationInfo,
            zervemedata.Data.DataModels.Data.ReportTypeInfo,
            zervemedata.Data.Entities.Keys.ReportConfigurationKey,
            zervemedata.Data.Entities.Keys.ReportKey,
            zervemedata.Data.DataModels.CreateMeta.ReportConfigurationCreationMetaData,
            zervemedata.Data.DataModels.CreateMeta.ReportTypeCreationMetaData,
            zervemedata.Data.DataModels.UpdateMeta.ReportConfigurationUpdateMetaData>;
    using IModelsResourceManager = zervemedata.Core.Contracts.Abstractions.IModelsResourceManager<
        zervemedata.Data.DataModels.Data.ModelsInfo,
        zervemedata.Data.DataModels.Data.ModelTypeInfo,
        zervemedata.Data.Entities.Keys.ModelsKey,
        zervemedata.Data.DataModels.CreateMeta.ModelsCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ModelsUpdateMetaData>;
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public IConfiguration Configuration { get; }
        private readonly string _policyName = "CorsPolicy";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserResourceManager, UsersResourceManager>();
            services.AddScoped<IUserRolesResourceManager, UsersRolesResourceManager>();
            services.AddScoped<IAuthenticationResourceManager, AuthenticationResourceManager>();
            services.AddScoped<IDatasetResourceManager, DatasetsResourceManager>();
            services.AddScoped<IDwResourceManager, DWHResourceManager>();
            services.AddScoped<IOrganizationResourceManager, OrganizationsResourceManager>();
            services.AddScoped<IProjectResourceManager, ProjectResourceManager>();
            services.AddScoped<IReportResourceManager, ReportResourceManager>();
            services.AddScoped<IJobResourceManager, JobResourceManager>();
            services.AddScoped<IReportDatasetResourceManager, ReportDatasetResourceManager>();
            services.AddScoped<IReportConfigurationResourceManager, ReportConfigurationResourceManager>();
            services.AddScoped<IModelsResourceManager, ModelsResourceManager>();
            
            services.AddSingleton<DatawarehouseHandlerService>();
            services.AddHostedService<JobSchedulerService>();
            
            services.AddAuthorization(options =>
            {
                // User role policy
                options.AddPolicy(RoleConstants.User, policy =>
                    policy.RequireRole(RoleConstants.User));

                // Admin role policy (includes User role)
                options.AddPolicy(RoleConstants.Admin, policy =>
                    policy.RequireRole(RoleConstants.Admin));

                // SuperAdmin role policy (includes Admin and User roles)
                options.AddPolicy(RoleConstants.SuperAdmin, policy =>
                    policy.RequireRole(RoleConstants.SuperAdmin));
            });
            
            services.AddHttpContextAccessor();
            
            services.AddDbContext<ZervemedataDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("WebApi")));

            services.AddControllers();


            services.AddCors(opt =>
            {
                opt.AddPolicy(name: _policyName, builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ZerveMe API",
                    Description = "Authentication + Data Management",
                });
                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
            },
            new string[] {}
            }
            });
            });
            // For Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ZervemedataDbContext>()
            .AddDefaultTokenProviders();
            // Adding Authentication
            services.Configure<IdentityOptions>(options =>
                {
                    // make weak passwords possible
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })

                // Adding Jwt Bearer
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"] ??
                                                   throw new InvalidOperationException()))
                    };
                });

            //var options = Configuration.GetAWSOptions();
            //IAmazonS3 client = options.CreateServiceClient<IAmazonS3>();
            //var options = Configuration.GetAWSOptions();
            //IAmazonS3 client = options.CreateServiceClient<IAmazonS3>();


            //var awsOptions = Configuration.GetAWSOptions("AWS");
            //awsOptions.Credentials = new BasicAWSCredentials(Configuration["AWS:AWSAccessKey"], Configuration["AWS:AWSSecretKey"]);

            //services.AddAWSService<IAmazonS3>(awsOptions);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
            , UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            ZervemedataDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "clientCentralNet v1"));
            }

            // Seeding roles :
            // http://www.binaryintellect.net/articles/5e180dfa-4438-45d8-ac78-c7cc11735791.aspx

            app.UseHttpsRedirection();
            app.UseRouting();

            //app.UseCors(_policyName);
            app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            );

            //app.UseHttpsRedirection(); 


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            IdentityDataInitializer.SeedData(userManager, roleManager, dbContext);

        }
    }
}
