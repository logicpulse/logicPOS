using Gtk;
using LogicPOS.Settings;

namespace LogicPOS.UI.Components.GridViews
{
    public class GridViewSettings
    {
        public TreeModelFilter Filter { get; set; }
        public TreeModelSort Sort { get; set; }
        public Pango.FontDescription ColumnTitleFont { get; set; }
        public Pango.FontDescription CellFont { get; set; }
        public GridViewMode Mode { get; set; }
        public TreeModel Model { get; set; }
        public TreePath Path { get; set; }
        public TreeIter Iterator;
        public short CurrentRowIndex { get; set; }
        public int ColumnSortColumnId { get; set; }
        public SortType ColumnSortType { get; set; }
        public bool ColumnSortIndicator { get; set; }
        public int ModelCheckBoxFieldIndex { get; set; }
        public int ModelFirstCustomFieldIndex { get; set; } = 1;

        public static readonly GridViewSettings Default = new GridViewSettings
        {
            ColumnTitleFont = Pango.FontDescription.FromString(AppSettings.Instance.fontGenericTreeViewColumnTitle),
            CellFont = Pango.FontDescription.FromString(AppSettings.Instance.fontGenericTreeViewColumn),
            Mode = GridViewMode.Default
        };


    }
}
