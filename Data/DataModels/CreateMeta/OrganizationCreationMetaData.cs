using System.ComponentModel.DataAnnotations;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;

    public class OrganizationCreationMetaData : ICreationMetaData
    {
        public string Name { get; set; } = "";

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

        public string? CurrentUser { get; set; }
    }
}