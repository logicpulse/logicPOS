using Gtk;
using logicpos.Classes.Enums.Dialogs;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class Page : Box
    {
        public TreeView TreeView { get; set; }
        public TreeModelFilter ListStoreModelFilter { get; set; }
        public object Entity { get; set; }
        public bool AllowRecordView { get; set; }
        public bool AllowRecordUpdate { get; set; }
        public bool AllowRecordDelete { get; set; }
        internal List<GridViewColumnProperty> Columns { get; set; }

        internal void Delete()
        {
            throw new NotImplementedException();
        }

        internal void Insert()
        {
            throw new NotImplementedException();
        }

        internal void NextRecord()
        {
            throw new NotImplementedException();
        }

        internal void PrevRecord()
        {
            throw new NotImplementedException();
        }

        internal void Refresh()
        {
            throw new NotImplementedException();
        }

        internal void Update(DialogMode view)
        {
            throw new NotImplementedException();
        }

        internal void Update()
        {
            throw new NotImplementedException();
        }
    }
}
