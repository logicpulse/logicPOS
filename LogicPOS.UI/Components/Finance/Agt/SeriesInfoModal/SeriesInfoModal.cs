using Gtk;
using LogicPOS.Api.Features.Finance.Agt.ListSeries;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class SeriesInfoModal : Modal
    {

        public SeriesInfoModal(AgtSeriesInfo seriesInfo, Window parent) : base(parent,
                                                     "Série Documental (AGT)",
                                                     new Size(550, 620),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            ShowData(seriesInfo);
        }

        private void ShowData(AgtSeriesInfo seriesInfo)
        {
            TxtCode.Text = seriesInfo?.Code ?? "";
            TxtYear.Text = seriesInfo?.Year ?? "";
            TxtDocumentType.Text = seriesInfo?.DocumentType ?? "";
            TxtStatus.Text = seriesInfo?.Status ?? "";
            TxtCreationDate.Text = seriesInfo?.SeriesCreationDate ?? "";
            TxtFirstDocument.Text = seriesInfo?.FirstDocumentCreated ?? "";
            TxtLastDocument.Text = seriesInfo?.LastDocumentCreated ?? "";
            TxtInvoicingMethod.Text = seriesInfo?.InvoicingMethod ?? "";
            TxtNif.Text = seriesInfo?.Nif;
            TxtName.Text = seriesInfo?.Name ?? "";
            TxtJoiningDate.Text = seriesInfo?.JoiningDate ?? "";
            TxtJoiningType.Text = seriesInfo?.JoiningType ?? "";
        }

        public static void Show(AgtSeriesInfo seriesInfo, Window parent)
        {
            var modal = new SeriesInfoModal(seriesInfo, parent);
            modal.Run();
            modal.Destroy();
        }

    }
}
