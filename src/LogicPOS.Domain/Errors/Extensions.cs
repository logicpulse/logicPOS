namespace LogicPOS.Domain.Errors;

public static class Extensions
{
   public static bool IsEntityNotFoundError(this Error error) =>
       error.Code == Error.EntityNotFoundError.Code;
}