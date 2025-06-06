using System;

namespace API.Models.DTOs;

public class AppUserDTO
{
    public required string Token { get; set; }
    public required string UserName { get; set; }
}
