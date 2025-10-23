using Google.Apis.Util;
using zervemedata.Data.DataModels.Responses;

namespace zervemedata.Core.Services
{
    using zervemedata.Data.DataModels.CreateMeta;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using zervemedata.Data;
    using zervemedata.Data.AuthData;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
    
    using IAuthenticationResourceManager = zervemedata.Core.Contracts.Abstractions.IAuthenticationResourceManager<
    zervemedata.Data.DataModels.CreateMeta.UserResponseMetaData, zervemedata.Data.DataModels.Responses.AuthUserResponse >;

    public class AuthenticationResourceManager : IAuthenticationResourceManager
    {
        private readonly UserManager<ApplicationUser> userManager;
        
        private readonly IConfiguration configuration;

        public AuthenticationResourceManager(ZervemedataDbContext dbContext, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;

            this.configuration = configuration;
        }


        public async Task<AuthUserResponse?> Authenticate(UserResponseMetaData responseMetaData)
        {
            var userAuthMetaData = (UserResponseMetaData) responseMetaData;

            var user = userAuthMetaData.userNameOrEmail.Contains("@")
                ? await this.userManager.FindByEmailAsync(userAuthMetaData.userNameOrEmail)
                : await this.userManager.FindByNameAsync(userAuthMetaData.userNameOrEmail);

            // First check if user exists
            if (user == null) return null;

            // Then check password
            if (!await this.userManager.CheckPasswordAsync(user, userAuthMetaData.password))
            {
                return null;
            }
    
            var userRoles = await this.userManager.GetRolesAsync(user);
            
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString()),
            };
    
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(this.configuration["JWT:SecretKey"] ?? throw new InvalidOperationException()));

            var token = new JwtSecurityToken(
                issuer: this.configuration["JWT:ValidIssuer"],
                audience: this.configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            user.Updated = DateTime.Now;
    
            await this.userManager.UpdateAsync(user);
    
            return new AuthUserResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                UserName = user.UserName // Include the username in the response
            };
        }

    }
}