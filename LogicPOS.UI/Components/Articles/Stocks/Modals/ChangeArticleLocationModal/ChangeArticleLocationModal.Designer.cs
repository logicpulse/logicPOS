
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ChangeArticleLocationModal
    {
        public override Size ModalSize => new Size(320, 450);
        public override string ModalTitleResourceName => "global_warehose_management";

        #region Components
        private TextBox _txtArticle { get; set; } = TextBox.Simple("global_article", true);
        private TextBox _txtSerialNumber { get; set; } = TextBox.Simple("global_serialNumber", false);
        private TextBox _txtQuantity { get; set; } = TextBox.Simple("global_quantity", true);
        private WarehouseSelectionField _locationField;
        #endregion

        protected override void Initialize()
        {
            var warehouseArticle = ArticlesService.GetWarehouseArticleById(_entity.Id);
            _locationField = new WarehouseSelectionField(warehouseArticle);

            _txtArticle.Component.Sensitive = false;
            _txtSerialNumber.Component.Sensitive = false;
            _txtQuantity.Component.Sensitive = false;
        }
 
        protected override void AddSensitiveFields()
        {

        }

        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(_locationField);
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            var tab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            tab.PackStart(_txtArticle.Component, false, false, 0);
            tab.PackStart(_txtSerialNumber.Component, false, false, 0);
            tab.PackStart(_locationField.WarehouseField.Component, false, false, 0);
            tab.PackStart(_locationField.LocationField.Component, false, false, 0);
            tab.PackStart(_txtQuantity.Component, false, false, 0);

            yield return (tab, GeneralUtils.GetResourceByName("window_title_article_location"));
        }

    }
}
