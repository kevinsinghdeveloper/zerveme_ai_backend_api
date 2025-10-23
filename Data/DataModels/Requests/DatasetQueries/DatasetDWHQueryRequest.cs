namespace zervemedata.Data.DataModels.Requests.DatasetQueries
{
    public class DatasetDWHQueryRequest
    {
        public Guid Id { get; set; }
        
        public DateTime PeriodStart { get; set; }
        
        public DateTime PeriodEnd { get; set; }
        
        public List<string> Attributes { get; set; }
        
        public List<string> KPIs { get; set; }

        public int RowLimit { get; set; } = 1000;
    }
}