using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Extensions;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsFilterModal
    {
        public void SelectCustomer(Customer entity)
        {
            TxtCustomer.Text= entity.Name;
            TxtCustomer.SelectedEntity = entity;
        }
        public void SelectDocumentType(DocumentType entity)
        {
            TxtDocumentType.Text = entity.Designation;
            TxtDocumentType.SelectedEntity = entity;
        }
        public void SelectVatRate(VatRate entity)
        {
            TxtVatRate.Text=entity.Value.ToString("F2");
            TxtVatRate.SelectedEntity= entity;
        }
        public void SelectWarehouse(DocumentType entity)
        {
            TxtWarehouse.Text= entity.Designation;
            TxtWarehouse.SelectedEntity= entity;
        }
        public void SelectArticle(ArticleViewModel entity)
        {
            var article=ArticlesService.GetArticlebById(entity.Id);
            TxtArticle.Text=entity.Designation;
            TxtArticle.SelectedEntity = article;
        }
        public void SelectArticleHistory(ArticleHistory entity)
        {
            TxtSerialNumber.Text=entity.SerialNumber;
            TxtSerialNumber.SelectedEntity= entity;
        }
        public void SelectDocument(Document entity)
        {
            TxtDocumentNumber.Text=entity.Number;
            TxtDocumentNumber.SelectedEntity= entity;
        }


    }
}
