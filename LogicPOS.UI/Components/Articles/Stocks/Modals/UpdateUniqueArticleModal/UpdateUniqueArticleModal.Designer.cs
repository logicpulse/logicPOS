
using Gtk;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal
    {
        public override Size ModalSize => new Size(450, 520);
        public override string ModalTitleResourceName => "global_article";

        protected override void AddSensitiveFields()
        {

        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateSerialNumberTab(), "Editar");
            yield return (CreateExchangeTab(), "Troca");
            yield return (CreateOutputTab(), "Movimento de Saída");
        }

        private VBox CreateSerialNumberTab()
        {
            var tab = new VBox();
            tab.PackStart(new Label($"Artigo: {_entity.Article}"), false, false, 5);
            tab.PackStart(SerialNumberField.Component, false, false, 0);
            return tab;
        }

        private VBox CreateOutputTab()
        {
            var tab = new VBox();
            tab.PackStart(TxtSaleDocument.Component, false, false, 0);
            tab.PackStart(TxtSaleDate.Component, false, false, 0);
            tab.PackStart(BtnSell, false, false, 0);
            return tab;
        }

        private VBox CreateExchangeTab()
        {
            var tab = new VBox();
            tab.PackStart(TxtArticle.Component, false, false, 0);
            tab.PackStart(TxtExchangeArticle.Component, false, false, 0);
            tab.PackStart(BtnExchange, false, false, 0);
            return tab;
        }
    }
}
