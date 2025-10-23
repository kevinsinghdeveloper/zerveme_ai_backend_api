using Data.Entities;
using zervemedata.Data.AuthData;

namespace zervemedata.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class ContactEntity : IBaseEntity
    {
        public Guid Id { get; init; }

        [MaxLength(255)]
        public string? Address1 { get; set; }

        [MaxLength(255)]
        public string? Address2 { get; set; }

        [MaxLength(255)]
        public string? Address3 { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(25)]
        public string? State { get; set; }

        [MaxLength(75)]
        public string? Country { get; set; }

        [MaxLength(15)]
        public string? ZipCode { get; set; }

        [MaxLength(20)]
        public string? PrimaryPhone { get; set; }

        [MaxLength(20)]
        public string? SecondaryPhone { get; set; }

        [MaxLength(50)]
        public string? NotificationEmail { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Deleted { get; set; }
    }
}