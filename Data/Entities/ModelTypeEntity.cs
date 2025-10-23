using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;

namespace zervemedata.Data.Entities
{
    using zervemedata.Data.Enumerations;

    public class ModelTypeEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [MaxLength(100)] public string? Name { get; set; } = null!;

        [MaxLength(255)] public string Description { get; set; } = null!;

        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}