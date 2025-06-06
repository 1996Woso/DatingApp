using System;
using API.Models;

namespace API.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser appUser);
}
