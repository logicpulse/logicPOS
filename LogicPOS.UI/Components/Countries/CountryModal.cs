using ErrorOr;
using LogicPOS.Api.Features.Countries.AddCountry;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;
using System;

namespace LogicPOS.UI.Components.Modals
{
    internal partial class CountryModal : EntityModal
    {
        public CountryModal(EntityModalMode modalMode) : base(modalMode)
        {
        }

        private AddCountryCommand CreateAddCountryCommand()
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

        protected override void ShowEntityData()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEntity()
        {
            throw new NotImplementedException();
        }

        protected override void AddEntity()
        {
            var command = CreateAddCountryCommand();
            var result =  _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleError(result.FirstError);
                this.Run();
                return;
            }
        }
    }
}

