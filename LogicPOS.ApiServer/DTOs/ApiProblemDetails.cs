using ErrorOr;

namespace LogicPOS.ApiServer.DTOs;

public sealed class ApiProblemDetails
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? TraceId { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public List<ApiProblemDetailsError> Errors { get; set; } = new();

    public static ApiProblemDetails FromErrors(IReadOnlyList<Error> errors, HttpContext httpContext)
    {
        var firstError = errors[0];
        var statusCode = GetStatusCode(firstError.Type);

        return new ApiProblemDetails
        {
            Type = firstError.Type.ToString(),
            Title = GetTitle(statusCode),
            Status = statusCode,
            TraceId = httpContext.TraceIdentifier,
            Detail = string.Join("; ", errors.Select(error => error.Description)),
            Instance = httpContext.Request.Path,
            Errors = errors.Select(error => new ApiProblemDetailsError
            {
                Name = error.Code,
                Reason = error.Description
            }).ToList()
        };
    }

    private static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Pedido inválido",
            StatusCodes.Status401Unauthorized => "Não autenticado",
            StatusCodes.Status403Forbidden => "Acesso negado",
            StatusCodes.Status404NotFound => "Recurso não encontrado",
            StatusCodes.Status409Conflict => "Conflito ao processar pedido",
            _ => "Erro inesperado"
        };
    }
}

public sealed class ApiProblemDetailsError
{
    public string Name { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
