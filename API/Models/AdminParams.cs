using System;

namespace API.Models;

public class AdminParams : PaginationParams
{
    public string? Username { get; set; }
}
