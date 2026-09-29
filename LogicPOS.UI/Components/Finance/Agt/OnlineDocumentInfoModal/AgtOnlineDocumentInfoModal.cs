using Gtk;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtOnlineDocumentInfoModal : Modal
    {
        public AgtOnlineDocumentInfoModal(string number, Window parent) : base(parent,
                                                     "Documento Online (AGT)",
                                                     new Size(550, 560),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            ShowData(number);
        }

        private void ShowData(string number)
        {

            var agtDocument = AgtService.GetOnlineDocument(number);
            TxtDate.Text = agtDocument?.Date;
            TxtDocumentType.Text = agtDocument?.Type;
            TxtDocumentNumber.Text = agtDocument?.Number;
            TxtStatus.Text = agtDocument?.Status;
            TxtCustomerNif.Text = agtDocument?.CustomerNif;
            TxtTaxPayable.Text = agtDocument?.TaxPayable;
            TxtNetTotal.Text = agtDocument?.NetTotal;
            TxtGrossTotal.Text = agtDocument?.GrossTotal;
        }

        public static void Show(string number, Window parent)
        {
            var modal = new AgtOnlineDocumentInfoModal(number, parent);
            modal.Run();
            modal.Destroy();
        }

    }
}
