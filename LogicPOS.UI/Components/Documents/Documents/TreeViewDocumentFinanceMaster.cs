using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Formatters;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewDocumentFinanceMaster : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceMaster() { }

        [Obsolete]
        public TreeViewDocumentFinanceMaster(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewDocumentFinanceMaster(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_documentfinancemaster);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinancemaster defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinancemaster : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(AppSettings.Instance.fontGenericTreeViewColumn);

            //Configure columnProperties
            List<GridViewColumn> columnProperties = new List<GridViewColumn>
            {
                new GridViewColumn("Date") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_document_date"), MinWidth = 140 },
                new GridViewColumn("DocumentNumber") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_document_number"), MinWidth = 120 }, /* IN009067 */
                //#if (DEBUG)
                new GridViewColumn("DocumentStatusStatus") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_document_status"), MinWidth = 50, MaxWidth = 50 },
                //#endif
                new GridViewColumn("EntityName")
                {
                    Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_entity"),
                    MinWidth = 260,
                    MaxWidth = 260,
                    FormatProvider = new DecryptFormatter() /* IN009075 - FormatterDecrypt() created */
                }, /* IN009067 */
                new GridViewColumn("EntityFiscalNumber")
                {
                    Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_fiscal_number"),
                    MinWidth = 100,
                    FormatProvider = new DecryptFormatter() /* IN009075 - FormatterDecrypt() created */
                }, /* IN009067 */
                new GridViewColumn("TotalFinal")
                { /* IN009166 */
                    Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_final"),
                    MinWidth = 100,
                    //Alignment = 1.0F,
                    FormatProvider = new DecimalCurrencyFormatter(),
                    CellRenderer = new CellRendererText()
                    {
                        FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                        Alignment = Pango.Alignment.Right,
                        Xalign = 1.0F
                    }
                }
            };

            /* IN009161 - changing columns order */
            /* IN009067 - Adding TotalOfCredit column */
            /* IN009151 - fix for "CreditTotals" (CreditInvoiceTotal column added) */
            /* #TODO - SQL refactoring */
            string queryForTotalOfCredit = @"
SELECT 
    SUM(CreditAmount) + SUM(CreditInvoiceTotal) as CreditTotals 
FROM 
    view_documentfinancepaymentdocumenttotal 
WHERE 
    DocumentPayed = '{0}';";


            columnProperties.Add(new GridViewColumn("TotalOfCredit")
            {
                Query = queryForTotalOfCredit,
                Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_column_total_credit_rc_nc_based"),
                MinWidth = 100,
                //Alignment = 1.0F,
                FormatProvider = new DecimalCurrencyFormatter(),
                /* IN009067 */
                CellRenderer = new CellRendererText()
                {
                    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                    Alignment = Pango.Alignment.Right,
                    Xalign = 1.0F
                }
            });

            string stringFormatIndexZero = "{0}";

            string queryForTotalDebit = $@"
SELECT
(
	CASE  
		WHEN DFM.DocumentType IN (
            '{DocumentSettings.XpoOidDocumentFinanceTypeBudget}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeConsignmentGuide}', 
            '{CustomDocumentSettings.CreditNoteId}', 
            '{CustomDocumentSettings.DeliveryNoteDocumentTypeId}', 
            '{DocumentSettings.InvoiceAndPaymentId}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeReturnGuide}', 
            '{DocumentSettings.SimplifiedInvoiceId}', 
            '{CustomDocumentSettings.TransportDocumentTypeId}'
        ) THEN NULL 
		ELSE (
			DFM.TotalFinal - (
				(
					SELECT 
						SUM(DocFinMaster.TotalFinal) AS TotalFinal
					FROM
						fin_documentfinancemaster AS DocFinMaster
					WHERE
						DocumentType = 'fa924162-beed-4f2f-938d-919deafb7d47'
						AND 
							DocFinMaster.DocumentParent = DFM.Oid
						AND
							( DocFinMaster.DocumentStatusStatus <> 'A' AND DocFinMaster.Disabled <> 1)
					GROUP BY
						DocFinMaster.DocumentParent
					UNION 
						SELECT 
							0 AS CreditAmount
						WHERE 
							NOT EXISTS (
								SELECT 1
								FROM 
									fin_documentfinancemaster AS DFMNC
								WHERE
									DocumentType = 'fa924162-beed-4f2f-938d-919deafb7d47'
									AND 
										DFMNC.DocumentParent = DFM.Oid
									AND
										( DFMNC.DocumentStatusStatus <> 'A' AND DFMNC.Disabled <> 1)
							) 
				) + 
				(
					SELECT 
						 SUM(DocFinMasterPay.CreditAmount) AS CreditAmount
					FROM
						fin_documentfinancemasterpayment AS DocFinMasterPay
					LEFT JOIN 
						fin_documentfinancepayment AS DocFinPay ON (DocFinPay.Oid = DocFinMasterPay.DocumentFinancePayment)
					WHERE
						DocFinMasterPay.DocumentFinanceMaster = DFM.Oid
						AND
							(DocFinPay.PaymentStatus <> 'A' AND DocFinPay.Disabled <> 1)
					GROUP BY
						DocFinMasterPay.DocumentFinanceMaster
					UNION 
						SELECT 
							0 AS CreditAmount
						WHERE 
							NOT EXISTS (
								SELECT 1
								FROM 
									fin_documentfinancemasterpayment AS DFMRC
								LEFT JOIN 
									fin_documentfinancepayment AS DFPRC ON (DFPRC.Oid = DFMRC.DocumentFinancePayment)
								WHERE
									DFMRC.DocumentFinanceMaster = DFM.Oid
									AND
										( DFPRC.PaymentStatus <> 'A' AND DFMRC.Disabled <> 1)
							) 
				) 
			)
		)
	END
)
	 AS Balance
FROM
	fin_documentfinancemaster DFM
WHERE DFM.Oid =  '{stringFormatIndexZero}';
";
            columnProperties.Add(new GridViewColumn("TotalDebit")
            {
                //This Query Exists 3 Locations, Find it and change in all Locations - Required "GROUP BY fmaOid,fmaTotalFinal" to work with SQLServer
                Query = queryForTotalDebit,
                Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_debit"),
                MinWidth = 100,
                //Alignment = 1.0F,
                FormatProvider = new DecimalCurrencyFormatter(),
                /* IN009067 */
                CellRenderer = new CellRendererText()
                {
                    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                    Alignment = Pango.Alignment.Right,
                    Xalign = 1.0F
                }
            });

            /* IN009067 - Adding RelatedDocuments column */
            string relatedDocumentsQuery = logicpos.DataLayer.GenerateRelatedDocumentsQuery();
            columnProperties.Add(new GridViewColumn("RelatedDocuments")
            {
                Query = relatedDocumentsQuery,
                Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_column_related_doc"),
                MinWidth = 100
            });

            //Sort Order
            SortProperty[] sortProperty = new SortProperty[2];
            /* IN009067 - setting up descending sort */
            sortProperty[0] = new SortProperty("Date", SortingDirection.Descending);
            sortProperty[1] = new SortProperty("DocumentNumber", SortingDirection.Descending);

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
            //New Sort collection for pagination created (3/7/19) IN009223 IN009227
            SortingCollection sortCollection = new SortingCollection(sortProperty);
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria, sortProperty) { TopReturnedObjects = POSSettings.PaginationRowsPerPage * CurrentPageNumber, Sorting = sortCollection };

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
