using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Currencies.AddCurrency;
using LogicPOS.Api.Features.Currencies.UpdateCurrency;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CurrencyModal : EntityEditionModal<Currency>
    {
        public CurrencyModal(EntityEditionModalMode modalMode, Currency currency = null) : base(modalMode, currency)
        {
        }

        private AddCurrencyCommand CreateAddCommand()
        {
            return new AddCurrencyCommand
            {
                Designation = _txtDesignation.Text,
                Acronym = _txtAcronym.Text,
                ExchangeRate = decimal.Parse(_txtExchangeRate.Text),
                Entity = _txtEntity.Text
            };
        }

        private UpdateCurrencyCommand CreateUpdateCommand()
        {
            return new UpdateCurrencyCommand
            {
                Id = _entity.Id,
                Order = uint.Parse(_txtOrder.Text),
                Code = _txtCode.Text,
                Designation = _txtDesignation.Text,
                Acronym = _txtAcronym.Text,
                ExchangeRate = decimal.Parse(_txtExchangeRate.Text),
                Entity = _txtEntity.Text,
                Symbol = _entity.Symbol,
                Notes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;
        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;


        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtAcronym.Text = _entity.Acronym;
            _txtExchangeRate.Text = _entity.ExchangeRate.ToString();
            _txtEntity.Text = _entity.Entity;
            _txtNotes.Value.Text = _entity.Notes;
            _checkDisabled.Active = _entity.IsDeleted;
        }
    }
}
