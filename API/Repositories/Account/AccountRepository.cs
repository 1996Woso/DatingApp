using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Interfaces;
using API.Models.Domain;
using API.Models.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Account;

public class AccountRepository : IAccountRepository
{
    private readonly DataContext dataContext;
    private readonly IUsersRepository usersRepository;
    private readonly IMapper mapper;

    public AccountRepository(DataContext dataContext
    , IUsersRepository usersRepository
    ,IMapper mapper
    )
    {
        this.dataContext = dataContext;
        this.usersRepository = usersRepository;
        this.mapper = mapper;
    }

    public async Task<AppUser> RegisterAsync(RegisterDTO registerDTO)
    {
        using var hmac = new HMACSHA512();
        var user = mapper.Map<AppUser>(registerDTO);
        user.UserName = registerDTO.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        user.PasswordSalt = hmac.Key;
        await dataContext.AddAsync(user);
        await dataContext.SaveChangesAsync();
        return user;
    }

    public async Task<bool> CorrectPasswordAsync(LoginDTO loginDTO)
    {
        var user = await usersRepository.GetUserByUsernameAsync(loginDTO.Username);

        using var hmac = new HMACSHA512(user!.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
                return false;
        }
        return true;
    }

    public async Task<AppUser> LoginAsync(LoginDTO loginDTO)
    {
        var user = await usersRepository.GetUserByUsernameAsync(loginDTO.Username);
        return user!;
    }
}
