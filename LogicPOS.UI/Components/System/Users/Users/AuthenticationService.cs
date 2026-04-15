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
            if(!UserHasPermission(UserProfilePermissions.HARDWARE_DRAWER_OPEN))
            {
                CustomAlerts.Information()
                             .WithMessage(GeneralUtils.GetResourceByName("open_cash_draw_permissions"))
                            .ShowAlert();
                 return;
            }
            var printer = new ESC_POS_USB_NET.Printer.Printer(TerminalService.Terminal.ThermalPrinter.Designation);
            printer.OpenDrawer();
            printer.PrintDocument();
            printer.Clear();
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
