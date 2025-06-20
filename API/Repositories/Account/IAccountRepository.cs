using System;
using System.Runtime.CompilerServices;
using API.Models.Domain;
using API.Models.DTOs;

namespace API.Repositories.Account;

public interface IAccountRepository
{
    Task<AppUser> RegisterAsync(RegisterDTO registerDTO);
    Task<bool> CorrectPasswordAsync(LoginDTO loginDTO);
    Task<AppUser> LoginAsync(LoginDTO loginDTO);
}
