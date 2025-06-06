using System;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class UsersController : BaseApiController
{
    private readonly IUsersRepository usersRepository;

    public UsersController(IUsersRepository usersRepository)
    {
        this.usersRepository = usersRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await usersRepository.GetUsersAsync();
        if (!users.Any())
        {
            return NotFound("No users found.");
        }
        return Ok(users);
    }
    [Authorize]
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await usersRepository.GetUserByIdAsync(id);
        if (user == null) return NotFound($"No user found with Id {id}.");
        return Ok(user);
    }
}
