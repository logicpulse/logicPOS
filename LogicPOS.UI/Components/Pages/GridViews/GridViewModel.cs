using Gtk;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components
{

    internal abstract class GridViewModel
    {
        private static readonly bool _showSystemColumns = false;

        public static void DumpColumnValues(ListStore store, int col)
        {
            foreach (object[] row in store) Console.WriteLine("Value of column {0} is {1}", col, row[col]);
        }


        public static ListStore InitModel(List<GridViewColumn> pColumnProperties, GridViewMode pGenericTreeViewMode)
        {
            List<GridViewColumn> _columnProperties = pColumnProperties;
            GridViewMode _treeViewMode = pGenericTreeViewMode;


            bool addSystemColumns = _columnProperties[0].Name != "RowIndex";

            if (addSystemColumns)
            {
                _columnProperties.Insert(0, new GridViewColumn("RowIndex") { Type = typeof(int), Visible = _showSystemColumns });

                bool hasOid = _columnProperties.Contains(new GridViewColumn("Oid"));

                switch (_treeViewMode)
                {
                    case GridViewMode.Default:
                        if (!hasOid) _columnProperties.Insert(1, new GridViewColumn("Oid") { Visible = _showSystemColumns });
                        break;
                    case GridViewMode.CheckBox:
                        _columnProperties.Insert(1, new GridViewColumn("RowCheckBox") { Title = string.Empty, PropertyType = GridViewPropertyType.CheckBox, MinWidth = 50, MaxWidth = 50 });
                        if (!hasOid) _columnProperties.Insert(2, new GridViewColumn("Oid") { Visible = _showSystemColumns });
                        break;
                    default:
                        break;
                }
            }

            Type[] columnTypes = new Type[_columnProperties.Count];


            for (int i = 0; i < _columnProperties.Count; i++)
            {
                if (_columnProperties[i].PropertyType == GridViewPropertyType.CheckBox)
                {
                    columnTypes[i] = typeof(bool);
                }
                else
                {
                    if (_columnProperties[i].Name == "RowIndex")
                    {
                        columnTypes[i] = _columnProperties[i].Type;
                    }
                    else
                    {
                        columnTypes[i] = typeof(string);
                    }
                };
            };

            ListStore resultListStore = new ListStore(columnTypes);

            return resultListStore;
        }
    }
}
