using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ChangeArticleLocationModal : EntityEditionModal<WarehouseArticle>
    {
        public ChangeArticleLocationModal(WarehouseArticle entity) : base(EntityEditionModalMode.Update, entity)
        {
        }

        protected override void AddEntity() => throw new System.NotImplementedException();
        
        protected override void ShowEntityData()
        {
            _txtArticle.SelectedEntity = _entity.Article;
            _txtArticle.Text = _entity.Article.Designation;
            _txtSerialNumber.Text = _entity.SerialNumber;
            _txtQuantity.Text = _entity.Quantity.ToString();
        }

        protected override void UpdateEntity()
        {
            throw new System.NotImplementedException();
        }

        private IEnumerable<Warehouse> GetWarehouses()
        {
            var result = _mediator.Send(new GetAllWarehousesQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return Enumerable.Empty<Warehouse>();
            }

            return result.Value;
        }
    }
}
