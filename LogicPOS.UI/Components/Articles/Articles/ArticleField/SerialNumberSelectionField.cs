using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using System;


namespace LogicPOS.UI.Components
{
    public class SerialNumberSelectionField : IValidatableField
    {
        public string FieldName => LocalizedString.Instance["global_serial_number"];
        private readonly Article _article;
        public Guid UniqueArticelId { get; set; } 
        public Widget Component { get; private set; }
        public TextBox TxtSerialNumber { get; private set; } = new TextBox(null,
                                                                            LocalizedString.Instance["global_serial_number"],
                                                                            isRequired: true,
                                                                            includeClearButton: false,
                                                                            style: TextBoxStyle.Lite);
        public SerialNumberSelectionField(Article article)
        {
            _article = article;
            InitializeFields();
            Component = CreateComponent();
        }

        private void InitializeFields()
        {
            TxtSerialNumber.Entry.IsEditable = false;
            TxtSerialNumber.Label.Text = _article.Designation;
            TxtSerialNumber.SelectEntityClicked += TxtSerialNumber_SelectEntityClicked;
        }

        private void TxtSerialNumber_SelectEntityClicked(object sender, EventArgs e)
        {
            var page = new WarehouseArticlesPage(null, PageOptions.SelectionPageOptions);
            page.ApplyFilter( x => x.ArticleId == _article.Id);
            var selectArticleModal = new EntitySelectionModal<WarehouseArticle>(page, LocalizedString.Instance["window_title_dialog_select_record"]);
            ResponseType response = (ResponseType)selectArticleModal.Run();
            selectArticleModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtSerialNumber.Text = page.SelectedEntity.SerialNumber;
                UniqueArticelId = page.SelectedEntity.Id;
            }
        }

        private Widget CreateComponent()
        {
            var hbox = new HBox(false, 2);
            hbox.PackStart(new VSeparator(), false, false, 20);
            hbox.PackStart(TxtSerialNumber.Component, true, true, 0);
            return hbox;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(TxtSerialNumber.Text);
        }
    }
}
