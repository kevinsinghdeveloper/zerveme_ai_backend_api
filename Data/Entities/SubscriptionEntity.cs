using System.ComponentModel.DataAnnotations.Schema;
using zervemedata.Data.AuthData;
using System.ComponentModel.DataAnnotations;
using Data.Entities;

namespace zervemedata.Data.Entities
{
    public class SubscriptionEntity : IBaseEntity
    {
        public Guid Id { get; set; }

        [ForeignKey("OrganizationEntity")]
        public Guid OrganizationId { get; set; }

        public virtual OrganizationEntity OrganizationEntity { get; set; } = null!;
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; }= DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}