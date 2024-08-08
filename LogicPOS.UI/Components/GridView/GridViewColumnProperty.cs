using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using System;
using System.Collections.Generic;
using System.Data;

namespace LogicPOS.UI.Components
{
    internal class GridViewColumnProperty : IEquatable<GridViewColumnProperty>, IComparable<GridViewColumnProperty>
    {
        private readonly string _fontGenericTreeViewColumnTitle = Settings.AppSettings.Instance.fontGenericTreeViewColumnTitle;
        private readonly string _fontGenericTreeViewColumn = Settings.AppSettings.Instance.fontGenericTreeViewColumn;
        public GridViewPropertyType PropertyType { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public string ChildName { get; set; }
        public string Title { get; set; }
        public int MinWidth { get; set; }
        public int MaxWidth { get; set; }
        public int FixedWidth { get; set; }
        public int Spacing { get; set; }
        public bool Visible { get; set; }
        public bool Resizable { get; set; }
        public bool Reorderable { get; set; }
        public string ForegroundGdkColor { get; set; }
        public bool Expand { get; set; }
        public NullBoolean Searchable { get; set; }
        public float Alignment { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        public Pango.FontDescription FontDescription { get; set; }
        public Pango.FontDescription FontDescriptionTitle { get; set; }
        public CellRendererText CellRenderer { get; set; }
        public TreeViewColumn Column { get; set; }
        public object InitialValue { get; set; }
        public string Query { get; set; }
        public bool DecryptValue { get; set; }
        public bool ResourceString { get; set; }

        public GridViewColumnProperty(string fieldName,
                                      GridViewColumnProperty defaultColumnProperty = null)
        {
            Name = fieldName;
            PropertyType = GridViewPropertyType.Text;

            if (Title == null)
            {
                Title = Name;
            }

            Visible = true;
            Resizable = true;
            Searchable = NullBoolean.Null;

            FontDescriptionTitle = Pango.FontDescription.FromString(_fontGenericTreeViewColumnTitle);
            FontDescription = Pango.FontDescription.FromString(_fontGenericTreeViewColumn);
            CellRenderer = new CellRendererText();
            if (FontDescription != null) CellRenderer.FontDesc = FontDescription;

            CellRenderer.SetFixedSize(0, 24);

            if (defaultColumnProperty != null)
            {
                InitDefaultColumnProperties(defaultColumnProperty);
            }

            InitDefaultPropertiesByFieldName(fieldName);
        }

        public void InitDefaultColumnProperties(GridViewColumnProperty pDefaultProperties)
        {
            if (pDefaultProperties != null)
            {
                if (pDefaultProperties.PropertyType != GridViewPropertyType.Text) PropertyType = pDefaultProperties.PropertyType;
                if (pDefaultProperties.MinWidth > 0) MinWidth = pDefaultProperties.MinWidth;
                if (pDefaultProperties.MaxWidth > 0) MaxWidth = pDefaultProperties.MaxWidth;
                if (pDefaultProperties.FixedWidth > 0) FixedWidth = pDefaultProperties.FixedWidth;
                if (pDefaultProperties.Spacing > 0) Spacing = pDefaultProperties.Spacing;
                if (pDefaultProperties.Visible == true || pDefaultProperties.Visible == false) Visible = pDefaultProperties.Visible;
                if (pDefaultProperties.Resizable == true || pDefaultProperties.Resizable == false) Resizable = pDefaultProperties.Resizable;
                if (pDefaultProperties.Reorderable == true || pDefaultProperties.Reorderable == false) Reorderable = pDefaultProperties.Reorderable;
                if (pDefaultProperties.Expand == true || pDefaultProperties.Expand == false) Expand = pDefaultProperties.Expand;
                if (pDefaultProperties.Searchable != NullBoolean.Null) Searchable = pDefaultProperties.Searchable;
                if (pDefaultProperties.Alignment >= 0) Alignment = pDefaultProperties.Alignment;
                if (pDefaultProperties.FontDescriptionTitle != null) FontDescriptionTitle = pDefaultProperties.FontDescriptionTitle;
                if (pDefaultProperties.FontDescription != null) FontDescription = pDefaultProperties.FontDescription;
                if (pDefaultProperties.CellRenderer != null) CellRenderer = pDefaultProperties.CellRenderer;
            }
        }

        public void InitDefaultPropertiesByFieldName(string pProperty)
        {
            switch (pProperty)
            {
                case "Code":
                    MinWidth = 60;
                    MaxWidth = 100;
                    CellRenderer = new CellRendererText()
                    {
                        Alignment = Pango.Alignment.Right,
                        Xalign = 1.0F,
                        ForegroundGdk = new Gdk.Color(255, 0, 0)
                    };

                    if (FontDescription != null) CellRenderer.FontDesc = FontDescription;
                    break;

                case "Designation":
                    MinWidth = 250;
                    MaxWidth = 800;
                    Expand = true;
                    CellRenderer = new CellRendererText()
                    {
                        Alignment = Pango.Alignment.Left,
                    };

                    if (FontDescription != null) CellRenderer.FontDesc = FontDescription;
                    break;

                case "Disabled":
                    CellRenderer = new CellRendererText()
                    {
                        Alignment = Pango.Alignment.Right
                    };

                    if (FontDescription != null) CellRenderer.FontDesc = FontDescription;
                    break;

                default:
                    break;
            }
        }

        public bool Equals(GridViewColumnProperty other)
        {
            if (Name == other.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CompareTo(GridViewColumnProperty other)
        {
            if (other == null) return 1;
            return Name.CompareTo(other.Name);
        }


        public static DataTable ColumnPropertiesToDataTableScheme(List<GridViewColumnProperty> pColumnProperties)
        {
            DataTable resultDataTable = new DataTable();

            foreach (GridViewColumnProperty column in pColumnProperties)
            {
                Type dataTableColumnType = column.Type != null ? column.Type : typeof(string);
                resultDataTable.Columns.Add(column.Name, dataTableColumnType);
            }

            return resultDataTable;
        }
    }
}
