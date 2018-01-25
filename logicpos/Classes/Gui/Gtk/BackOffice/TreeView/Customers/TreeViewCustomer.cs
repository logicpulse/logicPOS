using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewCustomer : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewCustomer() { }

        public TreeViewCustomer(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewCustomer(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(ERP_Customer);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            ERP_Customer defaultValue = (pDefaultValue != null) ? pDefaultValue as ERP_Customer : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogCustomer);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Title = Resx.global_record_code });
            columnProperties.Add(new GenericTreeViewColumnProperty("Name") { Title = Resx.global_name, MinWidth = 200 });
            columnProperties.Add(new GenericTreeViewColumnProperty("FiscalNumber") { Title = Resx.global_fiscal_number, MinWidth = 150 });
            columnProperties.Add(new GenericTreeViewColumnProperty("CardNumber") { Title = Resx.global_card_number, MinWidth = 150 });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria);

            //Custom Events
            //WIP: this.CursorChanged += TreeViewCustomer_CursorChanged;

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

            //Protected Records
            ProtectedRecords.Add(SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity);//FinalConsumerEntity
        }
    }
}