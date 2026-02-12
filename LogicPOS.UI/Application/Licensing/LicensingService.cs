using LogicPOS.Api.Features.System.Licensing.ActivateLicense;
using LogicPOS.Api.Features.System.Licensing.ConnectToWs;
using LogicPOS.Api.Features.System.Licensing.GetCountries;
using LogicPOS.Api.Features.System.Licensing.GetHardwareId;
using LogicPOS.Api.Features.System.Licensing.GetLicenseData;
using LogicPOS.Api.Features.System.Licensing.RefreshLicense;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Errors;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogicPOS.UI.Components.Licensing
{
    public static class LicensingService
    {
        public static string OFFLINE_ACTIVATION_FILE => "OfflineActivation.json";

        public static LicenseData Data { get; private set; } = new LicenseData();

        public static List<string> GetCountries()
        {
            var result = DependencyInjection.Mediator.Send(new GetLicensingCountriesQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return Enumerable.Empty<string>().ToList();
            }

            return result.Value.ToList();
        }

        public static string GetHardwareId()
        {
            var result = DependencyInjection.Mediator.Send(new GetHardwareIdQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.HardwareId;
        }

        public static ActivateLicenseResponse? ActivateLicense(ActivateLicenseCommand licenseData)
        {
            var result = DependencyInjection.Mediator.Send(licenseData).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }

        private static bool RefreshLicense()
        {
            var result = DependencyInjection.Mediator.Send(new RefreshLicenseCommand()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return true;
        }

        public static bool ActivateFromFile()
        {
            if (!File.Exists(OFFLINE_ACTIVATION_FILE))
            {
                return false;
            }
            var jsonText = File.ReadAllText(OFFLINE_ACTIVATION_FILE);
            var activationCommand = JsonConvert.DeserializeObject<ActivateLicenseCommand>(jsonText);

            var result = DependencyInjection.Mediator.Send(activationCommand).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }
            File.Delete(OFFLINE_ACTIVATION_FILE);
            return true;
        }

        private static LicenseData GetLicenseData()
        {
            var result = DependencyInjection.Mediator.Send(new GetLicenseDataQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.Data;
        }

        public static bool ConnectToWs()
        {
            var result = DependencyInjection.Mediator.Send(new ConnectToWsQuery()).Result;

            if (result.IsError)
            {

                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return result.Value.Connected;
        }

        private static void LoadLicenseData()
        {
            Data = GetLicenseData();
        }

        public static bool Initialize()
        {
            try
            {
                RefreshLicense();
                LoadLicenseData();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error initializing licensing service: " + ex.Message, ex);

                CustomAlerts.Error()
                        .WithTitle("Erro de Licença")
                        .WithMessage("Não foi possível inicializar a licença. Contacte o suporte técnico.")
                        .ShowAlert();

                return false;
            }
        }
    }
}
