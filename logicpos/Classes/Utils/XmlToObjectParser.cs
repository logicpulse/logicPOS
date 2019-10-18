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
        public static dynamic _expressionEvaluatorReference;

        public static dynamic ParseFromFile(string filename)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            if (File.Exists(filename))
            {
                return ParseFromXml(File.ReadAllText(filename));
            }
            else
            {
                throw new Exception("Error! Missing ThemeFile: [{filename}]");
            }
        }

        public static dynamic ParseFromXml(string xml)
        {
            IDictionary<string, object> parsedObject = new ExpandoObject();
            
            // ExpressionEvaluator, add reference to dynamic Object
            _expressionEvaluatorReference = parsedObject;
            GlobalApp.ExpressionEvaluator.Variables.Add("themeRoot", (_expressionEvaluatorReference as dynamic));

            XElement rootElement = rootElement = XElement.Parse(xml);

            ExpandoObject root = CreateChildElement(rootElement);

            parsedObject.Add(rootElement.Name.LocalName, root);

            //GlobalApp.ExpressionEvaluator.Variables["themeRoot"] =  _expressionEvaluatorReference;
            
            //_log.Debug(string.Format("Message: [{0}]", _expressionEvaluatorReference.Theme.Frontoffice.Window[0].Globals.Name));
            //(GlobalApp.ExpressionEvaluator.Variables["themeRoot"] as dynamic).Theme.Frontoffice.Window[0].Globals.Name

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
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                    if (pInput.Contains(string.Format("{0}[", patternPrefix))) funcGetValue = (x) => GlobalFramework.Settings[x];
                    break;
                case ReplaceType.Resource:
                    patternPrefix = "Resx";
                    if (pInput.Contains(string.Format("{0}[", patternPrefix))) funcGetValue = (x) => resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], x);
                    break;
                case ReplaceType.Evaluation:
                    patternPrefix = "Eval";
                    if (pInput.Contains(string.Format("{0}[", patternPrefix))) funcGetValue = (x) => GetEvaluationResult(x);
                    break;
                case ReplaceType.EvaluationDebug:
                    patternPrefix = "EvalDebug";
                    if (pInput.Contains(string.Format("{0}[", patternPrefix))) funcGetValue = (x) => GetEvaluationResult(x, true);
                    break;
                default:
                    break;
            }

            pattern = string.Format(patternBase, patternPrefix);
            MatchCollection matchCollection = Regex.Matches(pInput, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            foreach (Match match in matchCollection)
            {
                try
                {
                    nodeValue = funcGetValue(match.Value);
                    if (nodeValue != string.Empty) result = result.Replace(string.Format("{0}[{1}]", patternPrefix, match), nodeValue);
                    //if (_debug) _log.Debug(string.Format("item[{0}]: [{1}]=[{2}] result={3}", Enum.GetName(typeof(ReplaceType), pReplaceType), match, nodeValue, result));
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
            }

            return result;
        }

        private static string GetEvaluationResult(string expression)
        {
            return GetEvaluationResult(expression, false);
        }

        private static string GetEvaluationResult(string expression, bool debug)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            string result = expression;

            try
            {
                result = GlobalApp.ExpressionEvaluator.Evaluate(expression).ToString();
                
                // Trigger Debugger with a BreakPoint, this is usefull to Eval Expressions
                if (debug)
                {
                    string hardCodeExpression = "globalScreenSizeHeight - ((posMainWindowTicketPadButtonSize.Height * 5) + (posMainWindowComponentsMargin * 2)) - posMainWindowEventBoxStatusBar1And2Height * 2";
                    string hardCodeResult = GlobalApp.ExpressionEvaluator.Evaluate(hardCodeExpression).ToString();
                    //log.Debug(string.Format("result: [{0}]", GlobalApp.ExpressionEvaluator.Evaluate(hardCodeExpression).ToString()));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
