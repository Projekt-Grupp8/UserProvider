namespace Infrastructure.Models;

public enum StatusCode
{
    OK = 200,
    ERROR = 400,
    UNAUTHORIZED = 401,
    NOT_FOUND = 404,
    EXISTS = 409,
    INTERNAL = 500,
}

public class ResponseResult
{
    public StatusCode StatusCode { get; set; }
    public object? ContentResult { get; set; }
    public string? Message { get; set; }
    public bool? Succeeded { get; set; }
    //public override bool Equals(object obj)
    //{
    //    if (obj is ResponseResult other)
    //    {
    //        return ContentResult == other.ContentResult &&
    //               Message == other.Message &&
    //               StatusCode == other.StatusCode &&
    //               Succeeded == other.Succeeded;
    //    }
    //    return false;
    //}

    //public override int GetHashCode()
    //{
    //    return HashCode.Combine(ContentResult, Message, StatusCode, Succeeded);
    //}
}
