using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Users.UpdateUser
{
    public class UpdateUserCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint? NewOrder { get; set; }
        public string NewCode { get; set; }
        public Guid? NewProfileId { get; set; }
        public Guid? NewCommissionGroupId { get; set; }
        public string NewName { get; set; }
        public string NewResidence { get; set; }
        public string NewLocality { get; set; }
        public string NewZipCpde { get; set; }
        public string NewCity { get; set; }
        public string NewDateOfContract { get; set; }
        public string NewPhone { get; set; }
        public string NewMobilePhone { get; set; }
        public string NewEmail { get; set; }
        public string NewFiscalNumber { get; set; }
        public string NewLanguage { get; set; }
        public string NewAssignedSeating { get; set; }
        public string NewBaseConsumption { get; set; }
        public string NewBaseOffers { get; set; }
        public string NewPVPOffers { get; set; }
        public string NewNotes { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
