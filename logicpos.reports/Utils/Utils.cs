using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.reports.App;
using System;

namespace logicpos.reports.Utils
{
    public class Utils
  {
    //Log4Net
    private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static XPSelectData GetAllNameTable()
    {
      string SQLDatabase;
      XPSelectData database;
      String sql = String.Empty;
      string nameDatabase;

      if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("SqlConnection"))
      {
        SQLDatabase = "SELECT getdate() AS Now;";
        database = FrameworkUtils.GetSelectedDataFromQuery(GlobalFramework.SessionXpo, SQLDatabase);
        foreach (SelectStatementResultRow name in database.Data)
        {
          nameDatabase = name.Values[0].ToString();
          sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
        }
      }
      else if (GlobalFramework.SessionXpo.Connection.GetType().Name.Equals("MySqlConnection"))
      {
        SQLDatabase = "SELECT DATABASE()";

        database = FrameworkUtils.GetSelectedDataFromQuery(GlobalFramework.SessionXpo, SQLDatabase);
        foreach (SelectStatementResultRow name in database.Data)
        {
          nameDatabase = name.Values[0].ToString();
          sql = string.Format("Select TABLE_NAME from information_schema.tables WHERE TABLE_SCHEMA = '{0}' ORDER BY TABLE_NAME", nameDatabase);
        }
      }
      else
      {
        sql = string.Format("select tbl_name as TABLE_NAME from sqlite_master group by tbl_name");
      }
      return FrameworkUtils.GetSelectedDataFromQuery(GlobalFramework.SessionXpo, sql);
    }
  }
}