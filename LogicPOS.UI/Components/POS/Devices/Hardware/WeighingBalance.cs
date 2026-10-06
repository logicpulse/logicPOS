using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using LogicPOS.Globalization;

namespace logicpos.Classes.Logic.Hardware
{
    public class WeighingBalance
    {
        private const int ContinuousTimeoutMs = 600;
        private const int RequestTimeoutMs = 800;
        private const int PriceComputingTimeoutMs = 2000;
        private const int RepeatRequestMs = 300;

        private static readonly char[] FrameDelimiters = { '\r', '\n', '\x02', '\x03' };
        private static readonly Regex PriceComputingResponseRegex = new Regex(@"^\s*99([0-9])(\d{5})", RegexOptions.Compiled);
        private static readonly Regex WeightRegex = new Regex(@"([-+]?)\s*(\d+(?:[.,]\d+)?)\s*(kg|lb|oz|g)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex StatusFrameRegex = new Regex(@"^\s*([INS])\1{4,5}\s*$", RegexOptions.Compiled);
        private static readonly Regex UnstableFrameRegex = new Regex(@"(^|[^A-Z])(US|MO)([^A-Z]|$)|^\s*S\s+D\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly SerialPortService _communicationManager;
        private readonly object _sync = new object();
        private readonly List<ScaleProtocol> _protocols = new List<ScaleProtocol>
        {
            new ScaleProtocol("Continuous output", null, ContinuousTimeoutMs),
            new ScaleProtocol("ENQ (Toledo/Filizola/CAS/Urano)", price => new byte[] { 0x05 }, RequestTimeoutMs),
            new ScaleProtocol("Dibal/Epelsa 98/99 (price computing)", BuildPriceComputingRequest, PriceComputingTimeoutMs),
            new ScaleProtocol("SCP-01 W (NCI/Avery/Brecknell)", price => Encoding.ASCII.GetBytes("W\r"), RequestTimeoutMs),
            new ScaleProtocol("MT-SICS SI (Mettler Toledo)", price => Encoding.ASCII.GetBytes("SI\r\n"), RequestTimeoutMs),
            new ScaleProtocol("IP (Ohaus/A&D)", price => Encoding.ASCII.GetBytes("IP\r\n"), RequestTimeoutMs)
        };
        private ScaleProtocol _detectedProtocol;

        public WeighingBalance(WeighingMachine weighingMachine)
            : this(weighingMachine.BaudRate.ToString(), weighingMachine.Parity, weighingMachine.StopBits, weighingMachine.DataBits.ToString(), weighingMachine.PortName)
        {
        }

        public WeighingBalance(string baudRate, string parity, string stopBits, string dataBits, string portName)
        {
            //string baud, string par, string sBits, string dBits, string name
            _communicationManager = new SerialPortService(baudRate, parity, stopBits, dataBits, portName);
            _communicationManager.CurrentTransmissionType = SerialPortService.TransmissionType.Hex;
            // Start With OpenPort
            OpenPort();
        }

        public bool OpenPort()
        {
            try
            {
                return _communicationManager.OpenPort();
            }
            catch (Exception ex)
            {
                var message = string.Format(LocalizedString.Instance["dialog_message_error_initializing_weighing_balance"], TerminalService.Terminal.Designation, ex.Message);

                CustomAlerts.Error(LoginWindow.Instance)
                            .WithSize(new Size(500, 340))
                            .WithTitleResource("global_error")
                            .WithMessage(message)
                            .ShowAlert();

                Log.Error(ex,"Exception");
                return false;
            }
        }

        public bool ClosePort()
        {
            return _communicationManager.ClosePort();
        }

        public bool IsPortOpen()
        {
            return _communicationManager.IsPortOpen();
        }

        public SerialPort ComPort()
        {
            return _communicationManager.ComPort();
        }

        /// <summary>
        /// Reads a stable weight (kg) from the balance. The protocol is auto-detected on the first successful
        /// reading and tried first afterwards. Blocking call: run it outside the GTK thread.
        /// </summary>
        public bool TryWeigh(decimal articlePricePerKg, out decimal weightKg)
        {
            weightKg = 0;

            lock (_sync)
            {
                try
                {
                    if (!IsPortOpen() && !_communicationManager.OpenPort())
                    {
                        return false;
                    }

                    var candidates = _detectedProtocol == null
                        ? _protocols
                        : new[] { _detectedProtocol }.Concat(_protocols.Where(p => p != _detectedProtocol));

                    foreach (var protocol in candidates)
                    {
                        int timeoutMs = protocol == _detectedProtocol ? protocol.TimeoutMs * 2 : protocol.TimeoutMs;

                        bool success = TryWeigh(ComPort(), protocol, articlePricePerKg, timeoutMs, out weightKg, out bool recognized);

                        // A recognized answer (even zero/unstable) identifies the protocol: stop sending other requests.
                        if (success || recognized)
                        {
                            if (protocol != _detectedProtocol)
                            {
                                Log.Information("Weighing balance protocol detected: {Protocol}", protocol.Name);
                                _detectedProtocol = protocol;
                            }

                            return success;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error reading weighing balance");
                }
            }

            weightKg = 0;
            return false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Helper Methods

        private static bool TryWeigh(SerialPort port, ScaleProtocol protocol, decimal articlePricePerKg, int timeoutMs, out decimal weightKg, out bool recognized)
        {
            weightKg = 0;
            recognized = false;
            byte[] request = protocol.BuildRequest?.Invoke(articlePricePerKg);

            if (protocol.BuildRequest != null && request == null)
            {
                return false;
            }

            port.DiscardInBuffer();

            if (request != null)
            {
                port.Write(request, 0, request.Length);
            }

            var received = new StringBuilder();
            var stopwatch = Stopwatch.StartNew();
            long lastRequestMs = 0;

            while (stopwatch.ElapsedMilliseconds < timeoutMs)
            {
                int count = port.BytesToRead;

                if (count == 0)
                {
                    // Ask again while the scale keeps answering with a zero/unstable weight.
                    if (recognized && request != null && stopwatch.ElapsedMilliseconds - lastRequestMs >= RepeatRequestMs)
                    {
                        received.Clear();
                        port.Write(request, 0, request.Length);
                        lastRequestMs = stopwatch.ElapsedMilliseconds;
                    }

                    Thread.Sleep(20);
                    continue;
                }

                var bytes = new byte[count];
                int read = port.Read(bytes, 0, count);
                received.Append(Encoding.ASCII.GetString(bytes, 0, read));

                // A continuous stream may start in the middle of a frame.
                if (TryParseWeight(received.ToString(), request == null, out weightKg, out bool frameRecognized))
                {
                    recognized = true;
                    return true;
                }

                recognized |= frameRecognized;
            }

            return false;
        }

        /// <summary>
        /// 98: 0x39h y 0x38h
        /// PPPPP: 5 dígitos para el precio.
        /// C: Checksum, suma lógica (XOR) de todos los caracteres anteriores.
        /// CR: 0x0Dh LF: 0x0Ah
        /// </summary>
        private static byte[] BuildPriceComputingRequest(decimal articlePricePerKg)
        {
            long cents = (long)decimal.Round(articlePricePerKg * 100, 0, MidpointRounding.AwayFromZero);

            if (cents <= 0 || cents > 99999)
            {
                return null;
            }

            byte[] frame = Encoding.ASCII.GetBytes("98" + cents.ToString("00000", CultureInfo.InvariantCulture));
            byte checksum = 0;

            foreach (byte b in frame)
            {
                checksum ^= b;
            }

            return frame.Concat(new byte[] { checksum, 0x0D, 0x0A }).ToArray();
        }

        /// <summary>
        /// Parses the most recent complete frame. Returns true only for a stable weight greater than zero.
        /// </summary>
        internal static bool TryParseWeight(string data, bool skipFirstFrame, out decimal weightKg, out bool recognized)
        {
            weightKg = 0;
            recognized = false;
            string[] frames = data.Split(FrameDelimiters);
            int first = skipFirstFrame ? 1 : 0;

            // The last segment is incomplete until a delimiter arrives.
            for (int i = frames.Length - 2; i >= first; i--)
            {
                if (TryParseFrame(frames[i], out weightKg))
                {
                    recognized = true;
                    return weightKg > 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true when the frame is a weight frame; unstable or error frames yield 0.
        /// 99: 0x39h y 0x39h, S: estado del peso (0 correcto), WWWWW: peso en gramos, E + IIIIII: importe.
        /// </summary>
        private static bool TryParseFrame(string frame, out decimal weightKg)
        {
            weightKg = 0;

            if (string.IsNullOrWhiteSpace(frame))
            {
                return false;
            }

            // Toledo/Urano/Filizola status frames: IIIII unstable, NNNNN negative, SSSSS overload.
            if (StatusFrameRegex.IsMatch(frame))
            {
                return true;
            }

            Match priceComputing = PriceComputingResponseRegex.Match(frame);
            if (priceComputing.Success)
            {
                if (priceComputing.Groups[1].Value == "0")
                {
                    weightKg = decimal.Parse(priceComputing.Groups[2].Value, CultureInfo.InvariantCulture) / 1000m;
                }

                return true;
            }

            foreach (Match match in WeightRegex.Matches(frame))
            {
                string number = match.Groups[2].Value.Replace(',', '.');
                string unit = match.Groups[3].Value.ToLowerInvariant();

                if (unit.Length == 0 && !number.Contains("."))
                {
                    // Frames with only 5/6 digits (Toledo/Filizola) carry the weight in grams.
                    string digitsOnly = frame.Trim().TrimStart('+', '-').Trim();
                    if (digitsOnly != number || number.Length < 5 || number.Length > 6)
                    {
                        continue;
                    }

                    unit = "g";
                }

                if (UnstableFrameRegex.IsMatch(frame) || match.Groups[1].Value == "-")
                {
                    return true;
                }

                decimal value = decimal.Parse(number, CultureInfo.InvariantCulture);
                weightKg = Math.Round(ConvertToKg(value, unit), 3, MidpointRounding.AwayFromZero);
                return true;
            }

            return false;
        }

        private static decimal ConvertToKg(decimal value, string unit)
        {
            switch (unit)
            {
                case "g":
                    return value / 1000m;
                case "lb":
                    return value * 0.45359237m;
                case "oz":
                    return value * 0.028349523125m;
                default:
                    return value;
            }
        }

        private sealed class ScaleProtocol
        {
            public ScaleProtocol(string name, Func<decimal, byte[]> buildRequest, int timeoutMs)
            {
                Name = name;
                BuildRequest = buildRequest;
                TimeoutMs = timeoutMs;
            }

            public string Name { get; }
            public Func<decimal, byte[]> BuildRequest { get; }
            public int TimeoutMs { get; }
        }
    }
}
