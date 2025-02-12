
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal
    {
        public override Size ModalSize => new Size(450, 520);
        public override string ModalTitleResourceName => "global_article";

        protected override void AddSensitiveFields()
        {
            throw new System.NotImplementedException();
        }

    }
}
