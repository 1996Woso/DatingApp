using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using API.Models.DTOs;

namespace API.Models.Domain;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; }
    public string? PublicId { get; set; }
    //Navigation properties
    public int AppUserId { get; set; }
    [JsonIgnore]
    public AppUser AppUser { get; set; } = null!;

}
