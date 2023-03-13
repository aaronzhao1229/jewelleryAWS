using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
  public class TokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        public TokenService(UserManager<User> userManager, IConfiguration config)  // refers to the configurations in appsettings.Development.json
        {
            _config = config;
            _userManager = userManager;

        }

        public async Task<string> GenerateToken(User user)
        {
            // create the content to put inside the payload
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),

            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // create a signature
            // we need to ensure this key never leaves the server and it's protected on the server and in a safest place we can leave it in the server because anybody has access to ths key can then pretend to be any user in our application, including admin.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:TokenKey"])); // need configuration in appsetting.Development.json to match this, otherwise there will be an error
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenOptions = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds

            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}