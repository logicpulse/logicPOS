using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewDocumentFinancePayment : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinancePayment() { }

        public TreeViewDocumentFinancePayment(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinancePayment(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(FIN_DocumentFinancePayment);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            FIN_DocumentFinancePayment defaultValue = (pDefaultValue != null) ? pDefaultValue as FIN_DocumentFinancePayment : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(GlobalFramework.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("CreatedAt") { Title = Resx.global_document_date, MinWidth = 180 });
            columnProperties.Add(new GenericTreeViewColumnProperty("PaymentRefNo") { Title = Resx.global_document_number, MinWidth = 180 });
            columnProperties.Add(new GenericTreeViewColumnProperty("PaymentStatus") { Title = Resx.global_document_status, MinWidth = 50, MaxWidth = 50 });
            //Shared Query
            string query = "SELECT {0} as Result FROM view_documentfinancepayment WHERE fpaOid = '{1}' GROUP BY fpaOid;";
            columnProperties.Add(new GenericTreeViewColumnProperty("EntityName") { 
                Title = Resx.global_entity, MinWidth = 260, MaxWidth = 260,
                Query = string.Format(query, "cusName", "{0}"),
                DecryptValue = true
            });
            columnProperties.Add(new GenericTreeViewColumnProperty("EntityFiscalNumber") { 
                Title = Resx.global_fiscal_number, MinWidth = 70, MaxWidth = 120 ,
                Query = string.Format(query, "cusFiscalNumber", "{0}"),
                DecryptValue = true
            });
            columnProperties.Add(new GenericTreeViewColumnProperty("PaymentAmount")
            {
                Title = Resx.global_total,
                MinWidth = 100,
                //Alignment = 1.0F,
                FormatProvider = new FormatterDecimalCurrency(),
                //CellRenderer = new CellRendererText()
                //{
                //    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                //    Alignment = Pango.Alignment.Right,
                //    Xalign = 1.0F
                //}
            });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
            SortProperty[] sortProperty = new SortProperty[1];
            sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Ascending);
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria, sortProperty);

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
