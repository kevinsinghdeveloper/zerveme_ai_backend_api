namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class UserInfo : IResource
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

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

        public Guid? OrganizationId { get; set; }
    }
}