using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Reports.SystemAudits.GetSystemAuditsReportPdf;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Drawing;
using System.Globalization;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SystemAuditFilterModal : Modal
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;

        private SystemAuditFilterModal(Window parent) : base(parent,
                                                                       GeneralUtils.GetResourceByName("report_customer_balance_summary"),
                                                                       new Size(500, 509),
                                                                       AppSettings.Paths.Images + @"Icons\Windows\icon_window_date_picker.png")
        {
        }

        private void Initialize()
        {
            InitializeTxtTerminal();
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            AddEventsHandlers();
        }

        private void TxtTerminal_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtTerminal.Text))
            {
                TxtTerminal.Clear();
            }
        }
        private void AddEventsHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private GetSystemAuditsReportPdfQuery CreateQuery()
        {
            return new GetSystemAuditsReportPdfQuery(   DateTime.ParseExact(TxtStartDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                                        DateTime.ParseExact(TxtEndDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                                        TxtTerminal.SelectedEntity != null ? (TxtTerminal.SelectedEntity as Terminal).Id : Guid.Empty);
            
        }
        private void SelectCustomer(Terminal terminal)
        {
            TxtTerminal.SelectedEntity = terminal;
            TxtTerminal.Text = terminal.Designation;
        }
    }
}