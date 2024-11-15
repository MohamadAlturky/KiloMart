namespace KiloMart.Core.Models;

public class Result<T> : Result
{
    public T Data { get; set; }

    public Result(bool success, T data, string[] errors) : base(success, errors)
    {
        Success = success;
        Data = data;
    }

    public static Result<T> Ok(T data) => new Result<T>(true, data, Array.Empty<string>());
    public new static  Result<T> Fail(string[] errors) => new Result<T>(false, default!, errors);
}


public class Result
{
    public bool Success { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
    public Result(bool success, string[] errors)
    {
        Success = success;
        Errors = errors;
    }

    public static Result Ok() => new Result(true, Array.Empty<string>());
    public static Result Fail(string[] errors) => new Result(false, errors);
}

