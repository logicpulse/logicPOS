using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    //TODO: Create GenericTreeViewColumns Class to Remove List<GenericTreeViewColumnProperty> and Replace with Dictionary<string, GenericTreeViewColumnProperty>  
    //This way we can Get Columns by Key :)
    //class GenericTreeViewColumns : Dictionary<string, GenericTreeViewColumnProperty>
    //{
    //  fill Stub
    //}

    /// <summary>Class used to store TreeView Column Properties, used to configure used xpColection columns</summary>
    class GenericTreeViewColumnProperty : IEquatable<GenericTreeViewColumnProperty>, IComparable<GenericTreeViewColumnProperty>
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Settings
        private string _fontGenericTreeViewColumnTitle = GlobalFramework.Settings["fontGenericTreeViewColumnTitle"];
        private string _fontGenericTreeViewColumn = GlobalFramework.Settings["fontGenericTreeViewColumn"];

        //Aditional TreeView ColumnProperties
        public GenericTreeViewColumnPropertyType PropertyType { get; set; }
        public string Name { get; set; }
        //Used most for DataTable Mode
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
        public bool Expand { get; set; }
        public NullBoolean Searchable { get; set; }
        public float Alignment { get; set; }
        public IFormatProvider FormatProvider { get; set; }
        //Title Widget FontDesc
        public Pango.FontDescription FontDesc { get; set; }
        public Pango.FontDescription FontDescTitle { get; set; }
        /// <summary>Cell Renderer Properties</summary>
        public CellRendererText CellRenderer { get; set; }
        /// <summary>Store the TreeViewColumn Properties in ColumnProperties, its not a Column Property</summary>
        public TreeViewColumn Column { get; set; }
        //Use Default Initial Values for New Records
        public object InitialValue { get; set; }
        //Query: Implemented Here : GenericTreeeView.ColumnPropertyGetQuery
        public string Query { get; set; }
        //Decrypt Value
        public bool DecryptValue { get; set; }
        //ResourceString
        public bool ResourceString { get; set; }
        
        /// <summary>Constructor</summary>
        /// Note: DefaultColumnProperty Never used in Code
        public GenericTreeViewColumnProperty(String pFieldName, GenericTreeViewColumnProperty pDefaultColumnProperty = null)
        {
            //Parameters
            Name = pFieldName;
            //Default Type
            PropertyType = GenericTreeViewColumnPropertyType.Text;
            //Always have a Column Title
            if (Title == null) Title = Name;
            //Base DefaultProperties Values
            Visible = true;
            Resizable = true;
            Searchable = NullBoolean.Null;
            //Default Fonts      
            FontDescTitle = Pango.FontDescription.FromString(_fontGenericTreeViewColumnTitle);
            FontDesc = Pango.FontDescription.FromString(_fontGenericTreeViewColumn);
            CellRenderer = new CellRendererText();
            if (this.FontDesc != null) CellRenderer.FontDesc = this.FontDesc;
            //SetFixedSize, this will force all rows to be same size, even multile longtext Fields :)
            CellRenderer.SetFixedSize(0, 24);
            //CellRenderer.SingleParagraphMode = true;
            //CellRenderer.WrapMode = Pango.WrapMode.Word;

            //If have Default Properties parameter, Assign it
            if (pDefaultColumnProperty != null) {
                InitDefaultColumnProperties(pDefaultColumnProperty);
            }

            //After Defaults, Init Field Properties by Field Name
            InitDefaultPropertiesByFieldName(pFieldName);
        }

        public void InitDefaultColumnProperties(GenericTreeViewColumnProperty pDefaultProperties)
        {
            if (pDefaultProperties != null)
            {
                if (pDefaultProperties.PropertyType != GenericTreeViewColumnPropertyType.Text) PropertyType = pDefaultProperties.PropertyType;
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
                if (pDefaultProperties.FontDescTitle != null) FontDescTitle = pDefaultProperties.FontDescTitle;
                if (pDefaultProperties.FontDesc != null) FontDesc = pDefaultProperties.FontDesc;
                if (pDefaultProperties.CellRenderer != null) CellRenderer = pDefaultProperties.CellRenderer;
            }
        }

        // Detect Field Name and Assign Automatic Properties like Field Code
        public void InitDefaultPropertiesByFieldName(String pProperty)
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
                    // Always apply default FontDesc if not defined by User
                    if (this.FontDesc != null) CellRenderer.FontDesc = this.FontDesc;
                    break;

                case "Designation":
                    MinWidth = 250;
                    MaxWidth = 800;
                    Expand = true;
                    CellRenderer = new CellRendererText()
                    {
                        Alignment = Pango.Alignment.Left,
                    };
                    // Always apply default FontDesc if not defined by User
                    if (this.FontDesc != null) CellRenderer.FontDesc = this.FontDesc;
                    break;

                case "Disabled":
                    CellRenderer = new CellRendererText()
                    {
                        Alignment = Pango.Alignment.Right
                    };
                    // Always apply default FontDesc if not defined by User
                    if (this.FontDesc != null) CellRenderer.FontDesc = this.FontDesc;
                    break;

                default:
                    break;
            }
        }

        public bool Equals(GenericTreeViewColumnProperty other)
        {
            if (this.Name == other.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //NOT USED, because change fields order, but works well if use sort ex.: _columnProperties.Sort() to work
        //Use with [int CodeColumn = _columnProperties.BinarySearch(new TreeViewColumnProperty("Code"));]
        public int CompareTo(GenericTreeViewColumnProperty other)
        {
            if (other == null) return 1;
            return Name.CompareTo(other.Name);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Methods

        /// <summary>
        /// Static Helper Method to Create a DataTable Scheme from ColumnProperties, very usefull to Inialize new Empty DataTables Schemes
        /// </summary>
        /// <param name="pColumnProperties"></param>
        public static DataTable ColumnPropertiesToDataTableScheme(List<GenericTreeViewColumnProperty> pColumnProperties)
        {
            DataTable resultDataTable = new DataTable();
            Type dataTableColumnType = default(Type);

            //Add Columns with specific Types From Column Properties
            foreach (GenericTreeViewColumnProperty column in pColumnProperties)
            {
                dataTableColumnType = (column.Type != null) ? column.Type : typeof(String);
                resultDataTable.Columns.Add(column.Name, dataTableColumnType);
            }

            return resultDataTable;
        }
    }
}
