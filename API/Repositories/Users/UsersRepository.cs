using System;
using API.Data;
using API.Models.Domain;
using API.Models.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.Models;
using System.IO.Compression;

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
        //var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        return await dataContext.Users.FindAsync(id);
    }

    public async Task<PagedList<AppUser>> GetUsersAsync(UserParams userParams)
    {
        var query = dataContext.Users
            .ProjectTo<AppUser>(mapper.ConfigurationProvider);
        return await PagedList<AppUser>.CreateAsync(query,userParams.PageNumber, userParams.PageSize);
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

    public async Task<PagedList<AppUserDTO>> GetUsersDtoAsync(UserParams userParams)
    {
        var query = dataContext.Users.AsQueryable();
        query = query.Where(x => x.UserName != userParams.CurrentUsername);

        if (userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }
        //Age filtering
        var minDoB = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDoB = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
        query = query.Where(x => x.DateOfBirth >= minDoB && x.DateOfBirth <= maxDoB);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.LastActive)
        };
        
        return await PagedList<AppUserDTO>.CreateAsync(query.ProjectTo<AppUserDTO>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
            
    }

    public async Task<AppUserDTO?> GetUserDtoById(int id)
    {
        return await dataContext.Users
            .Where(x => x.Id == id)
            .ProjectTo<AppUserDTO>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }
}
