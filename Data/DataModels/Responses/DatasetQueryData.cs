namespace zervemedata.Data.DataModels.Responses
{
    using zervemedata.Core.Contracts.Abstractions;
    using Newtonsoft.Json.Linq;

    public class DatasetQueriedData : IDatasetQueriedResponse
    {
        // Dataset Id
        public Guid Id { get; set; }

        public int QueryRunTimeElapsedSecs { get; set; }
        
        public int RowLimit { get; set; }

        public int RowsReturned { get; set; }

        public List<string>? Error { get; set; }

        public object? Data { get; set; }
    }
}