namespace zervemedata.Data.DataModels.UpdateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    public class UserUpdateMetaData : IUpdateMetaData
    {
        public String Id { get; set; }

        public string? userName { get; set; }

        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public string? emailAddress { get; set; }

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