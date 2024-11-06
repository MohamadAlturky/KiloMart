namespace KiloMart.DataAccess.Models;

public class Result<T> : Result
{
    public T Data { get; set; }

    public Result(bool success, T data) : base(success)
    {
        Success = success;
        Data = data;
    }

    public static Result<T> Ok(T data) => new Result<T>(true, data);
    public static Result<T> Fail() => new Result<T>(false, default!);
}


public class Result
{
    public bool Success { get; set; }

    public Result(bool success)
    {
        Success = success;
    }

    public static Result Ok() => new Result(true);
    public static Result Fail() => new Result(false);
}

