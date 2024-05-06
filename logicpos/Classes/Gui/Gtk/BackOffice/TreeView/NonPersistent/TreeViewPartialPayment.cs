using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.shared.App;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewPartialPayment : GenericTreeViewDataTable
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewPartialPayment() { }

        public TreeViewPartialPayment(Window pSourceWindow)
            : this(pSourceWindow, null, null) { }

        //DataTable Mode
        public TreeViewPartialPayment(Window pSourceWindow, DataRow pDefaultValue, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            DataRow defaultValue = (pDefaultValue != null) ? pDefaultValue : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Defaults
            CellRendererText cellRendererCurrency = new CellRendererText() { Alignment = Pango.Alignment.Right, Xalign = 1.0F };
            int decimalsColumnWidth = 100;

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                /*00*/
                new GenericTreeViewColumnProperty("Oid") { Type = typeof(Guid), Visible = false },
                /*01*/
                new GenericTreeViewColumnProperty("Code") { Type = typeof(string), Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_code") },
                /*02*/
                new GenericTreeViewColumnProperty("Designation") { Type = typeof(string), Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_designation"), Expand = true },
                /*03*/
                new GenericTreeViewColumnProperty("PriceFinal") { Type = typeof(decimal), Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_price"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, CellRenderer = cellRendererCurrency },
                /*04*/
                new GenericTreeViewColumnProperty("Vat") { Type = typeof(decimal), Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_vat_rate"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency },
                /*05*/
                new GenericTreeViewColumnProperty("Discount") { Type = typeof(decimal), Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_discount"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency },
                /*06*/
                new GenericTreeViewColumnProperty("Place") { Type = typeof(string), Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_placetable_place") },
                //Other Invisible Fields
                /*07*/
                new GenericTreeViewColumnProperty("Price") { Type = typeof(decimal), Visible = false },
                /*08*/
                new GenericTreeViewColumnProperty("Quantity") { Type = typeof(decimal), Visible = false },
                /*09*/
                new GenericTreeViewColumnProperty("UnitMeasure") { Type = typeof(string), Visible = false },
                /*10*/
                new GenericTreeViewColumnProperty("PlaceOid") { Type = typeof(Guid), Visible = false },
                /*11*/
                new GenericTreeViewColumnProperty("TableOid") { Type = typeof(Guid), Visible = false },
                /*12*/
                new GenericTreeViewColumnProperty("PriceType") { Type = typeof(PriceType), Visible = false },
                /*13*/
                new GenericTreeViewColumnProperty("Token1") { Type = typeof(string), Visible = false },  //ClassifiedID
                /*14*/
                new GenericTreeViewColumnProperty("Token2") { Type = typeof(string), Visible = false },  //FriendlyID
                /*15*/
                new GenericTreeViewColumnProperty("Notes") { Type = typeof(string), Visible = false }
            };

            //init DataTable
            DataTable dataTable = GetDataTable(columnProperties);

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,        //Pass parameter 
              pDefaultValue,        //Pass parameter 
              pGenericTreeViewMode, //Pass parameter 
              pGenericTreeViewNavigatorMode,//Pass parameter 
              columnProperties,     //Created Here
              dataTable,            //Created Here
              typeDialogClass       //Created Here
            );

            //Use this in SelectRecord to format TreeView to Touch
            //this.FormatColumnPropertiesForTouch();
        }

        private DataTable GetDataTable(List<GenericTreeViewColumnProperty> pColumnProperties)
        {
            //Init Local Vars
            DataTable resultDataTable = new DataTable();
            Type dataTableColumnType;
            fin_article article;
            OrderMain orderMain = SharedFramework.SessionApp.OrdersMain[SharedFramework.SessionApp.CurrentOrderMainOid];
            ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(orderMain);
            pos_configurationplace configurationPlace;

            //Add Columns with specific Types From Column Properties
            foreach (GenericTreeViewColumnProperty column in pColumnProperties)
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
                article = (fin_article)DataLayerUtils.GetXPGuidObject(typeof(fin_article), item.Key.ArticleOid);
                if (article.Type.HavePrice)
                {
                    configurationPlace = (pos_configurationplace)DataLayerUtils.GetXPGuidObject(typeof(pos_configurationplace), item.Value.PlaceOid);

                    for (int i = 0; i < item.Value.Quantity; i++)
                    {
                        if (remainQuantity >= 1)
                        {
                            //Column Fields
                            dataRow[0] = item.Key.ArticleOid;
                            dataRow[1] = item.Value.Code;
                            dataRow[2] = item.Key.Designation;
                            dataRow[3] = item.Value.PriceFinal;
                            dataRow[4] = item.Key.Vat;
                            dataRow[5] = item.Key.Discount;
                            dataRow[6] = configurationPlace.Designation;
                            dataRow[7] = item.Key.Price;
                            dataRow[8] = 1;
                            dataRow[9] = item.Value.UnitMeasure;
                            dataRow[10] = item.Value.PlaceOid;
                            dataRow[11] = item.Value.TableOid;
                            dataRow[12] = item.Value.PriceType;
                            dataRow[13] = item.Value.Token1;
                            dataRow[14] = item.Value.Token2;
                            dataRow[15] = string.Empty;
                        }
                        else
                        {
                            //Column Fields
                            dataRow[0] = item.Key.ArticleOid;
                            dataRow[1] = item.Value.Code;
                            dataRow[2] = item.Key.Designation;
                            dataRow[3] = item.Value.PriceFinal * remainQuantity;
                            dataRow[4] = item.Key.Vat;
                            dataRow[5] = item.Key.Discount;
                            dataRow[6] = configurationPlace.Designation;
                            dataRow[7] = item.Key.Price;
                            dataRow[8] = remainQuantity;
                            dataRow[9] = item.Value.UnitMeasure;
                            dataRow[10] = item.Value.PlaceOid;
                            dataRow[11] = item.Value.TableOid;
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
