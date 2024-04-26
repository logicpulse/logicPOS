using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.shared.App;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosReportsQueryDialog : PosBaseDialog
    {
        private void entryBoxDateStart_ClosePopup(object sender, EventArgs e)
        {
            DateStart = _entryBoxDateStart.Value;
            // Call Validate
            Validate();
        }

        private void entryBoxDateEnd_ClosePopup(object sender, EventArgs e)
        {
            DateEnd = _entryBoxDateEnd.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            // Call Validate
            Validate();
        }

        /// <summary>
        /// It makes datepicker accepts text.
        /// Please see #IN005974
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void entryBoxDateStart_Changed(object sender, EventArgs e)
        {
            DateStart = _entryBoxDateStart.Value;
            // Call Validate
            Validate();
        }

        /// <summary>
        /// It makes datepicker accepts text.
        /// Please see #IN005974
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void entryBoxDateEnd_Changed(object sender, EventArgs e)
        {
            DateEnd = _entryBoxDateEnd.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            // Call Validate
            Validate();
        }

        private void _entryBoxSelectShared_ClosePopup(object sender, EventArgs e)
        {
            bool debug = false;
            Widget widget = (sender as Widget);

            // Helper to debug Senders
            //XPOEntryBoxSelectRecordValidation<fin_documentfinancetype, TreeViewDocumentFinanceType> entryBoxSelectDocumentFinanceType = (sender as XPOEntryBoxSelectRecordValidation<fin_documentfinancetype, TreeViewDocumentFinanceType>);

            if (_selectionBoxs.ContainsKey(widget.Name))
            {
                dynamic dynamicSelectedObject = _selectionBoxs[widget.Name];
                XPGuidObject dynamicSelectedXPOObject = dynamicSelectedObject.Value;
                if (debug) _logger.Debug(string.Format("Selected Type Key: [{0}] Value: [{1}]", widget.Name, dynamicSelectedXPOObject.Oid));
            }
        }

        private void _buttonOk_Clicked(object sender, EventArgs e)
        {
            // Call Validate
            Validate();

            List<string> result = GetComposedFilter();
            if (result.Count == 2)
            {
                FilterValue = result[0];
                FilterValueHumanReadble = result[1];
            }
            else
            {
                throw new Exception("Error! Cant get filter Values from GetComposedFilter");
            }
        }
        private void _buttonExportXls_Clicked(object sender, EventArgs e)
        {
            // Call Validate
            Validate();
            PosReportsDialog.exportType = CustomReportDisplayMode.ExportXls;
            
            List<string> result = GetComposedFilter();
            if (result.Count == 2)
            {
                FilterValue = result[0];
                FilterValueHumanReadble = result[1];
            }
            else
            {
                throw new Exception("Error! Cant get filter Values from GetComposedFilter");
            }
        }
        private void _buttonExportPdf_Clicked(object sender, EventArgs e)
        {
            // Call Validate
            Validate();
            PosReportsDialog.exportType = CustomReportDisplayMode.ExportPDF;
            
            List<string> result = GetComposedFilter();
            if (result.Count == 2)
            {
                FilterValue = result[0];
                FilterValueHumanReadble = result[1];
            }
            else
            {
                throw new Exception("Error! Cant get filter Values from GetComposedFilter");
            }
        }
        
        // Prevent Dialog Destroy, Validate Count Records, we must override OnResponse to prevent Close Dialog
        protected override void OnResponse(ResponseType pResponse)
        {
            //add responseType Export to Excel
            if (pResponse == ResponseType.Ok || pResponse == (ResponseType)DialogResponseType.ExportXls || pResponse == (ResponseType)DialogResponseType.ExportPdf)
            {
                // Test Query for Records
                int count = 0;
                string countQuerySql = string.Empty;
                if(pResponse == ResponseType.Ok)
                {
                    PosReportsDialog.exportType = CustomReportDisplayMode.ExportPDF;
                }

                try
                {
                    if (_databaseSourceObject == "fin_articleserialnumber" || _databaseSourceObject == "fin_articlewarehouse")
                    {
                        FilterValue = FilterValue.Replace("Date", "CreatedAt");
                    }
                    
                     countQuerySql = string.Format("SELECT COUNT(*) AS Count FROM {0} WHERE {1};", _databaseSourceObject, FilterValue);

                    DataTable dataTable = SharedUtils.GetDataTableFromQuery(countQuerySql);
                    count = Convert.ToInt32(Convert.ToDecimal(dataTable.Rows[0].ItemArray[0]));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    _logger.Error(string.Format("Error in countQuerySql: [{0}]", countQuerySql));
                }
                //if(_databaseSourceObject == "fin_articlewarehouse" && _filterValue.Contains("SerialNumber"))
                //{
                //    this.Destroy();
                //}

                if (count <= 0)
                {
					/* IN009062 */
                    logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, new Size(500, 240), MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_report_filter_no_records_with_criteria"));
                    //Keep Running
                    this.Run();
                }
                else
                {
                    // Dont Leave Dangling Dialog Arroung
                    this.Destroy();
                }
            }
        }

        private void Validate()
        {
            _buttonOk.Sensitive = (DateStart <= DateEnd);
        }

        // Get Composed Filter and Human Readable for Report Footer
        private List<string> GetComposedFilter()
        {
            // Init Local Vars
            bool debug = false;
            int i = 0;
            string filterSelectionBoxs = string.Empty;
            // Store Human Readable Filter
            string filterSelectionBoxsHumanReadable = string.Empty;
            string humanReadbleValue = string.Empty;
            string filterDateField = (_fieldsModeComponents[_reportsQueryDialogMode].ContainsKey(typeof(DateTime).Name)) 
                ? _fieldsModeComponents[_reportsQueryDialogMode][typeof(DateTime).Name]
                : "UNDEFINED_DATE_FIELD";
            XPGuidObject dynamicSelectedXPOObject;
            // Store Result Object
            List<string> result = new List<string>();

            foreach (var item in _selectionBoxs)
            {
                i++;
                // Reset Vars
                string fieldName = null;
                dynamicSelectedXPOObject = null;
                //_logger.Debug(string.Format("Key: [{0}]", item.Key));

                // FieldName
                //if (_fieldNames.ContainsKey(item.Key))
                if (_fieldsModeComponents[_reportsQueryDialogMode].ContainsKey(item.Key))
                {
                    fieldName = _fieldsModeComponents[_reportsQueryDialogMode][item.Key];
                }

                // Get selectionBox XPOObjet
                if (_selectionBoxs.ContainsKey(item.Key))
                {
                    dynamic dynamicSelectedObject = _selectionBoxs[item.Key];
                    if (dynamicSelectedObject != null)
                    {
                        dynamicSelectedXPOObject = dynamicSelectedObject.Value;
                        // Extract humanReadbleValue from EntryValidation, ex "Fatura"
                        humanReadbleValue = dynamicSelectedObject.EntryValidation.Text;
                    }
                    else
                    {
                        _logger.Error(string.Format("Error cant get _selectionBox[{0}]", item.Key));
                    }
                }

                if (fieldName != null && dynamicSelectedXPOObject != null)
                {
                    if (dynamicSelectedXPOObject.Oid != SharedSettings.XpoOidUndefinedRecord)
                    {
                        filterSelectionBoxs += string.Format("{0} = '{1}'{2}", fieldName, dynamicSelectedXPOObject.Oid, " AND ");
                        filterSelectionBoxsHumanReadable += string.Format("'{0}', ", humanReadbleValue);
                    }
                }
            }

            // Remove Last ' AND ' if Exists
            if (filterSelectionBoxs.EndsWith(" AND "))
            {
                filterSelectionBoxs = filterSelectionBoxs.Substring(0, filterSelectionBoxs.LastIndexOf(" AND "));
            }
            // Remove Last ', ' if Exists
            if (filterSelectionBoxsHumanReadable.EndsWith(", "))
            {
                filterSelectionBoxsHumanReadable = filterSelectionBoxsHumanReadable.Substring(0, filterSelectionBoxsHumanReadable.LastIndexOf(", "));
            }

            /* IN009010 */
            //if (_reportsQueryDialogMode.Equals(ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY))
            else if ("UNDEFINED_DATE_FIELD".Equals(filterDateField))//TO DO
            {
                filterDateField = "CustomerSinceDate";
                DateStart = new DateTime(1900, 1, 1, 0, 0, 0);// "1900-01-01 00:00:00"
                DateEnd = DateTime.Now;
            }

            if (Enums.Reports.ReportsQueryDialogMode.FINANCIAL_DETAIL_VAT.Equals(_reportsQueryDialogMode))
            {
                filterDateField = "fmDate";
            }
            string filter = filterSelectionBoxs;
            string filterHumanReadable = filterSelectionBoxsHumanReadable;
            // Combine final where Filter
            if (!Enums.Reports.ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY.Equals(_reportsQueryDialogMode))
            {
                string datesFilter = string.Format("{0} >= '{1}' AND {0} <= '{2}'", filterDateField, DateStart.ToString(SharedSettings.DateTimeFormat), DateEnd.ToString(SharedSettings.DateTimeFormat));
                filter = (!string.IsNullOrEmpty(filterSelectionBoxs))
                    ? string.Format("({0}) AND ({1})", datesFilter, filterSelectionBoxs)
                    : string.Format("({0})", datesFilter);

                //if (_reportsQueryDialogMode == Enums.Reports.ReportsQueryDialogMode.FILTER_ARTICLE_STOCK)
                //{
                //    string stkFilter = "artStk IS NOT NULL";
                //    filter = (!string.IsNullOrEmpty(filterSelectionBoxs))
                //    ? string.Format("({0}) AND ({1})", stkFilter, filterSelectionBoxs)
                //    : string.Format("({0})", stkFilter);
                //}
                // HumanReadable
                /* IN006004 */
                string datesFilterHumanReadable = string.Format(" {0} '{1}', {2} '{3}' ", resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_date_start"), DateStart.ToString(SharedSettings.DateTimeFormat), resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_date_end"), DateEnd.ToString(SharedSettings.DateTimeFormat));
                filterHumanReadable = (!string.IsNullOrEmpty(filterSelectionBoxsHumanReadable))
                    ? string.Format("{0}, {1}", datesFilterHumanReadable, filterSelectionBoxsHumanReadable)
                    : datesFilterHumanReadable;

                if (debug) _logger.Debug(string.Format("Filter: [{0}]", filter));
                if (debug) _logger.Debug(string.Format("filterHumanReadable: [{0}]", filterHumanReadable));


                /* IN009204 - RCs should be removed from this report, only AT Financial documents here */
                if (Enums.Reports.ReportsQueryDialogMode.COMPANY_BILLING.Equals(_reportsQueryDialogMode))
                {
                    string documentTypeOid = SharedSettings.XpoOidDocumentFinanceTypePayment.ToString();
                    string documentTypeDesignation = resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type_title_rc");
                    /* Based on "view_documentfinancecustomerbalancedetails" we are removing RCs ("a009168d-fed1-4f52-b9e3-77e280b18ff5") */
                    filter += $" AND DocumentTypeOid <> '{documentTypeOid}'";
                    // filterHumanReadable += $", {resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type} <> '{documentTypeDesignation}'";
                }
            }

            result.Add(filter);
            result.Add(string.Format("{0}: [{1}]", resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_filter"), filterHumanReadable));

            // Return Result Filter List
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        // Generic Method to Generate XPOEntryBoxSelectRecordValidation
        private XPOEntryBoxSelectRecordValidation<T1, T2> SelectionBoxFactory<T1, T2>(string labelText, string fieldDisplayValue = "Designation", string extraFilter = "")
            where T1 : XPGuidObject, new()
            where T2 : GenericTreeViewXPO, new()
        {
            XPOEntryBoxSelectRecordValidation<T1, T2> resultObject;

            // Helper to debug extraFilter
            //if (!string.IsNullOrEmpty(extraFilter))
            //{
            //    _logger.Debug("BREAK");
            //}
            T1 defaultValue = (T1)DataLayerUtils.GetXPGuidObject(DataLayerFramework.SessionXpo, typeof(T1), SharedSettings.XpoOidUndefinedRecord);            
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("((Disabled IS NULL OR Disabled  <> 1) OR (Oid = '{0}') OR (Oid = '{1}')) {2}", SharedSettings.XpoOidUndefinedRecord,SharedSettings.XpoOidUserRecord, extraFilter));
            resultObject = new XPOEntryBoxSelectRecordValidation<T1, T2>(this, labelText, fieldDisplayValue, "Oid", (defaultValue as T1), criteriaOperator, SharedSettings.RegexGuid, true);
            resultObject.Name = typeof(T1).Name;
            resultObject.EntryValidation.IsEditable = true;
            resultObject.ClosePopup += _entryBoxSelectShared_ClosePopup;

            return resultObject;
        }
    }
}
