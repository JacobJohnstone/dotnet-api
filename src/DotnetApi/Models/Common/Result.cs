namespace DotnetApi.Models.Common;

public class Result<TData>
{
    public TData Data { get; }
    public bool IsSuccess { get; }
    public Exception Exception { get; }

    // successful response constructor format
    protected Result(TData data)
    {
        Data = data;
        IsSuccess = true;
        Exception = null;
    }
    
    // unsuccessful response constructor format
    protected Result(Exception exception)
    {
        Data = default;
        IsSuccess = false;
        Exception = exception;
    }
    
    public static Result<TData> Success(TData value) => new(value);
    public static Result<TData> Failure(Exception exception) => new(exception);
}