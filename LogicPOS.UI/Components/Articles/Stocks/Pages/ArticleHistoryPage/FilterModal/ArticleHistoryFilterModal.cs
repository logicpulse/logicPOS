using Gtk;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Articles.Stocks.Modals.Filters
{
    public partial class ArticleHistoryFilterModal : Modal
    {
        public ArticleHistoryFilterModal(Window parent) :
            base(parent,
                LocalizedString.Instance["Filtrar Histórico do Artigo"],
                new Size(540, 568),
                AppSettings.Paths.Images + @"Icons\Windows\icon_window_date_picker.png")
        {
        }
        protected override void OnResponse(ResponseType response)
        {
            if (response == ResponseType.None)
            {
                Run();
                return;
            }
        }
        public bool AllFieldsAreValid() => GetValidatableFields().All(field => field.IsValid());

        private IEnumerable<IValidatableField> GetValidatableFields()
        {
            return new List<IValidatableField>
            {
                TxtStartDate,
                TxtEndDate
            };
        }

        private void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableFields(), this);

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnClear.Clicked += BtnClear_Clicked;
        }

        public ArticleHistoryFilterModalData? GetFilterData()
        {
            if(AllFieldsAreValid() == false)
            {
                return null;
            }

            var data = new ArticleHistoryFilterModalData();
           

            if (string.IsNullOrWhiteSpace(TxtStartDate.Text) == false)
            {
                data.StartDate = DateTime.Parse(TxtStartDate.Text);
            }

            if (string.IsNullOrWhiteSpace(TxtEndDate.Text) == false)
            {
                data.EndDate = DateTime.Parse(TxtEndDate.Text);
            }

            if (TxtArticle.SelectedEntity != null)
            {
                var article = TxtArticle.SelectedEntity as ArticleViewModel;
                data.ArticleId = article.Id;
            }

            if (TxtSerialNumber.SelectedEntity != null)
            {
                var articleHistory = (TxtSerialNumber.SelectedEntity as ArticleHistory);
                data.SerialNumber = articleHistory.SerialNumber;
            }
            if (!string.IsNullOrEmpty(TxtSerialNumber.Text))
            {
                data.SerialNumber = TxtSerialNumber.Text;
            }

            return data;
        }

    }
}
