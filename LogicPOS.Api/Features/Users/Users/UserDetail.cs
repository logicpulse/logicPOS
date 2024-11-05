using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class UserDetail : ApiEntity, IWithCode
    {
        public UserProfile Profile { get; set; }
        public Guid ProfileId { get; set; }

        public CommissionGroup CommissionGroup { get; set; }
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
        public string AccessPin { get; set; }
        public string AccessCardNumber { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool PasswordReset { get; set; }
        public DateTime? PasswordResetDate { get; set; }
        public string BaseConsumption { get; set; }
        public string BaseOffers { get; set; }
        public string PVPOffers { get; set; }
        public string Remarks { get; set; }
        public string ButtonImage { get; set; }

        public string Code { get; set; }
        public uint Order { get; set; }
    }
}
