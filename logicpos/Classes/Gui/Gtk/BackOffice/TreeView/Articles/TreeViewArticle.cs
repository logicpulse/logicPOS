using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewArticle : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticle() { }

        public TreeViewArticle(Window pSourceWindow)
            : this(pSourceWindow, null, null, null, GenericTreeViewMode.Default, GenericTreeViewNavigatorMode.Default) { }

        //XpoMode
        public TreeViewArticle(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(FIN_Article);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            FIN_Article defaultValue = (pDefaultValue != null) ? pDefaultValue as FIN_Article : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogArticle);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Title = Resx.global_record_code, MinWidth = 100 });
            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Title = Resx.global_designation, MinWidth = 200 });
            //To Test XPGuidObject InitialValue (InitialValue = xArticleFamily) : ArticleFamily xArticleFamily = (ArticleFamily)FrameworkUtils.GetXPGuidObjectFromSession(GlobalFramework.SessionXpoBackoffice, typeof(ArticleFamily), new Guid("471d8c1e-45c1-4dbe-8526-349c20bd53ef"));
            columnProperties.Add(new GenericTreeViewColumnProperty("Family") { Title = Resx.global_article_family, ChildName = "Designation" });
            columnProperties.Add(new GenericTreeViewColumnProperty("SubFamily") { Title = Resx.global_article_subfamily, ChildName = "Designation" });
            columnProperties.Add(new GenericTreeViewColumnProperty("Type") { Title = Resx.global_article_type, ChildName = "Designation" });
            //columnProperties.Add(new GenericTreeViewColumnProperty("Disabled") { Title = Resx.global_record_disabled });
            columnProperties.Add(new GenericTreeViewColumnProperty("TotalStock")
            {
                Query = "SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;",
                Title = Resx.global_total_stock,
                MinWidth = 100,
                Alignment = 1.0F,
                FormatProvider = new FormatterDecimal(),
                CellRenderer = new CellRendererText()
                {
                    Alignment = Pango.Alignment.Right,
                    FontDesc = new Pango.FontDescription() { Size = 50 },
                    Xalign = 1.0F
                }
            });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria);

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
    }
}
