using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.Classes.Formatters;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Modules.StockManagement;
using LogicPOS.UI.Components;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    //Gestão de Stocks : Janela de Gestão de Stocks [IN:016534]
    internal class TreeViewArticleDetailsStock : XpoGridView
    {
        private readonly XPCollection xpoCollection;
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleDetailsStock() { }

        [Obsolete]
        public TreeViewArticleDetailsStock(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewArticleDetailsStock(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
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
            int fontGenericTreeViewColumn = Convert.ToInt16(LogicPOS.Settings.AppSettings.Instance.fontGenericTreeViewColumn);

            //Configure columnProperties
            List<GridViewColumn> columnProperties = new List<GridViewColumn>
            {
                new GridViewColumn("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_article_code"), MinWidth = 100 },
                new GridViewColumn("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GridViewColumn("Accounting")
                {
                    Query = "SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;",
                    Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_total_stock"),
                    MinWidth = 100,
                    //Alignment = 1.0F,
                    FormatProvider = new DecimalFormatter(),
                    CellRenderer = StockCellRenderer

                },
                new GridViewColumn("MinimumStock")
                {
                    Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_minimum_stock"),
                    MinWidth = 100,
                    //Alignment = 1.0F,
                    FormatProvider = new DecimalFormatter(),
                    CellRenderer = MinStockCellRender
                },
                new GridViewColumn("UnitMeasure") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_unit_measure"), ChildName = "Designation", Expand = true },
                new GridViewColumn("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };


            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");

            CriteriaOperator criteria = pXpoCriteria;
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }

            SortProperty[] sortProperty = new SortProperty[2];
            sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Ascending);
            sortProperty[1] = new SortProperty("Ord", SortingDirection.Ascending);
            xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria, sortProperty);

            //Call Base Initializer
            base.InitObject(
              parentWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              navigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );
        }

        private void MinStockCellRender_Edited(object o, EditedArgs args)
        {
            try
            {
                if (Entity != null && (Entity as fin_article).MinimumStock != Convert.ToDecimal(args.NewText))
                {
                    (Entity as fin_article).MinimumStock = Convert.ToDecimal(args.NewText);
                    (Entity as fin_article).Save();
                    xpoCollection.Reload();
                    this.Refresh();
                }
            }
            catch (Exception) { }
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
                    //    _logger.Error("Error updating stock quantity" + ex.Message, ex);
                    //    return;
                    //}

                    //fin_article article = (fin_article)xpoCollection.BaseIndexer(Convert.ToInt32(args.Path));
                    (Entity as fin_article).Accounting = Convert.ToDecimal(args.NewText);
                    (Entity as fin_article).Save();
                    string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", (Entity as fin_article).Oid);
                    decimal getArticleStock = Convert.ToDecimal(Entity.Session.ExecuteScalar(stockQuery).ToString());
                    if (getArticleStock != (Entity as fin_article).Accounting)
                    {
                        var own_customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), XPOSettings.XpoOidUserRecord);
                        if (own_customer != null)
                        {
                            if (string.IsNullOrEmpty(own_customer.Name))
                            {
                                //update owner customer for internal stock moviments
                                own_customer.FiscalNumber = LogicPOS.Settings.GeneralSettings.PreferenceParameters["COMPANY_FISCALNUMBER"];
                                own_customer.Name = LogicPOS.Settings.GeneralSettings.PreferenceParameters["COMPANY_NAME"];
                                own_customer.Save();
                            }
                        }
                        if ((Entity as fin_article).Accounting > getArticleStock)
                        {
                            decimal quantity = (Entity as fin_article).Accounting - getArticleStock;
                            ProcessArticleStock.Add(ProcessArticleStockMode.In, own_customer, 1, DateTime.Now, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_internal_document_footer1"), (Entity as fin_article), quantity, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_internal_document_footer1"));
                        }
                        else
                        {
                            decimal quantity = getArticleStock - (Entity as fin_article).Accounting;
                            ProcessArticleStock.Add(ProcessArticleStockMode.Out, own_customer, 1, DateTime.Now, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_internal_document_footer1"), (Entity as fin_article), quantity, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_internal_document_footer1"));
                        }
                    }
                    xpoCollection.Reload();
                    this.Refresh();
                }
            }
            //New article
            catch
            {
                ProcessArticleStock.Add(ProcessArticleStockMode.In, null, 1, DateTime.Now, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_internal_document_footer1"), (Entity as fin_article), (Entity as fin_article).Accounting, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_internal_document_footer1"));
                xpoCollection.Reload();
                this.Refresh();
            }
        }
    }
}
