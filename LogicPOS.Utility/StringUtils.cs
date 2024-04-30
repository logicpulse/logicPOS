using System;
using System.Collections.Generic;

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
    }
}
