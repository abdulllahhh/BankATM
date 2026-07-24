namespace BuildingBlocks.Application.Results;

public sealed class Result<T>
{
    private readonly T? _value = default;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    private Result(T value)
    {
        IsSuccess = true;
        Error = Error.None;
        _value = value;
    }

    private Result(Error error)
    {
        if (error == Error.None)
        {
            throw new ArgumentException("Failure result must have a non-none error.", nameof(error));
        }

        IsSuccess = false;
        Error = error;
        _value = default;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);
}
