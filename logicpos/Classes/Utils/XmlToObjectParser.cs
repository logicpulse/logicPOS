using logicpos.App;
using logicpos.Classes.Enums.Xml;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

//ThirdParty Lib - Adapted to LogicPos
//Class Based on Third Party XmlToObjectParser
//https://github.com/tucaz/XmlToObjectParser/wiki/Getting-Started
//With minor Custom Modifications for LogicPos Theme XML, and Other Improvements like GetNodeValue() and ReplaceRegEx()

namespace logicpos
{
    public class XmlToObjectParser
    {
        public static dynamic ParseFromFile(string filename)
        {
            return ParseFromXml(File.ReadAllText(filename));
        }

        public static dynamic ParseFromXml(string xml)
        {
            IDictionary<string, object> parsedObject = new ExpandoObject();

            var rootElement = XElement.Parse(xml);

            var root = CreateChildElement(rootElement);

            parsedObject.Add(rootElement.Name.LocalName, root);

            return parsedObject;
        }

        private static dynamic CreateChildElement(XElement parent)
        {
            if (parent.Attributes().Count() == 0 && parent.Elements().Count() == 0)
                return null;

            IDictionary<string, object> child = new ExpandoObject();

            parent.Attributes().ToList().ForEach(attr =>
            {
                child.Add(attr.Name.LocalName, attr.Value);

                if (!child.ContainsKey("NodeName"))
                    child.Add("NodeName", attr.Parent.Name.LocalName);
            });

            parent.Elements().ToList().ForEach(childElement =>
            {
                var grandChild = CreateChildElement(childElement);

                if (grandChild != null)
                {
                    string nodeName = grandChild.NodeName;
                    if (child.ContainsKey(nodeName) && child[nodeName].GetType() != typeof(List<dynamic>))
                    {
                        var firstValue = child[nodeName];
                        child[nodeName] = new List<dynamic>();
                        ((dynamic)child[nodeName]).Add(firstValue);
                        ((dynamic)child[nodeName]).Add(grandChild);
                    }
                    else if (child.ContainsKey(nodeName) && child[nodeName].GetType() == typeof(List<dynamic>))
                    {
                        ((dynamic)child[nodeName]).Add(grandChild);
                    }
                    else
                    {
                        child.Add(childElement.Name.LocalName, CreateChildElement(childElement));
                        if (!child.ContainsKey("NodeName"))
                            child.Add("NodeName", parent.Name.LocalName);
                    }
                }
                else
                {
                    if (child.ContainsKey(childElement.Name.LocalName))
                    {
                        var firstValue = child[childElement.Name.LocalName];
                        child[childElement.Name.LocalName] = new List<dynamic>();
                        ((List<dynamic>)child[childElement.Name.LocalName]).Add(firstValue);
                        ((List<dynamic>)child[childElement.Name.LocalName]).Add(childElement.Value);
                    }
                    else
                    {
                        child.Add(childElement.Name.LocalName, GetNodeValue(childElement.Value));
                    }

                    if (!child.ContainsKey("NodeName"))
                        child.Add("NodeName", parent.Name.LocalName);
                }
            });

            return child;
        }

        //Convert Node String Value into Final String Value, replace Config and Resource vars etc
        private static string GetNodeValue(string pInput)
        {
            string result = pInput;

            foreach (ReplaceType type in Enum.GetValues(typeof(ReplaceType)))
            {
                result = ReplaceRegEx(result, type);
            }

            return result;
        }

        /// <summary>
        /// Replace Prefix[name] strings with values from Resources, Configs etc
        /// </summary>
        private static string ReplaceRegEx(string pInput, ReplaceType pReplaceType)
        {
            string patternBase = @"(?<={0}\[)(.*?)(?=\])";
            string patternPrefix = string.Empty;
            string pattern = string.Empty;
            string result = pInput;
            string nodeValue = string.Empty;
            Func<string, string> funcGetValue = null;

            switch (pReplaceType)
            {
                case ReplaceType.Config:
                    patternPrefix = "Cfg";
                    funcGetValue = (x) => GlobalFramework.Settings[x];
                    break;
                case ReplaceType.Resource:
                    patternPrefix = "Resx";
                    funcGetValue = (x) => Resx.ResourceManager.GetString(x);
                    break;
                default:
                    break;
            }

            pattern = string.Format(patternBase, patternPrefix);
            MatchCollection matchCollection = Regex.Matches(pInput, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            foreach (Match match in matchCollection)
            {
                nodeValue = funcGetValue(match.Value);
                if (nodeValue != string.Empty) result = result.Replace(string.Format("{0}[{1}]", patternPrefix, match), nodeValue);
                //if (_debug) _log.Debug(string.Format("item[{0}]: [{1}]=[{2}] result={3}", Enum.GetName(typeof(ReplaceType), pReplaceType), match, nodeValue, result));
            }

            return result;
        }
    }
}
