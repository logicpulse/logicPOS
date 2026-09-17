using Gtk;
using LogicPOS.UI.Components.Enums;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Extensions;
using System.Drawing;

namespace LogicPOS.UI.Components.Articles
{
    public partial class ArticleFieldsContainer
    {

        private void SetFieldStyle(ArticleField field)
        {
            switch (_mode)
            {
                case ArticlesBoxMode.StockMovement:
                    field.SetStockMovementStyle();
                    break;
                case ArticlesBoxMode.StockManagement:
                    field.SetStockManagementStyle();
                    break;
            }
        }

        private ScrolledWindow CreateScrolledWindow()
        {
            var swindow = new ScrolledWindow();
            swindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            swindow.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
            swindow.ShadowType = ShadowType.None;
            swindow.AddWithViewport(Container);
            return swindow;
        }

    }
}
