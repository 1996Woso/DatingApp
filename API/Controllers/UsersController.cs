using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Controllers;

[Authorize]
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
{
    // [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUsername();
        var users = await unitOfWork.UsersRepository.GetUsersDtoAsync(userParams);
        if (!users!.Any()) return NotFound("Users not found.");

        Response.AddPaginationHeader(users);
        return Ok(users);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await unitOfWork.UsersRepository.GetUserDtoById(id);
        if (user == null) return NotFound($"No user found with Id {id}.");
        return Ok(user);
    }
    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await unitOfWork.UsersRepository.GetUserDtoByUsernameAsync(username);
        if (user == null) return NotFound($"'{username}' is not found.");
        return Ok(user);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UpdateAppUserDTO updateAppUserDTO)
    {
        var user = await unitOfWork.UsersRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User is not found");
        mapper.Map(updateAppUserDTO, user);

        if (await unitOfWork.CompleteAsync()) return NoContent();
        return BadRequest("Faild to update the user.");
    }
   
}
