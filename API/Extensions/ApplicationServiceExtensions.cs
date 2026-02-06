using System;
using API.Data;
using API.Mappings;
using API.Models;
using API.Repositories;
using API.Repositories.Account;
using API.Services;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using API.SignalR;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services
        , IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddControllers();
        // services.AddDbContext<DataContext>(opt =>
        // {
        //     opt.UseSqlite(connectionString);
        // });
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlServer(connectionString);
        });
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddAutoMapper(typeof(AutoMapperProfiles));
        //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<ITokenService, TokenService>();
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

