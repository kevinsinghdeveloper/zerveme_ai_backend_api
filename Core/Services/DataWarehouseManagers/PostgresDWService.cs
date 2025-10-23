using Core.Contracts.ContractModels;
using Microsoft.IdentityModel.Tokens;
using zervemedata.Data.JsonDataModels;

namespace zervemedata.Core.Services.DataWarehouseManagers
{
    using System.Data;
    using Npgsql;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using zervemedata.Data.DataModels.Querying;
    using IDataWarehouseManager = zervemedata.Core.Contracts.Abstractions.IDataWarehouseManager<
        zervemedata.Data.DataModels.Querying.SelectQueryResponse
    >;

    public class PostgresDWService : IDataWarehouseManager
    {
        //private readonly IConfiguration configuration;
        private readonly string configurationString;
        public PostgresDWService(string configurationString)//IConfiguration? configuration)
        {
            //this.configuration = configuration;
            this.configurationString = configurationString;
        }

        public void ExecuteQuery(string query)
        {
            string connectionString = this.configurationString;//this.configuration.GetConnectionString("PostgresDwConnection");

            using var connection = new NpgsqlConnection(connectionString);
            
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            
            command.CommandType = CommandType.Text;

            command.ExecuteNonQuery();
        }


        public SelectQueryResponse? SelectQuery(string query)
        {
            string connectionString = this.configurationString;//this.configuration.GetConnectionString("PostgresDwConnection");

            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(query, connection);

            using var reader = command.ExecuteReader();

            var queriedResult = new SelectQueryResponse()
            {
                rowsReturned = 0
            };

            var rowCount = 0;
            
            if (reader.HasRows)
            {
                var jsonSerializer = new JsonSerializer();
                var streamWriter = new StringWriter();
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.WriteStartArray();
                    while (reader.Read())
                    {
                        var record = new JObject();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            record.Add(reader.GetName(i), JToken.FromObject(reader.GetValue(i)));
                        }
                        jsonSerializer.Serialize(jsonWriter, record);
                        rowCount += 1;
                    }
                    jsonWriter.WriteEndArray();
                }

                queriedResult.result = streamWriter.ToString();
                queriedResult.rowsReturned = rowCount;

                return queriedResult;
            }

            return queriedResult;

        }

        public string GetWhereClauseString(List<FilterCondition>? filters)
        {
            /*
             Supported ops
                BETWEEN
                =
                <>
                >=
                <=
             */
            // TODO re do this. In the op we should only support between and = then determine what to use below
            if (filters == null || filters.Count == 0)
                return "";

            var conditions = new List<string>();

            foreach (var filter in filters)
            {
                string condition;

                // Handle IN clause
                if (filter.Op == "=")
                {
                    if (!filter.Val.IsNullOrEmpty())
                    {
                        var inValues = string.Join(", ", filter.Val.Select(v => $"'{v.ToLower()}'"));
                        condition = $"lower({filter.Col}) IN ({inValues})";
                    }
                    else
                    {
                        condition = $"{filter.Col} IS NULL";
                    }
                }
                else if (filter.Op == "<>")
                {
                    if (!filter.Val.IsNullOrEmpty())
                    {
                        var inValues = string.Join(", ", filter.Val.Select(v => $"'{v.ToLower()}'"));
                        condition = $"lower({filter.Col}) NOT IN ({inValues})";
                    }
                    else
                    {
                        condition = $"{filter.Col} IS NOT NULL";
                    }
                }
                else if (filter.Op == ">=")
                {
                    if (filter.Val.Count == 1)
                    {
                        condition = $"{filter.Col} >= '{filter.Val[0]}'";
                    }
                    else
                    {
                        throw new ArgumentException($"'>=' operator requires exactly 1 value.");
                    }
                }
                else if (filter.Op == "<=")
                {
                    if (filter.Val.Count == 1)
                    {
                        condition = $"{filter.Col} <= '{filter.Val[0]}'";
                    }
                    else
                    {
                        throw new ArgumentException($"'<=' operator requires exactly 1 value.");
                    }
                }
                else if (filter.Op.ToUpper() == "BETWEEN")
                {
                    if (filter.Val.Count == 2)
                    {
                        condition = $"{filter.Col} BETWEEN '{filter.Val[0]}' AND '{filter.Val[1]}'";
                    }
                    else
                    {
                        throw new ArgumentException($"BETWEEN operator requires exactly 2 values.");
                    }
                }

                else
                {
                    throw new ArgumentException(
                        $"Unsupported filter operation: {filter.Op}. Supported operations are: =, <>, >=, <=, BETWEEN");
                }

                conditions.Add(condition);
            }

            return string.Join(" AND ", conditions);
        }

        public string GetAggregationMethod(string method)
        {
            if (!Enum.TryParse<AggregationType>(method, true, out var parsedMethod))
                throw new ArgumentException($"Unknown aggregation method: {method}");

            return parsedMethod switch
            {
                AggregationType.Average => "AVG",
                AggregationType.Sum => "SUM",
                AggregationType.Count => "COUNT",
                AggregationType.Min => "MIN",
                AggregationType.Max => "MAX",
                _ => throw new ArgumentException($"Unknown aggregation method: {method}")
            };
        }

        public string? ConstructSelectQuery(string tableName,
            string schemaName,
            List<string> kpiCols,
            List<string> attrCols,
            Dictionary<string, string> aggMethods,
            List<FilterCondition>? filters,
            int? rowLimit
        )
        {
            var selectColumns = new List<string>(attrCols);

            string? whereClause = null;
            if (filters is not null)
            {
                whereClause = GetWhereClauseString(filters);
            }

            foreach (var col in kpiCols)
            {
                if (aggMethods.TryGetValue(col, out var method))
                {
                    var aggMethod = GetAggregationMethod(method);
                    selectColumns.Add($"{aggMethod}({col}) AS {col}");
                }
                else
                {
                    // fallback: just include column
                    selectColumns.Add(col);
                }
            }

            var selectCols = selectColumns.Any() ? string.Join(",", selectColumns) : "*";
            
            var query = $@"
                SELECT {selectCols} FROM {schemaName}.{tableName} WHERE 1=1
            ";

            if (whereClause != null)
            {
                query += $" AND {whereClause}";
            }

            // Group by goes here
            if (kpiCols.Count > 0 && attrCols.Count > 0)
            {
                var groupByColumns = string.Join(",", attrCols);
                query += $" GROUP BY {groupByColumns}";
            }

            if (rowLimit != null)
            {
                query += $" LIMIT {rowLimit}";
            }
            
            query += ";";

            return query;
        }
    }
}