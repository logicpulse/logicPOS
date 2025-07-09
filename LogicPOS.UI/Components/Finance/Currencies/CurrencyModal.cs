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
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewAcronym = _txtAcronym.Text,
                NewExchangeRate = decimal.Parse(_txtExchangeRate.Text),
                NewEntity = _txtEntity.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());
        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());


        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtAcronym.Text = _entity.Acronym;
            _txtExchangeRate.Text = _entity.ExchangeRate.ToString();
            _txtEntity.Text = _entity.Entity;
            _checkDisabled.Active = _entity.IsDeleted;
        }
    }
}
