﻿using Infrastructure.Models;

namespace Infrastructure.Factories;

public class ResponseFactory
{
    public static ResponseResult Ok()
    {
        return new ResponseResult
        {
            Message = "OK",
            StatusCode = StatusCode.OK,
        };
    }

    public static ResponseResult Ok(string? message = "")
    {
        return new ResponseResult
        {
            Message = message ?? "OK",
            StatusCode = StatusCode.OK,
        };
    }
    public static ResponseResult Ok(object obj)
    {
        return new ResponseResult
        {
            ContentResult = obj,
            StatusCode = StatusCode.OK,
        };
    }

    public static ResponseResult Ok(object obj1, object obj2)
    {
        return new ResponseResult
        {
            ContentResult = new { obj1, obj2 },
            StatusCode = StatusCode.OK,
        };
    }

    public static ResponseResult Ok(object obj, string? message = "")
    {
        return new ResponseResult
        {
            ContentResult = obj,    
            Message = message ?? "OK.",
            StatusCode = StatusCode.OK,
        };
    }

    public static ResponseResult Error(string? message = null)
    {
        return new ResponseResult
        {
            Message = message ?? "Error.",
            StatusCode = StatusCode.ERROR,
        };
    }

    public static ResponseResult Unauthorized(string? message = null)
    {
        return new ResponseResult
        {
            Message = message ?? "Invalid credentials.",
            StatusCode = StatusCode.UNAUTHORIZED,
        };
    }

    public static ResponseResult NotFound(string? message = null)
    {
        return new ResponseResult
        {
            Message = message ?? "Not found.",
            StatusCode = StatusCode.NOT_FOUND
        };
    }

    public static ResponseResult Exists(string? message = null, bool? Succeeded = false)
    {
        return new ResponseResult
        {
            Message = message ?? "Already exists.",
            StatusCode = StatusCode.EXISTS,
            Succeeded = Succeeded
           
        };
    }

    public static ResponseResult InternalError(string? message = null)
    {
        return new ResponseResult
        {
            Message = message ?? "Woops, something went wrong.",
            StatusCode = StatusCode.INTERNAL
        };
    }
}
