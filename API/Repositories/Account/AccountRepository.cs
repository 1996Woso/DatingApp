using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Models.Domain;
using API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Repositories.Account;

public class AccountRepository : IAccountRepository
{
    private readonly DataContext dataContext;

    public IUsersRepository UsersRepository { get; }

    public AccountRepository(DataContext dataContext
    , IUsersRepository usersRepository)
    {
        this.dataContext = dataContext;
        UsersRepository = usersRepository;
    }

    public async Task<AppUser> RegisterAsync(RegisterDTO registerDTO)
    {
        // using var hmac = new HMACSHA512();
        // var user = new AppUser
        // {
        //     UserName = registerDTO.Username,
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
        //     PasswordSalt = hmac.Key
        // };
        // await dataContext.AddAsync(user);
        // await dataContext.SaveChangesAsync();
        return null;
    }
    public async Task<bool> CorrectPasswordAsync(LoginDTO loginDTO)
    {
        var user = await UsersRepository.GetUserByUsernameAsync(loginDTO.Username);

        using var hmac = new HMACSHA512(user!.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return false;
        }
        return true;
    }
    public async Task<AppUser> LoginAsync(LoginDTO loginDTO)
    {
        var user = await UsersRepository.GetUserByUsernameAsync(loginDTO.Username);
        return user!;
    }
}
