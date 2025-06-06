using System;
using API.Controllers;
using API.Models;
using API.Models.DTOs;
using AutoMapper;

namespace API.Mappings;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, AppUserDTO>().ReverseMap();
    }
}
