using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models.Domain;
using Microsoft.IdentityModel.Tokens;
using API.Interfaces;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration configuration;

    public TokenService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<string> CreateTokenAsync(AppUser appUser)
    {
        var tokenKey = configuration["TokenKey"] ?? throw new Exception("Cannot access token key from appsettings.");
        if (tokenKey.Length < 64) throw new Exception("Your tokenKey must be atleast 64 characters long");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, appUser.UserName)
        };
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
