using Gtk;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtOnlineDocumentInfoModal : Modal
    {
        public AgtOnlineDocumentInfoModal(string documentNumber, Window parent) : base(parent,
                                                     "Documento Online (AGT)",
                                                     new Size(550, 560),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            ShowData(documentNumber);
        }

        private void ShowData(string documentNumber)
        {

            var agtDocument = AgtService.GetOnlineDocument(documentNumber);
            TxtDate.Text = agtDocument?.Date;
            TxtDocumentType.Text = agtDocument?.Type;
            TxtDocumentNumber.Text = agtDocument?.Number;
            TxtStatus.Text = agtDocument?.Status;
            TxtCustomerNif.Text = agtDocument?.CustomerNif;
            TxtTaxPayable.Text = agtDocument?.TaxPayable;
            TxtNetTotal.Text = agtDocument?.NetTotal;
            TxtGrossTotal.Text = agtDocument?.GrossTotal;
        }

        public static void Show(string documentNumber, Window parent)
        {
            var modal = new AgtOnlineDocumentInfoModal(documentNumber, parent);
            modal.Run();
            modal.Destroy();
        }

    }
}
