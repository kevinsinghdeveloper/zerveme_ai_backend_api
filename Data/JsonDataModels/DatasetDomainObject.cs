using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using zervemedata.Core.Contracts.Abstractions;

namespace zervemedata.Data.JsonDataModels
{
    using System.Collections.Generic;

    public enum AggregationType
    {
        Sum,
        Average,
        Count,
        Min,
        Max
    }

    public enum ColumnType
    {
        Text,
        Numeric,
        Date
    }
    
    public class ColumnObject
    {
        public required string ColumnName { get; set; }
        public required ColumnType ColumnType { get; set; }

        public AggregationType AggregationType { get; set; } = AggregationType.Sum;
    }
    
    public class DomainData
    {
        [NotMapped] public List<ColumnObject>? KpiCols { get; set; }
        
        [NotMapped] public List<ColumnObject>? AttrCols { get; set; }

        [MaxLength(150)] public string? CustomSql { get; set; }

        [MaxLength(150)] public string? TableName { get; set; }
    }
}