using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Text;

namespace PCComm
{
    public class CommunicationManager
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Manager Enums
        /// <summary>
        /// enumeration to hold our transmission types
        /// </summary>
        public enum TransmissionType { Text, Hex }

        /// <summary>
        /// enumeration to hold our message types
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };
        #endregion

        #region Manager Variables
        //property variables
        private string _portName = string.Empty;

        //private RichTextBox _displayWindow;
        //global manager variables
        private readonly Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private readonly SerialPort comPort = new SerialPort();
        #endregion

        #region Manager Properties
        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public string BaudRate { get; set; } = string.Empty;

        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public string Parity { get; set; } = string.Empty;

        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public string StopBits { get; set; } = string.Empty;

        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public string DataBits { get; set; } = string.Empty;

        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        /// <summary>
        /// property to hold our TransmissionType
        /// of our manager class
        /// </summary>
        public TransmissionType CurrentTransmissionType { get; set; }

        /// <summary>
        /// property to hold our display window
        /// value
        /// </summary>
        //public RichTextBox DisplayWindow
        //{
        //    get { return _displayWindow; }
        //    set { _displayWindow = value; }
        //}
        #endregion

        #region Manager Constructors
        /// <summary>
        /// Constructor to set the properties of our Manager Class
        /// </summary>
        /// <param name="baudRate">Desired BaudRate</param>
        /// <param name="parity">Desired Parity</param>
        /// <param name="stopBits">Desired StopBits</param>
        /// <param name="dataBits">Desired DataBits</param>
        /// <param name="portName">Desired PortName</param>
        public CommunicationManager(string baudRate, string parity, string stopBits, string dataBits, string portName)
        {
            BaudRate = baudRate;
            Parity = parity;
            StopBits = stopBits;
            DataBits = dataBits;
            _portName = portName;
            // Removed we must handle event outside of this class, to receive data in context
            //now add an event handler
            //comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }

        /// <summary>
        /// Comstructor to set the properties of our
        /// serial port communicator to nothing
        /// </summary>
        public CommunicationManager()
        {
            BaudRate = string.Empty;
            Parity = string.Empty;
            StopBits = string.Empty;
            DataBits = string.Empty;
            _portName = "COM6";
            //_displayWindow = null;
            //add event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }
        #endregion

        #region WriteData
        public void WriteData(string msg)
        {
            switch (CurrentTransmissionType)
            {
                case TransmissionType.Text:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(comPort.IsOpen == true)) comPort.Open();
                    //send the message to the port
                    comPort.Write(msg);
                    //display the message
                    DisplayData(MessageType.Outgoing, msg + "\n");
                    break;
                case TransmissionType.Hex:
                    try
                    {
                        //convert the message to byte array
                        byte[] newMsg = HexToByte(msg);
                        //send the message to the port
                        comPort.Write(newMsg, 0, newMsg.Length);
                        //convert back to hex and display
                        DisplayData(MessageType.Outgoing, ByteToHex(newMsg) + "\n");
                    }
                    catch (FormatException ex)
                    {
                        //display error message
                        DisplayData(MessageType.Error, ex.Message);
                    }
                    finally
                    {
                        //_displayWindow.SelectAll();
                    }
                    break;
                default:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(comPort.IsOpen == true)) comPort.Open();
                    //send the message to the port
                    comPort.Write(msg);
                    //display the message
                    DisplayData(MessageType.Outgoing, msg + "\n");
                    break;
            }
        }
        #endregion

        #region HexToByte
        /// <summary>
        /// method to convert hex string into a byte array
        /// </summary>
        /// <param name="msg">string to convert</param>
        /// <returns>a byte array</returns>
        private byte[] HexToByte(string msg)
        {
            //remove any spaces from the string
            msg = msg.Replace(" ", "");
            //create a byte array the length of the
            //divided by 2 (Hex is 2 characters in length)
            byte[] comBuffer = new byte[msg.Length / 2];
            //loop through the length of the provided string
            for (int i = 0; i < msg.Length; i += 2)
                //convert each set of 2 characters to a byte
                //and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
            //return the array
            return comBuffer;
        }
        #endregion

        #region ByteToHex
        /// <summary>
        /// method to convert a byte array into a hex string
        /// </summary>
        /// <param name="comByte">byte array to convert</param>
        /// <returns>a hex string</returns>
        private string ByteToHex(byte[] comByte)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            //loop through each byte in the array
            foreach (byte data in comByte)
                //convert the byte to a string and add to the stringbuilder
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            //return the converted value
            return builder.ToString().ToUpper();
        }
        #endregion

        #region DisplayData
        /// <summary>
        /// method to display the data to & from the port
        /// on the screen
        /// </summary>
        /// <param name="type">MessageType of the message</param>
        /// <param name="msg">Message to display</param>
        [STAThread]
        private void DisplayData(MessageType type, string msg)
        {
            string outMsg = msg.Replace("\n", string.Empty);
            //_logger.Debug($"MessageType: [{type}], outMsg: [{outMsg}]");

            //    _displayWindow.Invoke(new EventHandler(delegate
            //{
            //    _displayWindow.SelectedText = string.Empty;
            //    _displayWindow.SelectionFont = new Font(_displayWindow.SelectionFont, FontStyle.Bold);
            //    _displayWindow.SelectionColor = MessageColor[(int)type];

            //    _displayWindow.AppendText(msg);
            if (msg.Substring(0, 2) == "99")
            {
                //_displayWindow.AppendText(CalculateFromHex(msg));
                List<int> result = CalculateFromHex(msg);
                //_logger.Debug($"Weight: [{result[0]}], Total: [{result[1]}]");
            }
            //    _displayWindow.ScrollToCaret();
            //}));
        }
        #endregion

        #region OpenPort
        public bool OpenPort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (comPort.IsOpen == true) comPort.Close();

                //set the properties of our SerialPort Object
                comPort.BaudRate = int.Parse(BaudRate);    //BaudRate
                comPort.DataBits = int.Parse(DataBits);    //DataBits
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBits);    //StopBits
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), Parity);    //Parity
                comPort.PortName = _portName;   //PortName
                //now open the port
                comPort.Open();
                //display message
                //DisplayData(MessageType.Normal, "Port opened at " + DateTime.Now + "\n");
                _logger.Debug(string.Format("Port {0} opened at {1}", _portName, DateTime.Now));
                //return true
                return true;
            }
            catch (Exception ex)
            {
                //DisplayData(MessageType.Error, ex.Message);
                //return false;
                // Forward Exception to me handled Outside
                throw ex;
            }
        }
        #endregion

        #region OpenDisplayPort
        public bool OpenDisplayPort(string port)
        {
            try
            {
                comPort.PortName = port;
                //first check if the port is already open
                //if its open then close it
                if (comPort.IsOpen == true) comPort.Close();

                //set the properties of our SerialPort Object
                //comPort.BaudRate = int.Parse(_baudRate);    //BaudRate
                //comPort.DataBits = int.Parse(_dataBits);    //DataBits
                //comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);    //StopBits
                //comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);    //Parity                
                //now open the port
                comPort.Open();
                //display message
                //DisplayData(MessageType.Normal, "Port opened at " + DateTime.Now + "\n");
                _logger.Debug(string.Format("Port {0} opened at {1}", port, DateTime.Now));
                //return true
                return true;
            }
            catch (Exception)
            {
                //DisplayData(MessageType.Error, ex.Message);
                return false;
                // Forward Exception to me handled Outside
                //throw ex;
            }
        }
        #endregion

        #region ClosePort
        public bool ClosePort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (comPort.IsOpen == true) comPort.Close();
                //return true
                return true;
            }
            catch (Exception ex)
            {
                DisplayData(MessageType.Error, ex.Message);
                return false;
            }
        }
        #endregion

        #region IsPortOpen
        public bool IsPortOpen()
        {
            return comPort.IsOpen;
        }
        #endregion

        #region ComPort
        public SerialPort ComPort()
        {
            return comPort;
        }
        #endregion

        #region comPort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //determine the mode the user selected (binary/string)
            switch (CurrentTransmissionType)
            {
                //user chose string
                case TransmissionType.Text:
                    //read data waiting in the buffer
                    string msg = comPort.ReadExisting();
                    //display the data to the user
                    DisplayData(MessageType.Incoming, msg + "\n");
                    break;
                //user chose binary
                case TransmissionType.Hex:

                    string inData = comPort.ReadLine();
                    DisplayData(MessageType.Incoming, inData + "\n");

                    ////retrieve number of bytes in the buffer
                    //int bytes = comPort.BytesToRead;
                    ////create a byte array to hold the awaiting data
                    //byte[] comBuffer = new byte[bytes];
                    ////read the data and store it
                    //comPort.Read(comBuffer, 0, bytes);
                    ////display the data to the user
                    //DisplayData(MessageType.Incoming, ByteToHex(comBuffer) + "\n");
                    break;
                default:
                    //read data waiting in the buffer
                    string str = comPort.ReadExisting();
                    //display the data to the user
                    DisplayData(MessageType.Incoming, str + "\n");
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 99: 0x39h y 0x39h
        /// S: Estado del peso. S: 0x30h Correcto. S: 0x31h Error.
        /// WWWWW: 5 dígitos para el PESO.
        /// E: Estado del importe. E: 0x30h Correcto. E: 0x31h Error.
        /// IIIIII: 6 dígitos para el importe.
        /// </summary>
        /// <param name="resultHex"></param>
        /// <returns></returns>
        public List<int> CalculateFromHex(string result)
        {
            List<int> resultList = new List<int>();
            string S = result.Substring(2, 1);
            //if (S =="0")
            string WWWWW = result.Substring(3, 5);

            string E = result.Substring(9, 1);
            //if (E == "0")
            string IIIIII = result.Substring(10, 6);
            resultList.Add(Convert.ToInt16(WWWWW));
            // Sanitize : Must Remove last Charcters like detected ones [= >]
            //resultList.Add(Convert.ToInt16(IIIIII.Replace("=", string.Empty)));
            IIIIII = IIIIII.Substring(0, IIIIII.Length - 1);
            resultList.Add(Convert.ToInt16(IIIIII));

            //return "Peso -" + WWWWW + "g --- Preço - " + IIIIII;
            return resultList;
        }

    }
}
