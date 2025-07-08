using System;
using API.Data;
using API.Mappings;
using API.Models;
using API.Repositories;
using API.Repositories.Account;
using API.Services;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services
        , IConfiguration configuration
    )
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddAutoMapper(typeof(AutoMapperProfiles));
        //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<ITokenService, TokenService>();
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>(); 

        return services;
    }
}
