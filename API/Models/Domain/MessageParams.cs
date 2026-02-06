using System;

namespace API.Models.Domain;

public class MessageParams : PaginationParams
{
    
    public string? Username { get; set; }
    public string Container { get; set; } = "Unread";
}
