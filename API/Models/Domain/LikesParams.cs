using System;

namespace API.Models.Domain;

public class LikesParams : PaginationParams
{
    public int UserId { get; set; }
    public required string Predicate { get; set; } = "liked";
}
