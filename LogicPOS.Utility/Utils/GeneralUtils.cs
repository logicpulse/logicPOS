using LogicPOS.DTOs.Common;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LogicPOS.Utility
{
    public static class GeneralUtils
    {
        public static bool IsNullable(Type pType)
        {
            return (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static bool CreateDirectory(string pPath)
        {
            if (Directory.Exists(pPath))
            {
                return true;
            }

            Directory.CreateDirectory(pPath);

            return true;
        }

        public static bool IsRunningOnMono => Type.GetType("Mono.Runtime") != null;

        public static void ExecuteExternalProcess(string processFileName)
        {
            var process = new Process();
            process.StartInfo.FileName = processFileName;
            process.Start();
        }

        public static bool CanConvert(string pValue, Type pType)
        {
            if (string.IsNullOrEmpty(pValue) || pType == null) return false;
            System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(pType);
            if (typeConverter.CanConvertFrom(typeof(string)))
            {
                typeConverter.ConvertFrom(null, CultureSettings.CurrentCultureNumberFormat, pValue);
                return true;
            }
            return false;
        }

        public static object GetFieldValueFromType(Type pType, string pFieldName)
        {
            object result = null;

            //pFieldName = "DatabaseName";//Works with Current Settings
            //pFieldName = "AppCompanyName";//Works with Base Class
            //object o = pType.BaseType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy);

            //Trick to get current and base class Fields
            FieldInfo fieldInfo = pType.GetField(pFieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            //Note: use first parameter has null, static classes cannot be instanced, else use object
            if (fieldInfo != null) result = fieldInfo.GetValue(null);

            return result;
        }

        public static bool ValidateString(string pValidate, string pRegExRule, Type pType = null)
        {
            //If Type Decimal and User uses "," replace it by "."
            if (pType == typeof(decimal)) pValidate = pValidate.Replace(',', '.');

            //Check if can Convert to type
            if (pType != null && !CanConvert(pValidate, pType))
            {
                return false;
            };

            //Check if is a Blank GUID 
            if (pValidate == new Guid().ToString() && pRegExRule == RegexUtils.RegexGuid)
            {
                return false;
            }

            if (pValidate != string.Empty && pRegExRule != string.Empty)
            {
                return Regex.IsMatch(pValidate, pRegExRule);
            }
            else
            {
                return false;
            }
        }

        public static List<T> MergeGenericLists<T>(List<List<T>> pLists)
        {
            List<T> result = new List<T>();

            for (int i = 0; i < pLists.Count; i++)
            {
                result = result.Concat(pLists[i]).ToList();
            }

            return result.Distinct().ToList();
        }

        public static Dictionary<string, string> CSVFileToDictionary(string pFilePath)
        {
            return CSVFileToDictionary(pFilePath, ',');
        }

        public static Dictionary<string, string> CSVFileToDictionary(string pFilePath, char pSplitter)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (File.Exists(pFilePath))
            {
                StreamReader streamReader = new StreamReader(File.OpenRead(pFilePath));


                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var values = line.Split(pSplitter);

                    if (values.Length == 2 && !string.IsNullOrEmpty(values[0]) && !string.IsNullOrEmpty(values[1]))
                    {
                        string val1 = values[0];
                        string val2 = values[1];

                        if (!result.ContainsKey(val1))
                        {
                            result.Add(val1, val2);
                        }
                    }
                }
                streamReader.Close();
            }

            return result;
        }

        public static bool Validate(string pValidateValue, string pRule, bool pRequired)
        {
            bool result = false;


            if (pRule == null || pRule == string.Empty)
            {
                if (pRequired)
                {
                    if (pValidateValue == string.Empty)
                    {
                        result = false;
                    }
                    else if (pValidateValue != string.Empty)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }
            else
            {
                if (pRequired)
                {
                    if (pValidateValue != string.Empty && ValidateString(pValidateValue, pRule))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    if (pValidateValue == string.Empty || pValidateValue == Convert.ToString(Guid.Empty))
                    {
                        result = true;
                    }
                    else
                        if (pValidateValue != string.Empty && ValidateString(pValidateValue, pRule))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }


            return result;
        }

        public static ValidateMaxLenghtMaxWordsResult ValidateMaxLenghtMaxWords(string pValue, string pIinitialLabelText, int pMaxLength, int pMaxWords)
        {
            ValidateMaxLenghtMaxWordsResult result = new ValidateMaxLenghtMaxWordsResult();

            result.Text = pValue;
            result.Length = pValue.Length;

            string lengthLabelText = string.Empty;
            string maxWordsLabelText = string.Empty;

            if (pMaxLength > 0)
            {
                if (result.Length > pMaxLength)
                {
                    result.Length = pMaxLength;
                    //Cut Text
                    result.Text = pValue.Substring(0, pMaxLength);
                }
                lengthLabelText = string.Format("{0}: {1}/{2}", CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_characters"), result.Length, pMaxLength);
            }
            var minWordLengthConsidered = Convert.ToInt16(GeneralSettings.Settings["MinWordLengthConsidered"]);

            result.Words = StringUtils.GetNumWords(
                result.Text,
                minWordLengthConsidered);

            if (pMaxWords > 0)
            {
                if (result.Words > pMaxWords)
                {
                    result.Words = pMaxWords;
                    result.Text = StringUtils.GetWords(result.Text, pMaxWords);
                }
                maxWordsLabelText = string.Format("{0}: {1}/{2}", CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_words"), result.Words, pMaxWords);
            }

            if (result.Length > 0)
            {
                string addToLabelText = string.Empty;
                if (lengthLabelText != string.Empty) addToLabelText += lengthLabelText;
                if (lengthLabelText != string.Empty && maxWordsLabelText != string.Empty) addToLabelText += " ";
                if (maxWordsLabelText != string.Empty) addToLabelText += maxWordsLabelText;

                if (addToLabelText != string.Empty)
                {
                    result.LabelText = string.Format("{0} : ({1})", pIinitialLabelText, addToLabelText);
                }
            }
            else
            {
                result.LabelText = pIinitialLabelText;
            }

            return result;
        }

        public static List<int> CommaDelimitedStringToIntList(string pInput)
        {
            return CommaDelimitedStringToIntList(pInput, ',');
        }

        public static List<int> CommaDelimitedStringToIntList(string pInput, char pSeparator)
        {
            string[] arrayString = pInput.Split(pSeparator);
            List<int> listResult = new List<int>();

            for (int i = 0; i < arrayString.Length; i++)
            {
                int integer;
                bool res = int.TryParse(arrayString[i], out integer);

                if (res)
                {
                    listResult.Add(integer);
                }

            }
            return listResult;
        }

    }
}
