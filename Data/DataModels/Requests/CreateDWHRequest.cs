namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;
    
    public class CreateDWHRequest
    {
        public string? Name { get; set; }
        
        public string Schema { get; set; } 
        
        public string ConnectionString { get; set; }

        public DWH? DWH { get; set; }
    }
}