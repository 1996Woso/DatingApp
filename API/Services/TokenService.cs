using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models.Domain;
using Microsoft.IdentityModel.Tokens;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration configuration;
    private readonly UserManager<AppUser> userManager;

    public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
    {
        this.configuration = configuration;
        this.userManager = userManager;
    }
    public async Task<string> CreateTokenAsync(AppUser appUser)
    {
        var tokenKey = configuration["TokenKey"] ?? throw new Exception("Cannot access token key from appsettings.");
        if (tokenKey.Length < 64) throw new Exception("Your tokenKey must be atleast 64 characters long");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        if (appUser.UserName == null) throw new Exception("Username cannot be null");
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new(ClaimTypes.Name, appUser.UserName)
        };

        var roles = await userManager.GetRolesAsync(appUser);
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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
