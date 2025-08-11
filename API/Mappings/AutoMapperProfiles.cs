using System;
using API.Controllers;
using API.Extensions;
using API.Models;
using API.Models.Domain;
using API.Models.DTOs;
using AutoMapper;

namespace API.Mappings;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, AppUserDTO>()
            .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
            .ForMember(d => d.PhotoUrl, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));

        CreateMap<Photo, PhotoDTO>().ReverseMap();

        CreateMap<UpdateAppUserDTO, AppUser>().ReverseMap();
        CreateMap<RegisterDTO, AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));

        CreateMap<Message, MessageDTO>()
            .ForMember(d => d.SenderPhotoUrl,
            o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d => d.RecipientPhotoUrl,
            o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        //To fix wrong dates displayed on the client
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
