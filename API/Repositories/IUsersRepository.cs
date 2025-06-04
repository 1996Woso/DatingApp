using System;
using API.Models;

namespace API;

public interface IUsersRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserById(int id);
}
