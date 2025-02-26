using ErrorOr;

namespace LogicPOS.Api.Errors
{
    public static class ApiErrors
    {
        public static readonly Error APICommunication = Error.Unexpected(code: nameof(APICommunication),
                                                                         description: "Erro ao se comunicar com a API.");

        public static readonly Error UnknownAPIResponse = Error.Unexpected(code: nameof(UnknownAPIResponse),
                                                                           description: "Não foi possível reconhecer a resposta da API.");
    }
}
