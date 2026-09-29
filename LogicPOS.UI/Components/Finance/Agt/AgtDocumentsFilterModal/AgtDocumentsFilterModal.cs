using Gtk;
using LogicPOS.Api.Features.Finance.Agt.ListOnlineDocuments;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AgtDocumentsFilterModal : Modal
    {
        public AgtDocumentsFilterModal(Window parent) :
            base(parent,
                LocalizedString.Instance["window_title_dialog_filter"],
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
                TxtStartDate,
                TxtEndDate
            };
        }
        private void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableFields(), this);

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnClear.Clicked += BtnClear_Clicked;
        }
        public ListOnlineDocumentsQuery GetOnlineDocumentsQuery()
        {
            if(AllFieldsAreValid() == false)
            {
                return null;
            }

            var query = new ListOnlineDocumentsQuery( DateTime.Parse(TxtStartDate.Text), DateTime.Parse(TxtEndDate.Text) );
        
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
            verticalLayout.PackStart(TextBox.CreateHbox(TxtStartDate, TxtEndDate), false, false, 0);

            return verticalLayout;
        }
    }
}
