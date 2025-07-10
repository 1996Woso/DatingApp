using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs;

public class RegisterDTO
{
    [Required]
    [StringLength(30, MinimumLength = 4)]
    public string Username { get; set; } = "";
    public required string KnownAs { get; set; }
    public required string Gender { get; set; }
    public required string DateOfBirth { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    [Required]
    [StringLength(30, MinimumLength = 4)]
    public string Password { get; set; } = "";
}
