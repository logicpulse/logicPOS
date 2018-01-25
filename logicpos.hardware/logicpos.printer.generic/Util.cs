using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Xml;

namespace logicpos.printer.generic
{
    public class BitmapData
    {
        public BitArray Dots
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }
    }

    public static class Util
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Settings
        //private static string _financeFinalConsumerFiscalNumber = "999999990";//<<< SettingsApp.FinanceFinalConsumerFiscalNumber | Cant Add Framework, circular dependency

        public static BitmapData GetBitmapData(string bmpFileName)
        {
            using (var bitmap = (Bitmap)Bitmap.FromFile(bmpFileName))
            {
                var threshold = 127;
                var index = 0;
                var dimensions = bitmap.Width * bitmap.Height;
                var dots = new BitArray(dimensions);

                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }

                return new BitmapData()
                {
                    Dots = dots,
                    Height = bitmap.Height,
                    Width = bitmap.Width
                };
            }
        }

        public static void TestReceipt(ref ThermalPrinter printer)
        {
            Dictionary<string, int> ItemList = new Dictionary<string, int>(100);
            //printer.Reset();

            printer.SetEncoding("PC860");

            printer.SetLineSpacing(0);
            printer.SetAlignCenter();
            printer.WriteLine("LogicPulse Lda",
                (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);

            printer.SetFont(0);

            printer.WriteLine("<Rua da República n23, Figueira da Foz>");
            printer.LineFeed();
            printer.LineFeed();

            printer.SetFont(1);

            ItemList.Add("Item #1", 8990);
            ItemList.Add("Item #2 goes here", 2000);
            ItemList.Add("Item #3", 1490);
            ItemList.Add("Item number four", 490);
            ItemList.Add("Item #5 is cheap", 245);
            ItemList.Add("Item #6", 2990);
            ItemList.Add("The seventh item", 790);

            int total = 0;
            foreach (KeyValuePair<string, int> item in ItemList)
            {
                CashRegister(ref printer, item.Key, item.Value);
                total += item.Value;
            }

            //printer.HorizontalLine(32);

            double dTotal = Convert.ToDouble(total) / 100;
            double VAT = 10.0;

            printer.SetFont(48);

            printer.WriteLine(String.Format("{0:0.00}", (dTotal)).PadLeft(32));

            printer.WriteLine("VAT 10,0%" + String.Format("{0:0.00}", (dTotal * VAT / 100)).PadLeft(23));

            printer.WriteLine(String.Format("$ {0:0.00}", dTotal * VAT / 100 + dTotal).PadLeft(16),
                ThermalPrinter.PrintingStyle.DoubleWidth);

            printer.LineFeed();
            printer.WriteLine("CASH" + String.Format("{0:0.00}", (double)total / 100).PadLeft(38));
            printer.LineFeed();
            printer.LineFeed();
            printer.SetAlignCenter();
            printer.WriteLine("Have a good day.", ThermalPrinter.PrintingStyle.Bold);

            printer.LineFeed();
            printer.SetAlignLeft();
            printer.WriteLine("Seller : Carlos Fernandes");
            printer.WriteLine(DateTime.Now.ToShortDateString());
            printer.LineFeed();
            printer.LineFeed();
            printer.LineFeed();
            printer.LineFeed();
            printer.LineFeed();

            printer.Cut(true);
        }

        static void CashRegister(ref ThermalPrinter printer, string item, int price)
        {
            printer.Reset();
            printer.Indent(0);

            if (item.Length > 24)
            {
                item = item.Substring(0, 23) + ".";
            }

            printer.WriteToBuffer(item.ToUpper());
            printer.Indent(25);
            string sPrice = String.Format("{0:0.00}", (double)price / 100);

            sPrice = sPrice.PadLeft(7);

            printer.WriteLine(sPrice);
            printer.Reset();
        }

        public static void TestBarcode(ref ThermalPrinter printer)
        {
            //ThermalPrinter.BarcodeType myType = ThermalPrinter.BarcodeType.ean13;
            string myData = "3350030103392";
            printer.Reset();

            //printer.SetBarcodeLeftSpace(10);

            printer.SelectFontHRIBarcode(0);
            printer.SelectPrintingPositionHRIBarcode(2);

            printer.SetAlignCenter();

            printer.SetBarcodeWidth(4);
            printer.PrintBarcode(ThermalPrinter.BarcodeType.ean13, myData);
            printer.SetBarcodeWidth(2);
            printer.LineFeed();
            printer.LineFeed();
            printer.LineFeed();
            printer.LineFeed();
            printer.Cut(false);
        }

        public static void TestImage(ref ThermalPrinter printer)
        {
            printer.Reset();

            // Render the logo
            printer.PrintImage("Logo.bmp");

            printer.FeedVerticalAndCut(1);
        }

        public static void TestPrint1(ref ThermalPrinter printer)
        {
            printer.Reset();

            printer.SetEncoding("PC860");

            printer.SetLineSpacing(0);
            printer.HorizontalLine(48);
            printer.SetAlignLeft();
            printer.WriteLine("1234567890123456789012345678901234567890123456789012345678901234567890",
                +(byte)ThermalPrinter.PrintingStyle.DoubleWidth);

            printer.WriteLine("1234567890123456789012345678901234567890123456789012345678901234567890",
                +(byte)ThermalPrinter.PrintingStyle.DoubleHeight);

            printer.WriteLine("1234567890123456789012345678901234567890123456789012345678901234567890",
               (byte)ThermalPrinter.PrintingStyle.DoubleHeight
               + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);

            //printer.FeedDots(2);
            printer.SetAlignLeft();
            //printer.Indent(10);
            printer.WriteLine("teste2");
            printer.WriteLine("1234567890123456789012345678901234567890123456789012345678901234567890");
            printer.HorizontalLine(48);

            printer.FeedVerticalAndCut(1);

            //printer.Cut(true);
        }

        public static List<PrintObject> CalculatePrintCoordinates(List<PrintObject> printObjects, int PrintCharWidth, int PrintLineHeight)
        {
            for (int i = 0; i < printObjects.Count; i++)
            {
                if (printObjects[i].ObjectType != 0) //text
                {
                    printObjects[i].Col = printObjects[i].PosX / PrintCharWidth;
                    printObjects[i].Line = printObjects[i].PosY / PrintLineHeight;
                    printObjects[i].Width = printObjects[i].Width / PrintCharWidth;
                    printObjects[i].Height = printObjects[i].Height / PrintLineHeight;
                }
            }

            return printObjects;
        }

        public static void PreparePrintDocument(ref ThermalPrinter printer, List<PrintObject> printObjects, int PrintCharWidth, int PrintLineHeight)
        {
            PreparePrintDocument(ref printer, printObjects, PrintCharWidth, PrintLineHeight, "");
        }

        public static void PreparePrintDocument(ref ThermalPrinter printer, List<PrintObject> printObjects, int PrintCharWidth, int PrintLineHeight, string barcodeValue)
        {
            if (printObjects.Count == 0)
                return;


            int canvasWidth = printObjects[0].Width;
            int canvasHeight = printObjects[0].Height;

            //int canvasLines = canvasHeight / PrintLineHeight;
            //int canvasident = canvasWidth / PrintCharWidth;

            int currentLine = 0;


            //printer.Reset();
            //printer.SetEncoding("PC860");

            string printText = "";
            bool first = true;
            string lastFontStyle = "";
            string lastFontSize = "";
            foreach (PrintObject pObject in printObjects)
            {
                if (pObject.ObjectType == 0) //canvas
                {
                    //nothing
                }

                else if (pObject.ObjectType == 1) //text
                {
                    //get to line
                    if (currentLine < pObject.Line)
                    {
                        if (lastFontStyle == "Bold" || lastFontStyle == "Negrito")
                        {
                            if (lastFontSize == "11")
                            {
                                //printer.WriteLine_Bold(printText);
                                printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold);
                            }
                            else
                            {
                                printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold
                                    //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                            }
                        }
                        else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
                        {
                            if (lastFontSize == "11")
                            {
                                printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline);
                            }
                            else
                            {
                                printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline
                                    //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                            }
                        }
                        else
                        {
                            if (lastFontSize == "11")
                            {
                                printer.WriteLine(printText);
                            }
                            else
                            {
                                printer.WriteLine(printText,
                                    //(byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    +(byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                            }

                        }
                        printer.LineFeed((byte)(pObject.Line - currentLine));
                        currentLine = pObject.Line;
                        printText = "";
                    }
                    else if (currentLine > pObject.Line)
                    {
                        //printer.LineFeed((byte)(currentLine - pObject.Line));
                        currentLine = pObject.Line;
                    }
                    else
                    {
                        if (!first)
                        {
                            if (lastFontStyle == "Bold" || lastFontStyle == "Negrito")
                            {
                                if (lastFontSize == "11")
                                {
                                    //printer.WriteLine_Bold(printText);
                                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold);
                                }
                                else
                                {
                                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold
                                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                                }
                            }
                            else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
                            {
                                if (lastFontSize == "11")
                                {
                                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline);
                                }
                                else
                                {
                                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline
                                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                                }
                            }
                            else
                            {
                                if (lastFontSize == "11")
                                {
                                    printer.WriteLine(printText);
                                }
                                else
                                {
                                    printer.WriteLine(printText,
                                        //(byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        +(byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                                }

                            }
                            //printer.WriteLine(printText);
                            printText = "";
                        }
                        first = false;
                    }

                    //calculate ident

                    //printText += pObject.Text;
                    if (pObject.Col != 0)
                    {
                        try
                        {
                            printText += (new String(' ', pObject.Col - 1 - printText.Length)) + pObject.Text;
                        }
                        catch
                        {
                            if (pObject.Col == printText.Length)
                            {
                                printText += pObject.Text;
                            }
                            else
                            {
                                try
                                {
                                    printText = printText.Remove(pObject.Col - 1, pObject.Text.Length).Insert(pObject.Col - 1, pObject.Text);
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(ex.Message, ex);
                                }
                            }
                            //printText += (new String(' ', pObject.Col - 1)) + pObject.Text;
                        }
                    }

                    if (pObject.Text_Align == "Center" || pObject.Text_Align == "Centro")
                    {
                        printer.SetAlignCenter();
                    }
                    else
                    {
                        printer.SetAlignLeft();

                        //if (pObject.Col != 0)
                        //{
                        //    printText = (new String(' ', pObject.Col - 1)) + printText;
                        //}
                    }

                    //if (pObject.FontStyle == "Bold")
                    //    printer.WriteLine_Bold(printText);
                    //else if (pObject.FontStyle == "Underline")
                    //    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline);
                    //else
                    //printer.WriteLine(printText);

                    lastFontStyle = pObject.FontStyle;
                    lastFontSize = pObject.FontSize.ToString();

                    currentLine++;



                }
                else if (pObject.ObjectType == 2) //Image
                {
                    //get to line
                    if (currentLine != pObject.Line)
                    {
                        printer.LineFeed((byte)(pObject.Line - currentLine));
                        currentLine = pObject.Line;
                    }

                    //pObject.Height 

                    //calculate ident

                    //string printText = pObject.Text;

                    //if (pObject.Text_Align == "Center")
                    //{
                    //    printer.SetAlignCenter();
                    //}
                    //else
                    //{
                    //    printer.SetAlignLeft();

                    //    if (pObject.Col != 0)
                    //    {
                    //        printText = (new String(' ', pObject.Col - 1)) + printText;
                    //    }
                    //}

                    printer.PrintImage(pObject.Value);


                    currentLine++;



                }
                else if (pObject.ObjectType == 3) //Barcode
                {
                    //get to line
                    if (currentLine < pObject.Line)
                    {
                        if (lastFontStyle == "Bold" || lastFontStyle == "Negrito")
                        {
                            if (lastFontSize == "11")
                            {
                                //printer.WriteLine_Bold(printText);
                                printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold);
                            }
                            else
                            {
                                printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold
                                    //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                            }
                        }
                        else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
                        {
                            if (lastFontSize == "11")
                            {
                                printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline);
                            }
                            else
                            {
                                printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline
                                    //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                            }
                        }
                        else
                        {
                            if (lastFontSize == "11")
                            {
                                printer.WriteLine(printText);
                            }
                            else
                            {
                                printer.WriteLine(printText,
                                    //(byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    +(byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                            }

                        }
                        printer.LineFeed((byte)(pObject.Line - currentLine));
                        currentLine = pObject.Line;
                        printText = "";
                    }
                    else if (currentLine > pObject.Line)
                    {
                        //printer.LineFeed((byte)(currentLine - pObject.Line));
                        currentLine = pObject.Line;
                    }
                    else
                    {
                        if (!first)
                        {
                            if (lastFontStyle == "Bold" || lastFontStyle == "Negrito")
                            {
                                if (lastFontSize == "11")
                                {
                                    //printer.WriteLine_Bold(printText);
                                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold);
                                }
                                else
                                {
                                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold
                                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                                }
                            }
                            else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
                            {
                                if (lastFontSize == "11")
                                {
                                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline);
                                }
                                else
                                {
                                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline
                                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                                }
                            }
                            else
                            {
                                if (lastFontSize == "11")
                                {
                                    printer.WriteLine(printText);
                                }
                                else
                                {
                                    printer.WriteLine(printText,
                                        //(byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        +(byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                                }

                            }
                            //printer.WriteLine(printText);
                            printText = "";
                        }
                        first = false;
                    }
                    printer.SetAlignCenter();
                    switch (pObject.BarcodeType)
                    {
                        case "UPC-A":
                        case "upc_a":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.upc_a, barcodeValue);
                            break;
                        case "UPC-E":
                        case "upc_e":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.upc_e, barcodeValue);
                            break;
                        case "EAN13":
                        case "ean13":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.ean13, barcodeValue);
                            break;
                        case "EAN8":
                        case "ean8":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.ean8, barcodeValue);
                            break;
                        case "CODE 39":
                        case "code39":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.code39, barcodeValue);
                            break;
                        case "I25":
                        case "i25":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.i25, barcodeValue);
                            break;
                        case "CODEBAR":
                        case "codebar":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.codebar, barcodeValue);
                            break;
                        case "CODE 93":
                        case "code93":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.code93, barcodeValue);
                            break;
                        case "CODE 128":
                        case "code128":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.code128, barcodeValue);
                            break;
                        case "CODE 11":
                        case "code11":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.code11, barcodeValue);
                            break;
                        case "MSI":
                        case "msi":
                            printer.PrintBarcode(ThermalPrinter.BarcodeType.msi, barcodeValue);
                            break;
                    }

                }



            }
            //get to line
            if (lastFontStyle == "Bold" || lastFontStyle == "Negrito")
            {
                if (lastFontSize == "11")
                {
                    //printer.WriteLine_Bold(printText);
                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold);
                }
                else
                {
                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Bold
                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                        + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                }
            }
            else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
            {
                if (lastFontSize == "11")
                {
                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline);
                }
                else
                {
                    printer.WriteLine(printText, (byte)ThermalPrinter.PrintingStyle.Underline
                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                        + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                }
            }
            else
            {
                if (lastFontSize == "11")
                {
                    printer.WriteLine(printText);
                }
                else
                {
                    printer.WriteLine(printText,
                        //(byte)ThermalPrinter.PrintingStyle.DoubleHeight
                        +(byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                }

            }

            //printer.WriteLine(printText);
            //printer.LineFeed((byte)(2));
            printer.FeedVerticalAndCut(1);


        }

        public static List<PrintObject> GetObjectsFromTemplate(XmlDocument appconfigdoc)
        {

            List<PrintObject> printObjects = new List<PrintObject>();
            string xml = string.Empty;

            XmlNodeList controls = appconfigdoc.GetElementsByTagName("Control");

            foreach (XmlNode dev in controls)
            {

                switch (dev.Attributes["ID"].InnerText)
                {
                    case "Window":
                        PrintObject windows = new PrintObject(0);
                        windows.loadCanvasXML(dev, 0, windows);
                        printObjects.Add(windows);
                        break;
                    case "Picture":
                        PrintObject Picture = new PrintObject(2);
                        Picture.loadImageXML(dev, 0);
                        printObjects.Add(Picture);
                        break;
                    case "Texto":
                        PrintObject Texto = new PrintObject(1);
                        Texto.loadTextXML(dev, 0);
                        printObjects.Add(Texto);
                        break;
                    case "Barcode":
                        PrintObject Barcode = new PrintObject(3);
                        Barcode.loadBarcodeXML(dev, 0);
                        printObjects.Add(Barcode);
                        break;
                }
            }

            return printObjects;
        }

        public static PrintObject applyTextProperties(PrintObject Texto, DataRow[] drTemp, PrintObject prt, int maxLenght, int dataCount)
        {
            try
            {
                if (drTemp != null && drTemp.Length > 0)
                {
                    Texto.Text = drTemp[0].ItemArray[1].ToString();
                }
            }
            catch
            {
                Texto.Text = "-";
            }

            if (Texto.PosY > prt.FirstProductPosition)
            {
                Texto.PosY += (prt.valueToAdd * dataCount);
            }

            if (Texto.Text.Length > maxLenght)
            {
                Texto.Text = Texto.Text.Substring(0, maxLenght);
            }

            Texto.Text = Texto.Text.Replace("º", "");
            Texto.Text = Texto.Text.Replace("ª", "");

            if (Texto.Text.Equals("[breakLine]"))
            {
                Texto.Text = " ";
            }
            else if (Texto.Text.Equals("[line1]"))
            {
                Texto.Text = "________________________________________________";
            }
            else if (Texto.Text.Equals("[line2]"))
            {
                Texto.Text = "................................................";
            }


            return Texto;
        }

        public static PrintObject applyTextPropertiesLoop(PrintObject Texto, PrintObject prt, int maxLenght, int dataCount, string text, int valueToAddPosY, bool needToHaveMaxLenght, bool addPosY)
        {

            if (addPosY)
            {
                if (Texto.PosY > prt.FirstProductPosition)
                {
                    Texto.PosY += (prt.valueToAdd * dataCount);
                }
            }
            else
            {
                Texto.PosY += (prt.valueToAdd * valueToAddPosY);
            }

            Texto.Text = text;

            if (Texto.Text.Equals("[breakLine]"))
            {
                Texto.Text = " ";
            }
            else if (Texto.Text.Equals("[line1]"))
            {
                Texto.Text = "________________________________________________";
            }
            else if (Texto.Text.Equals("[line2]"))
            {
                Texto.Text = "................................................";
            }
            else if (Texto.Text.Equals(string.Empty))
            {

            }
            else
            {
                if (Texto.Text.Length > maxLenght)
                {
                    Texto.Text = Texto.Text.Substring(0, maxLenght);
                }

                if (needToHaveMaxLenght)
                {
                    while (Texto.Text.Length < maxLenght)
                    {
                        Texto.Text = " " + Texto.Text;
                    }
                }
            }

            return Texto;
        }

        public static List<PrintObject> GetObjectsFromTemplate(PrintObject prt, XmlDocument appconfigdoc, DataTable dataLoop, DataTable dataStatic)
        {
            List<PrintObject> printObjects = new List<PrintObject>();
            string xml = string.Empty;

            int dataCount = 0;

            dataCount = dataLoop.Rows.Count - 1;

            XmlNodeList controls = appconfigdoc.GetElementsByTagName("Control");

            foreach (XmlNode dev in controls)
            {

                switch (dev.Attributes["ID"].InnerText)
                {
                    case "Window":
                        PrintObject windows = new PrintObject(0);
                        windows.loadCanvasXML(dev, dataCount, prt);
                        printObjects.Add(windows);
                        break;
                    case "Picture":
                        PrintObject Picture = new PrintObject(2);
                        Picture.loadImageXML(dev, dataCount);
                        printObjects.Add(Picture);
                        break;
                    case "Texto":
                        PrintObject Texto = new PrintObject(1);
                        Texto.loadTextXML(dev, dataCount);
                        if (Texto.Text.Equals(string.Empty))
                        {
                            continue;
                        }
                        switch ((ThermalPrinter.OptionsToText)Enum.Parse(typeof(ThermalPrinter.OptionsToText), Texto.Type.ToString()))
                        {
                            case ThermalPrinter.OptionsToText.Contribuinte:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_FISCALNUMBER'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Contribuinte_Cliente:
                                try
                                {
                                    //Detect if is FinalConsumer with FiscalNumber 999999990 and Hide it
                                    //if (dataLoop.Rows[0]["ContribuinteCliente"].ToString() == _financeFinalConsumerFiscalNumber)
                                    //{
                                    //    dataLoop.Rows[0]["ContribuinteCliente"] = SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
                                    //}
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["ContribuinteCliente"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'ContribuinteCliente'"), prt, 48, dataCount);
                                //if (Texto.Text.Equals("999999990"))
                                //{
                                //    Texto.Text = "-";
                                //}
                                break;
                            case ThermalPrinter.OptionsToText.Desconto:
                                Texto = applyTextPropertiesLoop(Texto, prt, 6, dataCount, dataLoop.Rows[0]["DescontoProduto"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 6, dataCount, dataLoop.Rows[i]["DescontoProduto"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }

                                break;
                            case ThermalPrinter.OptionsToText.IVA_Produto:
                                Texto = applyTextPropertiesLoop(Texto, prt, 5, dataCount, dataLoop.Rows[0]["IVAProduto"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 5, dataCount, dataLoop.Rows[i]["IVAProduto"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Nome_Produto:
                                Texto = applyTextPropertiesLoop(Texto, prt, 20, dataCount, dataLoop.Rows[0]["NomeProduto"].ToString(), 0, false, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 20, dataCount, dataLoop.Rows[i]["NomeProduto"].ToString(), i, false, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Preco_Unidade:
                                Texto = applyTextPropertiesLoop(Texto, prt, 6, dataCount, dataLoop.Rows[0]["PrecoProduto"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 6, dataCount, dataLoop.Rows[i]["PrecoProduto"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Quantidade:
                                Texto = applyTextPropertiesLoop(Texto, prt, 6, dataCount, dataLoop.Rows[0]["QuantidadeProduto"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 6, dataCount, dataLoop.Rows[i]["QuantidadeProduto"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Morada:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_ADDRESS'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Nome_Cliente:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["NomeCliente"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'NomeCliente'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Nome_Empresa:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_NAME'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Numero_Documento:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["NumeroDocumento"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'NumeroDocumento'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Telefone:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_TELEPHONE'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Tipo_Pagamento:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["TipoPagamento"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TipoPagamento'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Total_do_IVA:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 6, dataCount, dataLoop.Rows[0]["TotalIVA"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 6, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TotalIVA'"), prt, 6, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Total_Final:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 10, dataCount, dataLoop.Rows[0]["TotalFinal"].ToString(), 0, true, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 10, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TotalFinal'"), prt, 10, dataCount);
                                //while (Texto.Text.Length < 10)
                                //{
                                //    Texto.Text = " " + Texto.Text;
                                //}
                                break;
                            case ThermalPrinter.OptionsToText.Total_Quantidade:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 10, dataCount, dataLoop.Rows[0]["TotalQuantidade"].ToString(), 0, true, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 10, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TotalQuantidade'"), prt, 10, dataCount);
                                //while (Texto.Text.Length < 10)
                                //{
                                //    Texto.Text = " " + Texto.Text;
                                //}
                                break;
                            case ThermalPrinter.OptionsToText.Total_Produto:
                                Texto = applyTextPropertiesLoop(Texto, prt, 7, dataCount, dataLoop.Rows[0]["TotalProdutoComIVA"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 7, dataCount, dataLoop.Rows[i]["TotalProdutoComIVA"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Total_Sem_IVA:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 12, dataCount, dataLoop.Rows[0]["TotalFinalSemIva"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 12, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TotalFinalSemIva'"), prt, 12, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Taxa_IVA:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 13, dataCount, dataLoop.Rows[0]["IVATotalProdutos"].ToString(), 0, true, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 13, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'IVATotalProdutos'"), prt, 13, dataCount);
                                //while (Texto.Text.Length < 4)
                                //{
                                //    Texto.Text = " " + Texto.Text;
                                //}
                                break;
                            case ThermalPrinter.OptionsToText.Num_Mesa:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 13, dataCount, dataLoop.Rows[0]["NumMesa"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 13, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'NumMesa'"), prt, 13, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Num_Func:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 13, dataCount, dataLoop.Rows[0]["NumFunc"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 13, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'NumFunc'"), prt, 13, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Nome_Func:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 13, dataCount, dataLoop.Rows[0]["NomeFunc"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 13, dataCount, "-", 0, false, true);
                                }
                                break;
                            //Recibo/Payments
                            case ThermalPrinter.OptionsToText.Total_Extenso:
                                try
                                {
                                    //Partir linha
                                    string totalExtensoSource = dataLoop.Rows[0]["TotalExtenso"].ToString();
                                    string totalExtenso = String.Empty;
                                    int maxCharsPerLine = 48;
                                    int length = 0;
                                    if (totalExtensoSource.Length > maxCharsPerLine)
                                    {
                                        int lines = Convert.ToInt16(Math.Floor(Convert.ToDecimal(totalExtensoSource.Length / maxCharsPerLine)));
                                        for (int i = 0; i < lines + 1; i++)
                                        {
                                            length = (i < lines) ? maxCharsPerLine : totalExtensoSource.Length - (i * maxCharsPerLine);
                                            totalExtenso += totalExtensoSource.Substring(i * maxCharsPerLine, length).Trim();
                                            if (i < lines) totalExtenso += Environment.NewLine;
                                        }
                                    }
                                    else
                                    {
                                        totalExtenso = totalExtensoSource;
                                    }

                                    Texto = applyTextPropertiesLoop(Texto, prt, 300, dataCount, totalExtenso, 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 300, dataCount, "-", 0, false, true);
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Numero_Documento_Recibo:
                                try
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["NumeroDocumentoRecibo"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = applyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Numero_Documento_Fiscal:
                                Texto = applyTextPropertiesLoop(Texto, prt, 25, dataCount, dataLoop.Rows[0]["NumeroDocumentoFiscal"].ToString(), 0, false, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 25, dataCount, dataLoop.Rows[i]["NumeroDocumentoFiscal"].ToString(), i, false, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Total_Documento_Fiscal:
                                Texto = applyTextPropertiesLoop(Texto, prt, 10, dataCount, dataLoop.Rows[0]["TotalDocumentoFiscal"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 10, dataCount, dataLoop.Rows[i]["TotalDocumentoFiscal"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Total_Liquidado:
                                Texto = applyTextPropertiesLoop(Texto, prt, 10, dataCount, Convert.ToDecimal(dataLoop.Rows[0]["TotalLiquidado"]).ToString("F"), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 10, dataCount, Convert.ToDecimal(dataLoop.Rows[i]["TotalLiquidado"]).ToString("F"), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case ThermalPrinter.OptionsToText.Total_Divida:
                                Texto = applyTextPropertiesLoop(Texto, prt, 10, dataCount, Convert.ToDecimal(dataLoop.Rows[0]["TotalDivida"]).ToString("F"), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    //Mostra total divida apenas se tiver valor a liquidar Documento
                                    decimal TotalFinal = Convert.ToDecimal(dataLoop.Rows[i]["TotalFinal"]);
                                    decimal TotalLiquidado = Convert.ToDecimal(dataLoop.Rows[i]["TotalLiquidado"]);
                                    string totalDivida = (0m).ToString("F");
                                    if (TotalLiquidado > 0 && TotalFinal > TotalLiquidado)
                                    {
                                        totalDivida = (TotalFinal - TotalLiquidado).ToString("F");
                                    };

                                    TextoDup = applyTextPropertiesLoop(TextoDup, prt, 10, dataCount, totalDivida, i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                            case ThermalPrinter.OptionsToText.Titulo:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TICKET_TITLE'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Total_Dinheiro_Caixa:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'CASHDRAWER_MOVEMENT_AMOUNT'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Email:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_EMAIL'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Data_Abertura:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'SESSION_OPEN_DATETIME'"), prt, 20, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Data_Fecho:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'SESSION_CLOSE_DATETIME'"), prt, 20, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Total_Abertura:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'SESSION_OPEN_TOTAL_CASHDRAWER'"), prt, 20, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Total_Fecho:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'SESSION_CLOSE_TOTAL_CASHDRAWER'"), prt, 20, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Total_Entradas:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'SESSION_TOTAL_MONEY_IN'"), prt, 20, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Total_Saidas:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'SESSION_TOTAL_MONEY_OUT'"), prt, 20, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Utilizador_Autenticado:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TERMINAL_USERNAME'"), prt, 12, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Terminal_Autenticado:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TERMINAL_NAME'"), prt, 12, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Numerario:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'SESSION_MOVEMENT_MONEY_IN_OUT'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.TextoLivre1:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TICKET_FREE_TEXT1'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.TextoLivre2:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TICKET_FREE_TEXT2'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.TextoLivre3:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TICKET_FREE_TEXT3'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Movimento_Descricao:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'CASHDRAWER_MOVEMENT_DESCRIPTION'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.SubTitulo:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TICKET_SUB_TITLE'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Footer_Line1:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE1'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Footer_Line2:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE2'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Footer_Line3:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE3'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Footer_Line4:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE4'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Footer_Line5:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE5'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Footer_Line6:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE6'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Footer_Line7:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE7'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Footer_Line8:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE8'"), prt, 48, dataCount);
                                break;
                            case ThermalPrinter.OptionsToText.Titulo_Copia:
                                Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TICKET_COPY_TITLE'"), prt, 48, dataCount);
                                break;
                            default:
                                if (Texto.PosY > prt.FirstProductPosition)
                                {
                                    Texto.PosY += (prt.valueToAdd * dataCount);
                                }
                                break;
                        }
                        printObjects.Add(Texto);
                        break;
                    case "Barcode":
                        PrintObject Barcode = new PrintObject(3);
                        Barcode.loadBarcodeXML(dev, dataCount);
                        printObjects.Add(Barcode);
                        break;
                }
            }

            return printObjects;
        }
    }
}
