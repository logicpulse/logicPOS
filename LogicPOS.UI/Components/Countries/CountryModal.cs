using ErrorOr;
using LogicPOS.Api.Features.Countries.AddCountry;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;
using System;

namespace LogicPOS.UI.Components
{
    internal partial class CountryModal : EntityModal
    {
        public CountryModal(EntityModalMode modalMode) : base(modalMode)
        {
        }

        protected override void ButtonOk_Clicked(object sender, EventArgs e)
        {
            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    var addCountryResult = SendAddCountryCommand();
                    HandleAddCountryResult(addCountryResult);
                    break;
                case EntityModalMode.Update:
                    break;
            }
        }

        private void HandleAddCountryResult(ErrorOr<Guid> addCountryResult)
        {
            if (addCountryResult.IsError)
            {
                HandleError(addCountryResult.FirstError);
                return;
            }

            SimpleAlerts.Information()
                        .WithParent(this)
                        .WithMessage("País adicionado com sucesso.")
                        .WithTitle("Sucesso")
                        .Show();

            Destroy();
        }

        private ErrorOr<Guid> SendAddCountryCommand()
        {
            var command = CreateAddCountryCommand();
            return _mediator.Send(command).Result;
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

        protected override void ShowEntity()
        {
            throw new NotImplementedException();
        }
    }
}

