using ErrorOr;

namespace LogicPOS.Api.Errors
{
    public static class ApiErrors
    {
        public static readonly Error APICommunication = Error.Unexpected(code: nameof(APICommunication),
                                                                         description: "Erro ao comunicar-se com a API. Tente novamente.");

        public static readonly Error UnknownAPIResponse = Error.Unexpected(code: nameof(UnknownAPIResponse),
                                                                           description: "Não foi possível reconhecer a resposta da API. Contacte o suporte técnico");
    }
}
