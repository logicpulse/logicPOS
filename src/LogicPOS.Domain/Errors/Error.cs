namespace LogicPOS.Domain.Errors;

public partial record Error(
    string Code,
    string Message,
    ErrorType Type,
    string Title = "Erro"
    );

