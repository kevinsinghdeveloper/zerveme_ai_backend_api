using Core.Contracts.ContractModels;

namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IDataWarehouseManager<TQueriedResponse>
        where TQueriedResponse : IQueriedResponse
    {
        void ExecuteQuery(string query);

        TQueriedResponse? SelectQuery(string query);

        string? ConstructSelectQuery(string tableName,
            string schemaName,
            List<string> kpiCols,
            List<string> attrCols,
            Dictionary<string, string> aggMethods,
            List<FilterCondition>? filters,
            int? rowLimit
        );

        public string GetAggregationMethod(string method);

        public string GetWhereClauseString(List<FilterCondition>? filters);
    }
}