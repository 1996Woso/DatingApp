using API.Extensions;
using API.Interfaces;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository likesRepository;

        public LikesController(ILikesRepository likesRepository)
        {
            this.likesRepository = likesRepository;
        }

        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId = User.GetUserId();
            if (sourceUserId == targetUserId) return BadRequest("You can not like yourself");

            var existingLike = await likesRepository.GetUserLikeAsync(sourceUserId, targetUserId);
            if (existingLike == null)
            {
                var like = new UserLike
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };
                likesRepository.AddLike(like);
            }
            else
            {
                likesRepository.DeleteLike(existingLike);
            }

            if (await likesRepository.SaveChangesAsync()) return Ok();

            return BadRequest("Failed to update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult> GetCurrentUserLikeIds()
        {
            return Ok(await likesRepository.GetCurrentUserLikeIdsAsync(User.GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await likesRepository.GetUserLikesAsync(likesParams);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }
    }
}
