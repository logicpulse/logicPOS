﻿using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages.GridViews
{
    public static class Columns
    {
        public static TreeViewColumn CreateColumn(string title,
                                                  int sortColumnId,
                                                  TreeCellDataFunc renderFunction,
                                                  CellRenderer cellRenderer = null,
                                                  bool resizable = true,
                                                  bool clickable = true)
        {
            TreeViewColumn column = new TreeViewColumn();
            column.Title = title;
            column.Resizable = resizable;
            column.Clickable = clickable;
            column.SortColumnId = sortColumnId;
            column.SortIndicator = true;
            column.SortOrder = SortType.Descending;

            var label = new Label(column.Title);
            label.ModifyFont(CellRenderers.TitleFont);
            label.Show();
            column.Widget = label;

            cellRenderer = cellRenderer == null ?  CellRenderers.Text() : cellRenderer;
            column.PackStart(cellRenderer, true);

            column.SetCellDataFunc(cellRenderer, renderFunction);

            return column;
        }

        public static TreeViewColumn CreateCodeColumn(int sortColumnId)
        {
            var title = GeneralUtils.GetResourceByName("global_record_code");

            void RenderCode(TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var entity = (IWithCode)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = entity.Code;
            }

            var column = CreateColumn(title,
                                      sortColumnId,
                                      RenderCode,
                                      CellRenderers.Code());
            column.MinWidth = 60;
            column.MaxWidth = 100;

            return column;
        }

       
        public static TreeViewColumn CreateDesignationColumn(int sortColumnId)
        {
            var title = GeneralUtils.GetResourceByName("global_designation");

            void RenderDesignation(TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var entity = (IWithDesignation)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = entity.Designation;
            }

            var column = CreateColumn(title,
                                      sortColumnId,
                                      RenderDesignation);
            column.MinWidth = 250;
            column.MaxWidth = 800;

            return column;
        }


        public static TreeViewColumn CreateUpdatedAtColumn(int sortColumnId)
        {
            void RenderUpdatedAt(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var entity = (ApiEntity)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = entity.UpdatedAt.ToString();
                cell.Xalign = 1;
            }

            var title = GeneralUtils.GetResourceByName("global_record_date_updated");
            var col = CreateColumn(title, sortColumnId, RenderUpdatedAt);
            col.Alignment = 1;
            return col;
        }
    }
}
