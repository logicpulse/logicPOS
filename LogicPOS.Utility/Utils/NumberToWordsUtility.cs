using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace LogicPOS.Utility
{
    public class NumberToWordsUtility
    {
        /// <summary>
        /// Função para escrever por extenso os valores em Real (em C# - suporta até R$ 9.999.999.999,99)     
        /// Rotina Criada para ler um número e transformá-lo em extenso                                       
        /// Limite máximo de 9 Bilhões (9.999.999.999,99).
        /// Não aceita números negativos. 
        /// LinhadeCodigo.com.br Autor: José F. Mar / Milton P. Jr
        /// https://social.msdn.microsoft.com/Forums/pt-BR/a5ce3477-babd-4c7c-8e1b-e64d4d273b61/valor-por-extenso?forum=504
        /// Corrigido por Adriano Santos - 2007
        /// </summary> 
        /// <param name="pValue">Valor para converter em extenso. Limite máximo de 9 Bilhões (9.999.999.999,99).</param>
        /// <returns>String do valor por Extenso</returns> 
        /// Test With:
        /// ExtendValue extendValue = new ExtendValue();
        /// string extended = extendValue.Extenso_Valor(1772737377.33m);
        public string GetExtendedValue(decimal pValue, string pAcronym)
        {
            //string moedaSingular = "Euro";//Real
            //string moedaPlural = "Euros";//Reais
            string moedaSingular = pAcronym;//Real

            /* IN009185 */
            string moedaPlural = ApplyPluralizationForCurrency(pAcronym);

            /* IN009018 */

            string moedaDecimalSingular = CultureResources.GetResourceByLanguage("", "numbers_to_words_cent");//Centavo
            string moedaDecimalPlural = CultureResources.GetResourceByLanguage("", "numbers_to_words_cents");//Centavos
            if (GeneralSettings.Settings["cultureFinancialRules"] == "pt-MZ" || GeneralSettings.Settings["cultureFinancialRules"] == "pt-AO")
            {
                moedaDecimalSingular = CultureResources.GetResourceByLanguage(GeneralSettings.Settings["cultureFinancialRules"], "numbers_to_words_cent");//Centavo
                moedaDecimalPlural = CultureResources.GetResourceByLanguage(GeneralSettings.Settings["cultureFinancialRules"], "numbers_to_words_cents");//Centavos
            }

            string strValorExtenso = ""; //Variável que irá armazenar o valor por extenso do número informado

            decimal dblCentavos = 0;
            int intContador = 0;
            bool bln_Bilhao = false;
            bool bln_Milhao = false;
            bool bln_Mil = false;
            bool bln_Unidade = false;

            //Verificar se foi informado um dado indevido 
            //Fix
            //if (pValue == 0 || pValue <= 0)
            if (pValue == 0 || pValue <= 0.001M)
            {
                //throw new Exception("Valor não suportado pela Função. Verificar se há valor negativo ou nada foi informado");
                //return "Zero Euros";
                return string.Format("Zero {0}", moedaPlural);
            }
            if (pValue > (decimal)9999999999.99)
            {
                throw new Exception("Valor não suportado pela Função. Verificar se o Valor está acima de 9999999999.99");
            }
            else //Entrada padrão do método
            {
                //Gerar Extenso Centavos 
                pValue = (decimal.Round(pValue, 2));


                // Fix Force .00 2 zeros else errors arrise
                pValue = decimal.Parse(pValue.ToString("F"));
                dblCentavos = pValue - (long)pValue;

                //Gerar Extenso parte Inteira
                decimal dblValorInteiro = (long)pValue;
                string strNumero;
                if (dblValorInteiro > 0)
                {
                    if (dblValorInteiro > 999)
                    {
                        bln_Mil = true;
                    }
                    if (dblValorInteiro > 999999)
                    {
                        bln_Milhao = true;
                        bln_Mil = false;
                    }
                    if (dblValorInteiro > 999999999)
                    {
                        bln_Mil = false;
                        bln_Milhao = false;
                        bln_Bilhao = true;
                    }

                    for (int i = (dblValorInteiro.ToString().Trim().Length) - 1; i >= 0; i--)
                    {
                        // strNumero = Mid(dblValorInteiro.ToString().Trim(), (dblValorInteiro.ToString().Trim().Length - i) + 1, 1);
                        strNumero = Mid(dblValorInteiro.ToString().Trim(), (dblValorInteiro.ToString().Trim().Length - i) - 1, 1);
                        switch (i)
                        {            /*******/
                            case 9:  /*Bilhão*
                                 /*******/
                                {
                                    strValorExtenso = fcn_Numero_Unidade(strNumero) + ((int.Parse(strNumero) > 1) ? " Bilhões e" : " Bilhão e");
                                    bln_Bilhao = true;
                                    break;
                                }
                            case 8: /********/
                            case 5: //Centena*
                            case 2: /********/
                                {
                                    if (int.Parse(strNumero) > 0)
                                    {
                                        string strCentena = Mid(dblValorInteiro.ToString().Trim(), (dblValorInteiro.ToString().Trim().Length - i) - 1, 3);

                                        if (int.Parse(strCentena) > 100 && int.Parse(strCentena) < 200)
                                        {
                                            strValorExtenso = strValorExtenso + " Cento e ";
                                        }
                                        else
                                        {
                                            strValorExtenso = strValorExtenso + " " + fcn_Numero_Centena(strNumero);
                                        }
                                        if (intContador == 8)
                                        {
                                            bln_Milhao = true;
                                        }
                                        else if (intContador == 5)
                                        {
                                            bln_Mil = true;
                                        }
                                    }
                                    break;
                                }
                            case 7: /*****************/
                            case 4: //Dezena de Milhão*
                            case 1: /*****************/
                                {
                                    if (int.Parse(strNumero) > 0)
                                    {
                                        string strDezena = Mid(dblValorInteiro.ToString().Trim(), (dblValorInteiro.ToString().Trim().Length - i) - 1, 2);

                                        if (int.Parse(strDezena) > 10 && int.Parse(strDezena) < 20)
                                        {
                                            strValorExtenso = strValorExtenso + (Right(strValorExtenso, 5).Trim() == "entos" ? " e " : " ")
                                            + fcn_Numero_Dezena0(Right(strDezena, 1));//corrigido

                                            bln_Unidade = true;
                                        }
                                        else
                                        {
                                            strValorExtenso = strValorExtenso + (Right(strValorExtenso, 5).Trim() == "entos" ? " e " : " ")
                                            + fcn_Numero_Dezena1(Left(strDezena, 1));//corrigido 

                                            bln_Unidade = false;
                                        }
                                        if (intContador == 7)
                                        {
                                            bln_Milhao = true;
                                        }
                                        else if (intContador == 4)
                                        {
                                            bln_Mil = true;
                                        }
                                    }
                                    break;
                                }
                            case 6: /******************/
                            case 3: //Unidade de Milhão* 
                            case 0: /******************/
                                {
                                    if (int.Parse(strNumero) > 0 && !bln_Unidade)
                                    {
                                        if ((Right(strValorExtenso, 5).Trim()) == "entos"
                                        || (Right(strValorExtenso, 3).Trim()) == "nte"
                                        || (Right(strValorExtenso, 3).Trim()) == "nta")
                                        {
                                            strValorExtenso = strValorExtenso + " e ";
                                        }
                                        else
                                        {
                                            strValorExtenso = strValorExtenso + " ";
                                        }
                                        strValorExtenso = strValorExtenso + fcn_Numero_Unidade(strNumero);
                                    }
                                    if (i == 6)
                                    {
                                        if (bln_Milhao || int.Parse(strNumero) > 0)
                                        {
                                            strValorExtenso = strValorExtenso + ((int.Parse(strNumero) == 1) && !bln_Unidade ? " Milhão" : " Milhões");
                                            strValorExtenso = strValorExtenso + ((int.Parse(strNumero) > 1000000) ? " " : " e");
                                            bln_Milhao = true;
                                        }
                                    }
                                    if (i == 3)
                                    {
                                        if (bln_Mil || int.Parse(strNumero) > 0)
                                        {
                                            strValorExtenso = strValorExtenso + " Mil";
                                            strValorExtenso = strValorExtenso + ((int.Parse(strNumero) > 1000) ? " " : " e");
                                            bln_Mil = true;
                                        }
                                    }
                                    if (i == 0)
                                    {
                                        if ((bln_Bilhao && !bln_Milhao && !bln_Mil
                                        && Right((dblValorInteiro.ToString().Trim()), 3) == "0")
                                        || (!bln_Bilhao && bln_Milhao && !bln_Mil
                                        && Right((dblValorInteiro.ToString().Trim()), 3) == "0"))
                                        {
                                            strValorExtenso = strValorExtenso + " e ";
                                        }
                                        strValorExtenso = strValorExtenso + ((long.Parse(dblValorInteiro.ToString())) > 1 ? " " + moedaPlural : " " + moedaSingular);
                                    }
                                    bln_Unidade = false;
                                    break;
                                }
                        }
                    }
                }
                if (dblCentavos > 0)
                {

                    if (dblCentavos > 0 && dblCentavos < 0.1M)
                    {
                        strNumero = Right((decimal.Round(dblCentavos, 2)).ToString().Trim(), 1);
                        strValorExtenso = strValorExtenso + ((dblCentavos > 0) ? " e " : " ")
                        + fcn_Numero_Unidade(strNumero) + ((dblCentavos > 0.01M) ? " " + moedaDecimalPlural : " " + moedaDecimalSingular);
                    }
                    else if (dblCentavos > 0.1M && dblCentavos < 0.2M)
                    {
                        strNumero = Right(((decimal.Round(dblCentavos, 2) - (decimal)0.1).ToString().Trim()), 1);
                        /* IN009185 */
                        strValorExtenso = strValorExtenso + ((dblCentavos > 0) ? " e " : " ")
                        + fcn_Numero_Dezena0(strNumero) + " " + moedaDecimalPlural + " ";
                    }
                    else
                    {
                        strNumero = Right(dblCentavos.ToString().Trim(), 2);
                        string strDezCentavo = Mid(dblCentavos.ToString().Trim(), 2, 1);

                        //FIX
                        //strValorExtenso = strValorExtenso + ((int.Parse(strNumero) > 0) ? " e " : " ");
                        strValorExtenso = strValorExtenso + ((Convert.ToInt16(Convert.ToDouble(strNumero)) > 0) ? " e " : " ");
                        strValorExtenso = strValorExtenso + fcn_Numero_Dezena1(Left(strDezCentavo, 1));

                        if ((dblCentavos.ToString().Trim().Length) > 2)
                        {
                            strNumero = Right((decimal.Round(dblCentavos, 2)).ToString().Trim(), 1);
                            if (int.Parse(strNumero) > 0)
                            {
                                if (dblValorInteiro <= 0)
                                {
                                    if (Mid(strValorExtenso.Trim(), strValorExtenso.Trim().Length - 2, 1) == "e")
                                    {
                                        strValorExtenso = strValorExtenso + " e " + fcn_Numero_Unidade(strNumero);
                                    }
                                    else
                                    {
                                        strValorExtenso = strValorExtenso + " e " + fcn_Numero_Unidade(strNumero);
                                    }
                                }
                                else
                                {
                                    strValorExtenso = strValorExtenso + " e " + fcn_Numero_Unidade(strNumero);
                                }
                            }
                        }
                        //FIX
                        //strValorExtenso = strValorExtenso + " Centavos ";
                        strValorExtenso = strValorExtenso + " " + moedaDecimalPlural + " ";
                    }
                }
                //Fix
                //if (dblValorInteiro < 1) strValorExtenso = Mid(strValorExtenso.Trim(), 2, strValorExtenso.Trim().Length - 2);
                if (dblValorInteiro < 1) strValorExtenso = strValorExtenso.Trim();
                if (strValorExtenso.StartsWith("e ")) strValorExtenso = strValorExtenso.Substring(2, strValorExtenso.Length - 2);
            }

            //Fix to Replace extra space (more than one) in one space only
            strValorExtenso = Regex.Replace(strValorExtenso.Trim(), @"\s+", " ");

            return strValorExtenso.Trim();
        }

        /// <summary>
        /// Generates the plural form of currency.
        /// Now implemented for the ones that are not variables and the variables with 'L' at the end.
		/// Please see #IN009185 for more details.
        /// </summary>
        /// <param name="pCurrency"></param>
        /// <returns></returns>
        private static string ApplyPluralizationForCurrency(string pCurrency)
        {
            string currency = pCurrency;

            if (!string.IsNullOrEmpty(currency))
            {
                /* #TODO: Improve this block of code */
                if (currency.ToUpper().EndsWith("L")) /* Real, Metical, ... */
                {
                    int place = currency.LastIndexOf("l");

                    if (place != -1)
                    {
                        currency = currency.Remove(place, "l".Length).Insert(place, "is");
                    }
                }
                else
                {
                    currency = string.Format("{0}s", currency);/* Euros, ... */
                }
            }
            return currency;
        }

        private string fcn_Numero_Dezena0(string pstrDezena0)
        {
            //Vetor que irá conter o número por extenso 
            ArrayList array_Dezena0 = new ArrayList
            {
                "Onze",
                "Doze",
                "Treze",
                "Quatorze",
                "Quinze",
                "Dezesseis",
                "Dezessete",
                "Dezoito",
                "Dezenove"
            };

            return array_Dezena0[((int.Parse(pstrDezena0)) - 1)].ToString();
        }
        private string fcn_Numero_Dezena1(string pstrDezena1)
        {
            //Vetor que irá conter o número por extenso
            ArrayList array_Dezena1 = new ArrayList
            {
                "Dez",
                "Vinte",
                "Trinta",
                "Quarenta",
                "Cinquenta",
                "Sessenta",
                "Setenta",
                "Oitenta",
                "Noventa"
            };

            return array_Dezena1[short.Parse(pstrDezena1) - 1].ToString();
        }

        private string fcn_Numero_Centena(string pstrCentena)
        {
            //Vetor que irá conter o número por extenso
            ArrayList array_Centena = new ArrayList
            {
                "Cem",
                "Duzentos",
                "Trezentos",
                "Quatrocentos",
                "Quinhentos",
                "Seiscentos",
                "Setecentos",
                "Oitocentos",
                "Novecentos"
            };

            return array_Centena[((int.Parse(pstrCentena)) - 1)].ToString();
        }
        private string fcn_Numero_Unidade(string pstrUnidade)
        {
            //Vetor que irá conter o número por extenso
            ArrayList array_Unidade = new ArrayList
            {
                "Um",
                "Dois",
                "Três",
                "Quatro",
                "Cinco",
                "Seis",
                "Sete",
                "Oito",
                "Nove"
            };

            return array_Unidade[(int.Parse(pstrUnidade) - 1)].ToString();
        }

        //Começa aqui os Métodos de Compatibilazação com VB 6 .........Left() Right() Mid()
        public static string Left(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the 
            //left and with the specified lenght and assign it to a variable
            if (param == "")
                return "";
            string result = param.Substring(0, length);
            //return the result of the operation 
            return result;
        }
        public static string Right(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable 
            if (param == "")
                return "";
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable 
            string result = param.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            string result = param.Substring(startIndex);
            //return the result of the operation 
            return result;
        }
        ////Acaba aqui os Métodos de Compatibilazação com VB 6 .........
    }
}
