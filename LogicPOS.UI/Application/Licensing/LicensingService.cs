using LogicPOS.Api.Features.System.Licensing.ActivateLicense;
using LogicPOS.Api.Features.System.Licensing.AddMessage;
using LogicPOS.Api.Features.System.Licensing.ConnectToWs;
using LogicPOS.Api.Features.System.Licensing.GetCountries;
using LogicPOS.Api.Features.System.Licensing.GetCurrentVersion;
using LogicPOS.Api.Features.System.Licensing.GetHardwareId;
using LogicPOS.Api.Features.System.Licensing.GetLicense;
using LogicPOS.Api.Features.System.Licensing.GetLicenseInformation;
using LogicPOS.Api.Features.System.Licensing.GetVersion;
using LogicPOS.Api.Features.System.Licensing.IsLicensed;
using LogicPOS.Api.Features.System.Licensing.NeedToRegister;
using LogicPOS.Api.Features.System.Licensing.UpdateCurrentVersion;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application.Licensing;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
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

        public static byte[] GetLicense(string hardwareId, string version)
        {
            var result = DependencyInjection.Mediator.Send(new GetLicenseQuery() { HardwareId = hardwareId, Version = version }).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.LicenceData;
        }

        public static int UpdateCurrentVersion(UpdateCurrentVersionCommand command)
        {
            var result = DependencyInjection.Mediator.Send(command).Result;

            if (result.IsError)
            {

                ErrorHandlingService.HandleApiError(result);
                return 0;
            }

            return result.Value.Result;
        }

        public static Dictionary<string, string> GetLicenseInformation()
        {
            var result = DependencyInjection.Mediator.Send(new GetLicenseInformationQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.LicenseInformation;
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
            if (Data.DtLicenceKeys == null)
            {
                Data.DtLicenceKeys = new DataTable("keysLicence");
                Data.DtLicenceKeys.Columns.Add("name", typeof(string));
                Data.DtLicenceKeys.Columns.Add("value", typeof(string));
            }
            Data.DtLicenceKeys.Rows.Clear();
            Data.LicenceDate = DateTime.Now.ToString("dd/MM/yyyy");
            Data.LicenceVersion = "LOGICPOS_LICENSED";
            Data.LicenceName = "Nome DEMO";
            Data.LicenceCompany = "Empresa DEMO";
            Data.LicenceNif = "NIF DEMO";
            Data.LicenceAddress = "Morada DEMO";
            Data.LicenceEmail = "Email DEMO";
            Data.LicenceTelephone = "Telefone DEMO";
            Data.LicenceReseller = "LogicPulse";
            Data.ServerVersion = "1.0";
            Data.LicenceCountry = 168;
            Data.LicenceUpdateDate = DateTime.Now.AddDays(-1);
#if DEBUG
            Data.LicenceVersion = "LOGICPOS_CORPORATE";
            Data.LicenceName = "DEBUG";
            Data.LicenceCompany = "DEBUG";
            Data.LicenceAddress = "DEBUG";
            Data.LicenceEmail = "DEBUG";
            Data.LicenceTelephone = "DEBUG";
            Data.LicenceModuleStocks = true;
            Data.LicenceReseller = "Logicpulse";
            Data.LicenceCountry = 168;
#endif

            var licenseInfo = GetLicenseInformation();
            Data.ServerVersion = GetCurrentVersion();
            Log.Debug("licence info count:" + licenseInfo.Count.ToString());
            foreach (var kvp in licenseInfo)
            {
                string key = kvp.Key;
                string value = kvp.Value;
                Log.Debug("Licence Key:" + key + "=" + value);
                Data.DtLicenceKeys.Rows.Add(key, value);
                switch (key)
                {
                    case "hardwareID":
                        Data.ApiHardwareId = value;
                        break;
                    case "version":
                        Data.LicenceVersion = value;
                        break;
                    case "data":
                        Data.LicenceDate = value;
                        break;
                    case "name":
                        Data.LicenceName = value;
                        break;
                    case "company":
                        Data.LicenceCompany = value;
                        break;
                    case "nif":
                        Data.LicenceNif = value;
                        break;
                    case "adress":
                        Data.LicenceAddress = value;
                        break;
                    case "email":
                        Data.LicenceEmail = value;
                        break;
                    case "telefone":
                        Data.LicenceTelephone = value;
                        break;
                    case "reseller":
                        Data.LicenceReseller = value;
                        break;
                    case "logicpos_Module_Stocks":
                        Data.LicenceModuleStocks = Convert.ToBoolean(value);
                        break;

                    case "logicpos_Module_Fe":
                        Data.ModuleAgtFe = Convert.ToBoolean(value);
                        break;
                    case "all_UpdateExpirationDate":
                        Data.LicenceUpdateDate = Convert.ToDateTime(value);
                        break;
                    case "all_NumberDevices":
                        Data.NumberDevices = Convert.ToInt16(value);
                        break;
                    default:
                        break;
                }
            }
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
                Data.LicenceRegistered = IsLicensed();

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
