@ECHO OFF
CLS 
ECHO Use "TESTEwebservice" Password
REM -p TESTEwebservice
certutil -dump TesteWebService.pfx  > TesteWebService.info
pause
