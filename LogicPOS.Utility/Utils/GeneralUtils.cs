using System;
using System.IO;

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

    }
}
