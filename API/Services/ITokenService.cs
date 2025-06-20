using System;
using API.Models.Domain;

namespace API.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser appUser);
}
