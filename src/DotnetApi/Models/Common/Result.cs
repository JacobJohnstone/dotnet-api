using System.Net;

namespace DotnetApi.Models.Common;

public class Result<TData>
{
    public HttpStatusCode Status { get; }
    public TData? Data { get; }
    public bool IsSuccess { get; }
    public string? Exception { get; }

    // successful response constructor format
    // private to ensure static methods are used for instantiation
    private Result(TData data, HttpStatusCode status)
    {
        Status = status;
        Data = data;
        IsSuccess = true;
        Exception = null;
    }
    
    // unsuccessful response constructor format
    // private to ensure static methods are used for instantiation
    private Result(string exception, HttpStatusCode status)
    {
        Status = status;
        Data = default;
        IsSuccess = false;
        Exception = exception;
    }
    
    public static Result<TData> Success(TData value, HttpStatusCode status) => new(value, status);
    public static Result<TData> Failure(string exception, HttpStatusCode status) => new(exception, status);
}