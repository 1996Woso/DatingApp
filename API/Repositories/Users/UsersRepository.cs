using System;
using API.Data;
using API.Models.Domain;
using API.Models.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;

    public UsersRepository(DataContext dataContext
    , IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user!;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await dataContext.Users
            .Include(x => x.Photos)
            .ToListAsync();
    }
    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        var user = await dataContext.Users
               .Include(x => x.Photos)
               .SingleOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
        return user;
    }
    public async Task<bool> UserExistsAsync(string username)
    {
        return await dataContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
    public async Task<bool> SaveAllAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }

    public async Task<AppUserDTO?> GetUserDtoByUsernameAsync(string username)
    {
        return await dataContext.Users
            .Where(x => x.UserName.ToLower() == username.ToLower())
            .ProjectTo<AppUserDTO>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<AppUserDTO>?> GetUsersDtoAsync()
    {
        return await dataContext.Users
            .ProjectTo<AppUserDTO>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<AppUserDTO?> GetUserDtoById(int id)
    {
        return await dataContext.Users
            .Where(x => x.Id == id)
            .ProjectTo<AppUserDTO>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }
}
