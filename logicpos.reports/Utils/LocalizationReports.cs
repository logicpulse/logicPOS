using logicpos.financial;
using logicpos.reports.Resources.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace logicpos.reports
{
    public class LocalizationReports
    {
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public static void Localization_Reports()
        //{
        //    if (GlobalFramework.CurrentCulture.Name != "pt-PT")
        //    {
        //        FReportsFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

        //        string[] filePaths = Directory.GetFiles(FReportsFolder, "*.frx");

        //        try
        //        {
        //            for (int i = 0; i < filePaths.Length; i++)
        //            {
        //                string fileReport = filePaths[i].ToString();

        //                XDocument doc = XDocument.Load(fileReport);
        //                IEnumerable<XElement> elements = doc.Elements("Report");

        //                foreach (XElement node in elements)
        //                {
        //                    XElement attachment;

        //                    if (fileReport != "Reports/Conta_corrente_clientes_Detalhado.frx")
        //                    {
        //                        if (node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand") != null
        //                            || node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataHeaderBand") != null)
        //                        {
        //                            if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand") != null)
        //                            {
        //                                attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataFooterBand");

        //                                foreach (XElement child in attachment.Elements("TextObject"))
        //                                {
        //                                    string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                                    if (a != null)
        //                                    {
        //                                        child.Attribute("Text").Value = a;
        //                                    }
        //                                }
        //                            }

        //                            if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataHeaderBand") != null)
        //                            {
        //                                attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("DataHeaderBand");

        //                                foreach (XElement child in attachment.Elements("TextObject"))
        //                                {
        //                                    string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                                    if (a != null)
        //                                    {
        //                                        child.Attribute("Text").Value = a;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("DataBand") != null)
        //                        {
        //                            if (node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("DataHeaderBand") != null)
        //                            {
        //                                attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("DataHeaderBand");

        //                                foreach (XElement child in attachment.Elements("TextObject"))
        //                                {
        //                                    string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                                    if (a != null)
        //                                    {
        //                                        child.Attribute("Text").Value = a;
        //                                    }
        //                                }
        //                            }

        //                            if (node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("DataBand").Element("DataHeaderBand") != null)
        //                            {
        //                                attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("DataBand").Element("DataHeaderBand");

        //                                foreach (XElement child in attachment.Elements("TextObject"))
        //                                {
        //                                    string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                                    if (a != null)
        //                                    {
        //                                        child.Attribute("Text").Value = a;
        //                                    }
        //                                }
        //                            }

        //                            if (node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("GroupFooterBand") != null)
        //                            {
        //                                attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("GroupFooterBand");

        //                                foreach (XElement child in attachment.Elements("TextObject"))
        //                                {
        //                                    string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                                    if (a != null)
        //                                    {
        //                                        child.Attribute("Text").Value = a;
        //                                    }
        //                                }
        //                            }

        //                            if (node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("TableObject") != null)
        //                            {
        //                                attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupHeaderBand").Element("TableObject").Element("TableRow").Element("TableCell");

        //                                string trueText = "Yes";
        //                                string falseText = "No";

        //                                if (trueText != null && falseText != null)
        //                                {
        //                                    attachment.Attribute("Format.TrueText").Value = trueText;
        //                                    attachment.Attribute("Format.FalseText").Value = falseText;

        //                                }

        //                            }
        //                        }
        //                    }

        //                    if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("TableObject") != null)
        //                    {
        //                        attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand").Element("TableObject").Element("TableRow").Element("TableCell");

        //                        string trueText = "Yes";
        //                        string falseText = "No";

        //                        if (trueText != null && falseText != null)
        //                        {
        //                            attachment.Attribute("Format.TrueText").Value = trueText;
        //                            attachment.Attribute("Format.FalseText").Value = falseText;

        //                        }

        //                    }

        //                    if (node.Element("ReportPage").Element("GroupHeaderBand").Element("DataFooterBand") != null)
        //                    {
        //                        attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("DataFooterBand");

        //                        foreach (XElement child in attachment.Elements("TextObject"))
        //                        {
        //                            string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                            if (a != null)
        //                            {
        //                                child.Attribute("Text").Value = a;
        //                            }
        //                        }
        //                    }

        //                    if (node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupFooterBand") != null
        //                        || node.Element("ReportPage").Element("DataBand") == null && node.Element("ReportPage").Element("GroupHeaderBand").Element("DataBand") != null)
        //                    {
        //                        if (node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupFooterBand") != null)
        //                        {
        //                            attachment = node.Element("ReportPage").Element("GroupHeaderBand").Element("GroupFooterBand");

        //                            foreach (XElement child in attachment.Elements("TextObject"))
        //                            {
        //                                string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                                if (a != null)
        //                                {
        //                                    child.Attribute("Text").Value = a;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    if (node.Element("ReportPage").Element("ReportTitleBand") != null)
        //                    {
        //                        attachment = node.Element("ReportPage").Element("ReportTitleBand");

        //                        foreach (XElement child in attachment.Elements("TextObject"))
        //                        {
        //                            string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                            if (a != null)
        //                            {
        //                                child.Attribute("Text").Value = a;
        //                            }
        //                        }
        //                    }

        //                    if (node.Element("ReportPage").Element("DataBand") != null && node.Element("ReportPage").Element("DataBand").Element("DataHeaderBand") != null)
        //                    {
        //                        attachment = node.Element("ReportPage").Element("DataBand").Element("DataHeaderBand");

        //                        foreach (XElement child in attachment.Elements("TextObject"))
        //                        {
        //                            string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                            if (a != null)
        //                            {
        //                                child.Attribute("Text").Value = a;
        //                            }
        //                        }
        //                    }


        //                    if (node.Element("ReportPage").Element("PageFooterBand") != null)
        //                    {
        //                        attachment = node.Element("ReportPage").Element("PageFooterBand");

        //                        foreach (XElement child in attachment.Elements("TextObject"))
        //                        {
        //                            string a = Resx.ResourceManager.GetString(child.Attribute("Name").Value);

        //                            if (a != null)
        //                            {
        //                                child.Attribute("Text").Value = a;
        //                            }
        //                        }

        //                    }

        //                    doc.Save(fileReport);
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _log.Error(String.Format("GetCurrency(e.Message): {0}", ex.Message), ex);

        //        }
        //    }
       // }
    }
}
