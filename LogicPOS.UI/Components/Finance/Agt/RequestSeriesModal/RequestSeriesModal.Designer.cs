using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt.RequestSeriesModal
{
    public partial class RequestSeriesModal 
    {
        public override Size ModalSize => new Size(500, 330);

        public override string ModalTitleResourceName => "Solicitar Série";

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_comboFiscalYears.ComboBox);
            SensitiveFields.Add(_comboDocumentTypes.ComboBox);
            SensitiveFields.Add(_txtEstablishmentNumber.Entry);
            SensitiveFields.Add(_checkContingencyIndicator);
        }

        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(_comboFiscalYears);
            ValidatableFields.Add(_comboDocumentTypes);
            ValidatableFields.Add(_txtEstablishmentNumber);
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
        }

        private VBox CreateDetailsTab()
        {
            var tab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            tab1.PackStart(_comboFiscalYears.Component, false, false, 0);
            tab1.PackStart(_comboDocumentTypes.Component, false, false, 0);
            tab1.PackStart(_txtEstablishmentNumber.Component, false, false, 0);
            tab1.PackStart(_checkContingencyIndicator, false, false, 0);
            
            return tab1;
        }

    }
}
