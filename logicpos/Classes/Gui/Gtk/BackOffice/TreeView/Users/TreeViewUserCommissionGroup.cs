using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Formatters;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewUserCommissionGroup : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewUserCommissionGroup() { }

        public TreeViewUserCommissionGroup(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewUserCommissionGroup(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(POS_UserCommissionGroup);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            POS_UserCommissionGroup defaultValue = (pDefaultValue != null) ? pDefaultValue as POS_UserCommissionGroup : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogUserCommissionGroup);

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(GlobalFramework.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Title = Resx.global_record_code, MinWidth = 100 });
            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Title = Resx.global_designation, Expand = true });
            columnProperties.Add(new GenericTreeViewColumnProperty("Commission")
            {
                Title = Resx.global_commission,
                MinWidth = 100,
                //Alignment = 1.0F,
                FormatProvider = new FormatterDecimal(),
                //CellRenderer = new CellRendererText()
                //{
                //    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                //    Alignment = Pango.Alignment.Right,
                //    Xalign = 1.0F
                //}
            });
            columnProperties.Add(new GenericTreeViewColumnProperty("UpdatedAt") { Title = Resx.global_record_date_updated, MinWidth = 150, MaxWidth = 150 });

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
