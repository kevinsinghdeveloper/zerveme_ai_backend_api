using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using zervemedata.Data.AuthData;

namespace zervemedata.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class OrgUserEntity : IBaseEntity
    {
        public Guid Id { get; set; }


        [ForeignKey("OrganizationEntity")]
        public Guid OrganizationId { get; set; }

        [ForeignKey("User")] public string UserId { get; set; } = null!;

        public virtual OrganizationEntity OrganizationEntity { get; set; } =
            null!; // TODO this should be a list for many 

        public virtual ApplicationUser User { get; set; } =
            null!; // TODO this should be a list for many -- unsure if a user should exist more than once here

        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }= DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}