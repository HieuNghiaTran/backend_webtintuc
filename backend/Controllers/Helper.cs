using backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers
{
    public class Helper
    {
        private readonly IConfiguration _config; 

        public Helper(IConfiguration config)
        {
            this._config = config;
        }

        public string generateJWTToken(User user)
        {
            
            Console.Write($"Generating token for user: {user.username}");

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                new Claim(ClaimTypes.Name, user.username),
                
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["ApplicationSettings:JWT_Secret"]));

            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
