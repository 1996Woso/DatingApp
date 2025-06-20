using System;
using API.Models.Domain;
using API.Models.DTOs;

namespace API;

public interface IUsersRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<bool> UserExistsAsync(string username);
    Task<bool> SaveAllAsync();
    Task<AppUserDTO?> GetUserDtoByUsernameAsync(string username);
    Task<IEnumerable<AppUserDTO>?> GetUsersDtoAsync();
    Task<AppUserDTO?> GetUserDtoById(int id);
}
