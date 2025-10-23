using zervemedata.Data.Enumerations;

namespace zervemedata.Data.DataModels.Requests
{
    using System.ComponentModel.DataAnnotations;
    public class CreateUserRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "A UserName must be specified")]
        public string UserName { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "A FirstName must be specified")]
        public string FirstName { get; set; } = "";
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "A LastName must be specified")]
        public string LastName { get; set; } = "";
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "A Password must be specified")]
        public string Password { get; set; } = "";
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "A VerifyPassword must be specified")]
        public string VerifyPassword { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "A EmailAddress must be specified")]
        public string EmailAddress { get; set; } = "";

        public List<Role>? Roles { get; set; }

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
    }
}