using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.SizeUnits.GetAllSizeUnits;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.SizeUnits
{
    public static class SizeUnitsService
    {
        private static List<SizeUnit> _sizeUnits;

        public static SizeUnit DefaultSizeUnit => SizeUnits.FirstOrDefault(su => su.Code=="10");
        public static List<SizeUnit> SizeUnits
        {
            get
            {
                if (_sizeUnits == null)
                {
                    _sizeUnits = GetAllSizeUnits();
                }
                return _sizeUnits;
            }
        }
        public static void RefreshSizeUnitsCache()
        {
            _sizeUnits = GetAllSizeUnits();
        }

        private static List<SizeUnit> GetAllSizeUnits()
        {
            var query = new GetAllSizeUnitsQuery();
            var sizeUnits = DependencyInjection.Mediator.Send(query).Result;

            if (sizeUnits.IsError != false)
            {
                ErrorHandlingService.HandleApiError(sizeUnits);
                return new List<SizeUnit>();
            }

            return sizeUnits.Value.ToList();
        }
    }
}
