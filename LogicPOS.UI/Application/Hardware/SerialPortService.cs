using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace LogicPOS.Utility
{
    public class SerialPortService
    {
        public enum TransmissionType { Text, Hex }

        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };

        private readonly SerialPort _comPort = new SerialPort();
 
        public string BaudRate { get; set; } = string.Empty;
        public string Parity { get; set; } = string.Empty;
        public string StopBits { get; set; } = string.Empty;
        public string DataBits { get; set; } = string.Empty;
        public string PortName { get; set; } = string.Empty;
        public TransmissionType CurrentTransmissionType { get; set; }

        public SerialPortService(
            string baudRate, 
            string parity, 
            string stopBits, 
            string dataBits, 
            string portName)
        {
            BaudRate = baudRate;
            Parity = parity;
            StopBits = stopBits;
            DataBits = dataBits;
            PortName = portName;
        }

        public SerialPortService()
        {
            BaudRate = string.Empty;
            Parity = string.Empty;
            StopBits = string.Empty;
            DataBits = string.Empty;
            PortName = "COM6";
            _comPort.DataReceived += new SerialDataReceivedEventHandler(ComPort_DataReceived);
        }

        public void WriteData(string msg)
        {
            switch (CurrentTransmissionType)
            {
                case TransmissionType.Text:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(_comPort.IsOpen == true)) _comPort.Open();
                    //send the message to the port
                    _comPort.Write(msg);
                    //display the message
                    DisplayData(MessageType.Outgoing, msg + "\n");
                    break;
                case TransmissionType.Hex:
                    try
                    {
                        //convert the message to byte array
                        byte[] newMsg = HexToByte(msg);
                        //send the message to the port
                        _comPort.Write(newMsg, 0, newMsg.Length);
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
                    if (!(_comPort.IsOpen == true)) _comPort.Open();
                    //send the message to the port
                    _comPort.Write(msg);
                    //display the message
                    DisplayData(MessageType.Outgoing, msg + "\n");
                    break;
            }
        }
  
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
                comBuffer[i / 2] = Convert.ToByte(msg.Substring(i, 2), 16);
            //return the array
            return comBuffer;
        }

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
        public bool OpenPort()
        {
            try
            {
                if (_comPort.IsOpen == true) _comPort.Close();

                _comPort.BaudRate = int.Parse(BaudRate);   
                _comPort.DataBits = int.Parse(DataBits);   
                _comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBits);    
                _comPort.Parity = (Parity)Enum.Parse(typeof(Parity), Parity);  
                _comPort.PortName = PortName; 
                _comPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool OpenDisplayPort(string port)
        {
            try
            {
                _comPort.PortName = port;

                if (_comPort.IsOpen == true) _comPort.Close();

                _comPort.Open();
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
        public bool ClosePort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (_comPort.IsOpen == true) _comPort.Close();
                //return true
                return true;
            }
            catch (Exception ex)
            {
                DisplayData(MessageType.Error, ex.Message);
                return false;
            }
        }
        public bool IsPortOpen()
        {
            return _comPort.IsOpen;
        }
        public SerialPort ComPort()
        {
            return _comPort;
        }
        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //determine the mode the user selected (binary/string)
            switch (CurrentTransmissionType)
            {
                //user chose string
                case TransmissionType.Text:
                    //read data waiting in the buffer
                    string msg = _comPort.ReadExisting();
                    //display the data to the user
                    DisplayData(MessageType.Incoming, msg + "\n");
                    break;
                //user chose binary
                case TransmissionType.Hex:

                    string inData = _comPort.ReadLine();
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
                    string str = _comPort.ReadExisting();
                    //display the data to the user
                    DisplayData(MessageType.Incoming, str + "\n");
                    break;
            }
        }
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
