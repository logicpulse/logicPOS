using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.App;
using System;
using System.Collections.Generic;

namespace logicpos.datalayer.DataLayer.Xpo
{
    public class XPSelectData
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Private Members
        Dictionary<string, int> _fieldIndex;
        //Public Properties
        SelectStatementResultRow[] _meta;
        public SelectStatementResultRow[] Meta
        {
            get { return _meta; }
            set { _meta = value; }
        }

        SelectStatementResultRow[] _data;
        public SelectStatementResultRow[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public XPSelectData(SelectedData pSelectedData)
        {
            _meta = pSelectedData.ResultSet[0].Rows;
            _data = pSelectedData.ResultSet[1].Rows;

            int i = 0;
            _fieldIndex = new Dictionary<string, int>();
            foreach (SelectStatementResultRow field in _meta)
            {
                //_log.Debug(string.Format("FunctionName(): FieldName: {0}[{1}]", field.Values[0].ToString(), i));
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
                _log.Error(string.Format("FieldName: [{0}] : {1}", pFieldName, ex.Message), ex);
                return -1;
            }
        }

        public object GetValueFromField(string pSearchField, string pReturnField, string pSearchValue)
        {
            bool debug = false;

            //Find Key
            foreach (SelectStatementResultRow rowFieldNames in _meta)
            {
                if (pSearchField.ToUpper() == rowFieldNames.Values[0].ToString().ToUpper())
                {
                    if (debug) _log.Debug(string.Format("GetValueFromField(): FindKey : [{0}]==[{1}]", pSearchField.ToUpper(), rowFieldNames.Values[0].ToString().ToUpper()));

                    //Find First Value
                    foreach (SelectStatementResultRow rowData in _data)
                    {
                        // Get Field, require to check if Null
                        var field = rowData.Values[GetFieldIndex(rowFieldNames.Values[0].ToString())];

                        if (field != null && pSearchValue.ToUpper() == field.ToString().ToUpper())
                        {
                            if (debug) _log.Debug(string.Format("GetValueFromField(): FindValue : [{0}]==[{1}]", pSearchValue.ToUpper(), rowData.Values[GetFieldIndex(rowFieldNames.Values[0].ToString())].ToString().ToUpper()));
                            return rowData.Values[GetFieldIndex(pReturnField)];
                        }
                    }
                }
            }

            return null;
        }

        public object GetXPGuidObjectFromField(Type pType, string pSearchField, string pSearchValue)
        {
            return GetXPGuidObjectFromField(GlobalFramework.SessionXpo, pType, pSearchField, pSearchValue);
        }

        public object GetXPGuidObjectFromField(Session pSession, Type pType, string pSearchField, string pSearchValue)
        {
            return FrameworkUtils.GetXPGuidObject(pSession, pType, new Guid(GetValueFromField(pSearchField, "Oid", pSearchValue).ToString()));
        }

        public string GenMetaCsv(bool pLogOutput = false)
        {
            string csvOutput = String.Empty;

            foreach (SelectStatementResultRow row in _meta)
                csvOutput += string.Format("{0}\t{1}\t{2}{3}", row.Values[0], row.Values[1], row.Values[2], Environment.NewLine);

            if (pLogOutput) _log.Debug(string.Format("GenMetaCsv():{0}{1}{2}", Environment.NewLine, csvOutput, Environment.NewLine));

            return csvOutput;
        }

        public string GenDataCsv(bool pLogOutput = false)
        {
            string csvOutput = String.Empty;

            foreach (SelectStatementResultRow rowFieldNames in _meta)
            {
                csvOutput += string.Format("{0}\t", rowFieldNames.Values[0]);
            }
            csvOutput += Environment.NewLine;

            foreach (SelectStatementResultRow rowData in _data)
            {
                foreach (SelectStatementResultRow rowFieldNames in _meta)
                {
                    csvOutput += string.Format("{0}\t", rowData.Values[GetFieldIndex(rowFieldNames.Values[0].ToString())]);
                }
                csvOutput += Environment.NewLine;
            }

            if (pLogOutput) _log.Debug(string.Format("GenDataCsv():{0}{1}{2}", Environment.NewLine, csvOutput, Environment.NewLine));

            return csvOutput;
        }
    }
}
