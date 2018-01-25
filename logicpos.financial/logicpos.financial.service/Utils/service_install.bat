@ECHO OFF

CLS
REM from SERVICE_NAME = "LogicPulse LogicPos Financial Service";
SC create logicpulselogicposfinancialservice displayname= "LogicPulse LogicPos Financial Service" binpath= "\"c:\SVN\logicpos\trunk\src\logicpos_pos\logicpos.framework\logicpos.financial.service\bin\Debug\logicpos.financial.service.exe\"" start= auto

REM ECHO 
REM ECHO To Prevent : "O HTTP não conseguiu registar o URL http://+:50391/Service1.svc/."
REM ECHO Run logicpos.financial.service.exe with administrator privilges to add "urlacl"
REM ECHO and exit.
	
PAUSE
