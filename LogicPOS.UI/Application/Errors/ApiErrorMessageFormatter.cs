using ErrorOr;
using LogicPOS.Api.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPOS.UI.Errors
{
    /// <summary>
    /// Builds short, non-technical messages for GTK alerts. Full ProblemDetails stay in logs only.
    /// </summary>
    public static class ApiErrorMessageFormatter
    {
        private static readonly string[] TechnicalPhrases =
        {
            "bad request",
            "badrequest",
            "internal server error",
            "internalservererror",
            "one or more validation errors",
            "traceid",
            "stack trace",
            "exception",
            "problem+json",
            "application/problem",
            "http://",
            "https://",
            "status code",
            "rfc 7807",
            "unprocessable entity",
            "unsupported media type"
        };

        public static string ToUserMessage(Error error)
        {
            if (error.Metadata != null &&
                error.Metadata.ContainsKey("problem") &&
                error.Metadata["problem"] is ProblemDetails problemDetails)
            {
                var fromErrors = FormatProblemErrors(problemDetails.Errors);
                if (string.IsNullOrWhiteSpace(fromErrors) == false)
                {
                    return fromErrors;
                }

                if (IsUserFacingText(problemDetails.Detail))
                {
                    return problemDetails.Detail.Trim();
                }

                if (IsUserFacingText(problemDetails.Title))
                {
                    return problemDetails.Title.Trim();
                }

                return MessageForStatus(problemDetails.Status);
            }

            if (IsUserFacingText(error.Description))
            {
                return error.Description.Trim();
            }

            return MessageForErrorCode(error.Code, error.Type);
        }

        public static string ToUserMessage(IReadOnlyList<Error> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return "Ocorreu um erro. Tente novamente.";
            }

            if (errors.Count == 1)
            {
                return ToUserMessage(errors[0]);
            }

            var lines = errors
                .Select(ToUserMessage)
                .Where(m => string.IsNullOrWhiteSpace(m) == false)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (lines.Count == 0)
            {
                return "Ocorreu um erro. Tente novamente.";
            }

            if (lines.Count == 1)
            {
                return lines[0];
            }

            var sb = new StringBuilder();
            for (var i = 0; i < lines.Count; i++)
            {
                sb.Append(i + 1).Append(". ").AppendLine(lines[i]);
            }

            return sb.ToString().TrimEnd();
        }

        private static string FormatProblemErrors(List<ProblemDetailsError> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return null;
            }

            var reasons = errors
                .Select(e => string.IsNullOrWhiteSpace(e.Reason) ? null : e.Reason.Trim())
                .Where(IsUserFacingText)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (reasons.Count == 0)
            {
                return null;
            }

            return string.Join(Environment.NewLine, reasons);
        }

        private static bool IsUserFacingText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var normalized = text.Trim();
            if (normalized.Length < 3)
            {
                return false;
            }

            var lower = normalized.ToLowerInvariant();
            if (TechnicalPhrases.Any(p => lower.Contains(p)))
            {
                return false;
            }

            // Pure HTTP status names / codes are not useful to end users.
            if (IsHttpStatusLabel(normalized))
            {
                return false;
            }

            return true;
        }

        private static bool IsHttpStatusLabel(string text)
        {
            switch (text.Trim().ToLowerInvariant().Replace(" ", string.Empty))
            {
                case "badrequest":
                case "unauthorized":
                case "forbidden":
                case "notfound":
                case "conflict":
                case "unprocessableentity":
                case "internalservererror":
                case "serviceunavailable":
                case "400":
                case "401":
                case "403":
                case "404":
                case "409":
                case "422":
                case "500":
                case "503":
                    return true;
                default:
                    return false;
            }
        }

        private static string MessageForStatus(int status)
        {
            switch (status)
            {
                case 400:
                case 422:
                    return "Não foi possível concluir o pedido. Verifique os dados e tente novamente.";
                case 401:
                    return "Não foi possível autenticar. Verifique as credenciais e tente novamente.";
                case 403:
                    return "Não tem permissão para realizar esta operação.";
                case 404:
                    return "O registo pedido não foi encontrado.";
                case 409:
                    return "Este registo está em conflito com dados já existentes.";
                case 500:
                case 502:
                case 503:
                case 504:
                    return "Ocorreu um erro no servidor. Tente novamente mais tarde.";
                default:
                    return "Ocorreu um erro. Tente novamente.";
            }
        }

        private static string MessageForErrorCode(string code, ErrorType type)
        {
            if (IsHttpStatusLabel(code ?? string.Empty))
            {
                int status;
                if (int.TryParse(code, out status))
                {
                    return MessageForStatus(status);
                }

                switch ((code ?? string.Empty).Trim().ToLowerInvariant().Replace(" ", string.Empty))
                {
                    case "badrequest":
                        return MessageForStatus(400);
                    case "unauthorized":
                        return MessageForStatus(401);
                    case "forbidden":
                        return MessageForStatus(403);
                    case "notfound":
                        return MessageForStatus(404);
                    case "conflict":
                        return MessageForStatus(409);
                    case "unprocessableentity":
                        return MessageForStatus(422);
                    case "internalservererror":
                        return MessageForStatus(500);
                }
            }

            switch (type)
            {
                case ErrorType.Unauthorized:
                    return MessageForStatus(401);
                case ErrorType.Forbidden:
                    return MessageForStatus(403);
                case ErrorType.NotFound:
                    return MessageForStatus(404);
                case ErrorType.Conflict:
                    return MessageForStatus(409);
                case ErrorType.Validation:
                    return MessageForStatus(400);
                default:
                    return "Ocorreu um erro. Tente novamente.";
            }
        }
    }
}
