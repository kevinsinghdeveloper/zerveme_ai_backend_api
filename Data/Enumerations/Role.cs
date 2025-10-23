namespace zervemedata.Data.Enumerations
{
    public enum Role
    {
        User,

        Admin,
        
        SuperAdmin
    }
    
    public static class RoleConstants
    {
        public const string User = nameof(Role.User);
        public const string Admin = nameof(Role.Admin);
        public const string SuperAdmin = nameof(Role.SuperAdmin);
    }
}