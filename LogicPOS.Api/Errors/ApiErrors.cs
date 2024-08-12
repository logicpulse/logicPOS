using ErrorOr;

namespace LogicPOS.Api.Errors
{
    public static class ApiErrors
    {
        public static readonly Error CommunicationError = Error.Unexpected(
            code: nameof(CommunicationError),
            description: "Erro ao se comunicar com a API.");
    }
}
