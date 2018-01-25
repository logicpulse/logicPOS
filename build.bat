@ECHO OFF
CLS

REM NOTES
REM Fix Error
REM "C:\SVN\logicpos\trunk\src\WSInterface\logicposWSInterface.csproj" (Clean destino) (3) ->
REM C:\SVN\logicpos\trunk\src\WSInterface\logicposWSInterface.csproj(149,3): error MSB4019: Não foi possível localizar o
REM projecto importado "C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v11.0\WebApplications\Microsoft.WebApplicatio
REM n.targets". Confirme se o caminho na declaração <Import> está correcto e se o ficheiro existe no disco.
REM 
REM Microsoft.WebApplication.targets was not found, on the build server. What's your solution?
REM http://stackoverflow.com/questions/3980909/microsoft-webapplication-targets-was-not-found-on-the-build-server-whats-your
REM USED IN Projects
REM 
REM WSInterface
REM logicpos.financial.servicewcf



REM "C:\SVN\logicpos\trunk\src\logicpos.resources\logicpos.resources.csproj" (destino predefinido) (7:4) ->
REM   C:\Windows\Microsoft.NET\Framework\v4.0.30319\Microsoft.Common.targets(2863,5): error MSB3086: A tarefa não conseguiu
REM  localizar "AL.exe" utilizando o valor de SdkToolsPath "" ou a chave de registo "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\
REM Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools-x86". Certifique-se de que SdkToolsPath está definido, de que a ferram
REM enta existe na localização específica do processador correcta sob o SdkToolsPath e de que o Microsoft Windows SDK está
REM instalado [C:\SVN\logicpos\trunk\src\logicpos.resources\logicpos.resources.csproj]
REM 
REM "c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\al.exe" 


REM REMOVE ToolsVersion="x.0" node
REM logicpos.virtualprinter.csproj
REM logicpos.resources.csproj
REM logicpos.financial.csproj


REM Based on this post here you can simply download the Microsoft Visual Studio 2010 Shell (Integrated) Redistributable Package and the targets are installed.

REM This avoids the need to install Visual Studio on the build server.

REM I have just tried this out now, and can verify that it works:

REM Before:

REM error MSB4019: The imported project "C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v10.0\WebApplications\Microsoft.WebApplication.targets" was not found. Confirm that the path in the declaration is correct, and that the file exists on disk.

REM https://www.microsoft.com/en-us/download/details.aspx?id=1366

REM c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "C:\SVN\logicpos\trunk\src\logicpos.sln" /t:Clean;Build /p:Configuration=Release

REM c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "c:\SVN\logicpos\trunk\src\logicpos.framework\logicpos.virtualprinter\logicpos.virtualprinter.csproj" /t:Clean;Build /p:Configuration=Release
REM c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "C:\SVN\logicpos\trunk\src\logicpos.framework\logicpos.financial.console\logicpos.financial.console.csproj" /t:Clean;Build /p:Configuration=Release

c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "c:\SVN\logicpos\trunk\src\logicpos.framework\logicpos.financial\logicpos.financial.csproj" /t:Clean;Build /p:Configuration=Release

PAUSE