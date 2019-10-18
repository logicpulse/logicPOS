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
            Type xpoGuidObjectType = typeof(fin_documentfinancepayment);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinancepayment defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinancepayment : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(GlobalFramework.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("CreatedAt") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_date"), MinWidth = 140 }); /* IN009067 */
            columnProperties.Add(new GenericTreeViewColumnProperty("PaymentRefNo") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_number"), MinWidth = 120 }); /* IN009067 */
            columnProperties.Add(new GenericTreeViewColumnProperty("PaymentStatus") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_status"), MinWidth = 50, MaxWidth = 50 });
            //Shared Query
            /* IN009075 - removing call to view */
			// string query = "SELECT {0} as Result FROM view_documentfinancepayment WHERE fpaOid = '{1}' GROUP BY fpaOid,{0};";
            string queryForCustomerDetails = "SELECT {0} FROM erp_customer AS Customer LEFT JOIN fin_documentfinancepayment AS Payment ON (Payment.EntityOid = Customer.Oid) WHERE Payment.Oid = '{1}';";
            columnProperties.Add(new GenericTreeViewColumnProperty("EntityName") { 
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_entity"),
                MinWidth = 260,
                MaxWidth = 260,
                Query = string.Format(queryForCustomerDetails, "Name", "{0}"), /* IN009075 */
                DecryptValue = true
            });
			/* IN009067 */
            columnProperties.Add(new GenericTreeViewColumnProperty("EntityFiscalNumber") { 
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"),
                MinWidth = 100,
                Query = string.Format(queryForCustomerDetails, "FiscalNumber", "{0}"), /* IN009075 */
                DecryptValue = true
            });
            columnProperties.Add(new GenericTreeViewColumnProperty("PaymentAmount")
            {
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total"),
                MinWidth = 100,
                //Alignment = 1.0F,
                FormatProvider = new FormatterDecimalCurrency(),
				/* IN009067 */
                CellRenderer = new CellRendererText()
                {
                    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                    Alignment = Pango.Alignment.Right,
                    Xalign = 1.0F
                }
            });

            /* IN009067 - TODO */
            string relatedDocumentsQuery = logicpos.DataLayer.GenerateRelatedDocumentsQuery(true);
            columnProperties.Add(new GenericTreeViewColumnProperty("RelatedDocuments")
            {
                Query = relatedDocumentsQuery,
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_column_related_doc"),
                MinWidth = 100
            });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
            SortProperty[] sortProperty = new SortProperty[1];
            /* IN009067 - setting up descending sort */
            sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Descending);            
            //Configure Criteria/XPCollection/Model              
            //New IN009223 IN009227         
             XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria, sortProperty) { TopReturnedObjects = SettingsApp.PaginationRowsPerPage };

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
