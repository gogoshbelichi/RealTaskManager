using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealTaskManager.GraphQL.Configurations;
using RealTaskManager.GraphQL.Tasks;

namespace RealTaskManager.GraphQL;

public class TokenGenerator(IOptions<MagicOptions> magicOptions)
{
    public string GenerateToken(string username, string email, string userId, IEnumerable<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = DetermineDescriptor(username, email, userId, roles);
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private List<Claim> SetClaimsAsync(string username, string email, string userId, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.PreferredUsername, username )
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    private SecurityTokenDescriptor DetermineDescriptor(string username, string email, string userId, IEnumerable<string> roles)
    {
        var key = Encoding.UTF8.GetBytes(magicOptions.Value.MagicString);

        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(SetClaimsAsync(username, email, userId, roles)),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "https://auth.chillicream.com",
            Audience = "https://graphql.chillicream.com",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);
        return refreshToken;
    }
}