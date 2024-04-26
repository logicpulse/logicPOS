using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.App;
using logicpos.datalayer.Xpo;
using System;
using System.Collections.Generic;

namespace logicpos.datalayer.DataLayer.Xpo
{
    public class XPSelectData
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private readonly Dictionary<string, int> _fieldIndex;

        public SelectStatementResultRow[] Meta { get; set; }

        public SelectStatementResultRow[] Data { get; set; }

        public XPSelectData(SelectedData pSelectedData)
        {
            Meta = pSelectedData.ResultSet[0].Rows;
            Data = pSelectedData.ResultSet[1].Rows;

            int i = 0;
            _fieldIndex = new Dictionary<string, int>();
            foreach (SelectStatementResultRow field in Meta)
            {
                //_logger.Debug(string.Format("FunctionName(): FieldName: {0}[{1}]", field.Values[0].ToString(), i));
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
            return DataLayerUtils.GetXPGuidObject(pSession, pType, new Guid(GetValueFromField(pSearchField, "Oid", pSearchValue).ToString()));
        }

        public string GenMetaCsv(bool pLogOutput = false)
        {
            string csvOutput = string.Empty;

            foreach (SelectStatementResultRow row in Meta)
                csvOutput += string.Format("{0}\t{1}\t{2}{3}", row.Values[0], row.Values[1], row.Values[2], Environment.NewLine);

            if (pLogOutput) _logger.Debug(string.Format("GenMetaCsv():{0}{1}{2}", Environment.NewLine, csvOutput, Environment.NewLine));

            return csvOutput;
        }

        public string GenDataCsv(bool pLogOutput = false)
        {
            string csvOutput = string.Empty;

            foreach (SelectStatementResultRow rowFieldNames in Meta)
            {
                csvOutput += string.Format("{0}\t", rowFieldNames.Values[0]);
            }
            csvOutput += Environment.NewLine;

            foreach (SelectStatementResultRow rowData in Data)
            {
                foreach (SelectStatementResultRow rowFieldNames in Meta)
                {
                    csvOutput += string.Format("{0}\t", rowData.Values[GetFieldIndex(rowFieldNames.Values[0].ToString())]);
                }
                csvOutput += Environment.NewLine;
            }

            if (pLogOutput) _logger.Debug(string.Format("GenDataCsv():{0}{1}{2}", Environment.NewLine, csvOutput, Environment.NewLine));

            return csvOutput;
        }
    }
}
