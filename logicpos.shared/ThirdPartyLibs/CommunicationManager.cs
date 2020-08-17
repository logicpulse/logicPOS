using System;
using System.Text;
using System.Drawing;
using System.IO.Ports;
using System.Globalization;
using System.Collections.Generic;
//*****************************************************************************************
//                           LICENSE INFORMATION
//*****************************************************************************************
//   PCCom.SerialCommunication Version 1.0.0.0
//   Class file for managing serial port communication
//
//   Copyright (C) 2007  
//   Richard L. McCutchen 
//   Email: richard@psychocoder.net
//   Created: 20OCT07
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
//*****************************************************************************************
namespace PCComm
{
    public class CommunicationManager
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        private string _baudRate = string.Empty;
        private string _parity = string.Empty;
        private string _stopBits = string.Empty;
        private string _dataBits = string.Empty;
        private string _portName = string.Empty;
        private TransmissionType _transType;
        //private RichTextBox _displayWindow;
        //global manager variables
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private SerialPort comPort = new SerialPort();
        #endregion

        #region Manager Properties
        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public string StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public string DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

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
        public TransmissionType CurrentTransmissionType
        {
            get { return _transType; }
            set { _transType = value; }
        }

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
            _baudRate = baudRate;
            _parity = parity;
            _stopBits = stopBits;
            _dataBits = dataBits;
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
            _baudRate = string.Empty;
            _parity = string.Empty;
            _stopBits = string.Empty;
            _dataBits = string.Empty;
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

        string command = string.Empty;

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
            //_log.Debug($"MessageType: [{type}], outMsg: [{outMsg}]");

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
                //_log.Debug($"Weight: [{result[0]}], Total: [{result[1]}]");
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
                comPort.BaudRate = int.Parse(_baudRate);    //BaudRate
                comPort.DataBits = int.Parse(_dataBits);    //DataBits
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);    //StopBits
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);    //Parity
                comPort.PortName = _portName;   //PortName
                //now open the port
                comPort.Open();
                //display message
                //DisplayData(MessageType.Normal, "Port opened at " + DateTime.Now + "\n");
                _log.Debug(string.Format("Port {0} opened at {1}", _portName, DateTime.Now));
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
                _log.Debug(string.Format("Port {0} opened at {1}", port, DateTime.Now));
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

        #region SetParityValues
        public void SetParityValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(Parity)))
            {
                //((ComboBox)obj).Items.Add(str);
                _log.Debug($"SetParityValues: {str}");
            }
        }
        #endregion

        #region SetStopBitValues
        public void SetStopBitValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(StopBits)))
            {
                //((ComboBox)obj).Items.Add(str);
                _log.Debug($"SetStopBitValues: {str}");
            }
        }
        #endregion

        #region SetPortNameValues
        public void SetPortNameValues(object obj)
        {
            foreach (string str in SerialPort.GetPortNames())
            {
                //((ComboBox)obj).Items.Add(str);
                _log.Debug($"SetPortNameValues: {str}");
            }
        }
        #endregion

        #region comPort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
        /// 98: 0x38h y 0x39h
        /// PPPPP: 5 dígitos para el precio.
        /// C: Checksum, suma lógica (XOR) de todos los caracteres anteriores.
        /// CR: 0x0Dh LF: 0x0Ah
        /// </summary>
        /// <param name="pesoHex"></param>
        /// <returns></returns>
        public string CalculateHexFromPrice(string pesoHex)
        {
            string checsum = Convert.ToString(0x39 ^ 0x38
                ^ int.Parse(pesoHex.Substring(0, 2), NumberStyles.HexNumber)
                ^ int.Parse(pesoHex.Substring(2, 2), NumberStyles.HexNumber)
                ^ int.Parse(pesoHex.Substring(4, 2), NumberStyles.HexNumber)
                ^ int.Parse(pesoHex.Substring(6, 2), NumberStyles.HexNumber)
                ^ int.Parse(pesoHex.Substring(8, 2), NumberStyles.HexNumber)
                , 2); //35

            checsum = Convert.ToInt32(checsum, 2).ToString("X");

            string send = "3938 " + pesoHex + checsum + "0D0A";

            return send;
        }
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

            string WWWWW = string.Empty;
            string IIIIII = string.Empty;

            string S = result.Substring(2, 1);
            //if (S =="0")
            WWWWW = result.Substring(3, 5);

            string E = result.Substring(9, 1);
            //if (E == "0")
            IIIIII = result.Substring(10, 6);

            resultList.Add(Convert.ToInt16(WWWWW));
            // Sanitize : Must Remove last Charcters like detected ones [= >]
            //resultList.Add(Convert.ToInt16(IIIIII.Replace("=", string.Empty)));
            IIIIII = IIIIII.Substring(0, IIIIII.Length -1);
            resultList.Add(Convert.ToInt16(IIIIII));

            //return "Peso -" + WWWWW + "g --- Preço - " + IIIIII;
            return resultList;
        }

        /// <summary>
        /// Convert string to Hex
        /// </summary>
        /// <param name="str"></param>
        /// <returns>"48656C6C6F20776F726C64" for "Hello world"</returns>
        public string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.ASCII.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert Hex to string
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns>"Hello world" for "48656C6C6F20776F726C64"</returns>
        public string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.ASCII.GetString(bytes);
        }
    }
}
