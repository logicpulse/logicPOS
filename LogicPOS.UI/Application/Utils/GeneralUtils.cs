using LogicPOS.DTOs.Common;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LogicPOS.Utility
{
    public static class GeneralUtils
    {

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

        public static bool ValidateString(string pValidate, string pRegExRule, Type pType = null)
        {
            //If Type Decimal and User uses "," replace it by "."
            if (pType == typeof(decimal)) pValidate = pValidate.Replace(',', '.');

            //Check if can Convert to type
            if (pType != null && !CanConvert(pValidate, pType))
            {
                return false;
            }
            ;

            if (pValidate != string.Empty && pRegExRule != string.Empty)
            {
                return Regex.IsMatch(pValidate, pRegExRule);
            }
            else
            {
                return false;
            }
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
                lengthLabelText = string.Format("{0}: {1}/{2}", LocalizedString.Instance["global_characters"], result.Length, pMaxLength);
            }
            var minWordLengthConsidered = 1;

            result.Words = GetNumWords(
                result.Text,
                minWordLengthConsidered);

            if (pMaxWords > 0)
            {
                if (result.Words > pMaxWords)
                {
                    result.Words = pMaxWords;
                    result.Text = GetWords(result.Text, pMaxWords);
                }
                maxWordsLabelText = string.Format("{0}: {1}/{2}", LocalizedString.Instance["global_words"], result.Words, pMaxWords);
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

        public static string GetResourceByName(string resourceName)
        {
            return LocalizedString.Instance[resourceName];
        }

        public static string PrepareCutWord(string pText)
        {
            string tmpText = pText;

            tmpText = tmpText.Replace('.', ' ');
            tmpText = tmpText.Replace(',', ' ');
            tmpText = tmpText.Replace(';', ' ');
            tmpText = tmpText.Replace(':', ' ');
            tmpText = tmpText.Replace('+', ' ');
            tmpText = tmpText.Replace('/', ' ');
            tmpText = tmpText.Replace('$', ' ');
            tmpText = tmpText.Replace('=', ' ');
            tmpText = tmpText.Replace('#', ' ');
            tmpText = tmpText.Replace('"', ' ');
            tmpText = tmpText.Replace('!', ' ');

            return (tmpText);
        }

        public static string GetWords(string text, int numWords)
        {
            string result = string.Empty;

            text = text.Replace("\n", " ");
            text = text.Replace(".", " ");
            text = text.Replace(",", " ");
            text = text.Replace(";", " ");
            text = text.Replace(":", " ");
            text = text.Replace("+", " ");
            text = text.Replace("/", " ");
            text = text.Replace("$", " ");
            text = text.Replace("=", " ");
            text = text.Replace("#", " ");
            text = text.Replace("\\", " ");
            text = text.Replace("!", " ");
            string[] res = text.Split(' ');
            for (int i = 0; i < numWords; i++)
            {
                result += res[i];
                if (i < numWords - 1) result += " ";
            }

            return (result);
        }

        public static int GetNumWords(string text, int minWordLengthConsidered)
        {
            int result = 0;

            text = PrepareCutWord(text);

            text = text.Replace("\n", " ");
            string[] res = text.Split(' ');
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] != "")
                {
                    if (res[i].Length >= minWordLengthConsidered)
                    {
                        result++;
                    }
                }
            }

            return (result);
        }

    }
}
