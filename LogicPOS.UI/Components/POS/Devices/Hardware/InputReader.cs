using Gtk;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.Utility;
using Serilog;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.POS.Devices.Hardware
{
    public class InputReader
    {
        private readonly uint _timerInterval;
        private bool _timerEnabled = false;
        private static List<int> _barCodeReaderList;
        private static List<int> _cardReaderList;
        private static Dictionary<int, InputReaderType> _readers;

        public string Buffer { get; set; } = string.Empty;

        public Window Window { get; set; }
        public InputReaderType Device { get; set; }

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

        private Dictionary<int, InputReaderType> InitReaders()
        {
            Dictionary<int, InputReaderType> result = new Dictionary<int, InputReaderType>();

            try
            {
                if (TerminalService.Terminal.BarcodeReader != null)
                {
                    _barCodeReaderList = GeneralUtils.CommaDelimitedStringToIntList(TerminalService.Terminal.BarcodeReader.ReaderSizes);

                    for (int i = 0; i < _barCodeReaderList.Count; i++)
                    {
                        result.Add(Convert.ToInt16(_barCodeReaderList[i]), InputReaderType.BarCodeReader);
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
                            result.Add(Convert.ToInt16(_cardReaderList[i]), InputReaderType.CardReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Exception");
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
            Device = InputReaderType.None;

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
            if (Device != InputReaderType.None)
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
