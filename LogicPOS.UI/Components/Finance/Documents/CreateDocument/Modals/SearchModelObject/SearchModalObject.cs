using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.SearchModelObject
{
    public class SearchModalObject : Widget
    {
        private readonly List<string> _items;
        ListStore listStore = new ListStore(typeof(string));
        TreeView treeView;
        VBox vbox = new VBox();
        public SearchModalObject(Entry entry, List<string>items)
        {
            _items = items;
            treeView = new TreeView(listStore);
            treeView.AppendColumn("Suggestions", new CellRendererText(), "text", 0);
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.Add(treeView);
            scrolledWindow.SetSizeRequest(250, 100);
            scrolledWindow.Hide();

            scrolledWindow.ShowAll();
            entry.Changed += OnEntryChanged;


            vbox.PackStart(entry, false, false, 0);
            vbox.PackStart(scrolledWindow, true, true, 0);

           treeView.Parent.Show();
            

        }
        public VBox ShowObject()
        {
            base.Show();
            vbox.ShowAll();
            return vbox;
        }
        private void OnEntryChanged(object sender, EventArgs e)
        {
            string text = (sender as Entry).Text.ToLower();
            listStore.Clear();
            if (!string.IsNullOrEmpty(text))
            {
                if(_items == null || !_items.Any())
                {
                    return;
                }
                foreach (var item in _items)
                {
                        listStore.AppendValues(_items.ToArray());
                }
                treeView = new TreeView(listStore);
            }

        }
    }
}
