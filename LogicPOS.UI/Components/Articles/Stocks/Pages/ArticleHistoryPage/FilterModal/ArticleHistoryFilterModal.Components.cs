using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles.Stocks.Pages.StockMovementsPage;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Articles.Stocks.Modals.Filters
{
    public partial class ArticleHistoryFilterModal : Modal
    {
        private TextBox TxtStartDate { get; set; }
        private TextBox TxtEndDate { get; set; }
        private TextBox TxtArticle { get; set; }
        private TextBox TxtSerialNumber { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
    }
}
