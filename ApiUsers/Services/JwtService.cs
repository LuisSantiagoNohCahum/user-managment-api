using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiUsers.Services
{
    public sealed class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Generate(User user)
        {
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email ?? ""),
                new Claim("RolId", user.RolId.ToString())
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtToken:Key"] ?? "")),
                    SecurityAlgorithms.HmacSha256),
                Audience = _configuration.GetValue<string>("JwtToken:Audience"),
                Issuer = _configuration.GetValue<string>("JwtToken:Issuer"),
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return await Task.FromResult(tokenHandler.WriteToken(securityToken));
        }
    }
}
