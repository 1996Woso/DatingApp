using API.Extensions;
using API.Interfaces;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId = User.GetUserId();
            if (sourceUserId == targetUserId) return BadRequest("You can not like yourself");

            var existingLike = await unitOfWork.LikesRepository.GetUserLikeAsync(sourceUserId, targetUserId);
            if (existingLike == null)
            {
                var like = new UserLike
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };
                unitOfWork.LikesRepository.AddLike(like);
            }
            else
            {
                unitOfWork.LikesRepository.DeleteLike(existingLike);
            }

            if (await unitOfWork.CompleteAsync()) return Ok();

            return BadRequest("Failed to update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult> GetCurrentUserLikeIds()
        {
            return Ok(await unitOfWork.LikesRepository.GetCurrentUserLikeIdsAsync(User.GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await unitOfWork.LikesRepository.GetUserLikesAsync(likesParams);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }
    }
}
