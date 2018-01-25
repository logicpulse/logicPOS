1) Instalar .NET 4.5

   download http://box.logicpulse.pt/files/private/logicpos/NDP452-KB2901907-x86-x64-AllOS-ENU.exe
	 instalarNDP452-KB2901907-x86-x64-AllOS-ENU.exe

2) Instalar "GTK e Theme Nodoka"

	 download http://box.logicpulse.pt/files/private/logicpos/windowsruntime.zip
   instalar gtk-sharp-2.12.22.msi

3) Copiar o file libnodoka.dll (theme engine) para 
   "c:\Program Files\GtkSharp\2.12\lib\gtk-2.0\2.10.0\engines\" (32bit), or 
   "c:\Program Files (x86)\GtkSharp\2.12\lib\gtk-2.0\2.10.0\engines\" (64bit)

4) Restart do windows

5) Instalar o LogicPOS
	 
	 download a partir de http://box.logicpulse.pt/

	 Instalar

	 Usando a Base de Dados MySQL ou MSSqlServer
   ex usando o MySql server
   
   Instalar MySql Server, com o utilizador un: root & pass: adminx
     
   Se instalado manualmente a partir da versão debug
   alterar a ConnectionString em logicpos.exe.config (xpoConnectionString)
   para por ex
   
   <add key="databaseType" value="MySql" />
   <add key="xpoConnectionString" value="XpoProvider=MySql;server=localhost;database={0};user id=root;password=adminx;persist security info=true;CharSet=utf8;" />
   
   se instalado a partir do setup, usar na configuração da base de dados
   
   Tipo: MySql
   Servidor: localhost
   Login: root
   Password: adminx
   Base de Dados: logicposdb (pode alterar)

5) Arrancar com o LogicPos e esperar enquanto cria a base de dados

