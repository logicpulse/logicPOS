using Gtk;
using System;

namespace LogicPOS.UI.Components.GridViews
{
    public class GridViewSettings
    {
        public TreeModelFilter Filter { get; set; }
        public TreeModelSort Sort { get; set; }
        public TreeModel Model { get; set; }
        public TreePath Path { get; set; }
        public TreeIter Iterator;
        public Type EntityType { get; set; }
    }
}
