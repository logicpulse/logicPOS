#### Database

WSInterface\Web.config

  <connectionStrings>
    <add name="WSInterface" connectionString="XpoProvider=MSSqlServer;Data Source=lpdev\sql2008;Initial Catalog=logicposdb_mn_20160211;User ID=sa;Password=admin#"/>
  </connectionStrings>

WSInterfaceTester\App.config

	<add key="LogicposWS_Login" value="admin"/>
	<add key="LogicposWS_Password" value="admin"/>

public class LogicposHelper
	internal static string GetSecurityToken(string pLogin, string pPassword)
	select Oid from UserDetail where login = 'admin' and password = 'admin';
	
	GlobalFramework.SessionXpo.ConnectionString
	"Data Source=lpdev\\sql2008;Initial Catalog=logicposdb_mn_20160211;User ID=sa;"
