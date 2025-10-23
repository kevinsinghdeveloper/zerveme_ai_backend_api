namespace zervemedata.Data.DataModels.Responses
{
    using zervemedata.Core.Contracts.Abstractions;
    using Newtonsoft.Json.Linq;

    public class DatasetTemplateQueryData : IDatasetQueriedResponse
    {
        // Dataset Id
        public Guid Id { get; set; }
        public string? VizResponseTemplate { get; set; }
        public string? VizResponseData { get; set; }
    }
}