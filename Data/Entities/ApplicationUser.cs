using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.Entities;

namespace zervemedata.Data.AuthData
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IBaseEntity
    {
        [Required] [MaxLength(75)] public string FirstName { get; set; } = null!;

        [Required] [MaxLength(75)] public string LastName { get; set; } = null!;

        [ForeignKey("ContactEntity")]
        public Guid? ContactId { get; set; }

        public ContactEntity? ContactEntity { get; set; }

        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}