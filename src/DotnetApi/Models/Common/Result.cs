namespace DotnetApi.Models.Common;

public class Result<TData>
{
    public TData? Data { get; }
    public bool IsSuccess { get; }
    public string? Exception { get; }

    // successful response constructor format
    protected Result(TData data)
    {
        Data = data;
        IsSuccess = true;
        Exception = null;
    }
    
    // unsuccessful response constructor format
    protected Result(string exception)
    {
        Data = default;
        IsSuccess = false;
        Exception = exception;
    }
    
    public static Result<TData> Success(TData value) => new(value);
    public static Result<TData> Failure(string exception) => new(exception);
}