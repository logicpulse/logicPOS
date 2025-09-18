using LogicPOS.Api.Features.System.Licensing.ActivateLicense;
using LogicPOS.Api.Features.System.Licensing.AddMessage;
using LogicPOS.Api.Features.System.Licensing.ConnectToWs;
using LogicPOS.Api.Features.System.Licensing.GetCountries;
using LogicPOS.Api.Features.System.Licensing.GetCurrentVersion;
using LogicPOS.Api.Features.System.Licensing.GetHardwareId;
using LogicPOS.Api.Features.System.Licensing.GetLicense;
using LogicPOS.Api.Features.System.Licensing.GetLicenseFilename;
using LogicPOS.Api.Features.System.Licensing.GetLicenseInformation;
using LogicPOS.Api.Features.System.Licensing.GetVersion;
using LogicPOS.Api.Features.System.Licensing.IsLicensed;
using LogicPOS.Api.Features.System.Licensing.NeedToRegister;
using LogicPOS.Api.Features.System.Licensing.UpdateCurrentVersion;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Licensing
{
    public static class LicensingService
    {
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

        public static byte[] ActivateLicense(ActivateLicenseCommand licenseData)
        {
            var result = DependencyInjection.Mediator.Send(licenseData).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return Convert.FromBase64String(result.Value.LicenseData);
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
                return false;
            }
            return result.Value.NeedToRegister;
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
        
        public static string GetLicenseFilename()
        {
            var result = DependencyInjection.Mediator.Send(new GetLicenseFilenameQuery()).Result;

            if (result.IsError)
            {

                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value.Filename;
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
    }
}
