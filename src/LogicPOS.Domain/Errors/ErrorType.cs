namespace LogicPOS.Domain.Errors;

public enum ErrorType
{
    Validation,
    Conflict,
    NotFound,
    Unauthorized,
    InvalidOperation,
    None,
    Unexpected
}