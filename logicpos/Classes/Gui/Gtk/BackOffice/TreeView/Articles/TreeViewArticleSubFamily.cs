using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using System;
using System.Collections.Generic;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewArticleSubFamily : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleSubFamily() { }

        [Obsolete]
        public TreeViewArticleSubFamily(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewArticleSubFamily(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_articlesubfamily);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_articlesubfamily defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_articlesubfamily : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogArticleSubFamily);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Code") { Title = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_code"), MinWidth = 100 },
                new GenericTreeViewColumnProperty("Designation") { Title = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("Family") { Title = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article_family"), ChildName = "Designation" },
                new GenericTreeViewColumnProperty("Printer") { Title = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_device_printer"), ChildName = "Designation" },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria;
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);

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
