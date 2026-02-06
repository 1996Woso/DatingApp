using System.Threading.Tasks;
using API.Extensions;
using API.Models;
using API.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        
        [HttpGet("users-with-roles")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> GetUsersWithRoles([FromQuery] AdminParams adminParams)
        {
            var query = userManager.Users.AsQueryable()
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    Username = x.UserName,
                    Roles = x.UserRoles.Select(r => r.Role.Name).ToList()

                });

            if (!string.IsNullOrEmpty(adminParams.Username))
            {
                var collate = "SQL_Latin1_General_CP1_CI_AI";
                query = query.Where(x => 
                    EF.Functions.Like(
                        EF.Functions.Collate(x.Username, collate), $"%{adminParams.Username.Trim()}%"
                    )
                );
            }

            var users = await PagedList<dynamic>.CreateAsync(query,adminParams.PageNumber, adminParams.PageSize);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> EditRoles(string username, string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select atleast one role");

            var selectedRoles = roles.Split(",").ToArray();

            var user = await userManager.FindByNameAsync(username);
            if (user == null) return BadRequest($"{username.ToUpper()} is not found");

            var userRoles = await userManager.GetRolesAsync(user);

            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await userManager.GetRolesAsync(user));
        }

        [HttpGet("photos-to-moderate")]
        [Authorize(Policy = "ModeratePhotoRole")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Only Admins or Moderators can see this");
        }

    }
}
