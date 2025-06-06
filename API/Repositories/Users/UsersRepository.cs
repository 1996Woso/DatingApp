using System;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly DataContext dataContext;

    public UsersRepository(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user!;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await dataContext.Users.ToListAsync();
    }
    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
        return user;
    }
     public async Task<bool> UserExistsAsync(string username)
    {
        return await dataContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }

}
