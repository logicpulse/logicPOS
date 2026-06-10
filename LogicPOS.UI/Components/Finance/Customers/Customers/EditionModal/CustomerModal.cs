using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.Customers.AddCustomer;
using LogicPOS.Api.Features.Customers.HasDocumentsAssociated;
using LogicPOS.Api.Features.Customers.Types.GetAllCustomerTypes;
using LogicPOS.Api.Features.Customers.UpdateCustomer;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Finance.Agt;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.System.Users.Permissions;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerModal : EntityEditionModal<Customer>
    {
        public CustomerModal(EntityEditionModalMode modalMode, Customer entity = null) : base(modalMode, entity)
        {
            if (modalMode == EntityEditionModalMode.Update)
            {
                DisableFiscalInformationsEdition();
            }

            BtnFillCustomerData.Visible = false;
            _txtCardCredit.Entry.Sensitive = false;
        }

      
        private void AddFillCustomerDataBtn()
        {
            BtnFillCustomerData.Clicked += BtnFillCustomerData_Clicked;
            this.AddActionWidget(BtnFillCustomerData, Gtk.ResponseType.None);
            _txtFiscalNumber.Entry.Changed += Entry_Changed;
        }

        private void Entry_Changed(object sender, EventArgs e)
        {
            BtnFillCustomerData.Visible = _txtFiscalNumber.IsValid();
        }

        private void BtnFillCustomerData_Clicked(object sender, EventArgs e)
        {
            if (_txtFiscalNumber.IsValid() == false || string.IsNullOrWhiteSpace(_txtFiscalNumber.Text))
            {
                return;
            }

            var contributor = AgtService.GetAgtContributorInfo(_txtFiscalNumber.Text);

            if (contributor == null)
            {
                CustomAlerts.Warning(this).WithMessage("Não foi possível retornar os dados online do contribuinte.").ShowAlert();
                return;
            }

            _txtName.Text = contributor.Name ?? "";

            Run();
        }

        private void DisableFiscalInformationsEdition()
        {
            if (CustomerHasDocumentsAssociated())
            {
                _txtFiscalNumber.Component.Sensitive = false;
                _comboCountries.Component.Sensitive = false;
            }
        }

        private bool CustomerHasDocumentsAssociated()
        {
            var result = _mediator.Send(new CustomerHasDocumentsAssociatedQuery(_entity.Id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, source: this);
                return true;
            }

            return result.Value;
        }

        protected override void ShowEntityData()
        {
            _txtName.Text = _entity.Name;
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtBirthDate.Text = _entity.BirthDate?.ToString("yyyy/MM/dd");
            _txtAddress.Text = _entity.Address;
            _txtLocality.Text = _entity.Locality;
            _txtCity.Text = _entity.City;
            _txtPostalCode.Text = _entity.ZipCode;
            _txtPhone.Text = _entity.Phone;
            _txtMobile.Text = _entity.MobilePhone;
            _txtEmail.Text = _entity.Email;
            _txtFiscalNumber.Text = _entity.FiscalNumber;
            _txtCardNumber.Text = _entity.CardNumber;
            _txtCardCredit.Text = _entity.CardCredit.ToString("0.00");
            _comboCardMode.Active = (int)_entity.CardMode;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
            _checkSupplier.Active = _entity?.Supplier ?? false;
            _txtDiscount.Text = _entity.Discount.ToString("0.00");

        }

        private UpdateCustomerCommand CreateUpdateCommand()
        {
            return new UpdateCustomerCommand
            {
                Id = _entity.Id,
                Order = uint.Parse(_txtOrder.Text),
                Code = _txtCode.Text,
                Name = _txtName.Text,
                PriceTypeId = _comboPriceTypes.SelectedEntity.Id,
                TypeId = _comboCustomerTypes.SelectedEntity.Id,
                CountryId = _comboCountries.SelectedEntity.Id,
                BirthDate = string.IsNullOrWhiteSpace(_txtBirthDate.Text) ? (DateTime?)null : DateTime.Parse(_txtBirthDate.Text),
                Address = _txtAddress.Text,
                Locality = _txtLocality.Text,
                ZipCode = _txtPostalCode.Text,
                City = _txtCity.Text,
                Phone = _txtPhone.Text,
                MobilePhone = _txtMobile.Text,
                Discount = decimal.Parse(_txtDiscount.Text),
                Email = _txtEmail.Text,
                FiscalNumber = _txtFiscalNumber.Text,
                CardNumber = _txtCardNumber.Text,
                CardMode = (CardMode)_comboCardMode.Active,
                Notes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active,
                Supplier = _checkSupplier.Active
            };
        }

        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;

        private AddCustomerCommand CreateAddCommand()
        {
            var customer = new AddCustomerCommand
            {
                Name = _txtName.Text,
                PriceTypeId = _comboPriceTypes.SelectedEntity.Id,
                CustomerTypeId = _comboCustomerTypes.SelectedEntity.Id,
                CountryId = _comboCountries.SelectedEntity.Id,
                BirthDate = string.IsNullOrEmpty(_txtBirthDate.Text) ? null : (DateTime?)DateTime.Parse(_txtBirthDate.Text),
                Address = _txtAddress.Text,
                Locality = _txtLocality.Text,
                ZipCode = _txtPostalCode.Text,
                City = _txtCity.Text,
                Phone = _txtPhone.Text,
                MobilePhone = _txtMobile.Text,
                Email = _txtEmail.Text,
                Discount = decimal.Parse(_txtDiscount.Text),
                FiscalNumber = _txtFiscalNumber.Text,
                Notes = _txtNotes.Value.Text,
                CardNumber = _txtCardNumber.Text,
                CardMode = (CardMode)_comboCardMode.Active,
                Supplier = _checkSupplier.Active
            };
            return customer;
        }

        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;

        private IEnumerable<PriceType> GetPriceTypes() => ExecuteGetEntitiesQuery(new GetAllPriceTypesQuery());

        private IEnumerable<CustomerType> GetCustomerTypes() => ExecuteGetEntitiesQuery(new GetAllCustomerTypesQuery());

    }
}
