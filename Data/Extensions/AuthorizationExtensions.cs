namespace zervemedata.Data.Extensions
{   
    using Microsoft.AspNetCore.Authorization;
    using System;

    public static class AuthorizationExtensions
    {
        public static AuthorizationPolicyBuilder RequireRole(this AuthorizationPolicyBuilder policy, params Enum[] roles)
        {
            var roleNames = roles.Select(role => role.ToString()).ToArray();
            return policy.RequireRole(roleNames);
        }
    }
}