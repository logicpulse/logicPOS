using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.MeasurementUnits.GetAllMeasurementUnits;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.MeasurementUnits
{
    public static class MeasurementUnitsService
    {
        private static List<MeasurementUnit> _measurementUnits;
        public static MeasurementUnit DefaultMeasurementUnit => MeasurementUnits.FirstOrDefault(mu => mu.Code=="10");
        public static List<MeasurementUnit> MeasurementUnits
        {
            get
            {
                if (_measurementUnits == null)
                {
                   _measurementUnits = GetAllMeasurementUnits();
                }
                return _measurementUnits;
            }
        }
        public static void RefreshMeasurementUnitsCache()
        {
            _measurementUnits = GetAllMeasurementUnits();
        }

        private static List<MeasurementUnit> GetAllMeasurementUnits()
        {
            var query = new GetAllMeasurementUnitsQuery();
            var measurementUnits = DependencyInjection.Mediator.Send(query).Result;

            if (measurementUnits.IsError != false)
            {
                ErrorHandlingService.HandleApiError(measurementUnits);
                return new List<MeasurementUnit>();
            }

            return measurementUnits.Value.ToList();
        }
    }
}
