using System.ComponentModel.DataAnnotations.Schema;

namespace zervemedata.Data.JsonDataModels
{
    public enum FieldType
    {
        Variable = 0,
        List = 1,
    }

    public class Field
    {
        public required string FieldName { get; set; }
        public List<string>? PossibleOptions { get; set; } // dropdown options
        public FieldType? FieldType { get; set; } = JsonDataModels.FieldType.Variable;
        public bool? is_multi { get; set; }
    }

    public class ReportConfigObject
    {
        [NotMapped]
        public required List<Field> Fields { get; set; }

        // config fields report specific
        // company name
        // website name
        // Product category
        // Top competitor names 
        // Location (country, state, region, city, town)
        // Research focus – City, State, Country, Continents, World wide
        // Market tags

        // base queries
    }
}