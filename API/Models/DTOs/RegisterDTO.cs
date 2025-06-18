using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs;

public class RegisterDTO
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = "";
    [Required]
    [StringLength(100, MinimumLength = 4)]
    public string Password { get; set; } = "";
}
