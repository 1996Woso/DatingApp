using System;
using API.Controllers;
using API.Extensions;
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
    }
}
