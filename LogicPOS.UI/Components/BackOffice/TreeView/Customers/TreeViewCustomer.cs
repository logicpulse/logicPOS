using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewCustomer : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewCustomer() { }

        [Obsolete]
        public TreeViewCustomer(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewCustomer(Window pSourceWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(erp_customer);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            erp_customer defaultValue = (pDefaultValue != null) ? pDefaultValue as erp_customer : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogCustomer);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                new GenericTreeViewColumnProperty("Name") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_name"), MinWidth = 200, Expand = true, DecryptValue = true },
                new GenericTreeViewColumnProperty("FiscalNumber") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_fiscal_number"), MinWidth = 150, DecryptValue = true },
                new GenericTreeViewColumnProperty("CardNumber") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_card_number"), MinWidth = 150 },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
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
            /* IN009162 - customer sorting changes when "PosDocumentFinanceDialog" window */
            string sortedColumn = "Ord"; // Default one
            if ("PosDocumentFinanceDialog".Equals(pSourceWindow.GetType().Name))
            {
                sortedColumn = "UpdatedAt";
            }

            SortProperty sortPropertyForCustomer = new SortProperty(sortedColumn, DevExpress.Xpo.DB.SortingDirection.Descending);
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria, sortPropertyForCustomer);

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
            ProtectedRecords.Add(InvoiceSettings.FinalConsumerId);//FinalConsumerEntity
        }
    }
}
