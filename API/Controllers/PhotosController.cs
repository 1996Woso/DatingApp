using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class PhotosController(IPhotoService photoService, IUnitOfWork unitOfWork
    , IMapper mapper, DataContext dataContext) : BaseApiController
{
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
        user.Photos.Add(photo);
        if (await unitOfWork.CompleteAsync())
            return CreatedAtAction("GetUserByUsername", "Users",
            new { username = user.UserName }, mapper.Map<PhotoDTO>(photo));

        return BadRequest("Problem adding photo");
    }
    [HttpGet]
    public async Task<ActionResult> GetUnApprovedPhotos([FromQuery] PhotoParams photoParams)
    {
        var query = dataContext.Photos
            .Where(x => x.IsApproved == false);
        var photos = await PagedList<Photo>
            .CreateAsync(query, photoParams.PageNumber, photoParams.PageSize);
        Response.AddPaginationHeader(photos);
        
        return Ok(photos);
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await unitOfWork.UsersRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User is not found");

        var photo = user.Photos
            .Where(x => x.IsApproved == true)
            .FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("Cannot use this photo as main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        if (await unitOfWork.CompleteAsync()) return NoContent();

        return BadRequest("Problem setting main photo.");
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPut("approve-photo/{photoId:int}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var user = await unitOfWork.UsersRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User not found");

        var photo = dataContext.Photos
            .FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return BadRequest("Photo not found");
        photo.IsApproved = true;

        var currentMain = await dataContext.Photos.AnyAsync(x => x.AppUserId == photo.AppUserId && x.IsMain);
        if (!currentMain) photo.IsMain = true;

        if (await unitOfWork.CompleteAsync()) return NoContent();
        return BadRequest("Problem approving photo");

    }

    [HttpDelete("reject-photo/{photoId:int}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var user = await unitOfWork.UsersRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User not found");

        var photo = dataContext.Photos
            .FirstOrDefault(x => x.Id == photoId && x.IsApproved == false);

        if (photo == null) return BadRequest("Photo not found");
        
        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        dataContext.Photos.Remove(photo);
        if (await unitOfWork.CompleteAsync()) return Ok(new { message = "Photo has been rejected and removed successfully" });
        return BadRequest("Problem rejecting photo");
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
        if (await unitOfWork.CompleteAsync()) return Ok(new { message = "Photo has been deleted successfully" });
        return BadRequest("Problem deleting photo");
    }
}