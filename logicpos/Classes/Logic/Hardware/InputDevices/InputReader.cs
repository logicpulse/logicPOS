using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Hardware;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Logic.Hardware
{
    public class InputReader
    {
        //Log4Net
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private uint _timerInterval;
        private bool _timerEnabled = false;
        private static List<int> _barCodeReaderList;
        private static List<int> _cardReaderList;
        private static Dictionary<int, InputReaderDevice> _readers;

        //Buffer
        private string _captureBuffer = string.Empty;
        public string Buffer
        {
            get { return _captureBuffer; }
            set { _captureBuffer = value; }
        }
        private Window _sourceWindow;
        public Window Window
        {
            get { return _sourceWindow; }
            set { _sourceWindow = value; }
        }
        private InputReaderDevice _inputReaderDevice;
        public InputReaderDevice Device
        {
            get { return _inputReaderDevice; }
            set { _inputReaderDevice = value; }
        }

        //Public Event
        public event EventHandler Captured;

        //Constructor
        public InputReader()
        {
            //Init Time Interval
            _timerInterval = GlobalFramework.LoggedTerminal.InputReaderTimerInterval;
            //Init Readers
            _readers = InitReaders();
        }

        private Dictionary<int, InputReaderDevice> InitReaders()
        {
            Dictionary<int, InputReaderDevice> result = new Dictionary<int, InputReaderDevice>();

            try
            {
                if (GlobalFramework.LoggedTerminal.BarcodeReader != null)
                {
                    _barCodeReaderList = FrameworkUtils.CommaDelimitedStringToIntList(GlobalFramework.LoggedTerminal.BarcodeReader.ReaderSizes);
                    
                    for (int i = 0; i < _barCodeReaderList.Count; i++)
                    {
                        result.Add(Convert.ToInt16(_barCodeReaderList[i]), InputReaderDevice.BarCodeReader);
                    }
                }

                if (GlobalFramework.LoggedTerminal.CardReader != null)
                {
                    _cardReaderList = FrameworkUtils.CommaDelimitedStringToIntList(GlobalFramework.LoggedTerminal.CardReader.ReaderSizes);
                    
                    for (int i = 0; i < _cardReaderList.Count; i++)
                    {
                        //Check if BarCodeReader is Using same Value Size, if So Skip
                        if (!result.ContainsKey(Convert.ToInt16(_cardReaderList[i])))
                        {
                            result.Add(Convert.ToInt16(_cardReaderList[i]), InputReaderDevice.CardReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        private void OnCapture()
        {
            if (Captured != null)
            {
                Captured(this, EventArgs.Empty);
            }
        }

        public void StartTimer()
        {
            //Enable Timer
            _timerEnabled = true;
            // Every second call update_status' (1000 milliseconds)
            GLib.Timeout.Add(_timerInterval, new GLib.TimeoutHandler(UpdateTimer));
        }

        public void StopTimer()
        {
            //Disable Timer
            _timerEnabled = false;

            //Reset to Default
            _inputReaderDevice = InputReaderDevice.None;

            //Search For Valid InputReaderDevice
            foreach (var item in _readers)
            {
                if (_captureBuffer.Length == item.Key)
                {
                    //Assign Detected InputReaderDevice
                    _inputReaderDevice = item.Value;
                }
            }

            //Trigger Event
            if (_inputReaderDevice != InputReaderDevice.None)
            {
                OnCapture();
            }

            //Always Clean Buffer
            _captureBuffer = string.Empty;
        }

        private bool UpdateTimer()
        {
            StopTimer();

            // returning true means that the timeout routine should be invoked
            // again after the timeout period expires. Returning false would
            // terminate the timeout.
            return _timerEnabled;
        }

        public void KeyReleaseEvent(Window pSourceWindow, object o, KeyReleaseEventArgs args)
        {
            if (!_timerEnabled) StartTimer();
            string input = string.Empty;
            char keyChar;

            switch (args.Event.Key.ToString())
            {
                case "Key_0":
                case "Key_1":
                case "Key_2":
                case "Key_3":
                case "Key_4":
                case "Key_5":
                case "Key_6":
                case "Key_7":
                case "Key_8":
                case "Key_9":
                    input = args.Event.Key.ToString();
                    keyChar = input[input.Length - 1];
                    //Add to Buffer
                    _captureBuffer += keyChar;
                    break;
                default:
                    break;
            }
        }
    }
}
