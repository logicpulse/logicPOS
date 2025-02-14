
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
            throw new System.NotImplementedException();
        }
    }
}
