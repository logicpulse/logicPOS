using LogicPOS.Domain.Errors;

namespace LogicPOS.Domain.Results;

public partial class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (isSuccess == false && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public static implicit operator bool(Result result) => result.IsSuccess;

    public Result<TValue> ToGenericFailure<TValue>()
    {
        if(IsSuccess) throw new InvalidOperationException("Cannot convert successful result to failure result.");
        
        return Failure<TValue>(Error);
    } 
    
}