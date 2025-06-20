using System;

namespace API.Models.DTOs;

public class AppUserDTO
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Age { get; set; }
    public string? KnownAs { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }
    public string? Gender { get; set; }
    public string? Introduction { get; set; }
    public string? Interests { get; set; }
    public string? LookingFor { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public List<PhotoDTO>? Photos { get; set; }
    public string? Token { get; set; }
}
