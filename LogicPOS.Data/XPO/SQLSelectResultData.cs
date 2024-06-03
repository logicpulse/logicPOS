using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using System;
using System.Collections.Generic;

namespace LogicPOS.Data.XPO
{
    public class SQLSelectResultData
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private readonly Dictionary<string, int> _fieldIndex;

        public SelectStatementResultRow[] Meta { get; set; }

        public SelectStatementResultRow[] Data { get; set; }

        public SQLSelectResultData(SelectedData selectedData)
        {
            Meta = selectedData.ResultSet[0].Rows;
            Data = selectedData.ResultSet[1].Rows;

            int i = 0;
            _fieldIndex = new Dictionary<string, int>();
            foreach (SelectStatementResultRow field in Meta)
            {
                _fieldIndex.Add(field.Values[0].ToString(), i++);
            }
        }

        //Create a FieldIndex to Get Values From FieldNames
        public int GetFieldIndex(string pFieldName)
        {
            try
            {
                return _fieldIndex[pFieldName];
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("FieldName: [{0}] : {1}", pFieldName, ex.Message), ex);
                return -1;
            }
        }

        public object GetValueFromField(string pSearchField, string pReturnField, string pSearchValue)
        {
            bool debug = false;

            //Find Key
            foreach (SelectStatementResultRow rowFieldNames in Meta)
            {
                if (pSearchField.ToUpper() == rowFieldNames.Values[0].ToString().ToUpper())
                {
                    if (debug) _logger.Debug(string.Format("GetValueFromField(): FindKey : [{0}]==[{1}]", pSearchField.ToUpper(), rowFieldNames.Values[0].ToString().ToUpper()));

                    //Find First Value
                    foreach (SelectStatementResultRow rowData in Data)
                    {
                        // Get Field, require to check if Null
                        var field = rowData.Values[GetFieldIndex(rowFieldNames.Values[0].ToString())];

                        if (field != null && pSearchValue.ToUpper() == field.ToString().ToUpper())
                        {
                            if (debug) _logger.Debug(string.Format("GetValueFromField(): FindValue : [{0}]==[{1}]", pSearchValue.ToUpper(), rowData.Values[GetFieldIndex(rowFieldNames.Values[0].ToString())].ToString().ToUpper()));
                            return rowData.Values[GetFieldIndex(pReturnField)];
                        }
                    }
                }
            }

            return null;
        }

        public object GetXPGuidObjectFromField(Type pType, string pSearchField, string pSearchValue)
        {
            return GetXPGuidObjectFromField(XPOSettings.Session, pType, pSearchField, pSearchValue);
        }

        public object GetXPGuidObjectFromField(Session pSession, Type pType, string pSearchField, string pSearchValue)
        {
            return XPOHelper.GetXPGuidObject(pSession, pType, new Guid(GetValueFromField(pSearchField, "Oid", pSearchValue).ToString()));
        }
    }
}
