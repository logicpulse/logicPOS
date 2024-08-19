using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Countries.AddCountry;
using LogicPOS.Api.Features.Countries.UpdateCountry;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;
using System;

namespace LogicPOS.UI.Components.Modals
{
    internal partial class CountryModal : EntityModal
    {
        public CountryModal(
            EntityModalMode modalMode, 
            Country country = null) : base(modalMode,country)
        {
        }

        private AddCountryCommand CreateAddCommand()
        {
            return new AddCountryCommand
            {
                Designation = _txtDesignation.Text,
                Code2 = _txtCode2.Text,
                Code3 = _txtCode3.Text,
                Capital = _txtCapital.Text,
                TLD = ".api",
                Currency = _txtCurrency.Text,
                CurrencyCode = _txtCurrencyCode.Text,
                FiscalNumberRegex = _txtFiscalNumberRegex.Text,
                ZipCodeRegex = _txtZipCodeRegex.Text,
                Notes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        private UpdateCountryCommand CreateUpdateCommand() { 
            return new UpdateCountryCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewCode2 = _txtCode2.Text,
                NewCode3 = _txtCode3.Text,
                NewCapital = _txtCapital.Text,
                NewCurrency = _txtCurrency.Text,
                NewCurrencyCode = _txtCurrencyCode.Text,
                NewFiscalNumberRegex = _txtFiscalNumberRegex.Text,
                NewZipCodeRegex = _txtZipCodeRegex.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void ShowEntityData()
        {
            var country = _entity as Country;
            _txtOrder.Text = country.Order.ToString();
            _txtCode.Text = country.Code;
            _txtCode2.Text = country.Code2;
            _txtCode3.Text = country.Code3;
            _txtCapital.Text = country.Capital;
            _txtCurrency.Text = country.Currency;
            _txtCurrencyCode.Text = country.CurrencyCode;
            _txtFiscalNumberRegex.Text = country.FiscalNumberRegex;
            _txtZipCodeRegex.Text = country.ZipCodeRegex;
            _txtDesignation.Text = country.Designation;
            _txtNotes.Value.Text = country.Notes;
            _checkDisabled.Active = country.IsDeleted;
        }

        protected override void UpdateEntity()
        {
            var command = CreateUpdateCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }

        protected override void AddEntity()
        {
            var command = CreateAddCommand();
            var result =  _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }
    }
}

