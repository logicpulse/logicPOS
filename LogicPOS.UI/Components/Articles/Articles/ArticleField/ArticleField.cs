using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class ArticleField : IValidatableField
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public Article Article { get; set; }
        public string FieldName => Label.Text;
        public event System.Action<ArticleField, Article> OnRemove;
        public event System.Action OnAdd;
        private readonly bool _enableSerialNumbers;
        public ArticleField(Article article = null,
                            decimal quantity = 0,
                            bool enableSerialNumbers = false)
        {
            _enableSerialNumbers = enableSerialNumbers;
            Article = article;
            TxtQuantity.Text = quantity.ToString();
            Label.SetAlignment(0, 0.5f);
            InitializeButtons();

            if (enableSerialNumbers)
            {
                InitializeComboboxes();
            }
              
            PackComponents();
            AddEventHandlers();
            UpdateValidationColors();
            ShowEntity();
        }

        private void ShowEntity()
        {
            if (Article != null)
            {
                TxtCode.Text = Article.Code;
                TxtDesignation.Text = Article.Designation;
                TxtQuantity.Text = "1";
            }
        }

        private void UpdateValidationColors()
        {
            ValidationColors.Default.UpdateComponentFontColor(Label, IsValid());
            ValidationColors.Default.UpdateComponentBackgroundColor(TxtDesignation, Article != null);
            ValidationColors.Default.UpdateComponentBackgroundColor(TxtCode, Article != null);
            ValidationColors.Default.UpdateComponentBackgroundColor(TxtQuantity, QuantityIsValid());
        }

        private bool QuantityIsValid()
        {
            return Regex.IsMatch(TxtQuantity.Text, RegularExpressions.Quantity);
        }

        private void AddEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => OnRemove?.Invoke(this, Article);
            BtnAdd.Clicked += (s, e) => OnAdd?.Invoke();
            TxtQuantity.Changed += (s, e) => UpdateValidationColors();
            BtnSelect.Clicked += BtnSelect_Clicked;

            if(_enableSerialNumbers)
            {
                TxtQuantity.Changed += (s, e) =>
                {
                    UpdateSerialNumbersComponents();
                    Component.ShowAll();
                };
            }
           
        }

        private void BtnSelect_Clicked(object sender, System.EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectArticleModal = new EntitySelectionModal<Article>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectArticleModal.Run();
            selectArticleModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                Article = page.SelectedEntity;
                ShowEntity();
            }
        }

        public bool IsValid()
        {
            var result = (Article != null) && QuantityIsValid();

            if (_enableSerialNumbers)
            {
                result = result && _comboWarehouse.IsValid() && _comboWarehouseLocation.IsValid() && _serialNumberFields.All(f => f.IsValid());
            }

            return result;
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

        public IEnumerable<string> SerialNumbers => _serialNumberFields.Where(f => string.IsNullOrWhiteSpace(f.Text) == false).Select(f => f.Text);

        public decimal Price => string.IsNullOrEmpty(TxtPrice.Text) ? 0 : decimal.Parse(TxtPrice.Text);

        public Guid? WarehouseLocationId => _comboWarehouseLocation.SelectedEntity?.Id;
    }
}
