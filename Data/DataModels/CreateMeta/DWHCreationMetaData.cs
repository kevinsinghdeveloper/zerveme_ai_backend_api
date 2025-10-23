using System.ComponentModel.DataAnnotations;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;

    public class DWHCreationMetaData : ICreationMetaData
    {
        public string? Name { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "A schema is required")]
        public string Schema { get; set; } 
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "A Connection string to the datawarehouse is required")]
        public string ConnectionString { get; set; }
        
        public DWH? DWH { get; set; }
    }

}