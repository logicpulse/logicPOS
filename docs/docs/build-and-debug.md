Source Code Config > `logicpos\App.config`

!!! note "Note"
    If we are building from source code we edit `logicpos\App.config` if we are working with install version or build version we change `logicpos\bin\Debug\logicpos.exe.config`


Here we are working with Source Code  `logicpos\App.config`







# Logicpulse LogicPos : Build and Debug LogicPos Quick Setup Notes

## Open Solution in Visual Studio 2015/2017

!!! warning "Warning"
    Under Construction...

---
## How to Build and Debug `logicpos.financial.service` Project

1. First Check if certificates are in `logicpos.financial.service\Resources\Certificates` directory

```
logicpos.framework\logicpos.financial.service\Resources\Certificates\508278155.pfx
logicpos.framework\logicpos.financial.service\Resources\Certificates\508278155_1115.cer
logicpos.framework\logicpos.financial.service\Resources\Certificates\TesteWebService.pfx
logicpos.framework\logicpos.financial.service\Resources\Certificates\ChavePublicaAT.cer
```

2. Next set `logicpos.financial.service` project **Set as Startup Project**

3. Run project

4. Press [Y or Enter] to launch WebService in **interactive develop/debug**

```
BootStrap Framework Console Service Project....
ServicesATDCTestMode: [True]
Launch service? [Y or Enter] or any other key to run in interactive develop/debug mode

...

pressed y
...

The service is ready at http://localhost:50391/Service1.svc
Press any key to stop the service and exit
```

!!! note"Note"
    `ServicesATDCTestMode: [True]` : We are working on SandBox/Test Mode

5. Launch LogicPos `logicpos_pos\logicpos\bin\Debug\logicpos.exe` and Make some Financial/WayBill Documents

6. If everything is working we can see in console

**Simplified Invoice**

```
Send Document DocumentNumber: [FS FS002012018S01/27]/WayBillMode: [False]
Cert Subject: [CN=TesteWebServices, OU=ASI, O=AT - Autoridade Tributaria e Aduaneira, L=Lisboa, S=Lisboa, C=PT], NotBefore: [02/08/2017 15:06:39], NotAfter: [29/01/2018 14:06:39]
Send Document ReturnCode: [0]
Send Document ReturnMessage: [Documento registado com sucesso.]
```

**WayBill**

```
Send Document DocumentNumber: [GT GT002012018S01/4]/WayBillMode: [True]
Cert Subject: [CN=TesteWebServices, OU=ASI, O=AT - Autoridade Tributaria e Aduaneira, L=Lisboa, S=Lisboa, C=PT], NotBefore: [02/08/2017 15:06:39], NotAfter: [29/01/2018 14:06:39]
Send Document ReturnCode: [0]
Send Document ReturnMessage: [OK]
```

!!! note "Note"
    WayBills from FinalConsumer are never sent to WebService



when build from source code

**Location of Log File**

```
logicpos.framework\logicpos.financial.service\bin\Debug\logicpos.financial.service.log 
```

---
## To Enable/Disable AT Webservice 

To Enable/Disable sending documents to AT, we can change above parameters in config

```xml
<!-- AT Web Services -->
<add key="serviceATSendDocuments" value="true" />
<add key="serviceATSendDocumentsWayBill" value="true" />
```

---
## Debug AT WebService

To Debug AT WebService, we debug `logicpulse.financial.service` and launch **logicpos** from bin

Dados usados

https://faturas.portaldasfinancas.gov.pt/testarLigacaoWebService.action

passwordCertificate = "TESTEwebservice";
taxRegistrationNumber = "599999993";
accountFiscalNumber = "599999993/0037";
accountPassword = "testes1234";


Download 

https://faturas.portaldasfinancas.gov.pt/factemipf_static/java/certificados.zip



public static Dictionary<FIN_DocumentFinanceMaster, ServicesATSoapResult> ServiceSendPendentDocuments()



        //ATWS: Send Document to AT WebWebService
        public static ServicesATSoapResult SendDocumentToATWS(FIN_DocumentFinanceMaster pDocumentFinanceMaster)
        ...
        bool sendDocumentToATWS = SendDocumentToATWSEnabled(pDocumentFinanceMaster);




logicpos\

logicpos.framework\
  logicpos.financial\
  logicpos.financial.console\
  logicpos.financial.service\
  logicpos.financial.servicewcf\

logicpos.hardware\
  printer.generic\
  printer.genericlinux\
  printer.genericsocket\
  printer.genericusb\
  printer.genericwindows\

logicpos.plugins\
  PluginContracts\
  PluginLibrary\
  Plugins\
    FirstPlugin\
    LicenceManager\
    PluginModuleAImplA\
    PluginModuleAImplB\
    SecondPlugin\  

logicpos.resources\
