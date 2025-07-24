using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogicPOS.UI.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToMoneyString(this decimal valor)
        {
            string texto = valor.ToString("F2", CultureInfo.InvariantCulture);
            string[] partes = texto.Split('.');

            string parteInteira = partes[0];
            string parteDecimal = partes.Length > 1 ? partes[1] : "00";
            string parteInteiraFormatada = Regex.Replace(parteInteira, @"(\d)(?=(\d{3})+(?!\d))", "$1.");

            return $"{parteInteiraFormatada},{parteDecimal}";
        }
    }
}
