using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebSlon.Data.Entities.Identity;
using WebSlon.Interfaces;

namespace WebSlon.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<UserEntity> _userManager;

        public JwtTokenService(IConfiguration configuration, 
            UserManager<UserEntity> userManager)
        {
            _config = configuration;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(UserEntity user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            List<Claim> claims = new()
            {
                new Claim("email", user.Email),
                new Claim("name", $"{user.FirstName} {user.LastName}")
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            var singinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtSecretKey")));
            var signinCredentials = new SigningCredentials(singinKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                signingCredentials: signinCredentials,
                expires: DateTime.Now.AddDays(10),
                claims: claims
                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }
    }
}
