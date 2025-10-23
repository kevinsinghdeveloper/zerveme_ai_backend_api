using System.ComponentModel.DataAnnotations;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;
    public class UserCreationMetaData : ICreationMetaData
    {
        public string UserName { get; set; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string Password { get; set; } = "";

        public string EmailAddress { get; set; } = "";

        public List<Role> Roles { get; set; } = [];

        public string? Address1 { get; set; } 

        public string? Address2 { get; set; }

        public string? Address3 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? ZipCode { get; set; }

        public string? PrimaryPhone { get; set; }

        public string? SecondaryPhone { get; set; }

        public string? NotificationEmail { get; set; }

        public string CurrentUser { get; set; } = null!;
    }

}