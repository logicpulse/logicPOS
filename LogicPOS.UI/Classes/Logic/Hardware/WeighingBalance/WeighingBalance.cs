﻿using Gtk;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Text;

namespace logicpos.Classes.Logic.Hardware
{
    public class WeighingBalance
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SerialPortService _communicationManager;

        public WeighingBalance(sys_configurationweighingmachine weighingMachine)
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
                logicpos.Utils.ShowMessageBox(GlobalApp.StartupWindow, DialogFlags.Modal, new Size(500, 340), MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"),
                    string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_error_initializing_weighing_balance"), TerminalSettings.LoggedTerminal.WeighingMachine.Designation, ex.Message)
                    );
                _logger.Error(ex.Message, ex);
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
        /// Call weigh in the balance from OutSide, this Trigger the WeighingBalance to Calc the Weight and Total
        /// </summary>
        public void WeighArticle(decimal articlePricePerKg)
        {
            //_logger.Debug(string.Format("WeighArticle articlePricePerKg: [{0}]", articlePricePerKg));
            // Round Price to 0.00, to force ex 5,00, else we have 5 and it acts has 0,50
            string priceString = articlePricePerKg.ToString("0.00").ToString().Replace(",", string.Empty).Replace(".", string.Empty);
            // Format it
            string textSendFormatted = Convert.ToInt16(priceString).ToString("00000");
            // Convert to Hex
            string weightHex = ToHexString(textSendFormatted);
            // Write to Device
            WriteData(CalculateHexFromPrice(weightHex));
        }


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Helper Methods

        private void WriteData(string msg)
        {
            _communicationManager.WriteData(msg);
        }

        /// <summary>
        /// 98: 0x38h y 0x39h
        /// PPPPP: 5 dígitos para el precio.
        /// C: Checksum, suma lógica (XOR) de todos los caracteres anteriores.
        /// CR: 0x0Dh LF: 0x0Ah
        /// </summary>
        /// <param name="pesoHex"></param>
        /// <returns></returns>
        private string CalculateHexFromPrice(string pesoHex)
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

        /// <summary>
        /// Convert string to Hex
        /// </summary>
        /// <param name="str"></param>
        /// <returns>"48656C6C6F20776F726C64" for "Hello world"</returns>
        private string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.ASCII.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
