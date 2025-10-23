namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;
    
    public class DWHUpdateMetaData : IUpdateMetaData
    {
        public Guid Id { get; set;}
        
        public string? Name { get; set; }
        
        public string Schema { get; set; } 
        
        public string ConnectionString { get; set; }
        
        public DWH? DWH { get; set; }
    }

}