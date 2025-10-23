namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    using zervemedata.Data.Enumerations;

    public class UpdateProjectRequest
    {
        public Guid Id { get; set; }

        [MaxLength(150)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A name must be specified")]
        public string Name { get; set; } = null!;

        [MaxLength(1000)] public string? Description { get; set; }
    }
}