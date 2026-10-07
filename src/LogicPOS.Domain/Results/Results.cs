using LogicPOS.Domain.Errors;

namespace LogicPOS.Domain.Results;

public partial class Result
{
    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    private static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(false, error);

    public static Result Conflict(string message) => Failure(Error.Conflict(message));
    public static Result<TValue> Conflict<TValue>(string message) => Failure<TValue>(Error.Conflict(message));
    public static Result Unexpected(string message) => Failure(Error.Unexpected(message));
    public static Result<TValue> Unexpected<TValue>(string message) => Failure<TValue>(Error.Unexpected(message));

    public static Result NotFound(string entityName, string? entityId = null)
        => Failure(
            Error.NotFound($"Entidade não encontrada: {entityName}{(entityId != null ? $"({entityId})" : null)}"));

    public static Result<TValue> NotFound<TValue>(string entityName, string? entityId = null)
        => Failure<TValue>(
            Error.NotFound($"Entidade não encontrada: {entityName}{(entityId != null ? $"({entityId})" : null)}"));

    public static Result InvalidOperation(string message) => Failure(Error.InvalidOperation(message));

    public static Result<TValue> InvalidOperation<TValue>(string message) =>
        Failure<TValue>(Error.InvalidOperation(message));
    
    public static Result Unauthorized(string message) => Failure(Error.Unauthorized(message));
    public static Result<TValue> Unauthorized<TValue>(string message) => Failure<TValue>(Error.Unauthorized(message));

    public static readonly Result EntityInUse = new(false, Error.EntityInUseError);
}