using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.XPO
{
    public static class XPOHelper
    {
        public static Session Session;

        public static uint GetNextTableFieldID(string table, string field)
        {
            uint result = 0;

            string sql = string.Format("SELECT MAX({0}) FROM {1};", field, table);

            var resultInt = DataLayerFramework.SessionXpo.ExecuteScalar(sql);
            if (resultInt != null)
            {
                result = Convert.ToUInt32(resultInt) + 1;

                while (!("" + result).EndsWith("0"))
                {
                    result++;
                }
            }

            if (result <= 0)
            {
                result = 10;
            }

            return result;
        }
    }
}
