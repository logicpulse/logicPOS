using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void SelectTerminal(Terminal entity)
        {
            TxtTerminal.Text = entity.Designation;
            TxtTerminal.SelectedEntity = entity;
        }

        public void SetWarehouseLocations(Warehouse warehouse)
        {
            TxtWarehouseLocation.Clear();

            var locations = warehouse?.Locations?
                                      .Where(location => !location.IsDeleted)
                                      .OrderBy(location => location.Designation)
                                      .ToList() ?? new List<WarehouseLocation>();

            TxtWarehouseLocation.WithAutoCompletion(locations.Select(location => new AutoCompleteLine { Id = location.Id, Name = location.Designation }).ToList(),
                                                    id => locations.FirstOrDefault(location => location.Id == id));
            TxtWarehouseLocation.Component.Sensitive = locations.Count > 0;

            var defaultLocation = locations.FirstOrDefault(location => location.IsDefault) ?? locations.FirstOrDefault();
            if (defaultLocation != null)
            {
                TxtWarehouseLocation.Text = defaultLocation.Designation;
                TxtWarehouseLocation.SelectedEntity = defaultLocation;
            }
        }


    }
}
