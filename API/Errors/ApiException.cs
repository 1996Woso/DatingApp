using System;

namespace API.Errors;

public class ApiException
{
    public int StatusCode { get; }
    public string Message { get; }
    public string? Details { get; }
    public ApiException(int statusCode, string message, string? details)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}

// //Or 
// public class ApiException(int statusCode, string message, string? details)
// {
//     public int StatusCode { get; set; } = statusCode;
//     public string Message = message;
//     public string? Details = details;

// }