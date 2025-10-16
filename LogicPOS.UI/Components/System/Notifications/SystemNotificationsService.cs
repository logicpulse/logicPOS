using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.System.Notifications.GetAllSystemNotifications;
using LogicPOS.Api.Features.System.Notifications.UpdateNotification;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Services
{
    public class SystemNotificationsService
    {
        public static Dictionary<string, bool> Notifications { get; set; } = new Dictionary<string, bool>
        {
            ["SHOW_PRINTER_UNDEFINED"] = true
        };

        private static List<SystemNotificationViewModel> _systemNotifications;

        public static List<SystemNotificationViewModel> SystemNotifications
        {
            get
            {
                if (_systemNotifications == null)
                {
                    _systemNotifications = GetAllNotifications().ToList();
                }
                return _systemNotifications;
            }
        }

        public static void ShowNotifications()
        {
            if (SystemNotifications.Count() > 0)
            {
                foreach (var notification in SystemNotifications.OrderBy(x=>x.CreatedAt))
                {
                     var message = string.Format("{1}{0}{0}{2}", Environment.NewLine, notification.CreatedAt, notification.Message);
                    CustomAlerts.Information()
                                .WithSize(new Size(700, 480))
                                .WithTitleResource("window_title_dialog_notification")
                                .WithButtonsType(ButtonsType.Ok)
                                .WithMessage(message)
                                .ShowAlert();

                    var query = new UpdateNotificationCommand()
                    {
                        ReadingDate = DateTime.Now,
                        IsRead = true,
                        LastReadTerminalId = TerminalService.Terminal.Id,
                        LastReadUserId = AuthenticationService.User.Id
                    };
                    
                    var updateResult= DependencyInjection.Mediator.Send(query).Result;

                    if (updateResult.IsError != false)
                    {
                        ErrorHandlingService.HandleApiError(updateResult);
                        return;
                    }

                }
            }
            else
            {
                var message = string.Format(GeneralUtils.GetResourceByName("dialog_message_no_notification"), 5);
                CustomAlerts.Information()
                            .WithSize(new Size(700, 480))
                            .WithTitleResource("window_title_dialog_notification")
                            .WithButtonsType(ButtonsType.Ok)
                            .WithMessage(message)
                            .ShowAlert();
            }
        }

        private static IEnumerable<SystemNotificationViewModel> GetAllNotifications()
        {
            var query = new GetSystemNotificationsQuery();

            var notifications = DependencyInjection.Mediator.Send(query).Result;

            if (notifications.IsError != false)
            {
                ErrorHandlingService.HandleApiError(notifications);
                return new List<SystemNotificationViewModel>();
            }

            return notifications.Value;
        }
    }
}
