using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace LogicPOS.UI.Components.InputFields
{
    public class ArticleField : IValidatableField
    {
        public VBox Component { get; private set; } = new VBox(false, 2);
        public IconButton BtnSelect { get; set; }
        public IconButton BtnRemove { get; set; }
        public IconButton BtnAdd { get; set; }
        public Entry TxtDesignation { get; set; } = new Entry() { IsEditable = false };
        public Entry TxtQuantity { get; set; } = new Entry() { WidthRequest = 50};
        public Entry TxtCode { get; set; } = new Entry() { WidthRequest = 50, IsEditable = false };
        public Label Label { get; set; } = new Label(GeneralUtils.GetResourceByName("global_article"));
        public Article Article { get; set; }

        public string FieldName => Label.Text;

        public event System.Action<ArticleField, Article> OnRemove;
        public event System.Action OnAdd;

        public ArticleField(Article article = null, uint quantity = 0)
        {
            Article = article;
            TxtQuantity.Text = quantity.ToString();
            Label.SetAlignment(0, 0.5f);
            InitializeButtons();
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
            ValidationColors.Default.UpdateComponentBackgroundColor(TxtQuantity, IsValid());
        }

        private void InitializeButtons()
        {
            string iconSelectRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}";
            string iconClearRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_delete_record.png"}";
            string iconAddRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_nav_new.png"}";

            BtnSelect = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconSelectRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnRemove = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconClearRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnAdd = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconAddRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
        }

        private void PackComponents()
        {
            Component.PackStart(Label, false, false, 0);
            Component.PackStart(CreateHBox(), false, false, 0);
        }

        private HBox CreateHBox()
        {
            var hbox = new HBox(false, 2);

            hbox.PackStart(TxtCode, false, false, 0);
            hbox.PackStart(TxtDesignation, true, true, 0);
            hbox.PackStart(TxtQuantity, false, false, 0);
            hbox.PackStart(BtnSelect, false, false, 0);
            hbox.PackStart(BtnRemove, false, false, 0);
            hbox.PackStart(BtnAdd, false, false, 0);

            return hbox;
        }

        private void AddEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => OnRemove?.Invoke(this, Article);
            BtnAdd.Clicked += (s, e) => OnAdd?.Invoke();
            TxtQuantity.Changed += (s, e) => UpdateValidationColors();
            BtnSelect.Clicked += BtnSelect_Clicked;
        }

        private void BtnSelect_Clicked(object sender, System.EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectArticleModal = new EntitySelectionModal<Article>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectArticleModal.Run();
            selectArticleModal.Destroy();

            if(response == ResponseType.Ok && page.SelectedEntity != null)
            {
                Article = page.SelectedEntity;
                ShowEntity();
            }
        }

        public bool IsValid()
        {
            return (Article != null) && Regex.IsMatch(TxtQuantity.Text, RegularExpressions.IntegerNumber) && int.Parse(TxtQuantity.Text) > 0;
        }
    }
}
