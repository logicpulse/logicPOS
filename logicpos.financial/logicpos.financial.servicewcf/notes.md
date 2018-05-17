---
### WCF Service Application > logicpos.financial.servicewcf

1) Add WCF Service Application to Solution > "logicpos.financial.servicewcf"

2) Run it and get PORT ex 50391

3) Add "logicpos.financial.service" project reference to it

---
### Windows Service and Main Project > logicpos.financial.service

4) Add References 

	System.ServiceModel, 
	System.ServiceProcess, 
	System.ServiceModel.Web,
	System.Runtime.Serialization
	to logicpos.financial.service
	
5) Add to logicpos.framework\logicpos.financial.service\Program.cs	

    class Program
    {
		...
        //Service Private Members
		public static string SERVICE_NAME = "LogicPulse LogicPos Financial Service";
		private static ServiceHost _serviceHost;
        private static Uri _baseAddress;
        private static int _servicePort = 50391;
        public static int ServicePort
        {
            get { return Program._servicePort; }
        } 
		...
				
        static void Main(string[] args)
        {
            //Init Settings Main Config Settings
            GlobalFramework.Settings = ConfigurationManager.AppSettings;

            //Base Bootstrap Init from LogicPos
            Init();

            //Service Initialization
            string uri = string.Format("http://localhost:{0}/Service1.svc", _servicePort);
            _log.Debug(string.Format("Service URI: {0}", uri));
            _baseAddress = new Uri(uri);

            //Service Mode
            if (!Environment.UserInteractive)
            {
                // Running as service
                using (var service = new Service())
                {
                    _log.Debug("Service.Run(service)");
                    Service.Run(service);
                }
            }
            //Console Mode
            else
            {
                Console.WriteLine("Launch service? [Y/n] or any other key to run in interactive develop mode");
                ConsoleKeyInfo cki = Console.ReadKey();

                //Service Mode
                if (cki.Key.ToString().ToUpper() == "y".ToUpper() || cki.Key.ToString() == "Enter")
                {
                    // Running as console app
                    Start(args);

                    Console.Clear();
                    Console.WriteLine("The service is ready at {0}", _baseAddress);
                    Console.WriteLine("Press any key to stop the service and exit");
                    Console.ReadKey();
                    Stop();
                }
                //Interactive develop mode
                else
                {
                    //Init Test Actions
                    InitTestActions();
                    //Init Main
                    InitMain();
                }
            }
        }
		...
		bottom of class
			
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Service Methods

        public static void Start(string[] args)
        {
            _log.Debug("Service Started");

            //Call ModifyHttpSettings
            Utils.ModifyHttpSettings();

            //Init ServiceHost
            _serviceHost = new ServiceHost(typeof(Service1), _baseAddress);

            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            _serviceHost.Description.Behaviors.Add(smb);

            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each service contract implemented
            // by the service.
            _serviceHost.Open();
        }

        public static void Stop()
        {
            // onstop code here
            _log.Debug("Service Stoped");
            // Close the ServiceHost.
            _serviceHost.Close();
        }			

6) Add ModifyHttpSettings() to logicpos.financial.service\Objects\Utils\Utils.cs
	
	and use port from Program.ServicePort

7) Add Service Class Service logicpos.financial.service\Objects\Service\Service.cs

    class Service : ServiceBase
    {
        public Service()
        {
            ServiceName = Program.SERVICE_NAME;
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            Program.Start(args);
        }

        protected override void OnStop()
        {
            Program.Stop();
        }
    }

8) Now add Reference logicpos.financial.servicewcf to logicpos.financial.service to use Service1 and IService1 class 
	used in _serviceHost = new ServiceHost(typeof(Service1), _baseAddress);
	
9) Now move 
	logicpos.framework\logicpos.financial.servicewcf\IService1.cs
	logicpos.framework\logicpos.financial.servicewcf\Service1.svc
	logicpos.framework\logicpos.financial.servicewcf\Service1.svc.cs

	to 

	logicpos.financial.service\Objects\Service\IService1.cs
	logicpos.financial.service\Objects\Service\Service1.svc
	logicpos.financial.service\Objects\Service\Service1.svc.cs	
	
	else we have circular references
	
10)	Put a breakpoint in 
	if (!Environment.UserInteractive)
	and run logicpos.financial.service
	
	Additional information: O HTTP não conseguiu registar o URL http://+:50391/Service1.svc/. O processo não tem direitos de acesso a este espaço de nomes (consulte http://go.microsoft.com/fwlink/?LinkId=70353 para obter detalhes).
	
	Now run logicpos.financial.service.exe with admin privilges outside VS to use Utils.ModifyHttpSettings() to add "urlacl"
	
	After Run
	
	Test with http://localhost:50391/Service1.svc
	
	Ok in Console Mode Outside VS, now test it in VS after add "urlacl", ok done inside VS too
	
11) Add Services scripts

	logicpos.financial.service\Utils\service_install.bat

		@ECHO OFF
		CLS
		REM from SERVICE_NAME = "LogicPulse LogicPos Financial Service";
		SC create logicpulselogicposfinancialservice displayname= "LogicPulse LogicPos Financial Service" binpath= "\"c:\SVN\logicpos\trunk\src\logicpos.framework\logicpos.financial.service\bin\Debug\logicpos.financial.service.exe\"" start= auto
		PAUSE 
	
	logicpos.financial.service\Utils\service_start.bat
	
		@ECHO OFF
		CLS
		SC start logicpulselogicposfinancialservice
		PAUSE 
	
	logicpos.financial.service\Utils\service_stop.bat
	
		@ECHO OFF
		CLS
		SC stop logicpulselogicposfinancialservice
		PAUSE 	
		
	logicpos.financial.service\Utils\service_uninstall.bat	
	
		@ECHO OFF
		CLS
		SC delete logicpulselogicposfinancialservice
		PAUSE 
	
	run logicpos.financial.service\Utils\service_install.bat and test
	
	done
	
---
###	Logicpos.Financial

12) Start Service or Console mode logicpos.financial.service.exe

	add References > Add Service Reference > http://localhost:50391/Service1.svc
	with NameSpace ServiceReference
	
	Done it add to App.config
	
	```
  <!--WebService-->
  <system.serviceModel>
    <bindings>
      <basicHttpBinding>
        <binding name="BasicHttpBinding_IService1" />
      </basicHttpBinding>
    </bindings>
    <client>
      <endpoint address="http://localhost:50391/Service1.svc" binding="basicHttpBinding"
        bindingConfiguration="BasicHttpBinding_IService1" contract="ServiceReference.IService1"
        name="BasicHttpBinding_IService1" />
    </client>
  </system.serviceModel>	
	```
	
	use it that way

	```
	string endpointAddress = FrameworkUtils.GetEndpointAddress();
	if (FrameworkUtils.IsWebServiceOnline(endpointAddress))
	{
			try
			{
					//Init Client
					Service1Client serviceClient = new Service1Client();
					//GetData
					var result = serviceClient.GetData(14);
					_log.Debug(string.Format("Test GetData Service Result: {0}", result));
					//SendDocuments
					var resultResultObject = serviceClient.SendDocuments(new Guid("c708ad97-a5fb-46ee-b241-a73cb13242ac"));
					_log.Debug("Test AT WebService ResultObject:");
					_log.Debug(string.Format("ReturnCode: {0}", resultResultObject.ReturnCode));
					_log.Debug(string.Format("ReturnMessage: {0}", resultResultObject.ReturnMessage));
					_log.Debug(string.Format("ReturnRaw: {0}", resultResultObject.ReturnRaw));
					// Always Close Client
					serviceClient.Close();
			}
			catch (Exception ex)
			{
					_log.Error(ex.Message, ex);
			}
	}
	else
	{
			_log.Debug(string.Format("EndpointAddress OffLine, Please check URI: {0}", endpointAddress));
	}
	```
