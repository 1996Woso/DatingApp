using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionMiddleware> logger;
    private readonly IHostEnvironment hostEnvironment;

    public ExceptionMiddleware(RequestDelegate next
    , ILogger<ExceptionMiddleware> logger
    , IHostEnvironment hostEnvironment
    )
    {
        this.next = next;
        this.logger = logger;
        this.hostEnvironment = hostEnvironment;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = hostEnvironment.IsDevelopment()
                ? new ApiException(httpContext.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiException(httpContext.Response.StatusCode, ex.Message, "Internal server error!");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            await httpContext.Response.WriteAsync(json);
        }
    }

}
