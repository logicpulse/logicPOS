using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Extensions;
using System.Data.SqlTypes;
using System.Drawing;
using System.Xml.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class EditCompanyDetailsModal
    {
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            AddEventHandlers();

            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnDemo, ResponseType.None),
                new ActionAreaButton(BtnOk, ResponseType.Ok),
            };
        }

        protected override Widget CreateBody()
        {
            InitializeTextBoxes();

            var verticalLayout = new VBox(false, 2);

            InitializeTextBoxes();
            verticalLayout.PackStart(TxtCountry.Component, false, false, 0);
            verticalLayout.PackStart(TxtCurrency.Component, false, false, 0);
            verticalLayout.PackStart(TxtCompany.Component, false, false, 0);
            verticalLayout.PackStart(TxtBusiness.Component, false, false, 0);
            verticalLayout.PackStart(TxtAddress.Component, false, false, 0);
            verticalLayout.PackStart(TxtCity.Component, false, false, 0);
            verticalLayout.PackStart(TxtZipCode.Component, false, false, 0);
            verticalLayout.PackStart(TxtFiscalNumber.Component, false, false, 0);
            verticalLayout.PackStart(TxtStockCapital.Component, false, false, 0);
            verticalLayout.PackStart(TxtTaxEntity.Component, false, false, 0);
            verticalLayout.PackStart(TxtPhone.Component, false, false, 0);
            verticalLayout.PackStart(TxtMobile.Component, false, false, 0);
            verticalLayout.PackStart(TxtFax.Component, false, false, 0);
            verticalLayout.PackStart(TxtEmail.Component, false, false, 0);
            verticalLayout.PackStart(TxtWebsite.Component, false, false, 0);
            
            var sw = CreateScrolledWindow();
            sw.AddWithViewport(verticalLayout);

            return sw;
        }

        private ScrolledWindow CreateScrolledWindow()
        {
            var swindow = new ScrolledWindow();
            swindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            swindow.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
            swindow.ShadowType = ShadowType.None;
            return swindow;
        }
    }
}
