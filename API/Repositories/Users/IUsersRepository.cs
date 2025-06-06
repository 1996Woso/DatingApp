using System;
using API.Models;

namespace API;

public interface IUsersRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);
    Task<bool> UserExistsAsync(string username);
}
