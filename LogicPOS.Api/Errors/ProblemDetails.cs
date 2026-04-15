using System.Collections.Generic;

namespace LogicPOS.Api.Errors
{
    public struct ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
        public List<ProblemDetailsError> Errors { get; set; }

        public static ProblemDetails Unauthorized(string resource)
        {
            return new ProblemDetails
            {
                Type = "Autenticação",
                Title = "Não autenticado",
                Instance = resource,
                Status = 401,
                Detail = "Precisa estar autenticado para acessar este recurso.",
                Errors = new List<ProblemDetailsError>
                {
                    new ProblemDetailsError
                    {
                        Name = "Unauthorized",
                        Reason= "Usuário não autenticado."
                    }
                }
            };
        }

        public static ProblemDetails Forbidden(string resource)
        {
            return new ProblemDetails
            {
                Type = "Autorização",
                Title = "Acesso negado",
                Instance = resource,
                Status = 403,
                Detail = "Você não tem permissão para acessar este recurso.",
                Errors = new List<ProblemDetailsError>
                {
                    new ProblemDetailsError
                    {
                        Name = "Forbidden",
                        Reason= "Usuário sem permissão para acessar o recurso."
                    }
                }
            };
        }
    }
}
