using LogicPOS.Globalization;
using System.Collections.Generic;

namespace LogicPOS.Utility
{
    public static class PrintingUtils
    {
        public static string[] GetDocumentsCopiesNamesByNumbers(List<int> copiesNumbers)
        {
            string[] copyNames = new string[copiesNumbers.Count];


            for (int i = 0; i < copiesNumbers.Count; i++)
            {
                copyNames[i] = LocalizedString.Instance[$"global_print_copy_title{copiesNumbers[i] + 1}"];


            }
            return copyNames;
        }
    }
}
