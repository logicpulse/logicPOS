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

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                var receipt = model.GetValue(iterator, 0) as ReceiptViewModel;

                if (receipt != null && receipt.RefNo.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }
    }
}
