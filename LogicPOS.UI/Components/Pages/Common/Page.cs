using Gtk;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.UI.Components.GridViews;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class Page : Box
    {
        public Page(Window parent)
        {
            _parentWindow = parent;
        }

        protected readonly Window _parentWindow;
        public TreeView GridView { get; set; }
        public object SelectedEntity { get; set; }
        public bool AllowRecordView { get; set; }
        public bool AllowRecordUpdate { get; set; }
        public bool AllowRecordDelete { get; set; }
        public GridViewSettings GridViewSettings { get; set; } = GridViewSettings.Default;
        internal PageNavigator Navigator { get; set; }

        internal void Delete()
        {
            throw new NotImplementedException();
        }

        internal void Insert()
        {
            throw new NotImplementedException();
        }

        internal void Next()
        {
            GridViewSettings.Path.Next();
            GridView.SetCursor(GridViewSettings.Path, null, false);
        }

        internal void Previous()
        {
            GridViewSettings.Path.Prev();
            GridView.SetCursor(GridViewSettings.Path, null, false);
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
