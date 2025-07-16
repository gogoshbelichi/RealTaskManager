using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.Configurations;
using RealTaskManager.UseCases.Authentication.RefreshTokens;

namespace RealTaskManager.GraphQL;

public class TokenService(
    IOptions<MagicOptions> magicOptions,
    UserManager<TaskManagerUser> userManager,
    AddRefreshTokenHandler addRefreshToken,
    GetRefreshTokenHandler getRefreshToken)
{
    public async Task<string> GenerateAccessToken(string jti, TaskManagerUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var userId = user.Id;
        var username = user.UserName ?? "The username is not specified";
        var email = user.Email!; //checked in endpoint
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = SetClaims(jti, username, email, userId, roles);
        var tokenDescriptor = DetermineDescriptor(claims);
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private List<Claim> SetClaims(string jti, string username, string email, string userId, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.PreferredUsername, username )
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        return claims;
    }

    private SecurityTokenDescriptor DetermineDescriptor(List<Claim> claims)
    {
        var key = Encoding.UTF8.GetBytes(magicOptions.Value.MagicString);

        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(magicOptions.Value.MagicMinutesToAccess),
            Issuer = magicOptions.Value.MagicIssuer,
            Audience = magicOptions.Value.MagicAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), //better not to use symmetric keys
                SecurityAlgorithms.HmacSha256Signature)
            //ECDsaSecurityKey(ECDsa.Create(ECCurve.CreateFromFriendlyName("nistp256"))
        };
    }
    
    public async Task<string?> GenerateRefreshToken( string jti, TaskManagerUser user, CancellationToken ct)
    {
        var refreshToken = new RefreshTokenData
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Jti = jti,
            User = user,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(magicOptions.Value.MagicDaysForRefresh),
        };
        
        if (await addRefreshToken.HandleAsync(refreshToken, ct))
            return refreshToken.Token;
        
        return null;
    }
    
    /// <summary>
    /// Returns user if refresh token token is valid
    /// </summary>
    /// <returns>TaskManagerUser or null</returns>
    public async Task<TaskManagerUser?> TryGetUserByRefreshToken(GetRefreshTokenRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.RefreshToken) || string.IsNullOrEmpty(request.AccessToken))
            return null;

        var refreshToken = await getRefreshToken.HandleAsync(request, ct);
        
        if (refreshToken is null || refreshToken.IsUsed || refreshToken.IsExpired || refreshToken.IsRevoked )
            return null;

        refreshToken.UseRefreshToken(refreshToken);

        return ValidateRefreshToken(request.AccessToken, refreshToken.Jti) == false ? null : refreshToken.User;
    }
    
    /// <summary>
    /// Validates request tokens
    /// </summary>
    /// <returns>Boolean</returns>
    private bool ValidateRefreshToken(string accessToken, string realJti)
    {
        var key = Encoding.UTF8.GetBytes(magicOptions.Value.MagicString);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(
            accessToken,
            new TokenValidationParameters
            {
                ValidIssuer = magicOptions.Value.MagicIssuer,
                ValidAudience = magicOptions.Value.MagicAudience,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            },
            out var validatedToken 
        );
        
        var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        
        return !string.IsNullOrEmpty(jti) && jti == realJti;
    }
}