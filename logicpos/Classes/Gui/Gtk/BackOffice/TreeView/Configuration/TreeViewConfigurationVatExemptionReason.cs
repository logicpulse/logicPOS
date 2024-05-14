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
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewConfigurationVatExceptionReason : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewConfigurationVatExceptionReason() { }

        [Obsolete]
        public TreeViewConfigurationVatExceptionReason(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewConfigurationVatExceptionReason(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_configurationvatexemptionreason);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_configurationvatexemptionreason defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_configurationvatexemptionreason : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogConfigurationVatExemptionReason);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                new GenericTreeViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("Acronym") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_vat_rate") },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
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

            //Protected Records
            ProtectedRecords.Add(InvoiceSettings.XpoOidConfigurationVatExemptionReasonM99);//M99
        }
    }
}
