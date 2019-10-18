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
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;

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
            Type xpoGuidObjectType = typeof(fin_documentfinancemaster);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinancemaster defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinancemaster : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(GlobalFramework.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Date") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_date"), MinWidth = 140 });
            columnProperties.Add(new GenericTreeViewColumnProperty("DocumentNumber") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_number"), MinWidth = 120 }); /* IN009067 */
            //#if (DEBUG)
            columnProperties.Add(new GenericTreeViewColumnProperty("DocumentStatusStatus") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_status"), MinWidth = 50, MaxWidth = 50 });
            //#endif
            columnProperties.Add(new GenericTreeViewColumnProperty("EntityName")
            {
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_entity"),
                MinWidth = 260,
                MaxWidth = 260,
                FormatProvider = new FormatterDecrypt() /* IN009075 - FormatterDecrypt() created */
            }); /* IN009067 */
            columnProperties.Add(new GenericTreeViewColumnProperty("EntityFiscalNumber")
            {
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"),
                MinWidth = 100,
                FormatProvider = new FormatterDecrypt() /* IN009075 - FormatterDecrypt() created */
            }); /* IN009067 */
            columnProperties.Add(new GenericTreeViewColumnProperty("TotalFinal")
            { /* IN009166 */
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_final"), 
                MinWidth = 100,
                //Alignment = 1.0F,
                FormatProvider = new FormatterDecimalCurrency(),
                CellRenderer = new CellRendererText()
                {
                    FontDesc = new Pango.FontDescription() { Size = fontGenericTreeViewColumn },
                    Alignment = Pango.Alignment.Right,
                    Xalign = 1.0F
                }
            });

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

            /* Fixed by IN009151 */
            /*string query3 = @"
SELECT 
	 ISNULL(SUM(DocFinMasterPay.CreditAmount), 0)  +
	 (
		SELECT 
			ISNULL(SUM(DocFinMaster.TotalFinal), 0) AS TotalFinal
		FROM
			fin_documentfinancemaster AS DocFinMaster
		WHERE
			DocumentType = 'fa924162-beed-4f2f-938d-919deafb7d47'
			AND 
				DocFinMaster.DocumentParent = '{0}'
			AND
				( DocFinMaster.DocumentStatusStatus <> 'A' AND DocFinMaster.Disabled <> 1)
		GROUP BY
			DocFinMaster.DocumentParent				 
	 ) AS CreditAmount
FROM
	fin_documentfinancemasterpayment AS DocFinMasterPay
LEFT JOIN 
	fin_documentfinancepayment AS DocFinPay ON (DocFinPay.Oid = DocFinMasterPay.DocumentFinancePayment)
WHERE
	DocFinMasterPay.DocumentFinanceMaster = '{0}' 
	AND
		(DocFinPay.PaymentStatus <> 'A' AND DocFinPay.Disabled <> 1)
GROUP BY
	DocFinMasterPay.DocumentFinanceMaster;

";
            */
            columnProperties.Add(new GenericTreeViewColumnProperty("TotalOfCredit")
            {
                Query = queryForTotalOfCredit,
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_column_total_credit_rc_nc_based"),
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

            /* IN009067 - adding "ViewDocumentFinancePaymentDocumentTotal" result to TotalDebit column */
            /*string queryForTotalDebit = @"
SELECT 
    fmaTotalFinal - (
	    SELECT 
            SUM(CreditAmount) as CreditTotals 
        FROM 
            view_documentfinancepaymentdocumenttotal 
        WHERE 
            DocumentPayed = '{0}'
    ) AS Result 
FROM 
    view_documentfinancepayment 
WHERE 
    fmaOid = '{0}' AND 
    fpaPaymentStatus <> 'A' 
GROUP BY 
    fmaOid, fmaTotalFinal;";*/

            /* Fixed by IN009151 */
            /*
             * "UNION" with a "NOT EXISTS" was necessary to keep "0" value as part of calculation and then avoiding "NULL" cases that affect calculation result.
             * 
             * "CASE/WHEN" implemented to remove Debit values from credit-documents (NCs) and transport-documents (GTs)
             * 
             * Replacing use of "view_documentfinancepayment"
             */
            /* #TODO - SQL refactoring */
            string stringFormatIndexZero = "{0}";
            /* The following document types has no debit amount:
             * Orçamento
             * Guia ou Nota de Devolução
             * Guia de Transporte
             * Fatura Simplificada
             * Documento de Conferência
             * Fatura Pró-Forma
             * Fatura-Recibo
             * Nota de Crédito
             * Guia de Consignação
             * Guia de Remessa
             * Guia de Movimentação de Ativos Fixos Próprios
             */
            string queryForTotalDebit = $@"
SELECT
(
	CASE  
		WHEN DFM.DocumentType IN (
            '{SettingsApp.XpoOidDocumentFinanceTypeBudget}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeConsignmentGuide}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeCreditNote}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeDeliveryNote}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeReturnGuide}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice}', 
            '{SettingsApp.XpoOidDocumentFinanceTypeTransportationGuide}'
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
            columnProperties.Add(new GenericTreeViewColumnProperty("TotalDebit")
            {
                //This Query Exists 3 Locations, Find it and change in all Locations - Required "GROUP BY fmaOid,fmaTotalFinal" to work with SQLServer
                Query = queryForTotalDebit,
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_debit"),
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

            /* IN009067 - Adding RelatedDocuments column */
            string relatedDocumentsQuery = logicpos.DataLayer.GenerateRelatedDocumentsQuery();
            columnProperties.Add(new GenericTreeViewColumnProperty("RelatedDocuments")
            {
                Query = relatedDocumentsQuery,
                Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_column_related_doc"),
                MinWidth = 100
            });

            //Sort Order
            SortProperty[] sortProperty = new SortProperty[2];
            /* IN009067 - setting up descending sort */
            sortProperty[0] = new SortProperty("Date", SortingDirection.Descending);
            sortProperty[1] = new SortProperty("DocumentNumber", SortingDirection.Descending);

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;            

            //New Sort collection for pagination created (3/7/19) IN009223 IN009227
            SortingCollection sortCollection = new SortingCollection(sortProperty);
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria, sortProperty) { TopReturnedObjects = SettingsApp.PaginationRowsPerPage * base.CurrentPageNumber, Sorting = sortCollection };

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
