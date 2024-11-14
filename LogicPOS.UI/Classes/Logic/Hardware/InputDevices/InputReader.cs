using Gtk;
using logicpos.Classes.Enums.Hardware;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Logic.Hardware
{
    public class InputReader
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly uint _timerInterval;
        private bool _timerEnabled = false;
        private static List<int> _barCodeReaderList;
        private static List<int> _cardReaderList;
        private static Dictionary<int, InputReaderDevice> _readers;

        public string Buffer { get; set; } = string.Empty;

        public Window Window { get; set; }
        public InputReaderDevice Device { get; set; }

        //Public Event
        public event EventHandler Captured;

        //Constructor
        public InputReader()
        {
            //Init Time Interval
            _timerInterval = TerminalService.Terminal.TimerInterval;
            //Init Readers
            _readers = InitReaders();
        }

        private Dictionary<int, InputReaderDevice> InitReaders()
        {
            Dictionary<int, InputReaderDevice> result = new Dictionary<int, InputReaderDevice>();

            try
            {
                if (TerminalService.Terminal.BarcodeReader != null)
                {
                    _barCodeReaderList = GeneralUtils.CommaDelimitedStringToIntList(TerminalService.Terminal.BarcodeReader.ReaderSizes);
                    
                    for (int i = 0; i < _barCodeReaderList.Count; i++)
                    {
                        result.Add(Convert.ToInt16(_barCodeReaderList[i]), InputReaderDevice.BarCodeReader);
                    }
                }

                if (TerminalService.Terminal.CardReader != null)
                {
                    _cardReaderList = GeneralUtils.CommaDelimitedStringToIntList(TerminalService.Terminal.CardReader.ReaderSizes);
                    
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
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        private void OnCapture()
        {
            Captured?.Invoke(this, EventArgs.Empty);
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
            Device = InputReaderDevice.None;

            //Search For Valid InputReaderDevice
            foreach (var item in _readers)
            {
                if (Buffer.Length == item.Key)
                {
                    //Assign Detected InputReaderDevice
                    Device = item.Value;
                }
            }

            //Trigger Event
            if (Device != InputReaderDevice.None)
            {
                OnCapture();
            }

            //Always Clean Buffer
            Buffer = string.Empty;
        }

        private bool UpdateTimer()
        {
            StopTimer();

            // returning true means that the timeout routine should be invoked
            // again after the timeout period expires. Returning false would
            // terminate the timeout.
            return _timerEnabled;
        }

        public void KeyReleaseEvent(Window parentWindow, object o, KeyReleaseEventArgs args)
        {
            if (!_timerEnabled) StartTimer();
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
                    string input = args.Event.Key.ToString();
                    keyChar = input[input.Length - 1];
                    //Add to Buffer
                    Buffer += keyChar;
                    break;
                default:
                    break;
            }
        }
    }
}
