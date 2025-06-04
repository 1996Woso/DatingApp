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

    public async Task<AppUser> GetUserById(int id)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user!;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await dataContext.Users.ToListAsync();
    }
}
