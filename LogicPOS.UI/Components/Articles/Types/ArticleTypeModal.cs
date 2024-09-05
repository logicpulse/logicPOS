using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Types.AddArticleType;
using LogicPOS.Api.Features.Articles.Types.UpdateArticleType;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleTypeModal: EntityModal<ArticleType>
    {
        public ArticleTypeModal(EntityModalMode modalMode, ArticleType entity = null) : base(modalMode, entity)
        {
        }

        private AddArticleTypeCommand CreateAddCommand()
        {
           return new AddArticleTypeCommand
           {
               Designation = _txtDesignation.Text,
               HasPrice = _checkHasPrice.Active,
               Notes = _txtNotes.Value.Text
           };
        }

        private UpdateArticleTypeCommand CreateUpdateCommand()
        {
           return new UpdateArticleTypeCommand
           {
               Id = _entity.Id,
               NewOrder = uint.Parse(_txtOrder.Text),
               NewCode = _txtCode.Text,
               NewDesignation = _txtDesignation.Text,
               HasPrice = _checkHasPrice.Active,
               NewNotes = _txtNotes.Value.Text,
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
           _checkHasPrice.Active= _entity.HasPrice;
           _checkDisabled.Active = _entity.IsDeleted;
           _txtNotes.Value.Text = _entity.Notes;
        }
    }
}