using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Extensions;
using API.Models;
using API.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{

    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if (users == null) return;
        var roles = new List<AppRole>
        {
            new() {Name = "Member"},
            new() {Name = "Admin"},
            new() {Name = "Moderator"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }
        foreach (var user in users)
        {
            user.UserName = user.UserName!.ToLower();
            foreach (var photo in user.Photos)
            {
                photo.AppUser = user;
            }
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
        }
        var admin = new AppUser
        {
            UserName = "Eviwe",
            KnownAs = "Woso",
            Gender = "male",
            City = "Johannesburg",
            Country = "South Africa",
            DateOfBirth = new DateOnly(1996, 5, 31),
            Photos =
            [
                new() {
                    Url = "https://res.cloudinary.com/dfaqqc2ge/image/upload/v1768138416/dating-app/zfxam6fj3fuitpre4jtb.jpg",
                    IsMain = true,
                    IsApproved = true
                }
            ]
        };
        foreach (var photo in admin.Photos)
        {
            photo.AppUser = admin;
        }
        await userManager.CreateAsync(admin, "1123Woso@");
        await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
    }
}
