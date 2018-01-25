using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewPartialPayment : GenericTreeViewDataTable
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
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            /*00*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Oid") { Type = typeof(Guid), Visible = false });
            /*01*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Type = typeof(String), Title = Resx.global_record_code });
            /*02*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Type = typeof(String), Title = Resx.global_designation, MinWidth = 200, MaxWidth = 200 });
            /*03*/
            columnProperties.Add(new GenericTreeViewColumnProperty("PriceFinal") { Type = typeof(Decimal), Title = Resx.global_price, MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, CellRenderer = cellRendererCurrency });
            /*04*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Vat") { Type = typeof(Decimal), Title = Resx.global_vat_rate, MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency });
            /*05*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Discount") { Type = typeof(Decimal), Title = Resx.global_discount, MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency });
            /*06*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Place") { Type = typeof(String), Title = Resx.global_placetable_place });
            //Other Invisible Fields
            /*07*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Price") { Type = typeof(Decimal), Visible = false });
            /*08*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Quantity") { Type = typeof(Int16), Visible = false });
            /*09*/
            columnProperties.Add(new GenericTreeViewColumnProperty("UnitMeasure") { Type = typeof(String), Visible = false });
            /*10*/
            columnProperties.Add(new GenericTreeViewColumnProperty("PlaceOid") { Type = typeof(Guid), Visible = false });
            /*11*/
            columnProperties.Add(new GenericTreeViewColumnProperty("TableOid") { Type = typeof(Guid), Visible = false });
            /*12*/
            columnProperties.Add(new GenericTreeViewColumnProperty("PriceType") { Type = typeof(PriceType), Visible = false });
            /*13*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Token1") { Type = typeof(string), Visible = false });  //ClassifiedID
            /*14*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Token2") { Type = typeof(string), Visible = false });  //FriendlyID

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
            FIN_Article article;
            OrderMain orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
            ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(orderMain);
            POS_ConfigurationPlace configurationPlace;

            //Add Columns with specific Types From Column Properties
            foreach (GenericTreeViewColumnProperty column in pColumnProperties)
            {
                dataTableColumnType = (column.Type != null) ? column.Type : typeof(String);
                resultDataTable.Columns.Add(column.Name, dataTableColumnType);
            }

            //Init DataRow
            System.Object[] dataRow = new System.Object[pColumnProperties.Count];

            //Start Loop
            foreach (var item in articleBag)
            {
                article = (FIN_Article)FrameworkUtils.GetXPGuidObject(typeof(FIN_Article), item.Key.ArticleOid);
                if (article.Type.HavePrice)
                {
                    configurationPlace = (POS_ConfigurationPlace)FrameworkUtils.GetXPGuidObject(typeof(POS_ConfigurationPlace), item.Value.PlaceOid);

                    for (int i = 0; i < item.Value.Quantity; i++)
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

                        //Add Row
                        resultDataTable.Rows.Add(dataRow);
                    }
                }
            }
            return resultDataTable;
        }
    }
}
