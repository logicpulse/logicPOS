using Gtk;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.Orders;
using LogicPOS.UI.Components;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewPartialPayment : GridViewDataTable
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewPartialPayment() { }

        public TreeViewPartialPayment(Window parentWindow)
            : this(parentWindow, null, null) { }

        //DataTable Mode
        public TreeViewPartialPayment(Window parentWindow, DataRow pDefaultValue, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            DataRow defaultValue = (pDefaultValue != null) ? pDefaultValue : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Defaults
            CellRendererText cellRendererCurrency = new CellRendererText() { Alignment = Pango.Alignment.Right, Xalign = 1.0F };
            int decimalsColumnWidth = 100;

            //Configure columnProperties
            List<GridViewColumn> columnProperties = new List<GridViewColumn>
            {
                /*00*/
                new GridViewColumn("Oid") { Type = typeof(Guid), Visible = false },
                /*01*/
                new GridViewColumn("Code") { Type = typeof(string), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code") },
                /*02*/
                new GridViewColumn("Designation") { Type = typeof(string), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                /*03*/
                new GridViewColumn("PriceFinal") { Type = typeof(decimal), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_price"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, CellRenderer = cellRendererCurrency },
                /*04*/
                new GridViewColumn("Vat") { Type = typeof(decimal), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_vat_rate"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency },
                /*05*/
                new GridViewColumn("Discount") { Type = typeof(decimal), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_discount"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency },
                /*06*/
                new GridViewColumn("Place") { Type = typeof(string), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_placetable_place") },
                //Other Invisible Fields
                /*07*/
                new GridViewColumn("Price") { Type = typeof(decimal), Visible = false },
                /*08*/
                new GridViewColumn("Quantity") { Type = typeof(decimal), Visible = false },
                /*09*/
                new GridViewColumn("UnitMeasure") { Type = typeof(string), Visible = false },
                /*10*/
                new GridViewColumn("PlaceOid") { Type = typeof(Guid), Visible = false },
                /*11*/
                new GridViewColumn("TableOid") { Type = typeof(Guid), Visible = false },
                /*12*/
                new GridViewColumn("PriceType") { Type = typeof(PriceType), Visible = false },
                /*13*/
                new GridViewColumn("Token1") { Type = typeof(string), Visible = false },  //ClassifiedID
                /*14*/
                new GridViewColumn("Token2") { Type = typeof(string), Visible = false },  //FriendlyID
                /*15*/
                new GridViewColumn("Notes") { Type = typeof(string), Visible = false }
            };

            //init DataTable
            DataTable dataTable = GetDataTable(columnProperties);

            //Call Base Initializer
            base.InitObject(
              parentWindow,        //Pass parameter 
              pDefaultValue,        //Pass parameter 
              pGenericTreeViewMode, //Pass parameter 
              navigatorMode,//Pass parameter 
              columnProperties,     //Created Here
              dataTable,            //Created Here
              typeDialogClass       //Created Here
            );
        }

        private DataTable GetDataTable(List<GridViewColumn> pColumnProperties)
        {
            //Init Local Vars
            DataTable resultDataTable = new DataTable();
            Type dataTableColumnType;
            fin_article article;
            OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(orderMain);
            pos_configurationplace configurationPlace;

            //Add Columns with specific Types From Column Properties
            foreach (GridViewColumn column in pColumnProperties)
            {
                dataTableColumnType = (column.Type != null) ? column.Type : typeof(string);
                resultDataTable.Columns.Add(column.Name, dataTableColumnType);
            }

            //Init DataRow
            object[] dataRow = new object[pColumnProperties.Count];

            //Start Loop
            foreach (var item in articleBag)
            {
                //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
                decimal remainQuantity = item.Value.Quantity;
                article = XPOUtility.GetEntityById<fin_article>(item.Key.ArticleId);
                if (article.Type.HavePrice)
                {
                    configurationPlace = XPOUtility.GetEntityById<pos_configurationplace>(item.Value.PlaceId);

                    for (int i = 0; i < item.Value.Quantity; i++)
                    {
                        if (remainQuantity >= 1)
                        {
                            //Column Fields
                            dataRow[0] = item.Key.ArticleId;
                            dataRow[1] = item.Value.Code;
                            dataRow[2] = item.Key.Designation;
                            dataRow[3] = item.Value.PriceFinal;
                            dataRow[4] = item.Key.Vat;
                            dataRow[5] = item.Key.Discount;
                            dataRow[6] = configurationPlace.Designation;
                            dataRow[7] = item.Key.Price;
                            dataRow[8] = 1;
                            dataRow[9] = item.Value.UnitMeasure;
                            dataRow[10] = item.Value.PlaceId;
                            dataRow[11] = item.Value.TableId;
                            dataRow[12] = item.Value.PriceType;
                            dataRow[13] = item.Value.Token1;
                            dataRow[14] = item.Value.Token2;
                            dataRow[15] = string.Empty;
                        }
                        else
                        {
                            //Column Fields
                            dataRow[0] = item.Key.ArticleId;
                            dataRow[1] = item.Value.Code;
                            dataRow[2] = item.Key.Designation;
                            dataRow[3] = item.Value.PriceFinal * remainQuantity;
                            dataRow[4] = item.Key.Vat;
                            dataRow[5] = item.Key.Discount;
                            dataRow[6] = configurationPlace.Designation;
                            dataRow[7] = item.Key.Price;
                            dataRow[8] = remainQuantity;
                            dataRow[9] = item.Value.UnitMeasure;
                            dataRow[10] = item.Value.PlaceId;
                            dataRow[11] = item.Value.TableId;
                            dataRow[12] = item.Value.PriceType;
                            dataRow[13] = item.Value.Token1;
                            dataRow[14] = item.Value.Token2;
                            dataRow[15] = string.Empty;
                        }
                        //Add Row
                        resultDataTable.Rows.Add(dataRow);
                        remainQuantity--;
                    }
                }
            }
            return resultDataTable;
        }
    }
}
