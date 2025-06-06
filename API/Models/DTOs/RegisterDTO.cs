using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs;

public class RegisterDTO
{
    [Required]
    [MaxLength(100)]
    public required string Username { get; set; }
    [Required]
    [MaxLength(100)]
    public required string Password { get; set; }
}
