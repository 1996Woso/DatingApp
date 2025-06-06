using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
//When Client and API are not from the same origin e.g localhost:5002 and localhost:5001
builder.Services.AddCors();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseCors(x => x.AllowAnyHeader()
  .AllowAnyMethod()
//   .WithOrigins("http://localhost:4200", "https://localhost:4200")
  .AllowAnyOrigin()

);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
