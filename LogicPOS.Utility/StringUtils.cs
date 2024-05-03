using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LogicPOS.Utility
{
    public static class StringUtils
    {
        public static string GenerateRandomString(int size)
        {
            Random rand = new Random();
            string stringchars = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = stringchars[rand.Next(stringchars.Length)];
            }
            return new string(chars);
        }

        public static string ReplaceTextTokens(
            string input, 
            List<Dictionary<string, string>> tokensDictionaryList)
        {
            Dictionary<string, string> tokensDictionary = new Dictionary<string, string>();


            foreach (Dictionary<string, string> itemList in tokensDictionaryList)
            {
                foreach (var item in itemList)
                {
                    tokensDictionary.Add(item.Key, item.Value);
                }
            }

            return ReplaceTextTokens(input, tokensDictionary);
        }

        public static string ReplaceTextTokens(
            string input, 
            Dictionary<string, string> tokensDictionary)
        {
            string result = input;

            if (input != null)
            {
                string replaceToken;

                foreach (var item in tokensDictionary)
                {
                    replaceToken = string.Format("${{{0}}}", item.Key);

                    if (result.Contains(replaceToken))
                    {
                        result = result.Replace(replaceToken, item.Value);
                    }
                }
            }

            return result;
        }

        public static string RemoveCarriageReturn(string pInput)
        {
            return pInput.Replace("\r\n", string.Empty);
        }

        public static string RemoveExtraWhiteSpaces(string pInput)
        {
            return Regex.Replace(pInput, @"\s+", " ");
        }

        public static string RemoveCarriageReturnAndExtraWhiteSpaces(string pInput)
        {
            string result = RemoveCarriageReturn(RemoveExtraWhiteSpaces(pInput));

            if (result.StartsWith(" "))
            {
                result = result.Substring(1, result.Length - 1);
            }
            return result;
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

        public static string GetWords(
            string text, 
            int numWords)
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

        public static int GetNumWords(
            string text,
            int minWordLengthConsidered
            )
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
