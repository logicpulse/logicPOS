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

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewDocumentFinanceMaster : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceMaster() { }

        public TreeViewDocumentFinanceMaster(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceMaster(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(FIN_DocumentFinanceMaster);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            FIN_DocumentFinanceMaster defaultValue = (pDefaultValue != null) ? pDefaultValue as FIN_DocumentFinanceMaster : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Date") { Title = Resx.global_document_date, MinWidth = 140 });
            columnProperties.Add(new GenericTreeViewColumnProperty("DocumentNumber") { Title = Resx.global_document_number, MinWidth = 180 });
            //#if (DEBUG)
            columnProperties.Add(new GenericTreeViewColumnProperty("DocumentStatusStatus") { Title = Resx.global_document_status, MinWidth = 50, MaxWidth = 50 });
            //#endif
            columnProperties.Add(new GenericTreeViewColumnProperty("EntityName") { Title = Resx.global_entity, MinWidth = 160 });
            columnProperties.Add(new GenericTreeViewColumnProperty("EntityFiscalNumber") { Title = Resx.global_fiscal_number, MinWidth = 70, MaxWidth = 120 });
            columnProperties.Add(new GenericTreeViewColumnProperty("TotalFinalRound")
            {
                Title = Resx.global_total,
                MinWidth = 100,
                Alignment = 1.0F,
                FormatProvider = new FormatterDecimalCurrency(),
                CellRenderer = new CellRendererText()
                {
                    Alignment = Pango.Alignment.Right,
                    FontDesc = new Pango.FontDescription() { Size = 50 },
                    Xalign = 1.0F
                }
            });
            columnProperties.Add(new GenericTreeViewColumnProperty("TotalDebit")
            {
                //This Query Exists 3 Locations, Find it and change in all Locations - Required "GROUP BY fmaOid,fmaTotalFinal" to work with SQLServer
                Query = "SELECT fmaTotalFinal - SUM(fmpCreditAmount) as Result FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' GROUP BY fmaOid, fmaTotalFinal;",
                Title = Resx.global_debit,
                MinWidth = 100,
                Alignment = 1.0F,
                FormatProvider = new FormatterDecimalCurrency(),
                CellRenderer = new CellRendererText()
                {
                    Alignment = Pango.Alignment.Right,
                    FontDesc = new Pango.FontDescription() { Size = 50 },
                    Xalign = 1.0F
                }
            });

            //Sort Order
            SortProperty[] sortProperty = new SortProperty[2];
            sortProperty[0] = new SortProperty("Date", SortingDirection.Ascending);
            sortProperty[1] = new SortProperty("DocumentNumber", SortingDirection.Ascending);

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
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
