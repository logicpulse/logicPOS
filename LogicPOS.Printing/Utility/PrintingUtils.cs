using DevExpress.Xpo;
using logicpos.shared.Enums;
using LogicPOS.Api.Entities;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Settings.Terminal;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Common;
using LogicPOS.Printing.Documents;
using LogicPOS.Printing.Enums;
using LogicPOS.Reporting.Common;
using LogicPOS.Reporting.Utility;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace LogicPOS.Printing.Utility
{

    public static class PrintingUtils
    {
        public static List<PrintObject> CalculatePrintCoordinates(
            List<PrintObject> printObjects,
            int PrintCharWidth,
            int PrintLineHeight)
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

        public static void PreparePrintDocument(
            ThermalPrinter printer,
            List<PrintObject> printObjects,
            int PrintCharWidth,
            int PrintLineHeight)
        {
            PreparePrintDocument(
                printer,
                printObjects,
                PrintCharWidth,
                PrintLineHeight,
                string.Empty);
        }

        public static void PreparePrintDocument(
            ThermalPrinter printer,
            List<PrintObject> printObjects,
            int PrintCharWidth,
            int PrintLineHeight,
            string barcodeValue)
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
                                printer.WriteLine(printText, (byte)PrintingStyle.Bold);
                            }
                            else
                            {
                                printer.WriteLine(printText, (byte)PrintingStyle.Bold
                                    //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    + (byte)PrintingStyle.DoubleWidth);
                            }
                        }
                        else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
                        {
                            if (lastFontSize == "11")
                            {
                                printer.WriteLine(printText, (byte)PrintingStyle.Underline);
                            }
                            else
                            {
                                printer.WriteLine(printText, (byte)PrintingStyle.Underline
                                    //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    + (byte)PrintingStyle.DoubleWidth);
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
                                    +(byte)PrintingStyle.DoubleWidth);
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
                                    printer.WriteLine(printText, (byte)PrintingStyle.Bold);
                                }
                                else
                                {
                                    printer.WriteLine(printText, (byte)PrintingStyle.Bold
                                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        + (byte)PrintingStyle.DoubleWidth);
                                }
                            }
                            else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
                            {
                                if (lastFontSize == "11")
                                {
                                    printer.WriteLine(printText, (byte)PrintingStyle.Underline);
                                }
                                else
                                {
                                    printer.WriteLine(printText, (byte)PrintingStyle.Underline
                                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        + (byte)PrintingStyle.DoubleWidth);
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
                                        +(byte)PrintingStyle.DoubleWidth);
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
                                printer.WriteLine(printText, (byte)PrintingStyle.Bold);
                            }
                            else
                            {
                                printer.WriteLine(printText, (byte)PrintingStyle.Bold
                                    //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    + (byte)PrintingStyle.DoubleWidth);
                            }
                        }
                        else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
                        {
                            if (lastFontSize == "11")
                            {
                                printer.WriteLine(printText, (byte)PrintingStyle.Underline);
                            }
                            else
                            {
                                printer.WriteLine(printText, (byte)PrintingStyle.Underline
                                    //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                    + (byte)PrintingStyle.DoubleWidth);
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
                                    +(byte)PrintingStyle.DoubleWidth);
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
                                    printer.WriteLine(printText, (byte)PrintingStyle.Bold);
                                }
                                else
                                {
                                    printer.WriteLine(printText, (byte)PrintingStyle.Bold
                                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        + (byte)PrintingStyle.DoubleWidth);
                                }
                            }
                            else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
                            {
                                if (lastFontSize == "11")
                                {
                                    printer.WriteLine(printText, (byte)PrintingStyle.Underline);
                                }
                                else
                                {
                                    printer.WriteLine(printText, (byte)PrintingStyle.Underline
                                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                                        + (byte)PrintingStyle.DoubleWidth);
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
                                        +(byte)PrintingStyle.DoubleWidth);
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
                            printer.PrintBarcode(BarcodeType.upc_a, barcodeValue);
                            break;
                        case "UPC-E":
                        case "upc_e":
                            printer.PrintBarcode(BarcodeType.upc_e, barcodeValue);
                            break;
                        case "EAN13":
                        case "ean13":
                            printer.PrintBarcode(BarcodeType.ean13, barcodeValue);
                            break;
                        case "EAN8":
                        case "ean8":
                            printer.PrintBarcode(BarcodeType.ean8, barcodeValue);
                            break;
                        case "CODE 39":
                        case "code39":
                            printer.PrintBarcode(BarcodeType.code39, barcodeValue);
                            break;
                        case "I25":
                        case "i25":
                            printer.PrintBarcode(BarcodeType.i25, barcodeValue);
                            break;
                        case "CODEBAR":
                        case "codebar":
                            printer.PrintBarcode(BarcodeType.codebar, barcodeValue);
                            break;
                        case "CODE 93":
                        case "code93":
                            printer.PrintBarcode(BarcodeType.code93, barcodeValue);
                            break;
                        case "CODE 128":
                        case "code128":
                            printer.PrintBarcode(BarcodeType.code128, barcodeValue);
                            break;
                        case "CODE 11":
                        case "code11":
                            printer.PrintBarcode(BarcodeType.code11, barcodeValue);
                            break;
                        case "MSI":
                        case "msi":
                            printer.PrintBarcode(BarcodeType.msi, barcodeValue);
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
                    printer.WriteLine(printText, (byte)PrintingStyle.Bold);
                }
                else
                {
                    printer.WriteLine(printText, (byte)PrintingStyle.Bold
                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                        + (byte)PrintingStyle.DoubleWidth);
                }
            }
            else if (lastFontStyle == "Underline" || lastFontStyle == "Sublinhado")
            {
                if (lastFontSize == "11")
                {
                    printer.WriteLine(printText, (byte)PrintingStyle.Underline);
                }
                else
                {
                    printer.WriteLine(printText, (byte)PrintingStyle.Underline
                        //+ (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                        + (byte)PrintingStyle.DoubleWidth);
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
                        +(byte)PrintingStyle.DoubleWidth);
                }

            }

            //printer.WriteLine(printText);
            //printer.LineFeed((byte)(2));
            printer.FeedVerticalAndCut(1);


        }

        public static PrintObject ApplyTextProperties(
            PrintObject Texto,
            DataRow[] drTemp,
            PrintObject prt,
            int maxLenght,
            int dataCount)
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

        public static PrintObject ApplyTextPropertiesLoop(
            PrintObject Texto,
            PrintObject prt,
            int maxLenght,
            int dataCount,
            string text,
            int valueToAddPosY,
            bool needToHaveMaxLenght,
            bool addPosY)
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

        public static List<PrintObject> GetObjectsFromTemplate(
            PrintObject prt,
            XmlDocument appconfigdoc,
            DataTable dataLoop,
            DataTable dataStatic)
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
                        switch ((OptionsToText)Enum.Parse(typeof(OptionsToText), Texto.Type.ToString()))
                        {
                            case OptionsToText.Contribuinte:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_FISCALNUMBER'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Contribuinte_Cliente:
                                try
                                {
                                    //Detect if is FinalConsumer with FiscalNumber 999999990 and Hide it
                                    //if (dataLoop.Rows[0]["ContribuinteCliente"].ToString() == _financeFinalConsumerFiscalNumber)
                                    //{
                                    //    dataLoop.Rows[0]["ContribuinteCliente"] = SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
                                    //}
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["ContribuinteCliente"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'ContribuinteCliente'"), prt, 48, dataCount);
                                //if (Texto.Text.Equals("999999990"))
                                //{
                                //    Texto.Text = "-";
                                //}
                                break;
                            case OptionsToText.Desconto:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 6, dataCount, dataLoop.Rows[0]["DescontoProduto"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 6, dataCount, dataLoop.Rows[i]["DescontoProduto"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }

                                break;
                            case OptionsToText.IVA_Produto:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 5, dataCount, dataLoop.Rows[0]["IVAProduto"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 5, dataCount, dataLoop.Rows[i]["IVAProduto"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case OptionsToText.Nome_Produto:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 20, dataCount, dataLoop.Rows[0]["NomeProduto"].ToString(), 0, false, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 20, dataCount, dataLoop.Rows[i]["NomeProduto"].ToString(), i, false, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case OptionsToText.Preco_Unidade:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 6, dataCount, dataLoop.Rows[0]["PrecoProduto"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 6, dataCount, dataLoop.Rows[i]["PrecoProduto"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case OptionsToText.Quantidade:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 6, dataCount, dataLoop.Rows[0]["QuantidadeProduto"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 6, dataCount, dataLoop.Rows[i]["QuantidadeProduto"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case OptionsToText.Morada:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_ADDRESS'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Nome_Cliente:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["NomeCliente"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'NomeCliente'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Nome_Empresa:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_NAME'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Numero_Documento:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["NumeroDocumento"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'NumeroDocumento'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Telefone:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_TELEPHONE'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Tipo_Pagamento:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["TipoPagamento"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TipoPagamento'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Total_do_IVA:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 6, dataCount, dataLoop.Rows[0]["TotalIVA"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 6, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TotalIVA'"), prt, 6, dataCount);
                                break;
                            case OptionsToText.Total_Final:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 10, dataCount, dataLoop.Rows[0]["TotalFinal"].ToString(), 0, true, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 10, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TotalFinal'"), prt, 10, dataCount);
                                //while (Texto.Text.Length < 10)
                                //{
                                //    Texto.Text = " " + Texto.Text;
                                //}
                                break;
                            case OptionsToText.Total_Quantidade:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 10, dataCount, dataLoop.Rows[0]["TotalQuantidade"].ToString(), 0, true, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 10, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TotalQuantidade'"), prt, 10, dataCount);
                                //while (Texto.Text.Length < 10)
                                //{
                                //    Texto.Text = " " + Texto.Text;
                                //}
                                break;
                            case OptionsToText.Total_Produto:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 7, dataCount, dataLoop.Rows[0]["TotalProdutoComIVA"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 7, dataCount, dataLoop.Rows[i]["TotalProdutoComIVA"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case OptionsToText.Total_Sem_IVA:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 12, dataCount, dataLoop.Rows[0]["TotalFinalSemIva"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 12, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'TotalFinalSemIva'"), prt, 12, dataCount);
                                break;
                            case OptionsToText.Taxa_IVA:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 13, dataCount, dataLoop.Rows[0]["IVATotalProdutos"].ToString(), 0, true, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 13, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'IVATotalProdutos'"), prt, 13, dataCount);
                                //while (Texto.Text.Length < 4)
                                //{
                                //    Texto.Text = " " + Texto.Text;
                                //}
                                break;
                            case OptionsToText.Num_Mesa:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 13, dataCount, dataLoop.Rows[0]["NumMesa"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 13, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'NumMesa'"), prt, 13, dataCount);
                                break;
                            case OptionsToText.Num_Func:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 13, dataCount, dataLoop.Rows[0]["NumFunc"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 13, dataCount, "-", 0, false, true);
                                }
                                //Texto = applyTextProperties(Texto, dataStatic.Select("token = 'NumFunc'"), prt, 13, dataCount);
                                break;
                            case OptionsToText.Nome_Func:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 13, dataCount, dataLoop.Rows[0]["NomeFunc"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 13, dataCount, "-", 0, false, true);
                                }
                                break;
                            //Recibo/Payments
                            case OptionsToText.Total_Extenso:
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

                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 300, dataCount, totalExtenso, 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 300, dataCount, "-", 0, false, true);
                                }
                                break;
                            case OptionsToText.Numero_Documento_Recibo:
                                try
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, dataLoop.Rows[0]["NumeroDocumentoRecibo"].ToString(), 0, false, true);
                                }
                                catch
                                {
                                    Texto = ApplyTextPropertiesLoop(Texto, prt, 48, dataCount, "-", 0, false, true);
                                }
                                break;
                            case OptionsToText.Numero_Documento_Fiscal:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 25, dataCount, dataLoop.Rows[0]["NumeroDocumentoFiscal"].ToString(), 0, false, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 25, dataCount, dataLoop.Rows[i]["NumeroDocumentoFiscal"].ToString(), i, false, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case OptionsToText.Total_Documento_Fiscal:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 10, dataCount, dataLoop.Rows[0]["TotalDocumentoFiscal"].ToString(), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 10, dataCount, dataLoop.Rows[i]["TotalDocumentoFiscal"].ToString(), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case OptionsToText.Total_Liquidado:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 10, dataCount, Convert.ToDecimal(dataLoop.Rows[0]["TotalLiquidado"]).ToString("F"), 0, true, false);

                                for (int i = 1; i < dataLoop.Rows.Count; i++)
                                {
                                    PrintObject TextoDup = new PrintObject(1);
                                    TextoDup.loadTextXML(dev, dataCount);

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 10, dataCount, Convert.ToDecimal(dataLoop.Rows[i]["TotalLiquidado"]).ToString("F"), i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            case OptionsToText.Total_Divida:
                                Texto = ApplyTextPropertiesLoop(Texto, prt, 10, dataCount, Convert.ToDecimal(dataLoop.Rows[0]["TotalDivida"]).ToString("F"), 0, true, false);

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

                                    TextoDup = ApplyTextPropertiesLoop(TextoDup, prt, 10, dataCount, totalDivida, i, true, false);

                                    if (!TextoDup.Text.Equals(string.Empty))
                                    {
                                        printObjects.Add(TextoDup);
                                    }
                                }
                                break;
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                            case OptionsToText.Titulo:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'TICKET_TITLE'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Total_Dinheiro_Caixa:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'CASHDRAWER_MOVEMENT_AMOUNT'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Email:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'COMPANY_EMAIL'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Data_Abertura:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'SESSION_OPEN_DATETIME'"), prt, 20, dataCount);
                                break;
                            case OptionsToText.Data_Fecho:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'SESSION_CLOSE_DATETIME'"), prt, 20, dataCount);
                                break;
                            case OptionsToText.Total_Abertura:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'SESSION_OPEN_TOTAL_CASHDRAWER'"), prt, 20, dataCount);
                                break;
                            case OptionsToText.Total_Fecho:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'SESSION_CLOSE_TOTAL_CASHDRAWER'"), prt, 20, dataCount);
                                break;
                            case OptionsToText.Total_Entradas:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'SESSION_TOTAL_MONEY_IN'"), prt, 20, dataCount);
                                break;
                            case OptionsToText.Total_Saidas:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'SESSION_TOTAL_MONEY_OUT'"), prt, 20, dataCount);
                                break;
                            case OptionsToText.Utilizador_Autenticado:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'TERMINAL_USERNAME'"), prt, 12, dataCount);
                                break;
                            case OptionsToText.Terminal_Autenticado:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'TERMINAL_NAME'"), prt, 12, dataCount);
                                break;
                            case OptionsToText.Numerario:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'SESSION_MOVEMENT_MONEY_IN_OUT'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.TextoLivre1:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'TICKET_FREE_TEXT1'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.TextoLivre2:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'TICKET_FREE_TEXT2'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.TextoLivre3:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'TICKET_FREE_TEXT3'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Movimento_Descricao:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'CASHDRAWER_MOVEMENT_DESCRIPTION'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.SubTitulo:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'TICKET_SUB_TITLE'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Footer_Line1:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE1'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Footer_Line2:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE2'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Footer_Line3:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE3'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Footer_Line4:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE4'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Footer_Line5:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE5'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Footer_Line6:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE6'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Footer_Line7:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE7'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Footer_Line8:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'FOOTER_LINE8'"), prt, 48, dataCount);
                                break;
                            case OptionsToText.Titulo_Copia:
                                Texto = ApplyTextProperties(Texto, dataStatic.Select("token = 'TICKET_COPY_TITLE'"), prt, 48, dataCount);
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


        public static string GetPrinterToken(string printerToken)
        {
            return PrintingSettings.PrintPDFEnabled ? "REPORT_EXPORT_PDF" : printerToken;
        }

        private static bool SystemPrintInsert(
            Document pDocumentFinanceMaster,
            string pPrinterDesignation,
            int pPrintCopies,
            List<int> pCopyNames,
            bool pSecondPrint,
            string pPrintMotive)
        {
            return XPOUtility.InsertSystemPrint(
                pDocumentFinanceMaster,
                null,
                pPrinterDesignation,
                pPrintCopies,
                pCopyNames,
                pSecondPrint,
                pPrintMotive,
                XPOSettings.LoggedUser.Oid,
                TerminalSettings.LoggedTerminal.Oid);
        }

        private static bool SystemPrintInsert(
            Document pDocumentFinancePayment,
            string pPrinterDesignation,
            int pPrintCopies,
            List<int> pCopyNames)
        {
            return XPOUtility.InsertSystemPrint(
                pDocumentFinancePayment,null,
                pPrinterDesignation,
                pPrintCopies,
                pCopyNames,
                false,
                string.Empty,
                XPOSettings.LoggedUser.Oid,
                TerminalSettings.LoggedTerminal.Oid);
        }

        public static bool PrintFinanceDocument(
            PrinterDto printer,
            string terminalDesignation,
            string userName,
            CompanyInformationsDto companyInformationsDto,
            Document financeMasterDto,
            List<int> copyNumbers,
            bool secondCopy,
            string motive)
        {
            bool result = false;

            if (printer != null)
            {
                int printCopies = copyNumbers.Count;
                //Get Hash4Chars from Hash
                string hash4Chars = CryptographyUtils.GetDocumentHash4Chars(financeMasterDto.Hash);

                //Init Helper Vars
                bool resultSystemPrint;
                switch (GetPrinterToken(printer.Token))
                {

                    case "THERMAL_PRINTER_WINDOWS":
                    case "THERMAL_PRINTER_SOCKET":

                        var financeMasterViewReports = ReportDataHelper.GetFinanceMasterViewReportDataList(financeMasterDto.Id).List;
                        var financeMasterViewReportsDtos = financeMasterViewReports.ConvertAll(view => ReportDataMapping.GetFinanceMasterViewReportDto(view));

                        FinanceMaster thermalPrinterFinanceDocument = new FinanceMaster(
                            printer,
                            terminalDesignation,
                            userName,
                            financeMasterDto,
                            companyInformationsDto,
                            copyNumbers,
                            secondCopy,
                            motive);

                        thermalPrinterFinanceDocument.Print();
                        //Add to SystemPrint Audit
                        resultSystemPrint = SystemPrintInsert(financeMasterDto, printer.Designation, printCopies, copyNumbers, secondCopy, motive);
                        break;
                    case "GENERIC_PRINTER_WINDOWS":
                        Reporting.Common.FastReport.ProcessReportFinanceDocument(
                            CustomReportDisplayMode.Print, 
                            financeMasterDto.Id, 
                            hash4Chars, 
                            copyNumbers, 
                            secondCopy, 
                            motive);

                        //Add to SystemPrint Audit
                        resultSystemPrint = SystemPrintInsert(financeMasterDto, printer.Designation, printCopies, copyNumbers, secondCopy, motive);
                        break;
                    case "REPORT_EXPORT_PDF":
                        Reporting.Common.FastReport.ProcessReportFinanceDocument(CustomReportDisplayMode.ExportPDF, financeMasterDto.Id, hash4Chars, copyNumbers, secondCopy, motive);
                        //Add to SystemPrint Audit : Developer : Use here Only to Test SystemPrintInsert
                        resultSystemPrint = SystemPrintInsert(financeMasterDto, printer.Designation, printCopies, copyNumbers, secondCopy, motive);
                        break;
                }
                result = true;

            }

            return result;
        }

        public static bool PrintFinanceDocument(PrinterDto printer,
            Document pDocumentFinanceMaster)
        {
            return PrintFinanceDocument(printer, pDocumentFinanceMaster);
        }

        public static bool PrintFinanceDocument(
            PrinterDto printer,
            PrintDocumentMasterDto financeMaster)
        {
            List<int> printCopies = new List<int>();
            for (int i = 0; i < financeMaster.DocumentType.PrintCopies; i++)
            {
                printCopies.Add(i);
            }

            return PrintFinanceDocument(
                printer,
                financeMaster,
                printCopies,
                false,
                string.Empty);
        }

        public static bool PrintFinanceDocument(
            PrinterDto pPrinter,
            PrintDocumentMasterDto financeMaster,
            List<int> pCopyNames,
            bool pSecondCopy,
            string pMotive)
        {
            bool result;

            //Commented By Tchialo: Appearently this is not used
            //DocumentProcessingSeriesUtils.GetDocumentFinanceYearSerieTerminal(
            //    pPrinter,
            //    financeMaster.DocumentType.Id);

            var printer = LoggedTerminalSettings.GetPrinterDto();

            PrintFinanceDocument(
                printer,
                financeMaster,
                pCopyNames,
                pSecondCopy,
                pMotive);

            if (financeMaster.HasValidPaymentMethod)
            {
                OpenDoor();
            }
            result = true;

            return result;
        }

        public static bool PrintFinanceDocumentPayment(
            PrinterDto printer,
            string terminalDesignation,
            string userName,
            CompanyInformationsDto companyInformationsDto,
            Document pDocumentFinancePayment)
        {
            bool result = false;

            if (printer != null)
            {
                //Initialize CopyNames List from PrintCopies
                List<int> copyNames = new List<int>();
                copyNames.Add(0);
                int printCopies = copyNames.Count;

                //Init Helper Vars
                bool resultSystemPrint;
                switch (GetPrinterToken(printer.Token))
                {
                    case "THERMAL_PRINTER_WINDOWS":
                    case "THERMAL_PRINTER_SOCKET":

                        var financePaymentViewReports = ReportDataHelper.GetFinancePaymentViewReportDataList(pDocumentFinancePayment.Id).List;
                        var financePaymentViewReportsDtos = financePaymentViewReports.ConvertAll(view => ReportDataMapping.GetFinancePaymentViewReportDto(view));

                        FinancePayment thermalPrinterFinanceDocumentPayment = 
                            new FinancePayment(
                                printer, 
                                pDocumentFinancePayment, 
                                terminalDesignation,
                                userName,
                                companyInformationsDto,

                                copyNames, 
                                false,
                                financePaymentViewReportsDtos
                                );

                        thermalPrinterFinanceDocumentPayment.Print();
                        resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, printer.Designation, printCopies, copyNames);
                        break;
                    case "GENERIC_PRINTER_WINDOWS":
                        Reporting.Common.FastReport.ProcessReportFinanceDocumentPayment(CustomReportDisplayMode.Print, pDocumentFinancePayment.Id, copyNames);
                        //Add to SystemPrint Audit
                        resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, printer.Designation, printCopies, copyNames);
                        break;
                    case "REPORT_EXPORT_PDF":
                        Reporting.Common.FastReport.ProcessReportFinanceDocumentPayment(CustomReportDisplayMode.ExportPDF, pDocumentFinancePayment.Id, copyNames);
                        //Add to SystemPrint Audit : Developer : Use here Only to Test SystemPrintInsert
                        resultSystemPrint = SystemPrintInsert(pDocumentFinancePayment, printer.Designation, printCopies, copyNames);
                        break;
                }
                result = true;

            }
            return result;
        }

        public static bool PrintWorkSessionMovement(
            PrinterDto printer,
            string terminalDesignation,
            PrintWorkSessionDto workSessionPeriod,
            string workSessionMovementPrintingFileTemplate,
            Hashtable sessionPeriodSummaryDetails
            )
        {
            bool result = false;

            if (printer != null)
            {

                switch (GetPrinterToken(printer.Token))
                {
                    case "THERMAL_PRINTER_WINDOWS":
                    case "THERMAL_PRINTER_SOCKET":

                        WorkSessionPrinter workSessiontPrinter = new WorkSessionPrinter(
                            printer, 
                            terminalDesignation,
                            workSessionPeriod, 
                            SplitCurrentAccountMode.NonCurrentAcount,
                            workSessionMovementPrintingFileTemplate,
                            sessionPeriodSummaryDetails);

                        workSessiontPrinter.Print();

                        if (Convert.ToBoolean(GeneralSettings.PreferenceParameters["USE_CC_DAILY_TICKET"]))
                        {
                            workSessiontPrinter = new WorkSessionPrinter(
                                printer,                               
                                terminalDesignation,
                                workSessionPeriod, 
                                SplitCurrentAccountMode.CurrentAcount,
                                workSessionMovementPrintingFileTemplate,
                                sessionPeriodSummaryDetails);

                            workSessiontPrinter.Print();
                        }
                        break;
                }
                result = true;

            }
            return result;
        }

        public static bool PrintArticleRequest(PrintOrderTicketDto orderTicketDto, 
            string terminalDesignation,
            string userName,
            CompanyInformationsDto companyInformationsDto)
        {
            bool result;

            List<PrinterDto> articlesPrinters = new List<PrinterDto>();

            foreach (var orderDetailDto in orderTicketDto.OrderDetails)
            {
                if (
                    orderDetailDto.ArticlePrinter != null &&
                    orderDetailDto.ArticlePrinter.Id != Guid.Empty &&
                    orderDetailDto.ArticlePrinter.IsThermal)
                {
                    if (articlesPrinters.Any(p => p.Id == orderDetailDto.ArticlePrinter.Id))
                    {
                        continue;
                    }

                    articlesPrinters.Add(orderDetailDto.ArticlePrinter);
                }
            }

            //Print Tickets for Article Printers
            if (articlesPrinters.Count > 0)
            {
                foreach (var itemPrinter in articlesPrinters)
                {
                    OrderRequest thermalPrinterInternalDocumentOrderRequest = new OrderRequest(itemPrinter, orderTicketDto,terminalDesignation,userName,companyInformationsDto, true);
                    thermalPrinterInternalDocumentOrderRequest.Print();
                }
            }
            result = true;

            return result;
        }

        //Used for Money Movements and Open/Close Terminal/Day Sessions
        public static bool PrintCashDrawerOpenAndMoneyInOut(
            PrinterDto printer,
            string terminalDesignation,
            string pTicketTitle,
            decimal pMovementAmount,
            decimal pTotalAmountInCashDrawer,
            string pMovementDescription)
        {
            bool result = false;

            if (printer != null)
            {
                switch (GetPrinterToken(printer.Token))
                {
                    case "THERMAL_PRINTER_WINDOWS":
                    case "THERMAL_PRINTER_SOCKET":
                        CashDrawer internalDocumentCashDrawer = new CashDrawer(
                            printer,
                            pTicketTitle,
                            pTotalAmountInCashDrawer,
                            pMovementAmount,
                            terminalDesignation);
                            

                        internalDocumentCashDrawer.Print();
                        break;
                }
                result = true;
            }
            return result;
        }

        public static bool OpenDoor()
        {
            bool result = false;

            if (TerminalSettings.HasLoggedTerminal)
            {
                bool hasPermission = GeneralSettings.LoggedUserHasPermissionTo("HARDWARE_DRAWER_OPEN");
                ThermalPrinterOpenDrawerValues openDrawerValues = LoggedTerminalSettings.GetThermalPrinterOpenDrawerValues();

                PrintObject printObjectSINOCAN = new PrintObject(0);

                if (hasPermission)
                {
                    switch (TerminalSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token)
                    {
                        //Impressora SINOCAN em ambiente Windows
                        case "THERMAL_PRINTER_WINDOWS":
                            printObjectSINOCAN.OpenDoor(TerminalSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token, TerminalSettings.LoggedTerminal.ThermalPrinter.Designation, openDrawerValues);
                            break;
                        //Impressora SINOCAN em ambiente Linux
                        case "THERMAL_PRINTER_SOCKET":
                            // Deprecated
                            //int m = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueM"]);
                            //int t1 = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueT1"]);
                            //int t2 = Convert.ToInt32(LogicPOS.Settings.GeneralSettings.Settings["DoorValueT2"]);
                            // Open Drawer
                            //TK016249 - Impressoras - Diferenciação entre Tipos
                            printObjectSINOCAN.OpenDoor(TerminalSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token, TerminalSettings.LoggedTerminal.ThermalPrinter.NetworkName, openDrawerValues);
                            //Audit
                            XPOUtility.Audit("CASHDRAWER_OPEN", GeneralUtils.GetResourceByName("audit_message_cashdrawer_open"));

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
