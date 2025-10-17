using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
     

        protected override void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(Document), typeof(bool));

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
