using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement;
using LogicPOS.Api.Features.Articles.StockManagement.ExchangeUniqueArticle;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.StockManagement.UpdateUniqueArticle;
using LogicPOS.Api.Features.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Pages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal : EntityEditionModal<ArticleHistory>
    {
        public UpdateUniqueArticleModal(ArticleHistory entity) : base(EntityEditionModalMode.Update, entity)
        {
            InitializeComponents();
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            TxtSaleDocument.SelectEntityClicked += TxtSaleDocument_SelectEntityClicked;
            BtnSell.Clicked += BtnRegisterOutput_Clicked;
            TxtSaleDate.SelectEntityClicked += TxtSaleDate_SelectEntityClicked;
            TxtExchangeArticle.SelectEntityClicked += TxtExchangeArticle_SelectEntityClicked;
            BtnExchange.Clicked += BtnExchange_Clicked;
        }

        private void TxtExchangeArticle_SelectEntityClicked(object sender, EventArgs e)
        {
            var page = new WarehouseArticlesPage(null, PageOptions.SelectionPageOptions);
            page.ApplyFilter(x => x.Id != _entity.WarehouseArticle.ArticleId && x.SerialNumber != null);
            var selectArticleModal = new EntitySelectionModal<WarehouseArticle>(page, LocalizedString.Instance["window_title_dialog_select_record"]);
            ResponseType response = (ResponseType)selectArticleModal.Run();
            selectArticleModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtExchangeArticle.Text = page.SelectedEntity.SerialNumber;
                TxtExchangeArticle.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnExchange_Clicked(object sender, EventArgs e)
        {
            if (TxtExchangeArticle.IsValid() == false)
            {
                ValidationUtilities.ShowValidationErrors(new IValidatableField[] {TxtExchangeArticle}, this);
                return;
            }

            var result = ExecuteUpdateCommand(CreateExchangeCommand());

            if (result.IsError == false)
            {
                Respond(ResponseType.Ok);
            }
        }

        private ExchangeUniqueArticleCommand CreateExchangeCommand()
        {
            return new ExchangeUniqueArticleCommand
            {
                ReturnedArticleId = _entity.WarehouseArticle.Id,
                ExchangeArticleId = (TxtExchangeArticle.SelectedEntity as ApiEntity).Id
            };
        }

        private void TxtSaleDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                TxtSaleDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }
        }

        private void BtnRegisterOutput_Clicked(object sender, EventArgs e)
        {
            if (TxtSaleDocument.IsValid() == false || TxtSaleDate.IsValid() == false)
            {
                ValidationUtilities.ShowValidationErrors(new IValidatableField[] { TxtSaleDocument, TxtSaleDate }, this);
                return;
            }

            var result = ExecuteCommand(CreateSellUniqueArticleCommand());

            if(result.IsError == false)
            {
                Respond(ResponseType.Ok);
            }
        }

        private void TxtSaleDocument_SelectEntityClicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(this, selectionMode: true);
            ResponseType response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok && modal.Page.SelectedEntity != null)
            {
                TxtSaleDocument.Text = modal.Page.SelectedEntity.Number;
                TxtSaleDocument.SelectedEntity = modal.Page.SelectedEntity;
            }

            modal.Destroy();
        }

        protected override void AddEntity() { }

        protected override void ShowEntityData()
        {
            SerialNumberField.TxtSerialNumber.Text = _entity.WarehouseArticle.SerialNumber;
            TxtArticle.Text = _entity.WarehouseArticle.Article.Designation;

            if (_entity.WarehouseArticle.Status == Api.Enums.ArticleSerialNumberStatus.Sold || 
               _entity.WarehouseArticle.Status == Api.Enums.ArticleSerialNumberStatus.Exchanged)
            {
                TxtSaleDocument.Text = _entity.OutStockMovement?.DocumentNumber;
                TxtSaleDate.Text = _entity.OutStockMovement?.Date.ToString("yyyy-MM-dd");
            }

            SerialNumberField.LoadUniqueArticleChildren(_entity.WarehouseArticle.Id);
        }

        protected override void UpdateEntity()
        {
            ExecuteUpdateCommand(CreateUpdateCommand());
        }

        private UpdateUniqueArticleCommand CreateUpdateCommand()
        {
            var childUniqueArticles = SerialNumberField.Children.Select(x => x.UniqueArticelId).ToList();
            return new UpdateUniqueArticleCommand
            {
                Id = _entity.WarehouseArticle.Id,
                SerialNumber = SerialNumberField.TxtSerialNumber.Text,
                ChildUniqueArticles = childUniqueArticles
            };
        }

        private AddStockMovementCommand CreateSellUniqueArticleCommand()
        {
            return new AddStockMovementCommand
            {
                SupplierId = (TxtSaleDocument.SelectedEntity as Document).CustomerId,
                DocumentNumber = TxtSaleDocument.Text,
                Date = DateTime.Parse(TxtSaleDate.Text),
                Items = new StockMovementItem[]
                {
                    new StockMovementItem
                    {
                        ArticleId = _entity.WarehouseArticle.ArticleId,
                        Quantity = -1,
                        SerialNumber = _entity.WarehouseArticle.SerialNumber,
                        Price = _entity.WarehouseArticle.Article.Price1.Value,
                    }
                }
            };
        }
    }
}
