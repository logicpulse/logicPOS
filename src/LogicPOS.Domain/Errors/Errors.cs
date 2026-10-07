namespace LogicPOS.Domain.Errors;

public partial record Error
{
    #region Errors

    public static readonly Error None = new(
          Code: "None",
          Message: string.Empty,
          Type: ErrorType.None);

    public static Error Conflict(
        string message)
    {
        return new Error(
            Code: "Conflict",
            Message: message,
            Type: ErrorType.Conflict,
            Title: "Conflito"
        );
    }

    public static Error NotFound(
        string message)
    {
        return new Error(
            Code:  "NotFound",
            Message: message,
            Type: ErrorType.NotFound,
            Title: "Não encontrado"
        );
    }

    public static Error Unauthorized(
        string message)
    {
        return new Error(
            Code: "Unauthorized",
            Message: message,
            Type: ErrorType.Unauthorized,
            Title: "Não autorizado"
        );
    }

    public static Error Validation(
        string message)
    {
        return new Error(
            Code: "Validation",
            Message: message,
            Type: ErrorType.Validation,
            Title: "Validação"
        );
    }

    public static Error InvalidOperation(
        string message)
    {
        return new Error(
            Code:"InvalidOperation",
            Message: message,
            Type: ErrorType.InvalidOperation,
            Title: "Operação Inválida"
        );
    }

    public static Error Unexpected(
        string message)
    {
        return new Error(
            Code: "Unexpected",
            Message: message,
            Type: ErrorType.Unexpected,
            Title: "Erro inesperado"
        );
    }

    public static readonly Error DesignationExistsError =
        Conflict("Designação já existe");
    

    public static readonly Error EntityNotFoundError =
        NotFound("Entidade não encontrada");

    public static readonly Error EntityInUseError =
        Conflict("Entidade em uso");

    #endregion
    
}

