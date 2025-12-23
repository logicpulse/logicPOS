using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.System.Licensing.ActivateLicense;
using LogicPOS.Api.Features.System.Licensing.AddMessage;
using LogicPOS.Api.Features.System.Licensing.ConnectToWs;
using LogicPOS.Api.Features.System.Licensing.GetCountries;
using LogicPOS.Api.Features.System.Licensing.GetCurrentVersion;
using LogicPOS.Api.Features.System.Licensing.GetHardwareId;
using LogicPOS.Api.Features.System.Licensing.GetLicenseInformation;
using LogicPOS.Api.Features.System.Licensing.GetVersion;
using LogicPOS.Api.Features.System.Licensing.IsLicensed;
using LogicPOS.Api.Features.System.Licensing.NeedToRegister;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

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

        public static string GetCurrentVersion()
        {
            var result = DependencyInjection.Mediator.Send(new GetCurrentVersionQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return string.Empty;
            }
            return result.Value.Version;
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

        public static string GetVersion()
        {
            var result = DependencyInjection.Mediator.Send(new GetVersionQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.Version;
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

        public static bool IsLicensed()
        {
            var result = DependencyInjection.Mediator.Send(new IsLicensedQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return result.Value.IsLicensed;
        }

        public static bool NeedToRegister()
        {
            var result = DependencyInjection.Mediator.Send(new NeedToRegisterQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return true;
            }
            return result.Value.NeedToRegister;
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


        public static LicenseData GetLicenseInformation()
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

        public static bool AddMessage(AddMessageCommand command)
        {
            var result = DependencyInjection.Mediator.Send(command).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return result.Value.Success;
        }

        public static void LoadLicenseInformation()
        {
            Data = GetLicenseInformation();
        }


        public static string GetTerminalHardwareID()
        {
            string result = string.Empty;
            try
            {
                result = TerminalService.GetTerminalHardwareId();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return result;
        }

        public static string GetMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddresses;
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHashSHA256(inputString))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static byte[] GetHashSHA256(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static bool Initialize()
        {
            try
            {
                LoadLicenseInformation();
                Data.TerminalHardwareId = GetTerminalHardwareID();
                Data.IsValid = IsLicensed();

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
