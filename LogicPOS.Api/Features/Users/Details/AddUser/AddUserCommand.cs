using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Users.AddUser
{
    public class AddUserCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid ProfileId { get; set; }
        public Guid? CommissionGroupId { get; set; }
        public string Name { get; set; } 
        public string Residence { get; set; }
        public string Locality { get; set; }
        public string ZipCpde { get; set; }
        public string City { get; set; }
        public string DateOfContract { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string FiscalNumber { get; set; }
        public string Language { get; set; }
        public string AssignedSeating { get; set; }
        public string BaseConsumption { get; set; }
        public string BaseOffers { get; set; }
        public string PVPOffers { get; set; }
        public string Notes { get; set; }
    }
}
