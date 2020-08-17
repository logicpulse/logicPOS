# [LogicPOS OpenSource](https://logic-pos.com/)

Open source Point of Sale Solution


## Manual

- [Portuguese](http://help.logic-pos.com/)
- [English](http://help.logic-pos.com/en/)

### PDF - [Portuguese](https://bit.ly/2WcF1wN) / [English](https://bit.ly/348hxvz)

## TechStack

- [.NET C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [GTK#](https://www.gtk.org/)
- [FastReports](https://www.fast-report.com)
- [DevExpress eXpressPersistent Objects™ (XPO)](https://www.devexpress.com/products/net/orm/)
- Databases
	- [Microsoft SqlServer](https://www.microsoft.com/en-us/sql-server/sql-server-2017)
	- [MySql Server](https://dev.mysql.com/downloads/mysql/)
	- [Sqlite](https://sqlite.org/)

## Tools/Ides

- Visual Studio 2019 Community

## Directory Structure

- **docs** _(Minimal Documentation)_
- **libs** _(Reference Libs)_
	- **fastreport** _(FastReport Dll Reference)_
	- **xpo_14_1_5** _(Xpo Dll Reference)_
- **logicpos** _(LogicPos Main Application)_
- **logicpos.datalayer** _(DataLayer Project)_
- **logicpos.documentviewer** _(Document Viewer Project)_
	- **logicpos.documentviewer** _(Document Viewer Window Project )_
	- **logicpos.documentviewer.source** _(Document Viewer Library Project )_
- **logicpos.financial** _(Financial Projects)_
	- **logicpos.financial.console** _(Console Test Financial Project )_
	- **logicpos.financial.library** _(Financial Library Project )_
	- **logicpos.financial.service** _(Autoridade Tributária : WebService Financial Project )_
	- **logicpos.financial.servicewcf**_(Autoridade Tributária : Windows Communication Foundation WebService Project )_
- **logicpos.hardware** _(Hardware Projects )_
	- **logicpos.printer.generic**  _(Thermal Printer Base)_
	- **logicpos.printer.genericlinux** _(Thermal Printer Linux)_
	- **logicpos.printer.genericsocket** _(Thermal Printer Socket)_
	- **logicpos.printer.genericusb** _(Thermal Printer Usb)_
	- **logicpos.printer.genericwindows** _(Thermal Printer Windows)_
- **logicpos.plugins** _(Plugins Projects and Plugin Implementations)_
	- **logicpos.plugin.contracts** _(Plugin Contracts/Interfaces)_
	- **logicpos.plugin.library** _(Plugin Main Library)_
	- **Medsphere.Widgets** _(Draw Graphics Plugin)_
	- **plugins** _(Plugin Implementations)_
		- **acme** _(Sample Plugins)_
			- **acme.softwarevendor.plugin** _(Sample Plugin SoftwareVendor Implementation)_
- **logicpos.resources** _(Resources Project)_
- **logicpos.shared** _(main Solution Shared Library Project)_
- **others** _(Others)_
	- **windowsruntime** _(Windows Runtime/GTK)_
- **packages** _(NuGet Packages)_
	- **LibUsbDotNet.2.2.8** _(Used in Usb Hardware Devices)_
	- **log4net.2.0.5** _(Used in Log)_
	- **MSBuild.Microsoft.VisualStudio.Web.targets.14.0.0.3**  _(Required Dependency)_
	- **MySql.Data.6.9.9**  _(MySql Connector)_
	- **MySqlBackup.NET.2.0.9.4** _(MySql Backup)_
	- **Newtonsoft.Json.9.0.1** _(Used in Json Serialization)_
	- **System.Data.SQLite.Core.1.0.103** _(Sqlite Dependency)_
	- **Unofficial.Ionic.Zip.1.9.1.8** _(Zip Dependency, used in Backups)_
- **tools** _(Helper Tools)_
	- **FastReport.Net (FastReports Runtime Designer)** _(FastReports Designer Runtime)_

## Install Development Environment

- Install [.NET 4.5](https://www.microsoft.com/pt-pt/download/details.aspx?id=30653) _(If not already in system)_

- [Install Visual Studio 2017 Community](https://www.visualstudio.com)

- Install GTK Runtime [others\windowsruntime\gtk-sharp-2.12.22.msi](https://github.com/logicpulse/logicPOS/blob/master/others/windowsruntime/gtk-sharp-2.12.22.msi?raw=true)
	
	Read the [others\windowsruntime\README.txt](https://github.com/logicpulse/logicPOS/blob/master/others/windowsruntime/README.txt) to install `libnodoka.dll`

- Open Visual Studio 2017

	- Open Solution `logicpos.sln` or Folder Source Folder

	- Rebuild Solution

	- Rebuild Plugins ex `acme.softwarevendor.plugin`

	- Set **logicpos** as Startup Project
	
	- Set application settings on App.config

	- Run **logicpos** project
