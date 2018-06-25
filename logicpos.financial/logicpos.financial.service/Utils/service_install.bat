@ECHO OFF

CLS

REM Uncoment to use Setup Service
SET SERVICE_PATH=c:\Program Files (x86)\Logicpulse\logicpos\LogicposFinancialService.exe

REM Uncoment to use Debug Service
REM SET SERVICE_PATH=c:\SVN\logicpos\trunk\src\logicpos\logicpos_pos_opensource\logicpos\bin\Debug\LogicposFinancialService.exe

SC create logicpulselogicposfinancialservice displayname="LogicPos Financial Service" binpath="\"%SERVICE_PATH%\"" start=auto

REM ECHO 
REM ECHO To Prevent : "O HTTP não conseguiu registar o URL http://+:50391/Service1.svc/."
REM ECHO Run logicpos.financial.service.exe with administrator privileges to add "urlacl"
REM ECHO and exit.
	
PAUSE
