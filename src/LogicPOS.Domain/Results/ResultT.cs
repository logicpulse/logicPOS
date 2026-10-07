using LogicPOS.Domain.Errors;

namespace LogicPOS.Domain.Results;

public class Result<TValue> : Result
{
    public TValue? Value { get; init; }

    protected internal Result(TValue value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        Value = value;
    }

    protected internal Result(bool isSuccess, Error error) : base(isSuccess, error) { }

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}