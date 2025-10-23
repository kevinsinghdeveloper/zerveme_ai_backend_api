namespace zervemedata.Data.JsonDataModels
{
    public class IntermediateStorageType
    {
        public InMemory in_memory { get; set; }
    }

    public class InMemory
    {
        public bool push_to_staging_per_increment { get; set; }
        public int buffer_size { get; set; }
    }

    public class DataParameters
    {
        public DataSourceType? json { get; set; }
        public DataSourceType? csv { get; set; }
        public DataSourceType? excel { get; set; }
    }

    public class DataSourceType
    {
        public Options? options { get; set; }
    }
    
    public class Options
    {
        public List<string>? nested_keys { get; set; }
        public List<string>? parent_keys { get; set; }
        public bool? flatten_structure { get; set; }
        public string? sheet_name { get; set; }
        public int? header { get; set; }
    }

    public class Credentials
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

    public class Authentication
    {
        public string? authentication_url { get; set; }
        public Credentials? credentials { get; set; }
    }

    public class SourcePaginationConfig
    {
        public string? next_cursor { get; set; }
        public string? previous_cursor { get; set; }
        public string? query { get; set; }
        public int? max_request_per_timeout { get; set; }
        public int? timeout_secs { get; set; }
    }
    public class EtlConfig
    {
        public string identifier { get; set; }
        public string load_type { get; set; }
        public List<string> key { get; set; }
        public Dictionary<string, string> date_columns_format_string { get; set; }
        public string source_name { get; set; }
        public string source_type { get; set; }
        public List<string> source_file_type { get; set; }
        public string source_url { get; set; }
        public SourcePaginationConfig? source_pagination_config { get; set; }
        public Authentication? authentication { get; set; }
        public int frequency { get; set; }
        public IntermediateStorageType intermediate_storage_type { get; set; }
        public Dictionary<string, string> cloud_storage_config { get; set; }
        public Dictionary<string, string> dbt_config { get; set; }
        public DataParameters data_parameters { get; set; }
        public string? pre_transform_code_block { get; set; }
    }

    public class DatasetEtlConfiguration
    {
        public Guid? Id { get; set; } = Guid.NewGuid();
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Deleted { get; set; }

        public EtlConfig? Config { get; set; }
    }

    public class DbtConfig
    {
    }
    
    
}