using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.financial.library.Classes.Stocks;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
	//Gestão de Stocks : Janela de Gestão de Stocks [IN:016534]
    class TreeViewArticleDetailsStock : GenericTreeViewXPO
    {
        XPCollection xpoCollection;
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleDetailsStock() { }

        public TreeViewArticleDetailsStock(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewArticleDetailsStock(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_article);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_article defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_article : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogArticle);
            CellRendererText StockCellRenderer = new CellRendererText()
            {
                //FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                //Alignment = Pango.Alignment.Right,
                //Xalign = 1.0F,
                Editable = true,
            };
            StockCellRenderer.Edited += TreeViewArticleDetailsStock_Edited;
            CellRendererText MinStockCellRender = new CellRendererText()
            {
                //FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                //Alignment = Pango.Alignment.Right,
                //Xalign = 1.0F,
                Editable = true,                
            };
            MinStockCellRender.Edited += MinStockCellRender_Edited;
            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(GlobalFramework.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();

            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article_code"), MinWidth = 100 });

            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true });
            columnProperties.Add(new GenericTreeViewColumnProperty("Accounting")
            {
                Query = "SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;",
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_stock"),
                MinWidth = 100,
                //Alignment = 1.0F,
                FormatProvider = new FormatterDecimal(),
                CellRenderer = StockCellRenderer

            });


            columnProperties.Add(new GenericTreeViewColumnProperty("MinimumStock")
            {
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_minimum_stock"),
                MinWidth = 100,
                //Alignment = 1.0F,
                FormatProvider = new FormatterDecimal(),
                CellRenderer = MinStockCellRender
                //CellRenderer = new CellRendererText()
                //{
                //    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                //    Alignment = Pango.Alignment.Right,
                //    Xalign = 1.0F
                //}
            });

            columnProperties.Add(new GenericTreeViewColumnProperty("UnitMeasure") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_unit_measure"), ChildName = "Designation", Expand = true });

            columnProperties.Add(new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 });


            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");

            CriteriaOperator criteria = pXpoCriteria;
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria.ToString()}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }

            SortProperty[] sortProperty = new SortProperty[2];
            sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Ascending);
            sortProperty[1] = new SortProperty("Ord", SortingDirection.Ascending);
            xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria, sortProperty);

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              pGenericTreeViewNavigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );
        }

        private void MinStockCellRender_Edited(object o, EditedArgs args)
        {
            try
            {                
                if (_dataSourceRow != null && (_dataSourceRow as fin_article).MinimumStock != Convert.ToDecimal(args.NewText))
                {
                    (_dataSourceRow as fin_article).MinimumStock = Convert.ToDecimal(args.NewText);
                    (_dataSourceRow as fin_article).Save();
                    xpoCollection.Reload();
                    this.Refresh();
                }
            }
            catch (Exception ex) { }
        }

        private void TreeViewArticleDetailsStock_Edited(object o, EditedArgs args)
        {
            try
            {
                fin_article article = (fin_article)xpoCollection.BaseIndexer(Convert.ToInt32(args.Path));
                if (article.Accounting != Convert.ToDecimal(args.NewText))
                {
                    //TODO Check for Minimum stock
                    //try
                    //{
                    //    
                    //    bool showMessage;
                    //    if (GlobalFramework.CheckStocks)
                    //    {

                    //        if (!Utils.ShowMessageMinimumStock(_sourceWindow, article.Oid, Convert.ToDecimal(args.NewText), out showMessage))
                    //        {
                    //            if (showMessage)
                    //            {
                    //                return;
                    //            }
                    //        }
                    //    }

                    //}
                    //catch (Exception ex)
                    //{
                    //    _log.Error("Error updating stock quantity" + ex.Message, ex);
                    //    return;
                    //}

                    //fin_article article = (fin_article)xpoCollection.BaseIndexer(Convert.ToInt32(args.Path));
                    (_dataSourceRow as fin_article).Accounting = Convert.ToDecimal(args.NewText);
                    (_dataSourceRow as fin_article).Save();
                    string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", (_dataSourceRow as fin_article).Oid);
                    decimal getArticleStock = Convert.ToDecimal(_dataSourceRow.Session.ExecuteScalar(stockQuery).ToString());
                    if (getArticleStock != (_dataSourceRow as fin_article).Accounting)
                    {
                        var own_customer = (erp_customer)GlobalFramework.SessionXpo.GetObjectByKey(typeof(erp_customer), SettingsApp.XpoOidUserRecord);
                        if (own_customer != null)
                        {
                            if (string.IsNullOrEmpty(own_customer.Name))
                            {
                                //update owner customer for internal stock moviments
                                own_customer.FiscalNumber = GlobalFramework.PreferenceParameters["COMPANY_FISCALNUMBER"];
                                own_customer.Name = GlobalFramework.PreferenceParameters["COMPANY_NAME"];
                                own_customer.Save();
                            }
                        }
                        if ((_dataSourceRow as fin_article).Accounting > getArticleStock)
                        {
                            decimal quantity = (_dataSourceRow as fin_article).Accounting - getArticleStock;
                            ProcessArticleStock.Add(datalayer.Enums.ProcessArticleStockMode.In, own_customer, 1, DateTime.Now, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer1"), (_dataSourceRow as fin_article), quantity, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer1"));
                        }
                        else
                        {
                            decimal quantity = getArticleStock - (_dataSourceRow as fin_article).Accounting;
                            ProcessArticleStock.Add(datalayer.Enums.ProcessArticleStockMode.Out, own_customer, 1, DateTime.Now, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer1"), (_dataSourceRow as fin_article), quantity, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer1"));
                        }
                    }
                    xpoCollection.Reload();
                    this.Refresh();
                }
            }
            //New article
            catch
            {
                ProcessArticleStock.Add(datalayer.Enums.ProcessArticleStockMode.In, null, 1, DateTime.Now, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer1"), (_dataSourceRow as fin_article), (_dataSourceRow as fin_article).Accounting, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer1"));
                xpoCollection.Reload();
                this.Refresh();
            }
        }
    }
}
