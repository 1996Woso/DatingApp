using System;

namespace API.Models;

public class UserParams: PaginationParams
{
    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 120;
    public string? City { get; set; }
    public string? Country { get; set; }

    public string OrderBy { get; set; } = "lastActive";
}
