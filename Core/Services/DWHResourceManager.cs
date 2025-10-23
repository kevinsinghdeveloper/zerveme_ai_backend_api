using zervemedata.Core.Contracts.Abstractions;
using zervemedata.Data.DataModels.Data;

namespace zervemedata.Core.Services
{
    using zervemedata.Core.Extensions;
    using zervemedata.Data.DataModels.CreateMeta;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using zervemedata.Data;
    using zervemedata.Data.AuthData;
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    using zervemedata.Data.Enumerations;
    
    using IDwResourceManager = zervemedata.Core.Contracts.Abstractions.IDwResourceManager<
        zervemedata.Data.DataModels.CreateMeta.DWHCreationMetaData, zervemedata.Data.DataModels.CreateMeta.DWHUpdateMetaData,
        zervemedata.Data.DataModels.Data.DWHData
    >;

    public class DWHResourceManager(
        ZervemedataDbContext dbContext)
        : IDwResourceManager
    {
        public async Task<bool> Create(DWHCreationMetaData creationMetaData)
        {
            var dwhEntity = new DWHEntity()
            {
                Id = Guid.NewGuid(),
                Name = creationMetaData.Name,
                PrimarySchema = creationMetaData.Schema,
                ConnectionString = creationMetaData.ConnectionString,
                Dwh = creationMetaData.DWH,
                DwhLoadingStatus = DWHLoadingStatus.Ready
            };

            await dbContext.AddAsync(dwhEntity);

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<DWHData> Update(DWHUpdateMetaData updateMetaData)
        {
            var dwhUpdateMetaData = updateMetaData;

            if (dwhUpdateMetaData.Id == Guid.Empty)
            {
                throw new Exception("The Id is empty");
            }

            var entity = await dbContext.DwhEntities.Where(d => d.Id == dwhUpdateMetaData.Id)
                .FirstOrDefaultAsync() ?? throw new Exception("The DWH could not be found!");

            entity.Name = dwhUpdateMetaData.Name;
            entity.PrimarySchema = dwhUpdateMetaData.Schema;
            entity.ConnectionString = dwhUpdateMetaData.ConnectionString;
            entity.Dwh = dwhUpdateMetaData.DWH;

            dbContext.Update(entity);

            await dbContext.SaveChangesAsync();
            
            // should return a model
            return entity.ToDataModel();
        }

        public async Task<DWHData> Get(Guid Id)
        {
            var entity = await dbContext.DwhEntities.Where(d => d.Id == Id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception("The DWH could not be found!");
            }

            return entity.ToDataModel();
        }

        public async Task<IEnumerable<DWHData>> GetAll()
        {
            var entities = await dbContext.DwhEntities.ToListAsync();

            if (!entities.Any())
            {
                throw new Exception("No DWH found!");
            }

            return entities.Select(e => e.ToDataModel());
        }

        public async Task<DWHData> SwapSchemas(Guid Id)
        {
            var entity = await dbContext.DwhEntities.Where(d => d.Id == Id)
                .FirstOrDefaultAsync() ?? throw new Exception("The DWH could not be found!");

            if (entity.SecondarySchema is null)
            {
                throw new Exception("Secondary schema is not defined!");
            }

            (entity.PrimarySchema, entity.SecondarySchema) = (entity.SecondarySchema, entity.PrimarySchema);

            dbContext.Update(entity);

            await dbContext.SaveChangesAsync();
            
            // should return a model
            return entity.ToDataModel();
        }
    }
}