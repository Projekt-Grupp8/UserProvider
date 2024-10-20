namespace Infrastructure.Models;

public enum StatusCode
{
    OK = 200,
    ERROR = 400,
    INVALID_CREDENTIALS = 401,
    NOT_FOUND = 404,
    EXISTS = 409,
    INTERNAL = 500,
}

public class ResponseResult
{
    public StatusCode StatusCode { get; set; }
    public object? ContentResult { get; set; }
    public string? Message { get; set; }
}
