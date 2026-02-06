using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Interfaces;
using API.Models.Domain;
using API.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Account;

public class AccountRepository : IAccountRepository
{
    private readonly IMapper mapper;
    private readonly UserManager<AppUser> userManager;

    public AccountRepository(IMapper mapper
    , UserManager<AppUser> userManager
    )
    {
        this.mapper = mapper;
        this.userManager = userManager;
    }

    public async Task<AppUser> RegisterAsync(RegisterDTO registerDTO)
    {
        await Task.Delay(0);
        var user = mapper.Map<AppUser>(registerDTO);
        user.UserName = registerDTO.Username.ToLower();
        return user;
    }

    public async Task<bool> CorrectPasswordAsync(LoginDTO loginDTO)
    {
        var result = await userManager.CheckPasswordAsync(await LoginAsync(loginDTO), loginDTO.Password);

        if (!result) return false;
        return true;
    }

    public async Task<AppUser> LoginAsync(LoginDTO loginDTO)
    {
        // var user = await usersRepository.GetUserByUsernameAsync(loginDTO.Username);
        var user = await userManager.Users
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(x =>
                x.NormalizedUserName == loginDTO.Username.ToUpper());
        return user!;
    }
}
