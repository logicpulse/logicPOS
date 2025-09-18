using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.AddMessage
{
    public class AddMessageCommand : IRequest<ErrorOr<AddMessageResponse>>
    {
        public string HardwareId { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Message { get; set; }
        public string Client { get; set; }
    }
}
