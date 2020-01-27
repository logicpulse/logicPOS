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
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        int _objectType = 0; //0 canvas //1 text //2 image //3 barcode

        public int ObjectType
        {
            get { return _objectType; }
            set { _objectType = value; }
        }

        string _text = string.Empty;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        int index = 0;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }


        double _fontSize = 0;
        public double FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        string _fontStyle = "normal";
        /// <summary>
        /// Normal; Bold; Italic
        /// </summary>
        public string FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        string _style = "Normal";
        /// <summary>
        /// Normal; Bold; Italic
        /// </summary>
        public string Style
        {
            get { return _style; }
            set { _style = value; }
        }

        string _type = "";
        /// <summary>
        /// Normal; Bold; Italic
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        string _text_Align = "Justify";

        /// <summary>
        /// "Center" , "Justify", "Left", "Right"
        /// </summary>
        public string Text_Align
        {
            get { return _text_Align; }
            set { _text_Align = value; }
        }


        int _posX = 0;
        public int PosX
        {
            get { return _posX; }
            set { _posX = value; }
        }

        int _posY = 0;
        public int PosY
        {
            get { return _posY; }
            set { _posY = value; }
        }

        int _width = 0;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        int _height = 0;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        int _firstProductPosition = 0;
        public int FirstProductPosition
        {
            get { return _firstProductPosition; }
            set { _firstProductPosition = value; }
        }

        int _line = -1;

        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }
        int _col = -1;

        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }

        string _value = string.Empty;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        string _barcodeType = string.Empty;

        public string BarcodeType
        {
            get { return _barcodeType; }
            set { _barcodeType = value; }
        }

        string _textLocation = string.Empty;

        public string TextLocation
        {
            get { return _textLocation; }
            set { _textLocation = value; }
        }

        public PrintObject(int objectType)
        {
            _objectType = objectType;
        }

        public int CompareTo(PrintObject other)
        {
            return _posY.CompareTo(other._posY);
        }

        public void loadCanvasXML(XmlNode dev, int dataCount, PrintObject prtObj)
        {
            foreach (XmlElement elem in dev.ChildNodes)
            {

                switch (elem.Name)
                {
                    case "SizeX":
                        _width = Convert.ToInt32(elem.InnerText);
                        break;

                    case "SizeY":
                        _height = Convert.ToInt32(elem.InnerText);
                        _height += (valueToAdd * dataCount);
                        break;
                    case "FirstProdutPosition":
                        _firstProductPosition = Convert.ToInt32(elem.InnerText);
                        break;
                }
            }
            prtObj.FirstProductPosition = _firstProductPosition;
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
                        int index = 0;
                        try
                        {
                            index = Convert.ToInt32(elem.InnerText);
                        }
                        catch { }
                        break;

                    case "Text":
                        _text = elem.InnerText;
                        break;

                    case "FontStyle":
                        _fontStyle = elem.InnerText;
                        break;

                    case "TextAlign":
                        _text_Align = elem.InnerText;
                        break;

                    case "FontWeight":
                        _fontStyle = elem.InnerText;
                        break;

                    case "FontSize":
                        _fontSize = Convert.ToDouble(elem.InnerText);
                        break;

                    case "LocationX":
                        _posX = Convert.ToInt32(elem.InnerText);
                        break;

                    case "LocationY":
                        _posY = Convert.ToInt32(elem.InnerText);
                        break;

                    case "SizeX":
                        _width = Convert.ToInt32(elem.InnerText);
                        break;

                    case "SizeY":
                        _height = Convert.ToInt32(elem.InnerText);
                        break;
                    case "Size":
                        if (elem.InnerText.Equals("Normal"))
                        {
                            _fontSize = 11;
                        }
                        else
                        {
                            _fontSize = 16;
                        }

                        break;
                    case "Style":
                        _style = elem.InnerText;
                        break;
                    case "Type":
                        _type = elem.InnerText;
                        break;
                    case "Align":
                        if (elem.InnerText.Equals("Direita"))
                        {
                            _text_Align = "Right";
                        }//"Center" , "Justify", "Left", "Right"
                        else if (elem.InnerText.Equals("Centro"))
                        {
                            _text_Align = "Center";
                        }
                        else
                        {
                            _text_Align = "Left";
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
                        _text = elem.InnerText;
                        break;
                    case "Value":
                        _value = elem.InnerText;
                        break;
                    case "LocationX":
                        _posX = Convert.ToInt32(elem.InnerText);
                        break;
                    case "LocationY":
                        _posY = Convert.ToInt32(elem.InnerText);

                        if (_posY > _firstProductPosition)
                        {
                            _posY += (valueToAdd * dataCount);
                        }

                        break;
                    case "SizeX":
                        _width = Convert.ToInt32(elem.InnerText);
                        break;

                    case "SizeY":
                        _height = Convert.ToInt32(elem.InnerText);
                        break;
                    case "Type":
                        _barcodeType = elem.InnerText;
                        break;
                    case "LocationText":
                        _textLocation = elem.InnerText;
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
                        _value = elem.InnerText;
                        break;
                    case "LocationX":
                        _posX = Convert.ToInt32(elem.InnerText);
                        break;
                    case "LocationY":
                        _posY = Convert.ToInt32(elem.InnerText);
                        if (_posY > _firstProductPosition)
                        {
                            _posY += (valueToAdd * dataCount);
                        }
                        break;
                    case "SizeX":
                        _width = Convert.ToInt32(elem.InnerText);
                        break;
                    case "SizeY":
                        _height = Convert.ToInt32(elem.InnerText);
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
            _log.Debug($"BO printTicket {pDriver}:pPrinterName{pPrinterName}");

            int coluneSize = 8;
            int LineSize = 20;

            string strXml = string.Empty;

            XmlDocument xml = new XmlDocument();

            strXml = GetXmlString(pathXml);
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
                case "THERMAL_PRINTER_LINUX":
                    //Impressora SINOCAN em ambiente Linux 
                    genericlinux.Print.LinuxPrint(pPrinterName, printer.getByteArray());
                    break;
                case "THERMAL_PRINTER_SOCKET":
                    //Impressora SINOCAN em ambiente Linux Socket
                    genericsocket.Print.SocketPrint(pPrinterName, printer.getByteArray());
                    break;
            }

            _log.Debug("EO printTicket");
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

        private XmlDocument replaceReservedStringsFromDatabase(System.Data.DataTable data, string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();

            //load all reserved strings and values from POS database
            Hashtable ReserverHValues = new Hashtable();
            ReserverHValues.Add("[DateTime]", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            ReserverHValues.Add("[Date]", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            ReserverHValues.Add("[Time]", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));


            foreach (DictionaryEntry pair in ReserverHValues)
            {
                xml = xml.Replace((string)pair.Key, (string)pair.Value);
            }

            xmlDoc.InnerXml = xml;

            return xmlDoc;
        }

        private XmlDocument replaceReservedStrings(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();

            //load all reserved strings and values from POS database
            Hashtable ReserverHValues = new Hashtable();
            ReserverHValues.Add("[DateTime]", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            ReserverHValues.Add("[Date]", DateTime.Now.ToString("dd/MM/yyyy"));
            ReserverHValues.Add("[Time]", DateTime.Now.ToString("HH:mm:ss"));
            ReserverHValues.Add("º", "");
            ReserverHValues.Add("ª", "");


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
                ThermalPrinter printer = new ThermalPrinter("PC860");

                printer.WakeUp();

                printer.GeneratePulse(m, t1, t2);

                switch (pDriver)
                {
                    case "THERMAL_PRINTER_WINDOWS":
                        //Impressora SINOCAN em ambiente Windows 
					//TK016310 Configuração Impressoras Windows 
                        genericwindows.Print.USBPrintWindows(pPrinterName, printer.getByteArray());
                        break;

                    case "THERMAL_PRINTER_LINUX":
                        //Impressora SINOCAN em ambiente Linux 
                        genericlinux.Print.LinuxPrint(pPrinterName, printer.getByteArray());
                        break;
                    case "THERMAL_PRINTER_SOCKET":
                        //Impressora SINOCAN em ambiente Linux Socket
                        genericsocket.Print.SocketPrint(pPrinterName, printer.getByteArray());
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error("void OpenDoor(string pDriver, string pPrinterName, int m, int t1, int t2) :: " + ex.Message, ex);
            }
        }
    }
}
