using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Classes.AddArticleClass;
using LogicPOS.Api.Features.Articles.Classes.UpdateArticleClass;



namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleClassModal : EntityModal<ArticleClass>
    {
        public ArticleClassModal(EntityModalMode modalMode, ArticleClass entity = null) : base(modalMode, entity)
        {
        }

        private AddArticleClassCommand CreateAddCommand()
        {
            return new AddArticleClassCommand
            {
                Designation = _txtDesignation.Text,
                Acronym = _txtAcronym.Text,
                WorkInStock = _checkWorkInStock.Active,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateArticleClassCommand CreateUpdateCommand()
        {
            return new UpdateArticleClassCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewAcronym = _txtAcronym.Text,
                WorkInStock = _checkWorkInStock.Active,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtAcronym.Text = _entity.Acronym;
            _checkWorkInStock.Active = _entity.WorkInStock;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void UpdateEntity()=> ExecuteUpdateCommand(CreateUpdateCommand());
    }
}
