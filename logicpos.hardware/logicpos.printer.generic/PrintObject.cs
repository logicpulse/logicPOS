using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;

namespace logicpos.printer.generic
{
    public class PrintObject : IComparable<PrintObject>
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int ObjectType { get; set; } = 0;

        public string Text { get; set; } = string.Empty;

        public int Index { get; set; } = 0;

        public double FontSize { get; set; } = 0;

        /// <summary>
        /// Normal; Bold; Italic
        /// </summary>
        public string FontStyle { get; set; } = "normal";

        /// <summary>
        /// Normal; Bold; Italic
        /// </summary>
        public string Style { get; set; } = "Normal";

        /// <summary>
        /// Normal; Bold; Italic
        /// </summary>
        public string Type { get; set; } = "";

        /// <summary>
        /// "Center" , "Justify", "Left", "Right"
        /// </summary>
        public string Text_Align { get; set; } = "Justify";

        public int PosX { get; set; } = 0;

        public int PosY { get; set; } = 0;

        public int Width { get; set; } = 0;

        public int Height { get; set; } = 0;

        public int FirstProductPosition { get; set; } = 0;

        public int Line { get; set; } = -1;

        public int Col { get; set; } = -1;

        public string Value { get; set; } = string.Empty;

        public string BarcodeType { get; set; } = string.Empty;

        public string TextLocation { get; set; } = string.Empty;

        public PrintObject(int objectType)
        {
            ObjectType = objectType;
        }

        public int CompareTo(PrintObject other)
        {
            return PosY.CompareTo(other.PosY);
        }

        public void loadCanvasXML(XmlNode dev, int dataCount, PrintObject prtObj)
        {
            foreach (XmlElement elem in dev.ChildNodes)
            {

                switch (elem.Name)
                {
                    case "SizeX":
                        Width = Convert.ToInt32(elem.InnerText);
                        break;

                    case "SizeY":
                        Height = Convert.ToInt32(elem.InnerText);
                        Height += (valueToAdd * dataCount);
                        break;
                    case "FirstProdutPosition":
                        FirstProductPosition = Convert.ToInt32(elem.InnerText);
                        break;
                }
            }
            prtObj.FirstProductPosition = FirstProductPosition;
            //this._width = _width + 20;
            //this._height = _height + 40;
        }

        public void loadTextXML(XmlNode dev, int dataCount)
        {
            foreach (XmlElement elem in dev.ChildNodes)
            {
                switch (elem.Name)
                {
                    case "zIndex":
                        try
                        {
                            int index = Convert.ToInt32(elem.InnerText);
                        }
                        catch { }
                        break;

                    case "Text":
                        Text = elem.InnerText;
                        break;

                    case "FontStyle":
                        FontStyle = elem.InnerText;
                        break;

                    case "TextAlign":
                        Text_Align = elem.InnerText;
                        break;

                    case "FontWeight":
                        FontStyle = elem.InnerText;
                        break;

                    case "FontSize":
                        FontSize = Convert.ToDouble(elem.InnerText);
                        break;

                    case "LocationX":
                        PosX = Convert.ToInt32(elem.InnerText);
                        break;

                    case "LocationY":
                        PosY = Convert.ToInt32(elem.InnerText);
                        break;

                    case "SizeX":
                        Width = Convert.ToInt32(elem.InnerText);
                        break;

                    case "SizeY":
                        Height = Convert.ToInt32(elem.InnerText);
                        break;
                    case "Size":
                        if (elem.InnerText.Equals("Normal"))
                        {
                            FontSize = 11;
                        }
                        else
                        {
                            FontSize = 16;
                        }

                        break;
                    case "Style":
                        Style = elem.InnerText;
                        break;
                    case "Type":
                        Type = elem.InnerText;
                        break;
                    case "Align":
                        if (elem.InnerText.Equals("Direita"))
                        {
                            Text_Align = "Right";
                        }//"Center" , "Justify", "Left", "Right"
                        else if (elem.InnerText.Equals("Centro"))
                        {
                            Text_Align = "Center";
                        }
                        else
                        {
                            Text_Align = "Left";
                        }
                        //_type = elem.InnerText;
                        break;

                }
            }
        }

        public void loadBarcodeXML(XmlNode dev, int dataCount)
        {
            foreach (XmlElement elem in dev.ChildNodes)
            {

                switch (elem.Name)
                {
                    case "Text":
                        Text = elem.InnerText;
                        break;
                    case "Value":
                        Value = elem.InnerText;
                        break;
                    case "LocationX":
                        PosX = Convert.ToInt32(elem.InnerText);
                        break;
                    case "LocationY":
                        PosY = Convert.ToInt32(elem.InnerText);

                        if (PosY > FirstProductPosition)
                        {
                            PosY += (valueToAdd * dataCount);
                        }

                        break;
                    case "SizeX":
                        Width = Convert.ToInt32(elem.InnerText);
                        break;

                    case "SizeY":
                        Height = Convert.ToInt32(elem.InnerText);
                        break;
                    case "Type":
                        BarcodeType = elem.InnerText;
                        break;
                    case "LocationText":
                        TextLocation = elem.InnerText;
                        break;

                }
            }

        }

        public void loadImageXML(XmlNode dev, int dataCount)
        {
            foreach (XmlElement elem in dev.ChildNodes)
            {

                switch (elem.Name)
                {
                    case "Path":
                        Value = elem.InnerText;
                        break;
                    case "LocationX":
                        PosX = Convert.ToInt32(elem.InnerText);
                        break;
                    case "LocationY":
                        PosY = Convert.ToInt32(elem.InnerText);
                        if (PosY > FirstProductPosition)
                        {
                            PosY += (valueToAdd * dataCount);
                        }
                        break;
                    case "SizeX":
                        Width = Convert.ToInt32(elem.InnerText);
                        break;
                    case "SizeY":
                        Height = Convert.ToInt32(elem.InnerText);
                        break;
                    case "Name":

                        break;
                    case "FullPath":

                        break;

                }
            }

        }

        public int valueToAdd = 20;

        public void printTicket(DataTable dataLoop, DataTable dataStatic, string pDriver, string pPrinterName, string pathXml)
        {
            _logger.Debug($"BO printTicket {pDriver}:pPrinterName{pPrinterName}");

            int coluneSize = 8;
            int LineSize = 20;
            XmlDocument xml = new XmlDocument();

            string strXml = GetXmlString(pathXml);
            xml.LoadXml(strXml);


            //xml = replaceReservedStringsFromDatabase(dataProducts, xml.InnerXml);
            XmlDocument xmlDoc = replaceReservedStrings(xml.InnerXml);

            List<PrintObject> printobjects = Util.GetObjectsFromTemplate(this, xmlDoc, dataLoop, dataStatic);

            printobjects.Sort(); // sort by x coordinate

            List<PrintObject> printobjectsCalculate = Util.CalculatePrintCoordinates(printobjects, coluneSize, LineSize);

            ThermalPrinter printer = new ThermalPrinter("PC860");

            Util.PreparePrintDocument(ref printer, printobjectsCalculate, coluneSize, LineSize);

            switch (pDriver)
            {
                case "THERMAL_PRINTER_WINDOWS":
                    //Impressora SINOCAN em ambiente Windows 
					//TK016310 Configuração Impressoras Windows 
                    genericwindows.Print.USBPrintWindows(pPrinterName, printer.getByteArray());
                    break;
                case "THERMAL_PRINTER_SOCKET":
                    //Impressora SINOCAN em ambiente Linux Socket
                    genericsocket.Print.SocketPrint(pPrinterName, printer.getByteArray());
                    break;
            }

            _logger.Debug("EO printTicket");
        }

        private string GetXmlString(string strFile)
        {
            // Load the xml file into XmlDocument object.
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(strFile);
            }
            catch (XmlException e)
            {
                string err = e.ToString();
            }
            // Now create StringWriter object to get data from xml document.
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xmlDoc.WriteTo(xw);
            return sw.ToString();
        }

        private XmlDocument replaceReservedStrings(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();

            //load all reserved strings and values from POS database
            Hashtable ReserverHValues = new Hashtable
            {
                { "[DateTime]", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                { "[Date]", DateTime.Now.ToString("dd/MM/yyyy") },
                { "[Time]", DateTime.Now.ToString("HH:mm:ss") },
                { "º", "" },
                { "ª", "" }
            };


            foreach (DictionaryEntry pair in ReserverHValues)
            {
                xml = xml.Replace((string)pair.Key, (string)pair.Value);
            }

            xmlDoc.InnerXml = xml;

            return xmlDoc;
        }

        public void OpenDoor(string pDriver, string pPrinterName, int m, int t1, int t2)
        {
            try
            {
                bool defaultValue = false;

                if(m == 0 && t1 == 0 && t2 == 0)
                {
                    defaultValue = true;
                }

                ThermalPrinter printer = new ThermalPrinter("PC860");

                printer.WakeUp();

                printer.GeneratePulse(m, t1, t2);

                switch (pDriver)
                {
                    case "THERMAL_PRINTER_WINDOWS":
                        //Impressora SINOCAN em ambiente Windows 
                        //TK016310 Configuração Impressoras Windows 
                        genericusb.Print.USBPrintWindows(pPrinterName, printer.getByteArray(), defaultValue);
                        //genericwindows.Print.USBPrintWindows(pPrinterName, printer.getByteArray());
                        break;
                    case "THERMAL_PRINTER_SOCKET":
                        //Impressora SINOCAN em ambiente Linux Socket
                        genericusb.Print.USBPrintWindows(pPrinterName, printer.getByteArray(), defaultValue);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("void OpenDoor(string pDriver, string pPrinterName, int m, int t1, int t2) :: " + ex.Message, ex);
            }
        }
    }
}
