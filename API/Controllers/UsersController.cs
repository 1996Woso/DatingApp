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
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService) : BaseApiController
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
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
    {
        var user = await unitOfWork.UsersRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Cannot find user");

        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;
        user.Photos.Add(photo);
        if (await unitOfWork.CompleteAsync())
            return CreatedAtAction(nameof(GetUserByUsername),
            new { username = user.UserName },mapper.Map<PhotoDTO>(photo));

        return BadRequest("Problem adding photo");
    }
    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await unitOfWork.UsersRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User is not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("Can use this photo as main photo");

        var  currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        if (await unitOfWork.CompleteAsync()) return NoContent();

        return BadRequest("Problem setting main photo.");
    }
    
    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await unitOfWork.UsersRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User not foudd");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        if (photo == null || photo.IsMain) return BadRequest("Photo cannot be deleted");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);
        if (await unitOfWork.CompleteAsync()) return Ok(new{message = "Photo has been deleted successfully"});
         return BadRequest("Problem deleting photo");
    }
}
