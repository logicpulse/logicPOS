using ErrorOr;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Countries.AddCountry;
using LogicPOS.UI.Alerts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LogicPOS.UI.Components
{
    internal partial class CountryModal : Dialog
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();
        public CountryModal(
            Window parent,
            DialogMode dialogMode)
        {
            DesignUI(dialogMode);
            ShowAll();
        }

        private void ButtonOk_Clicked(object sender, EventArgs e)
        {
            switch (_dialogMode)
            {
                case DialogMode.Insert:
                    var addCountryResult = SendAddCountryCommand();
                    HandleAddCountryResult(addCountryResult);
                    break;
                case DialogMode.Update:
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

        private void HandleError(Error error)
        {
            switch (error.Type)
            {
                case ErrorType.Validation:
                    var problem = error.Metadata["problem"] as ProblemDetails;
                    SimpleAlerts.Error()
                                .WithParent(this)
                                .WithMessage(problem.Errors.First().Reason)
                                .WithTitle(problem.Title)
                                .Show();
                    break;

                default:
                    SimpleAlerts.Error()
                                .WithParent(this)
                                .WithMessage(error.Description ?? "Ocorreu um erro")
                                .WithTitle("Erro inesperado")
                                .Show();
                    break;
            }

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
    }
}

