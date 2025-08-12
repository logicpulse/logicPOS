using Gtk;
using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ReceiptsPage
    {
        protected override void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(ReceiptViewModel), typeof(bool));

            InitializeGridViewModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
        }

        
    }
}
