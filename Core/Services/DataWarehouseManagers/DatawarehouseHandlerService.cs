namespace zervemedata.Core.Services.DataWarehouseManagers
{
    using zervemedata.Data.Entities;
    using zervemedata.Data.Enumerations;
    using IDataWarehouseManager = zervemedata.Core.Contracts.Abstractions.IDataWarehouseManager<
        zervemedata.Data.DataModels.Querying.SelectQueryResponse>;
    public class DatawarehouseHandlerService
    {
        public IDataWarehouseManager? GetDatawarehouseService(DWHEntity? dwhEntity)
        {
            if (dwhEntity != null)
            {
                return dwhEntity.Dwh switch
                {
                    // TODO how to make dynamic? ENUM because these services would need to be coded, cannot be dynamic
                    DWH.Postgres => new PostgresDWService(dwhEntity.ConnectionString),
                    DWH.Bigquery => new BigqueryDWService(dwhEntity.ConnectionString),
                    _ => null
                };
            }

            return null;
        }
    }
}