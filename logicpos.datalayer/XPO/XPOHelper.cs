using System;

namespace logicpos.datalayer.Xpo
{
    public static class XPOHelper
    {
        public static uint GetNextTableFieldID(string table, string field)
        {
            uint result = 0;

            string sql = string.Format("SELECT MAX({0}) FROM {1};", field, table);

            var resultInt = XPOSettings.Session.ExecuteScalar(sql);
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
