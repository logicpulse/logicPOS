using Gtk;

namespace LogicPOS.UI.Components.GridViews
{
    public class GridViewSettings
    {
        public TreeModelFilter Filter { get; set; }
        public TreeModelSort Sort { get; set; }
        public Pango.FontDescription ColumnTitleFont { get; set; } = CellRenderers.TitleFont;
        public Pango.FontDescription CellFont { get; set; } = CellRenderers.CellFont;
        public TreeModel Model { get; set; }
        public TreePath Path { get; set; }
        public TreeIter Iterator;

        public static readonly GridViewSettings Default = new GridViewSettings
        {
        };
    }
}
