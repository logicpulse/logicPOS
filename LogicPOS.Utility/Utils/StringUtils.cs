using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LogicPOS.Utility
{
    public static class StringUtils
    {
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
