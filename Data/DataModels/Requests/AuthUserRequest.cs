using System.ComponentModel.DataAnnotations;

namespace zervemedata.Data.DataModels.Requests
{
    public class AuthUserRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "A UserName or Email must be specified")]
        public string UserNameOrEmail { get; set; } = "";
        
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "A Password must be specified")]
        public string Password { get; set; } = "";
    }
}