using zervemedata.Data.Enumerations;

namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    public class UpdateDWHRequest
    {
        public Guid Id { get; set;}
        
        public string? Name { get; set; }
        
        public string Schema { get; set; } 
        
        public string ConnectionString { get; set; }
        
        public DWH? DWH { get; set; }
    }
}