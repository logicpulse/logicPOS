using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using logicpos.Classes.Enums.GenericTreeView;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewDocumentFinanceArticle : GenericTreeViewDataTable
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceArticle() { }

        public TreeViewDocumentFinanceArticle(Window pSourceWindow)
            : this(pSourceWindow, null, null) { }

        //DataTable Mode
        public TreeViewDocumentFinanceArticle(Window pSourceWindow, DataRow pDefaultValue, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            DataRow defaultValue = (pDefaultValue != null) ? pDefaultValue : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(PosDocumentFinanceArticleDialog);

            //Column Formats
            CellRendererText cellRendererCurrency = new CellRendererText() { Alignment = Pango.Alignment.Right, Xalign = 1.0F };
            int decimalsColumnWidth = 100;

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            /*00*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Oid") { Type = typeof(Guid), Visible = false });
            /*01*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Article.Code")
            {
                Type = typeof(fin_article),
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"),
                ChildName = "Code",
                MinWidth = 60,
                MaxWidth = 150,
                Alignment = 1.0F,
                CellRenderer = new CellRendererText() { Alignment = Pango.Alignment.Right, Xalign = 1.0F, ForegroundGdk = new Gdk.Color(255, 0, 0) }
            });
            /*02*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Article.Designation") { Type = typeof(fin_article), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), ChildName = "Designation", MinWidth = 170, MaxWidth = 170 });
            /*03*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Quantity")
            {
                Type = typeof(Decimal),
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quantity_acronym"),
                MinWidth = 70,
                MaxWidth = 100,
                Alignment = 1.0F,
                InitialValue = 1.0m,
                CellRenderer = new CellRendererText() { Alignment = Pango.Alignment.Right, Xalign = 1.0F, }
            });
            /*04: Used to store DefaultCurrency price, Set visible = true to show it, Default is Hidden */
            columnProperties.Add(new GenericTreeViewColumnProperty("Price") { Type = typeof(Decimal), Title = string.Format("{0}{1}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_price"), "*"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency, Visible = false });
            /*05: Visible Display Value, In Current Selected Currency*/
            columnProperties.Add(new GenericTreeViewColumnProperty("PriceDisplay") { Type = typeof(Decimal), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_price"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency });
            /*06 IN009206*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Discount") { Type = typeof(Decimal), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_discount"), MinWidth = 60, MaxWidth = 60, Alignment = 1.0F, CellRenderer = cellRendererCurrency });
            /*07 IN009206*/
            columnProperties.Add(new GenericTreeViewColumnProperty("VatExemptionReason.Acronym") { Type = typeof(fin_configurationvatexemptionreason), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_exemption_reason_acronym"), ChildName = "Acronym", MinWidth = 60, MaxWidth = 60, Visible = false });
            /*08*/
            columnProperties.Add(new GenericTreeViewColumnProperty("ConfigurationVatRate.Value")
            {
                Type = typeof(fin_configurationvatrate),
                ChildName = "Value",
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_rate"),
                MinWidth = 60,
                MaxWidth = 60,
                Alignment = 1.0F,
                //TODO: Put to Work for SqlServer else appears has 23.0000 not 23.00
                FormatProvider = new FormatterDecimal(),
                CellRenderer = new CellRendererText() { Alignment = Pango.Alignment.Right, Xalign = 1.0F, }
            });
            /*09*/
            columnProperties.Add(new GenericTreeViewColumnProperty("TotalNet") { Type = typeof(Decimal), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_article_tab"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency });
            /*10*/ /* IN009206 */
            columnProperties.Add(new GenericTreeViewColumnProperty("TotalFinal") { Type = typeof(Decimal), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_per_item_vat"), MinWidth = decimalsColumnWidth, MaxWidth = decimalsColumnWidth, Alignment = 1.0F, CellRenderer = cellRendererCurrency });
            //Other Invisible Fields
            /*11*/
            columnProperties.Add(new GenericTreeViewColumnProperty("PriceFinal") { Type = typeof(Decimal), Visible = false });
            /*12*/
            columnProperties.Add(new GenericTreeViewColumnProperty("PriceType") { Type = typeof(PriceType), Visible = false });
            /*13*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Token1") { Type = typeof(string), Visible = false });  //MediaNova:ClassifiedID
            /*14*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Token2") { Type = typeof(string), Visible = false });  //MediaNova:FriendlyID
            /*15*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Notes") { Type = typeof(string), Visible = false });

            //init DataTable
            DataTable dataTable = GetDataTable(columnProperties, false);

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,                //Pass parameter 
              pDefaultValue,                //Pass parameter 
              pGenericTreeViewMode,         //Pass parameter 
              pGenericTreeViewNavigatorMode,//Pass parameter 
              columnProperties,             //Created Here
              dataTable,                    //Created Here
              typeDialogClass               //Created Here
            );
        }

        private DataTable GetDataTable(List<GenericTreeViewColumnProperty> pColumnProperties, bool pGetArticlesFromCurrentOrderMain)
        {
            //Get a New DataTable Scheme Ready to Fill with Rows
            DataTable resultDataTable = GenericTreeViewColumnProperty.ColumnPropertiesToDataTableScheme(pColumnProperties);

            //Usefull to Tests
            if (pGetArticlesFromCurrentOrderMain)
            {
                //Check if we have a Valid Session Order
                if (GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp.CurrentOrderMainOid))
                {
                    //Init Local Vars
                    fin_article article;
                    fin_configurationvatrate configurationVatRate;
                    //WIP: ConfigurationUnitMeasure configurationUnitMeasure;
                    OrderMain orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                    ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(orderMain);

                    //Init DataRow
                    System.Object[] dataRow = new System.Object[pColumnProperties.Count];

                    //Start Loop
                    foreach (var item in articleBag)
                    {
                        //Get XPGuidObjects to Assign to Columns
                        article = (fin_article)FrameworkUtils.GetXPGuidObject(typeof(fin_article), item.Key.ArticleOid);
                        configurationVatRate = (fin_configurationvatrate)FrameworkUtils.GetXPGuidObject(typeof(fin_configurationvatrate),
                          FrameworkUtils.GetGuidFromQuery(string.Format(@"SELECT Oid FROM fin_configurationvatrate WHERE (Disabled IS NULL OR Disabled  <> 1) AND Value = '{0}';", item.Key.Vat))
                        );
                        //WIP: configurationUnitMeasure = (ConfigurationUnitMeasure)FrameworkUtils.GetXPGuidObjectFromSession(typeof(ConfigurationUnitMeasure), 
                        //  FrameworkUtils.GetGuidFromQuery(string.Format(@"SELECT Oid FROM cfg_configurationunitmeasure WHERE (Disabled IS NULL OR Disabled  <> 1) AND Acronym = '{0}';", item.Value.UnitMeasure))
                        //);

                        //Column Fields
                        dataRow[0] = item.Key.ArticleOid;
                        dataRow[1] = article;                   //Article.Code
                        dataRow[2] = article;                   //Article.Designation
                        dataRow[3] = item.Value.Quantity;
                        //dataRow[4] = configurationUnitMeasure;// ConfigurationUnitMeasure.Acronym
                        dataRow[4] = item.Key.Price;
                        dataRow[5] = null;                      //PriceDisplay: Show/Work in Currency ex USD
                        dataRow[6] = configurationVatRate;      // ConfigurationVatRate.Value
                        //To Store VatExemptionReason : Get Value From ArticleBag, When Used in Tickets
                        dataRow[7] = null;                      // VatExemptionReason.Designation
                        dataRow[8] = item.Key.Discount;
                        //dataRow[9] = item.Value.TotalGross;
                        dataRow[9] = item.Value.TotalNet;
                        dataRow[10] = item.Value.TotalFinal;
                        //Invisible Fields
                        dataRow[11] = item.Value.PriceFinal;
                        dataRow[12] = item.Value.PriceType;
                        dataRow[13] = string.Empty;             //Token1
                        dataRow[14] = string.Empty;             //Token2
                        dataRow[15] = string.Empty;             //Notes
                        //Add Row
                        resultDataTable.Rows.Add(dataRow);
                    }
                }
            }
            return resultDataTable;
        }
    }
}
