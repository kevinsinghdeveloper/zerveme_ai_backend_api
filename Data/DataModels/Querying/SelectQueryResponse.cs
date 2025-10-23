namespace zervemedata.Data.DataModels.Querying
{
    using zervemedata.Core.Contracts.Abstractions;

    public class SelectQueryResponse : IQueriedResponse
    {
        public int rowsReturned { get; set; }
        public string? result { get; set; }
        public string? error { get; set; }
    }
}