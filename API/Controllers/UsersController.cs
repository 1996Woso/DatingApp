using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Models;
using API.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUsersRepository usersRepository;
    private readonly IMapper mapper;

    public UsersController(IUsersRepository usersRepository
        , IMapper mapper
    )
    {
        this.usersRepository = usersRepository;
        this.mapper = mapper;
    }
    // [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await usersRepository.GetUsersDtoAsync();
        if (!users!.Any()) return NotFound("Users not found.");
        return Ok(users);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await usersRepository.GetUserDtoById(id);
        if (user == null) return NotFound($"No user found with Id {id}.");
        return Ok(user);
    }
    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await usersRepository.GetUserDtoByUsernameAsync(username);
        if (user == null) return NotFound($"'{username}' is not found.");
        return Ok(user);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UpdateAppUserDTO updateAppUserDTO)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null) return BadRequest("No username found in token");

        var user = await usersRepository.GetUserByUsernameAsync(username);
        if (user == null) return BadRequest("User is not found");
        mapper.Map(updateAppUserDTO, user);

        if (await usersRepository.SaveAllAsync()) return NoContent();
        return BadRequest("Faild to update the user.");
    }
}
