using System;
using API.Models.Domain;

namespace API.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser appUser);
}
