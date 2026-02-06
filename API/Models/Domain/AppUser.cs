using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using API.Extensions;
using API.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace API.Models.Domain;

public class AppUser : IdentityUser<int>
{
    public DateOnly DateOfBirth { get; set; }
    [StringLength(100)]
    public required string KnownAs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    [StringLength(10)]
    public required string Gender { get; set; }
    [StringLength(1000)]
    public string? Introduction { get; set; }
    [StringLength(1000)]
    public string? Interests { get; set; }
    [StringLength(100)]
    public string? LookingFor { get; set; }
    [StringLength(50)]
    public required string City { get; set; }
    [StringLength(50)]
    public required string Country { get; set; }
    // [JsonIgnore]
    public List<Photo> Photos { get; set; } = [];

    public List<UserLike> LikedByUsers { get; set; } = [];
    public List<UserLike> LikedUsers { get; set; } = [];
    public List<Message> MessagesSent { get; set; } = [];
    public List<Message> MessagesReceived { get; set; } = [];
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
}

