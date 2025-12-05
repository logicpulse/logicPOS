using Gtk;
using LogicPOS.Api.Features.Finance.Agt.ListOnlineSeries;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AgtSeriesFilterModal : Modal
    {
        public AgtSeriesFilterModal(Window parent) :
            base(parent,
                "Filtrar Séries Online - AGT",
                new Size(540, 468),
                AppSettings.Paths.Images + @"Icons\Windows\icon_window_date_picker.png")
        {
            WindowSettings.Close.Hide();

        }
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            AddEventHandlers();

            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnClear, ResponseType.None),
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }
        public bool AllFieldsAreValid() => GetValidatableFields().All(field => field.IsValid());

        private IEnumerable<IValidatableField> GetValidatableFields()
        {
            return new List<IValidatableField>
            {
                TxtDocumentType,
                TxtStatus,
                TxtCode,
                TxtYear,
                TxtEstablishmentNumber
            };
        }
        private void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableFields(), this);

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnClear.Clicked += BtnClear_Clicked;
        }
        public ListOnlineSeriesQuery GetOnlineSeriesQuery()
        {
            if(AllFieldsAreValid() == false)
            {
                return null;
            }

            var query = new ListOnlineSeriesQuery( );
            query.DocumentType = string.IsNullOrWhiteSpace(TxtDocumentType.Text)? null:TxtDocumentType.Text;
            query.Status = string.IsNullOrWhiteSpace(TxtStatus.Text)?null:TxtStatus.Text;
            query.Code = string.IsNullOrWhiteSpace(TxtCode.Text) ? null : TxtCode.Text;
            query.Year= string.IsNullOrWhiteSpace(TxtYear.Text) ? null : TxtYear.Text;
            query.EstablishmentNumber = string.IsNullOrWhiteSpace(TxtEstablishmentNumber.Text) ? null : TxtEstablishmentNumber.Text;
            return query;
        }
        protected override void OnResponse(ResponseType response)
        {
            if (response == ResponseType.None)
            {
                Run();
                return;
            }
        }
        protected override Widget CreateBody()
        {
            var verticalLayout = new VBox(false, 2);
            InitializeTextBoxes();
            verticalLayout.PackStart(TxtDocumentType.Component, false, false, 0);
            verticalLayout.PackStart(TxtYear.Component, false, false, 0);
            verticalLayout.PackStart(TxtCode.Component, false, false, 0);
            verticalLayout.PackStart(TxtEstablishmentNumber.Component, false, false, 0);
            verticalLayout.PackStart(TxtStatus.Component, false, false, 0);

            return verticalLayout;
        }
    }
}
