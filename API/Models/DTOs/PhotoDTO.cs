using System;

namespace API.Models.DTOs;

public class PhotoDTO
{
    public int Id { get; set; }
    public  string? Url { get; set; }
    public bool IsMain { get; set; }
}
