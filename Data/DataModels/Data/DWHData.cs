namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;

    public class DWHData : IResource
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        
        public string Schema { get; set; } 
        
        public string ConnectionString { get; set; }
        
        public string dwhName { get; set;}
        
    }
}