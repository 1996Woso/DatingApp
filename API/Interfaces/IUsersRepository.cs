using System;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;

namespace API.Interfaces;

public interface IUsersRepository
{
    Task<PagedList<AppUser>> GetUsersAsync(UserParams userParams);
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<bool> UserExistsAsync(string username);
    Task<AppUserDTO?> GetUserDtoByUsernameAsync(string username);
    Task<PagedList<AppUserDTO>> GetUsersDtoAsync(UserParams userParams);
    Task<AppUserDTO?> GetUserDtoById(int id);
}
