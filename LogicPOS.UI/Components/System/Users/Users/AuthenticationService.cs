using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Authentication;
using LogicPOS.Api.Features.Authentication.Login;
using LogicPOS.Api.Features.Users.GetUserPermissions;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.System.Users.Permissions;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using Serilog;

namespace LogicPOS.UI.Components.Users
{
    public static class AuthenticationService
    {
        private static readonly IMediator _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public static User User { get; private set; }
        public static List<string> Permissions { get; private set; } = new List<string>();

        public static void HardwareOpenDrawer()
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }
            if(!UserHasPermission(UserProfilePermissions.Hardware.HARDWARE_DRAWER_OPEN))
            {
                CustomAlerts.Information()
                             .WithMessage(LocalizedString.Instance["open_cash_draw_permissions"])
                            .ShowAlert();
                 return;
            }
            try
            {
                var printer = new ESC_POS_USB_NET.Printer.Printer(TerminalService.Terminal.ThermalPrinter.Designation);
                printer.OpenDrawer();
                printer.PrintDocument();
                printer.Clear();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error printing...");
                global::Gtk.Application.Invoke(delegate
                {
                    CustomAlerts.Error(CustomAlerts.ResolveParentWindow())
                                .WithMessage($"Erro ao abrir caixa registradora. \n\n{ex.Message}")
                                .ShowAlert();
                });
            }
        }
       
        public static bool UserHasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        public static void LoginUser(User user, string jwtToken)
        {
            User = user;
            AuthenticationData.Token = jwtToken;
            LoadPermissions();
        }

        public static ErrorOr<string> Authenticate(Guid userId, string password)
        {
            var loginResult = _mediator.Send(new LoginQuery(TerminalService.Terminal.Id, userId, password)).Result;
            return loginResult;
        }
        
        public static void RefreshPermissions()
        {
            LoadPermissions();
        }
       
        private static void LoadPermissions()
        {
            Permissions.Clear();

            var mediator = DependencyInjection.Mediator;
            var getPermissionsResult = mediator.Send(new GetUserPermissionsQuery(User.Id)).Result;

            if (getPermissionsResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getPermissionsResult, closeApplication: true);
                return;
            }

            Permissions.AddRange(getPermissionsResult.Value);
        }
    }
}
