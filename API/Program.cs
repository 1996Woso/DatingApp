using API;
using API.Data;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
//When Client and API are not from origin e.g localhost:5002 and localhost:5001
builder.Services.AddCors();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseCors(x => x.AllowAnyHeader()
  .AllowAnyMethod()
//   .WithOrigins("http://localhost:4200", "https://localhost:4200")
  .AllowAnyOrigin()

);
app.MapControllers();

app.Run();
