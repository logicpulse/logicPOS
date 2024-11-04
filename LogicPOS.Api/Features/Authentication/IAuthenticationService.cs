using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Authentication
{
    public interface IAuthenticationService
    {
        Task<string> AuthenticateAsync();
    }
}
