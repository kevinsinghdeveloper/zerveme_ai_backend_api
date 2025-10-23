namespace zervemedata.Core.Contracts.Abstractions
{
    public interface IQueriedResponse
    {
        public int rowsReturned { get; set; }
        public string? result { get; set; }
        public string? error { get; set; }
    }
}