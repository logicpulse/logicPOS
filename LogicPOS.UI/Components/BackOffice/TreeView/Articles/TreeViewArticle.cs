using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Formatters;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewArticle : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticle() { }

        [Obsolete]
        public TreeViewArticle(Window parentWindow)
            : this(parentWindow, null, null, null, GridViewMode.Default, GridViewNavigatorMode.Default) { }

        //XpoMode
        [Obsolete]
        public TreeViewArticle(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_article);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_article defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_article : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogArticle);

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(AppSettings.Instance.fontGenericTreeViewColumn);

            //Configure columnProperties
            List<GridViewColumnProperty> columnProperties = new List<GridViewColumnProperty>
            {
                new GridViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                new GridViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GridViewColumnProperty("TotalStock")
                {
                    Query = "SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;",
                    Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_stock"),
                    MinWidth = 100,
                    //Alignment = 1.0F,
                    FormatProvider = new DecimalFormatter(),
                    //CellRenderer = new CellRendererText()
                    //{
                    //    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                    //    Alignment = Pango.Alignment.Right,
                    //    Xalign = 1.0F
                    //}
                },
                //To Test XPGuidObject InitialValue (InitialValue = xArticleFamily) : ArticleFamily xArticleFamily = (ArticleFamily)XPOUtility.GetXPGuidObjectFromSession(XPOSettings.SessionBackoffice, typeof(ArticleFamily), new Guid("471d8c1e-45c1-4dbe-8526-349c20bd53ef"));
                new GridViewColumnProperty("IsComposed") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_composite_article") },
                //Artigos Compostos [IN:016522]
                new GridViewColumnProperty("Family") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_family"), ChildName = "Designation" },
                new GridViewColumnProperty("SubFamily") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_subfamily"), ChildName = "Designation" },
                new GridViewColumnProperty("Type") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_article_type"), ChildName = "Designation" },
                new GridViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model
            //Default Criteria with XpoOidUndefinedRecord
            CriteriaOperator criteria;
            // Override Criteria adding XpoOidHiddenRecordsFilter
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            //Custom Criteria hidding all Hidden Oids
            //CriteriaOperator criteria = CriteriaOperator.Parse($"(Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR Oid NOT LIKE '{SettingsApp.XpoOidHiddenRecordsFilter}')");
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);

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
    }
}
