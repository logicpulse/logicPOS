using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerCurrentAccountFilterModal
    {
        private void InitializeTxtEndDate()
        {
            TxtEndDate = new TextBox(this,
                                         GeneralUtils.GetResourceByName("global_date_end"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtEndDate.Entry.IsEditable = false;
            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            TxtEndDate.Entry.Changed += TxtEndDate_Entry_Changed;
            TxtEndDate.SelectEntityClicked += TxtEndDate_SelectEntityClicked;
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new TextBox(this,
                                       GeneralUtils.GetResourceByName("global_customer"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: true,
                                       includeKeyBoardButton: false);

            TxtCustomer.Entry.IsEditable = true;
            var customers = CustomersForCompletion.Select(c => (c as object, c.Name)).ToList();
            TxtCustomer.WithAutoCompletion(customers);
            TxtCustomer.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtCustomer.Entry.Changed += TxtCustomer_Changed;
            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;

            ValidatableFields.Add(TxtCustomer);
        }

        private void InitializeTxtStartDate()
        {
            TxtStartDate = new TextBox(this,
                                      GeneralUtils.GetResourceByName("global_date_start"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: true,
                                      includeKeyBoardButton: false);

            TxtStartDate.Entry.IsEditable = false;
            TxtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            TxtStartDate.Entry.Changed+=TxtStartDate_Entry_Changed;
            TxtStartDate.SelectEntityClicked += TxtStartDate_SelectEntityClicked;
        }
        private void TxtStartDate_Entry_Changed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtStartDate.Text) && TxtStartDate.Text.Length >= 10)
            {
                if (TxtStartDate.IsValid())
                {
                    TxtStartDate.Text = TxtStartDate.Text.ValidateDate();
                }
                return;
            }
        }

        private void TxtEndDate_Entry_Changed(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(TxtEndDate.Text)) && TxtEndDate.Text.Length >= 10)
            {
                if (TxtEndDate.IsValid())
                {
                    TxtEndDate.Text = TxtEndDate.Text.ValidateDate();
                }
                return;
            }
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var verticalLayout = new VBox(false, 0);

            verticalLayout.PackStart(TxtCustomer.Component, false, false, 0);
            verticalLayout.PackStart(TxtStartDate.Component, false, false, 0);
            verticalLayout.PackStart(TxtEndDate.Component, false, false, 0);

            return verticalLayout;
        }

        protected void Validate()
        {
            if (AllFieldsAreValid())
            {
                return;
            }

            ValidationUtilities.ShowValidationErrors(ValidatableFields);

            Run();
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }

        public static void ShowModal(Window parent)
        {
            var modal = new CustomerCurrentAccountFilterModal(parent);
            modal.Run();
            modal.Destroy();
        }
    }
}
