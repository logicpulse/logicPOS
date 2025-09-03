using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.VatExemptionReasons.GetAllVatExemptionReasons;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.VatExemptionReasons
{
    public static class VatExemptionReasonsService
    {
        private static List<VatExemptionReason> _reasons;

        public static List<VatExemptionReason> Reasons
        {
            get
            {
                if (_reasons == null)
                {
                    _reasons = GetAllReasons();
                }
                return _reasons;
            }
        }

        private static List<VatExemptionReason> GetAllReasons()
        {
            var getReasons = DependencyInjection.Mediator.Send(new GetAllVatExemptionReasonsQuery()).Result;

            if (getReasons.IsError)
            {
                ErrorHandlingService.HandleApiError(getReasons);
                return null;
            }

            return getReasons.Value.ToList();
        }
    }
}
