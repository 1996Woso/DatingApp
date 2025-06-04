using System;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
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
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await usersRepository.GetUserById(id);
        if (user == null) return NotFound($"No user found with Id {id}.");
        return Ok(user);
    }
}
