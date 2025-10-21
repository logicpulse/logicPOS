using Gtk;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WorkSessionsModal : Modal
    {
        private readonly ISender _meditaor = DependencyInjection.Services.GetRequiredService<IMediator>();
        public WorkSessionsPage Page { get; set; }

        public WorkSessionsModal(Window parent) : base(parent,
                                                    GeneralUtils.GetResourceByName("window_title_select_worksession_period_day"),
                                                    AppSettings.MaxWindowSize,
                                                    $"{AppSettings.Paths.Images}{@"Icons/Windows/icon_window_select_record.png"}",
                                                    render: false)
        {
            Render();
            BtnPrintDay.Sensitive = AuthenticationService.UserHasPermission("WORKSESSION_ALL");
        }


    }

}
