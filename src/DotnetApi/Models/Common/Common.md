### Common

---

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Common contains all the shared models found in 
the budget app api. Things that are more for standardizations and shared functionalities
are found here. An example is the "Result" class, which creates a standard response
shape for our api, allowing me to handle both successful and unsuccessful operations
cleanly. It offers a success flag, and contains fields for storing both the data (if 
successful) and the exception (if unsuccessful):

```
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
```