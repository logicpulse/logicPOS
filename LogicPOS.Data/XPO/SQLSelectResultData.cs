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
        private readonly Dictionary<string, int> _fieldIndex;

        public SelectStatementResultRow[] MetaDataRows { get; set; }

        public SelectStatementResultRow[] DataRows {  get; set; }

        private string GetFieldNameFromRow(SelectStatementResultRow row)
        {
            return row.Values[0].ToString();
        }

        private bool RowNameIs(SelectStatementResultRow row, string name)
        {
            return GetFieldNameFromRow(row).ToUpper() == name.ToUpper();
        }

        public SQLSelectResultData(SelectedData selectedData)
        {
            MetaDataRows = selectedData.ResultSet[0].Rows;
            DataRows = selectedData.ResultSet[1].Rows;

            int i = 0;
            _fieldIndex = new Dictionary<string, int>();
            foreach (SelectStatementResultRow field in MetaDataRows)
            {
                _fieldIndex.Add(field.Values[0].ToString(), i++);
            }
        }

        public int GetFieldIndexFromName(string fieldName)
        {
            return _fieldIndex[fieldName];
        }

        public object GetValueFromField(
            string searchField, 
            string returnField, 
            string searchValue)
        {
            foreach (SelectStatementResultRow metaDataRow in MetaDataRows)
            {
                if (RowNameIs(metaDataRow,searchField))
                {
                  
                    foreach (SelectStatementResultRow datRow in DataRows)
                    {
                        var fieldName = GetFieldNameFromRow(metaDataRow);
                        var field = datRow.Values[GetFieldIndexFromName(fieldName)];

                        if (field != null && searchValue.ToUpper() == field.ToString().ToUpper())
                        {
                          
                            return datRow.Values[GetFieldIndexFromName(returnField)];
                        }
                    }
                }
            }

            return null;
        }

        public object GetXPGuidObjectFromField(
            Type type, 
            string searchField, 
            string searchValue)
        {
            return GetXPGuidObjectFromField(
                XPOSettings.Session, 
                type, 
                searchField, 
                searchValue);
        }

        private object GetXPGuidObjectFromField(
            Session session, 
            Type type, 
            string searchField, 
            string searchValue)
        {
            Guid id = new Guid(GetValueFromField(
                searchField, 
                "Oid", 
                searchValue).ToString());

            return XPOUtility.GetXPGuidObject(
                session, 
                type, 
                id);
        }
    }
}
