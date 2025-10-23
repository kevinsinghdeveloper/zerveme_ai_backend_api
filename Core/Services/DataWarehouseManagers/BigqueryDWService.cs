/*
To set up the connection string, you'll need to first obtain a JSON key file for a service account that has access to your BigQuery project.
Then you can use the following format for the connection string:
Data Source=projects/{project-id}/jsonKey={path-to-json-key-file}

*/

using Core.Contracts.ContractModels;

namespace zervemedata.Core.Services.DataWarehouseManagers
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Google.Cloud.BigQuery.V2;
    using zervemedata.Data.DataModels.Querying;
    using IDataWarehouseManager = zervemedata.Core.Contracts.Abstractions.IDataWarehouseManager<
        zervemedata.Data.DataModels.Querying.SelectQueryResponse>;
    
    public class BigqueryDWService : IDataWarehouseManager
    {
        //private readonly IConfiguration configuration;
        private readonly string configurationString;
        public BigqueryDWService(string configurationString)//IConfiguration? configuration)
        {
            //this.configuration = configuration;
            this.configurationString = configurationString;
        }

        public void ExecuteQuery(string query)
        {
            var client = BigQueryClient.Create(this.configurationString);
            var result = client.ExecuteQuery(query, parameters: null);
        }

        public SelectQueryResponse? SelectQuery(string query)
        {
            
            var client = BigQueryClient.Create(this.configurationString);
            var results = client.ExecuteQuery(query, parameters: null);

            var queriedResult = new SelectQueryResponse()
            {
                rowsReturned = 0
            };

            if ((results?.TotalRows ?? 0) > 0)
            {
                var jsonSerializer = new JsonSerializer();
                var streamWriter = new StringWriter();
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.WriteStartArray();
                    foreach (var row in results)
                    {
                        var record = new JObject();
                        foreach (var field in row.Schema.Fields)
                        {
                            var value = row[field.Name];
                            if (value != null)
                            {
                                record.Add(field.Name, JToken.FromObject(value));
                            }
                            else
                            {
                                record.Add(field.Name, JValue.CreateNull());
                            }
                        }
                        jsonSerializer.Serialize(jsonWriter, record);
                    }
                    jsonWriter.WriteEndArray();
                }

                queriedResult.result = streamWriter.ToString();
                return queriedResult;
            }

            return queriedResult;
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
            throw new NotImplementedException();
        }

        public string GetAggregationMethod(string method)
        {
            throw new NotImplementedException();
        }

        public string GetWhereClauseString(List<FilterCondition>? filters)
        {
            throw new NotImplementedException("PostgresDWService.GetWhereClauseString");
        }
    }
}