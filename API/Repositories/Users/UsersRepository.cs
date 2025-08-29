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
using Microsoft.AspNetCore.Identity;

namespace API.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;
    private readonly UserManager<AppUser> userManager;

    public UsersRepository(DataContext dataContext
    , IMapper mapper
    ,UserManager<AppUser> userManager
    )
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
        this.userManager = userManager;
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
               .SingleOrDefaultAsync(x => x.NormalizedUserName == username.ToUpper());
        return user;
    }
    public async Task<bool> UserExistsAsync(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }

    public async Task<AppUserDTO?> GetUserDtoByUsernameAsync(string username)
    {
        return await dataContext.Users
            .Where(x => x.NormalizedUserName == username.ToUpper())
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
        //Search by filtering
        var searchString = userParams.SearchString;
        if (!string.IsNullOrEmpty(searchString))
        {

            query = query.Where(x =>
                EF.Functions.Like(x.City, $"%{searchString}%")
                || EF.Functions.Like(x.Country, $"%{searchString}%")
                || EF.Functions.Like(x.UserName, $"%{searchString}%")
                || EF.Functions.Like(x.KnownAs, $"%{searchString}%")
            );
        }
        //Order by
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
