using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.shared.Enums;
using logicpos.shared.Enums.ThermalPrinter;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Globalization;
using LogicPOS.Printing.Common;
using LogicPOS.Printing.Documents;
using LogicPOS.Reporting;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Xml;

namespace LogicPOS.Printing.Utility
{

    public static class PrintingUtils
    {
        public static BitmapData GetBitmapData(string bmpFileName)
        {
            using (var bitmap = (Bitmap)Image.FromFile(bmpFileName))
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

        public static BitmapData GetBitmapData(Bitmap bmpFileName)
        {
            using (var bitmap = bmpFileName)
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
                            printText += (new string(' ', pObject.Col - 1 - printText.Length)) + pObject.Text;
                        }
                        catch
                        {
                            if (pObject.Col == printText.Length)
                            {
                                printText += pObject.Text;
                            }
                            else
                            {
                                printText = printText.Remove(pObject.Col - 1, pObject.Text.Length).Insert(pObject.Col - 1, pObject.Text);
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
            int dataCount = dataLoop.Rows.Count - 1;
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
                                    string totalExtenso = string.Empty;
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

        //Get Printer Token 
        public static string GetPrinterToken(string pPrinterToken)
        {
            string result = pPrinterToken;

            //Check if Developer Enabled PDF Printer
            if (PrintingSettings.PrintPDFEnabled)
            {
                result = "REPORT_EXPORT_PDF";
            }

            return result;
        }

        private static bool SystemPrintInsert(fin_documentfinancemaster pDocumentFinanceMaster, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames, bool pSecondPrint, string pPrintMotive)
        {
            return SystemPrintInsert(pDocumentFinanceMaster, null, pPrinterDesignation, pPrintCopies, pCopyNames, pSecondPrint, pPrintMotive, XPOSettings.LoggedUser, XPOSettings.LoggedTerminal);
        }

        private static bool SystemPrintInsert(fin_documentfinancepayment pDocumentFinancePayment, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames)
        {
            return SystemPrintInsert(null, pDocumentFinancePayment, pPrinterDesignation, pPrintCopies, pCopyNames, false, string.Empty, XPOSettings.LoggedUser, XPOSettings.LoggedTerminal);
        }

        private static bool SystemPrintInsert(fin_documentfinancemaster pDocumentFinanceMaster, fin_documentfinancepayment pDocumentFinancePayment, string pPrinterDesignation, int pPrintCopies, List<int> pCopyNames, bool pSecondPrint, string pPrintMotive, sys_userdetail pUserDetail, pos_configurationplaceterminal pConfigurationPlaceTerminal)
        {
            bool result = false;

            //Start UnitOfWork
            using (UnitOfWork uowSession = new UnitOfWork())
            {
                string designation = string.Empty;
                //Get Objects into Current UOW Session
                sys_userdetail userDetail = (sys_userdetail)XPOHelper.GetXPGuidObject(uowSession, typeof(sys_userdetail), pUserDetail.Oid);
                pos_configurationplaceterminal configurationPlaceTerminal = (pos_configurationplaceterminal)XPOHelper.GetXPGuidObject(uowSession, typeof(pos_configurationplaceterminal), pConfigurationPlaceTerminal.Oid);

                //Convert CopyNames List to Comma Delimited String
                string copyNamesCommaDelimited = CustomReport.CopyNamesCommaDelimited(pCopyNames);

                //SystemPrint
                sys_systemprint systemPrint = new sys_systemprint(uowSession)
                {
                    Date = XPOHelper.CurrentDateTimeAtomic(),
                    Designation = designation,
                    PrintCopies = pPrintCopies,
                    CopyNames = copyNamesCommaDelimited,
                    SecondPrint = pSecondPrint,
                    UserDetail = userDetail,
                    Terminal = configurationPlaceTerminal
                };
                if (pPrintMotive != string.Empty) systemPrint.PrintMotive = pPrintMotive;

                //Mode: DocumentFinanceMaster
                if (pDocumentFinanceMaster != null)
                {
                    fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)XPOHelper.GetXPGuidObject(uowSession, typeof(fin_documentfinancemaster), pDocumentFinanceMaster.Oid);
                    systemPrint.DocumentMaster = documentFinanceMaster;
                    designation = string.Format("{0} {1} : {2}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_printed"), documentFinanceMaster.DocumentType.Designation, documentFinanceMaster.DocumentNumber);
                    //Update DocumentFinanceMaster
                    if (!documentFinanceMaster.Printed) documentFinanceMaster.Printed = true;
                }
                //Mode: DocumentFinancePayment
                if (pDocumentFinancePayment != null)
                {
                    fin_documentfinancepayment documentFinancePayment = (fin_documentfinancepayment)XPOHelper.GetXPGuidObject(uowSession, typeof(fin_documentfinancepayment), pDocumentFinancePayment.Oid);
                    systemPrint.DocumentPayment = documentFinancePayment;
                    designation = string.Format("{0} {1} : {2}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_printed"), documentFinancePayment.DocumentType.Designation, documentFinancePayment.PaymentRefNo);
                }
                systemPrint.Designation = designation;

                try
                {
                    //Commit UOW Changes : Before get current OrderMain
                    uowSession.CommitChanges();
                    //Audit
                    XPOHelper.Audit("SYSTEM_PRINT_FINANCE_DOCUMENT", designation);
                    result = true;
                }
                catch (Exception ex)
                {
                    uowSession.RollbackTransaction();
                }
            }

            return result;
        }

        public static bool PrintFinanceDocument(sys_configurationprinters pPrinter, fin_documentfinancemaster pDocumentFinanceMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive)
        {
            bool result = false;

            if (pPrinter != null)
            {
                int printCopies = pCopyNames.Count;
                //Get Hash4Chars from Hash
                string hash4Chars = CryptographyUtils.GetDocumentHash4Chars(pDocumentFinanceMaster.Hash);

                //Init Helper Vars
                bool resultSystemPrint;
                switch (GetPrinterToken(pPrinter.PrinterType.Token))
                {
                    //Impressora SINOCAN em ambiente Windows
                    case "THERMAL_PRINTER_WINDOWS":
                    //Impressora SINOCAN em ambiente Linux
                    //Impressora SINOCAN em ambiente WindowsLinux/Socket
                    case "THERMAL_PRINTER_SOCKET":
                        ThermalPrinterFinanceDocumentMaster thermalPrinterFinanceDocument = new ThermalPrinterFinanceDocumentMaster(pPrinter, pDocumentFinanceMaster, pCopyNames, pSecondCopy, pMotive);
                        thermalPrinterFinanceDocument.Print();
                        //Add to SystemPrint Audit
                        resultSystemPrint = SystemPrintInsert(pDocumentFinanceMaster, pPrinter.Designation, printCopies, pCopyNames, pSecondCopy, pMotive);
                        break;
                    case "GENERIC_PRINTER_WINDOWS":
                        CustomReport.ProcessReportFinanceDocument(CustomReportDisplayMode.Print, pDocumentFinanceMaster.Oid, hash4Chars, pCopyNames, pSecondCopy, pMotive);
                        //Add to SystemPrint Audit
                        resultSystemPrint = SystemPrintInsert(pDocumentFinanceMaster, pPrinter.Designation, printCopies, pCopyNames, pSecondCopy, pMotive);
                        break;
                    case "REPORT_EXPORT_PDF":
                        CustomReport.ProcessReportFinanceDocument(CustomReportDisplayMode.ExportPDF, pDocumentFinanceMaster.Oid, hash4Chars, pCopyNames, pSecondCopy, pMotive);
                        //Add to SystemPrint Audit : Developer : Use here Only to Test SystemPrintInsert
                        resultSystemPrint = SystemPrintInsert(pDocumentFinanceMaster, pPrinter.Designation, printCopies, pCopyNames, pSecondCopy, pMotive);
                        break;
                }
                result = true;

            }

            return result;
        }

        //Quick function to get Printer and Template and Send to Base PrintFinanceDocument, and Open Drawer if Has a Valid Payment Method
        public static bool PrintFinanceDocument(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            return PrintFinanceDocument(XPOSettings.Session, pDocumentFinanceMaster);
        }

        public static bool PrintFinanceDocument(Session pSession, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            List<int> printCopies = new List<int>();
            for (int i = 0; i < pDocumentFinanceMaster.DocumentType.PrintCopies; i++)
            {
                printCopies.Add(i);
            }

            return PrintFinanceDocument(pSession, pDocumentFinanceMaster, printCopies, false, string.Empty);
        }

        public static bool PrintFinanceDocument(Session pSession, fin_documentfinancemaster pDocumentFinanceMaster, List<int> pCopyNames, bool pSecondCopy, string pMotive)
        {
            bool result;

            //Finish Payment with Print Job + Open Drawer (If Not TableConsult)
            fin_documentfinanceyearserieterminal xDocumentFinanceYearSerieTerminal = DocumentProcessingSeriesUtils.GetDocumentFinanceYearSerieTerminal(pSession, pDocumentFinanceMaster.DocumentType.Oid);
            PrintFinanceDocument(XPOSettings.LoggedTerminal.Printer, pDocumentFinanceMaster, pCopyNames, pSecondCopy, pMotive);

            //Open Door if Has Valid Payment
            if (pDocumentFinanceMaster.PaymentMethod != null)
            {
                OpenDoor(XPOSettings.LoggedTerminal.ThermalPrinter);
            }
            result = true;

            return result;
        }

        public static bool PrintFinanceDocumentPayment(sys_configurationprinters pPrinter, fin_documentfinancepayment pDocumentFinancePayment)
        {
            bool result = false;

            if (pPrinter != null)
            {
                //Initialize CopyNames List from PrintCopies
                List<int> copyNames = CustomReport.CopyNames(pDocumentFinancePayment.DocumentType.PrintCopies);
                int printCopies = copyNames.Count;

                //Init Helper Vars
                bool resultSystemPrint;
                switch (GetPrinterToken(pPrinter.PrinterType.Token))
                {
                    //Impressora SINOCAN em ambiente Windows
                    case "THERMAL_PRINTER_WINDOWS":
                    //Impressora SINOCAN em ambiente Linux
                    case "THERMAL_PRINTER_LINUX":
                    //Impressora SINOCAN em ambiente WindowsLinux/Socket
                    case "THERMAL_PRINTER_SOCKET":
                        ThermalPrinterFinanceDocumentPayment thermalPrinterFinanceDocumentPayment = new ThermalPrinterFinanceDocumentPayment(pPrinter, pDocumentFinancePayment, copyNames, false);
                        thermalPrinterFinanceDocumentPayment.Print();
                        //Add to SystemPrint Audit
                        resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, pPrinter.Designation, printCopies, copyNames);
                        break;
                    case "GENERIC_PRINTER_WINDOWS":
                        CustomReport.ProcessReportFinanceDocumentPayment(CustomReportDisplayMode.Print, pDocumentFinancePayment.Oid, copyNames);
                        //Add to SystemPrint Audit
                        resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, pPrinter.Designation, printCopies, copyNames);
                        break;
                    case "REPORT_EXPORT_PDF":
                        CustomReport.ProcessReportFinanceDocumentPayment(CustomReportDisplayMode.ExportPDF, pDocumentFinancePayment.Oid, copyNames);
                        //Add to SystemPrint Audit : Developer : Use here Only to Test SystemPrintInsert
                        resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, pPrinter.Designation, printCopies, copyNames);
                        break;
                }
                result = true;

            }
            return result;
        }

        public static bool PrintWorkSessionMovement(sys_configurationprinters pPrinter, pos_worksessionperiod pWorkSessionPeriod)
        {
            bool result = false;

            if (pPrinter != null)
            {

                switch (GetPrinterToken(pPrinter.PrinterType.Token))
                {
                    //Impressora SINOCAN em ambiente Windows
                    case "THERMAL_PRINTER_WINDOWS":
                    //Impressora SINOCAN em ambiente Linux
                    case "THERMAL_PRINTER_LINUX":
                    //Impressora SINOCAN em ambiente WindowsLinux/Socket
                    case "THERMAL_PRINTER_SOCKET":
                        //NonCurrentAcount
                        ThermalPrinterInternalDocumentWorkSession thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, pWorkSessionPeriod, SplitCurrentAccountMode.NonCurrentAcount);
                        thermalPrinterInternalDocumentWorkSession.Print();
                        //CurrentAcount
                        //Use Config to print this
                        if (Convert.ToBoolean(LogicPOS.Settings.GeneralSettings.PreferenceParameters["USE_CC_DAILY_TICKET"]))
                        {
                            thermalPrinterInternalDocumentWorkSession = new ThermalPrinterInternalDocumentWorkSession(pPrinter, pWorkSessionPeriod, SplitCurrentAccountMode.CurrentAcount);
                            thermalPrinterInternalDocumentWorkSession.Print();
                        }
                        break;
                    case "GENERIC_PRINTER_WINDOWS":
                        break;
                    case "REPORT_EXPORT_PDF":
                        break;
                }
                result = true;

            }
            return result;
        }

        public static bool PrintArticleRequest(fin_documentorderticket pOrderTicket)
        {
            bool result;

            //Initialize printerArticleQueue to Store Articles > Printer Queue
            List<sys_configurationprinters> printerArticles = new List<sys_configurationprinters>();
            foreach (fin_documentorderdetail item in pOrderTicket.OrderDetail)
            {
                if (item.Article.Printer != null && item.Article.Printer.PrinterType.ThermalPrinter)
                {
                    //Add Printer
                    if (!printerArticles.Contains(item.Article.Printer)) printerArticles.Add(item.Article.Printer);
                }
            }

            //Print Tickets for Article Printers
            if (printerArticles.Count > 0)
            {
                foreach (sys_configurationprinters item in printerArticles)
                {
                    ThermalPrinterInternalDocumentOrderRequest thermalPrinterInternalDocumentOrderRequest = new ThermalPrinterInternalDocumentOrderRequest(item, pOrderTicket, true);
                    thermalPrinterInternalDocumentOrderRequest.Print();
                }
            }
            result = true;

            return result;
        }

        //Used for Money Movements and Open/Close Terminal/Day Sessions
        public static bool PrintCashDrawerOpenAndMoneyInOut(sys_configurationprinters pPrinter, string pTicketTitle, decimal pMovementAmount, decimal pTotalAmountInCashDrawer, string pMovementDescription)
        {
            bool result = false;

            if (pPrinter != null)
            {
                switch (GetPrinterToken(pPrinter.PrinterType.Token))
                {
                    //Impressora SINOCAN em ambiente Windows
                    case "THERMAL_PRINTER_WINDOWS":
                    //Impressora SINOCAN em ambiente Linux
                    case "THERMAL_PRINTER_LINUX":
                    //Impressora SINOCAN em ambiente WindowsLinux/Socket
                    case "THERMAL_PRINTER_SOCKET":
                        ThermalPrinterInternalDocumentCashDrawer thermalPrinterInternalDocumentCashDrawer = new ThermalPrinterInternalDocumentCashDrawer(pPrinter, pTicketTitle, pTotalAmountInCashDrawer, pMovementAmount, pMovementDescription);
                        thermalPrinterInternalDocumentCashDrawer.Print();
                        break;
                }
                result = true;
            }
            return result;
        }

        public static bool OpenDoor(sys_configurationprinters pPrinter)
        {
            bool result = false;
            if (XPOSettings.LoggedTerminal.ThermalPrinter != null)
            {
                bool hasPermission = GeneralSettings.HasPermissionTo("HARDWARE_DRAWER_OPEN");
                int m = XPOSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueM;
                int t1 = XPOSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueT1;
                int t2 = XPOSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueT2;
                PrintObject printObjectSINOCAN = new PrintObject(0);
                if (XPOSettings.LoggedTerminal.ThermalPrinter != null && hasPermission)
                {
                    switch (XPOSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token)
                    {
                        //Impressora SINOCAN em ambiente Windows
                        case "THERMAL_PRINTER_WINDOWS":
                            printObjectSINOCAN.OpenDoor(XPOSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token, XPOSettings.LoggedTerminal.ThermalPrinter.Designation, m, t1, t2);
                            break;
                        //Impressora SINOCAN em ambiente Linux
                        case "THERMAL_PRINTER_SOCKET":
                            // Deprecated
                            //int m = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueM"]);
                            //int t1 = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueT1"]);
                            //int t2 = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueT2"]);
                            // Open Drawer
                            //TK016249 - Impressoras - Diferenciação entre Tipos
                            printObjectSINOCAN.OpenDoor(XPOSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token, XPOSettings.LoggedTerminal.ThermalPrinter.NetworkName, m, t1, t2);
                            //Audit
                            XPOHelper.Audit("CASHDRAWER_OPEN", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "audit_message_cashdrawer_open"));

                            break;
                    }

                    result = true;
                }

                return result;
            }

            return result;

        }

    }
}
