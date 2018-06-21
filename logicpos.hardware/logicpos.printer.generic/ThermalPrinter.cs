using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace logicpos.printer.generic
{
    public class ThermalPrinter
	{
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MemoryStream _memStream = new MemoryStream();

        private BinaryWriter _binaryStream;

        public BinaryWriter BinaryStream
        {
            get { return _binaryStream; }
            set { _binaryStream = value; }
        }
		
		/// <summary>
		/// Delay between two picture lines. (in ms)
		/// </summary>
		public int PictureLineSleepTimeMs = 40;
		/// <summary>
		/// Delay between two text lines. (in ms)
		/// </summary>
		public int WriteLineSleepTimeMs = 0;
		/// <summary>
		/// Current encoding used by the printer.
		/// </summary>
		public string Encoding { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ThermalDotNet.ThermalPrinter"/> class.
		/// </summary>
		/// <param name='serialPort'>
		/// Serial port used by printer.
		/// </param>
        public ThermalPrinter(string encoding)
		{
            UTF8Encoding utf8 = new UTF8Encoding();

            this.Encoding = encoding;
            this._binaryStream = new BinaryWriter(this._memStream, utf8);

            Reset();

            SetEncoding(this.Encoding);
		}


		/// <summary>
		/// Prints the line of text.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteLine(string text)
		{
            //martelado pelo carlos
            //SetFont(49);
            //
            WriteToBuffer(text);
            WriteByte(AsciiControlChars.PrintAndLineFeed);
		}
		
		/// <summary>
		/// Sends the text to the printer buffer. Does not print until a line feed (0x10) is sent.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteToBuffer(string text)
		{
            string textStrip = ReplaceDiacritics(text);

            _binaryStream.Write(UTF8Encoding.Default.GetBytes(textStrip));
		}

        public string ReplaceDiacritics(string source)
        {
            source = ReplaceDiacriticsChars(source);

            string sourceInFormD = source.Normalize(NormalizationForm.FormD);

            var output = new StringBuilder();
            foreach (char c in sourceInFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    output.Append(c);
            }

            return (output.ToString().Normalize(NormalizationForm.FormC));
        }

        public string ReplaceDiacriticsChars(string source)
        {
            string result = source;

            try
            {
                Dictionary<char, char> replaceChars = new Dictionary<char, char>();
                replaceChars.Add('º', 'o');
                replaceChars.Add('ª', 'a');

                foreach (var item in replaceChars)
                {
                    result = result.Replace(item.Key, item.Value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
		
		/// <summary>
		/// Prints the line of text, white on black.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteLine_Invert(string text)
		{
			//Sets inversion on
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.ReversePrintingMode);
            WriteByte(1);
			
			//Sends the text
			WriteLine(text);
			
			//Sets inversion off
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.ReversePrintingMode);
            WriteByte(0);
			
			LineFeed();
		}
		
		/// <summary>
		/// Prints the line of text, double size.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteLine_Big(string text)
		{
			const byte DoubleHeight = 1 << 4;
			const byte DoubleWidth = 1 << 5;
			const byte Bold = 1 << 3;
			
			//big on 
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(DoubleHeight + DoubleWidth + Bold);
			
			//Sends the text
			WriteLine(text);
			
			//big off
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(0);
		}
	
		/// <summary>
		/// Prints the line of text, double height.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteLine_DoubleWidth(string text)
		{
			const byte DoubleWidth = 1 << 5;
			const byte Bold = 1 << 3;
			
			//big on 
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(DoubleWidth + Bold);
			
			//Sends the text
			WriteLine(text);
			
			//big off
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(0);
		}

		/// <summary>
		/// Prints the line of text, double height bold.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteLine_DoubleWidthBold(string text)
		{
			const byte DoubleWidth = 1 << 5;
			const byte Bold = 1 << 3;
			
			//big on 
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(DoubleWidth + Bold);
			
            //bold on
            BoldOn();
			//Sends the text
			WriteLine(text);
            //bold on
            BoldOff();
			
			//big off
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(0);
		}

		/// <summary>
		/// Prints the line of text, double height.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteLine_DoubleHeight(string text)
		{
			const byte DoubleHeight = 1 << 4;
			const byte Bold = 1 << 3;
			
			//big on 
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(DoubleHeight + Bold);
			
			//Sends the text
			WriteLine(text);
			
			//big off
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(0);
		}

		/// <summary>
		/// Prints the line of text, double height.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteLine_DoubleHeightBold(string text)
		{
			const byte DoubleHeight = 1 << 4;
			const byte Bold = 1 << 3;
			
			//big on 
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(DoubleHeight + Bold);

            //bold on
            BoldOn();
			//Sends the text
			WriteLine(text);
            //bold on
            BoldOff();
			
			//big off
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(0);
		}

		/// <summary>
		/// Prints the line of text.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		/// <param name='style'>
		/// Style of the text.
		/// </param> 
		public void WriteLine(string text, PrintingStyle style)
		{
			WriteLine(text,(byte)style);
		}
		
		/// <summary>
		/// Prints the line of text.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		/// <param name='style'>
		/// Style of the text. Can be the sum of PrintingStyle enums.
		/// </param>
		public void WriteLine(string text, byte style)
		{
			byte underlineHeight = 0;
			
			if (_BitTest(style, 0))
			{
				style = _BitClear(style, 0);
				underlineHeight = 1;
			}
			
			if (_BitTest(style, 7))
			{
				style = _BitClear(style, 7);
				underlineHeight = 2;
			}
			
			if (underlineHeight != 0) {
                WriteByte(AsciiControlChars.Escape);
                WriteByte(AsciiControlChars.TurnUnderline);
                WriteByte(underlineHeight);
			}
			
			//style on
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte((byte)style);
			
			//Sends the text
			WriteLine(text);
			
			//style off
			if (underlineHeight != 0) 
            {
                WriteByte(AsciiControlChars.Escape);
                WriteByte(AsciiControlChars.TurnUnderline);
                WriteByte(0);
			}

            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte(0);
			
		}
		
		/// <summary>
		/// Prints the line of text in bold.
		/// </summary>
		/// <param name='text'>
		/// Text to print.
		/// </param>
		public void WriteLine_Bold(string text)
		{
			//bold on
			BoldOn();
			
			//Sends the text
			WriteLine(text);
			
			//bold off
			BoldOff();
			
			//mario Commented : This always print extra Blank Line
            //LineFeed();
		}
		
		/// <summary>
		/// Sets bold mode on.
		/// </summary>
		public void BoldOn()
		{
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.CharacterSpacing);
            WriteByte(1);
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.TurnEmphasized);
            WriteByte(1);
		}
		
		/// <summary>
		/// Sets bold mode off.
		/// </summary>
		public void BoldOff()
		{
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.CharacterSpacing);
            WriteByte(0);
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.TurnEmphasized);
			WriteByte(0);
		}
		
		/// <summary>
		/// Sets white on black mode on.
		/// </summary>
		public void WhiteOnBlackOn()
		{
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.ReversePrintingMode);
			WriteByte(1);
		}
		
		/// <summary>
		/// Sets white on black mode off.
		/// </summary>
		public void WhiteOnBlackOff()
		{
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.ReversePrintingMode);
			WriteByte(0);
		}
		
		/// <summary>
		/// Sets the text size.
		/// </summary>
		/// <param name='doubleWidth'>
		/// Double width
		/// </param>
		/// <param name='doubleHeight'>
		/// Double height
		/// </param>
		public void SetSize(bool doubleWidth, bool doubleHeight)
		{
			int sizeValue = (Convert.ToInt32(doubleWidth))*(0xF0) + (Convert.ToInt32(doubleHeight))*(0x0F);
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.SelectPrintMode);
			WriteByte((byte)sizeValue);
		}

        public void SetSize2(int sizeValue) //0; 16;32; 48; 64; 80; 96; 112
        {
            WriteByte(AsciiControlChars.SelectCharacterSize);
            WriteByte(AsciiControlChars.SelectPrintMode);
            WriteByte((byte)sizeValue);
        }

        public void SetFont(int font) 
        {
            WriteByte(27);
            WriteByte(77);
            WriteByte((byte)font);
        }
		
		///	<summary>
		/// Prints the contents of the buffer and feeds one line.
		/// </summary>
		public void LineFeed()
		{
            WriteByte(AsciiControlChars.PrintAndLineFeed);
		}

        ///	<summary>
        /// Prints the contents of the buffer and feeds one line.
        /// </summary>
        public void InvertText()
        {
            WriteByte(27);
            WriteByte(123);
            WriteByte(1);
        }

        /// <summary>
        /// Prints the contents of the buffer and feeds n lines.
        /// </summary>
        /// <param name='lines'>
        /// Number of lines to feed.
        /// </param>
        public void LineFeed(byte lines)
		{
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.PrintAndFeedNLines);
			WriteByte(lines);
		}
		
		/// <summary>
		/// Idents the text.
		/// </summary>
		/// <param name='columns'>
		/// Number of columns.
		/// </param>
		public void Indent(byte columns)
		{
			if (columns < 0 || columns > 31) {
				columns = 0;
			}

            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.ReversePrintingMode);
			WriteByte(columns);
		}
		
		/// <summary>
		/// Sets the line spacing.
		/// </summary>
		/// <param name='lineSpacing'>
		/// Line spacing (in dots), default value: 32 dots.
		/// </param>
		public void SetLineSpacing(byte lineSpacing)
		{
            WriteByte(AsciiControlChars.Escape);
			WriteByte(AsciiControlChars.SetLineSpacing);
			WriteByte(lineSpacing);
		}
		
		/// <summary>
		/// Aligns the text to the left.
		/// </summary>
		public void SetAlignLeft()
		{
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectJustification);
			WriteByte(0);
		}
		
		/// <summary>
		/// Centers the text.
		/// </summary>		
		public void SetAlignCenter()
		{
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectJustification);
			WriteByte(1);
		}
		
		/// <summary>
		/// Aligns the text to the right.
		/// </summary>
		public void SetAlignRight()
		{
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SelectJustification);
			WriteByte(2);
		}
		
		/// <summary>
		/// Prints a horizontal line.
		/// </summary>
		/// <param name='length'>
		/// Line length (in characters) (max 32).
		/// </param>
		public void HorizontalLine(int length)
		{
			if (length > 0) {
				if (length > 48) {
					length = 48;
				}
				
				for (int i = 0; i < length; i++) {
					_binaryStream.Write('-');
				}

                WriteByte(AsciiControlChars.PrintAndLineFeed);
			}
		}
		
		/// <summary>
		/// Resets the printer.
		/// </summary>
		public void Reset()
		{
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.InitializePrinter);	
			System.Threading.Thread.Sleep(50);
		}
		
		/// <summary>
		/// List of supported barcode types.
		/// </summary>
		public enum BarcodeType
		{
			/// <summary>
			/// UPC-A
			/// </summary>
			upc_a = 0,
			/// <summary>
			/// UPC-E
			/// </summary>
			upc_e = 1,
			/// <summary>
			/// EAN13
			/// </summary>
			ean13 = 2,
			/// <summary>
			/// EAN8
			/// </summary>
			ean8 = 3,
			/// <summary>
			/// CODE 39
			/// </summary>
			code39 = 4,
			/// <summary>
			/// I25
			/// </summary>
			i25 = 5,
			/// <summary>
			/// CODEBAR
			/// </summary>
			codebar = 6,
			/// <summary>
			/// CODE 93
			/// </summary>
			code93 = 7,
			/// <summary>
			/// CODE 128
			/// </summary>
			code128 = 8,
			/// <summary>
			/// CODE 11
			/// </summary>
			code11 = 9,
			/// <summary>
			/// MSI
			/// </summary>
			msi = 10
		}

        public enum OptionsToText
        {
            /// <summary>
            /// Nenhum
            /// </summary>
            Nenhum = 0,
            /// <summary>
            /// Data
            /// </summary>
            Data = 1,
            /// <summary>
            /// Hora
            /// </summary>
            Hora = 2,
            /// <summary>
            /// Data e Hora
            /// </summary>
            Data_Hora = 3,
            /// <summary>
            /// Nome Empresa
            /// </summary>
            Nome_Empresa = 4,
            /// <summary>
            /// Morada
            /// </summary>
            Morada = 5,
            /// <summary>
            /// Contribuinte
            /// </summary>
            Contribuinte = 6,
            /// <summary>
            /// Telefone
            /// </summary>
            Telefone = 7,
            /// <summary>
            /// Numero Documento
            /// </summary>
            Numero_Documento = 8,
            /// <summary>
            /// Nome Cliente
            /// </summary>
            Nome_Cliente = 9,
            /// <summary>
            /// Contribuinte Cliente
            /// </summary>
            Contribuinte_Cliente = 10,
            /// <summary>
            /// Nome produto
            /// </summary>
            Nome_Produto = 11,
            /// <summary>
            /// Preço Unidade
            /// </summary>
            Preco_Unidade = 12,
            /// <summary>
            /// Quantidade
            /// </summary>
            Quantidade = 13,
            /// <summary>
            /// Total Produto
            /// </summary>
            Total_Produto = 14,
            /// <summary>
            /// IVA Produto
            /// </summary>
            IVA_Produto = 15,
            /// <summary>
            /// Total Final
            /// </summary>
            Total_Final = 16,
            /// <summary>
            /// Total Sem IVA
            /// </summary>
            Total_Sem_IVA = 17,
            /// <summary>
            /// Total do IVA
            /// </summary>
            Total_do_IVA = 18,
            /// <summary>
            /// Desconto
            /// </summary>
            Desconto = 19,
            /// <summary>
            /// Tipo pagamento
            /// </summary>
            Tipo_Pagamento = 20,
            /// <summary>
            /// Taxa do IVA
            /// </summary>
            Taxa_IVA = 21,
            /// <summary>
            /// Numero Mesa
            /// </summary>
            Num_Mesa = 22,
            /// <summary>
            /// Numero Funcionário
            /// </summary>
            Num_Func = 23,
            /// <summary>
            /// Nome Funcionário
            /// </summary>
            Nome_Func = 24,
            /// <summary>
            /// Numero da abertura de caixa
            /// </summary>
//MISS in util and PreviewForm          
            Num_Abertura_Caixa = 25,
            /// <summary>
            /// Dinheiro em caixa na abertura
            /// </summary>
//MISS in util and PreviewForm          
            Dinheiro_Caixa = 26,
            /// <summary>
            /// Data de abertura da caixa
            /// </summary>
//MISS in util and PreviewForm (Usado em Tickets e Funciona)
            Data_Abertura_Caixa = 27,
            /// <summary>
            /// Total faturado no dia
            /// </summary>
//MISS in util and PreviewForm (Usado em Tickets e Funciona)
            Total_Faturado_Dia = 28,
            /// <summary>
            /// Total dinheiro em caixa
            /// </summary>
            Total_Dinheiro_Caixa = 29,
            /// <summary>
            /// Email da empresa
            /// </summary>
            Email = 30,
            /// <summary>
            /// Total da quantidade
            /// </summary>
            Total_Quantidade = 31,
            /// <summary>
            /// Título
            /// </summary>
            Titulo = 32,
            /// <summary>
            /// Data de Abertura
            /// </summary>
            Data_Abertura = 33,
            /// <summary>
            /// Data de Fecho
            /// </summary>
            Data_Fecho = 34,
            /// <summary>
            /// Total Abertura
            /// </summary>
            Total_Abertura = 35,
            /// <summary>
            /// Total Fecho
            /// </summary>
            Total_Fecho = 36,
            /// <summary>
            /// Total Entradas
            /// </summary>
            Total_Entradas = 37,
            /// <summary>
            /// Total Saidas
            /// </summary>
            Total_Saidas = 38,
            /// <summary>
            /// Terminal name
            /// </summary>
            Utilizador_Autenticado = 39,
            /// <summary>
            /// Terminal
            /// </summary>
            Terminal_Autenticado = 40,
            /// <summary>
            /// Entrada ou saida de numerario
            /// </summary>
            Numerario = 41,
            /// <summary>
            /// Texto Livre 1
            /// </summary>
            TextoLivre1 = 42,
            /// <summary>
            /// Texto Livre 2
            /// </summary>
            TextoLivre2 = 43,
            /// <summary>
            /// Texto Livre 3
            /// </summary>
            TextoLivre3 = 44,
            /// <summary>
            /// Descrição do Movimento de Caixa
            /// </summary>
            Movimento_Descricao = 45,
            /// <summary>
            /// SubTítulo
            /// </summary>
            SubTitulo = 46,
            /// <summary>
            /// Footer Line 1
            /// </summary>
            Footer_Line1 = 47,
            /// <summary>
            /// Footer Line 2
            /// </summary>
            Footer_Line2 = 48,
            /// <summary>
            /// Footer Line 3
            /// </summary>
            Footer_Line3 = 49,
            /// <summary>
            /// Footer Line 4
            /// </summary>
            Footer_Line4 = 50,
            /// <summary>
            /// Footer Line 5
            /// </summary>
            Footer_Line5 = 51,
            /// <summary>
            /// Footer Line 6
            /// </summary>
            Footer_Line6 = 52,
            /// <summary>
            /// Footer Line 7
            /// </summary>
            Footer_Line7 = 53,
            /// <summary>
            /// Footer Line 8
            /// </summary>
            Footer_Line8 = 54,
            /// <summary>
            /// Título Cópia
            /// </summary>
            Titulo_Copia = 55,
            /// <summary>
            /// Total Liquidado
            /// </summary>
            Total_Liquidado = 56,
            /// <summary>
            /// Total Divida
            /// </summary>
            Total_Divida = 57,
            /// <summary>
            /// Numero Documento
            /// </summary>
            Numero_Documento_Recibo = 58,
            /// <summary>
            /// Numero Documento
            /// </summary>
            Numero_Documento_Fiscal = 59,
            /// <summary>
            /// Total extenso
            /// </summary>
            Total_Extenso = 60,
            /// <summary>
            /// Numero Documento
            /// </summary>
            Total_Documento_Fiscal = 61
        }
        
        /// <summary>
		/// Prints the barcode data.
		/// </summary>
		/// <param name='type'>
		/// Type of barcode.
		/// </param>
		/// <param name='data'>
		/// Data to print.
		/// </param>
		public void PrintBarcode(BarcodeType type, string data)
		{
			byte[] originalBytes;
			byte[] outputBytes;
			
			if (type == BarcodeType.code93 || type == BarcodeType.code128)
			{
				originalBytes = System.Text.Encoding.UTF8.GetBytes(data);
				outputBytes = originalBytes;
			} else {
				originalBytes = System.Text.Encoding.UTF8.GetBytes(data.ToUpper());
				outputBytes = System.Text.Encoding.Convert(System.Text.Encoding.UTF8,System.Text.Encoding.UTF8,originalBytes);
			}
			
			switch (type) {
			case BarcodeType.upc_a:
				if (data.Length ==  11 || data.Length ==  12) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(0);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.upc_e:
				if (data.Length ==  11 || data.Length ==  12) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(1);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.ean13:
				if (data.Length ==  12 || data.Length ==  13) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(2);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.ean8:
				if (data.Length ==  7 || data.Length ==  8) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(3);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.code39:
				if (data.Length > 1) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(4);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.i25:
				if (data.Length > 1 || data.Length % 2 == 0) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(5);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.codebar:
				if (data.Length > 1) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(6);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.code93: //TODO: overload PrintBarcode method with a byte array parameter
				if (data.Length > 1) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(7); //TODO: use format 2 (init string : 29,107,72) (0x00 can be a value, too)
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.code128: //TODO: overload PrintBarcode method with a byte array parameter
				if (data.Length > 1) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(8); //TODO: use format 2 (init string : 29,107,73) (0x00 can be a value, too)
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.code11:
				if (data.Length > 1) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(9);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			case BarcodeType.msi:
				if (data.Length > 1) {
                    WriteByte(AsciiControlChars.GroupSeparator);
                    WriteByte(AsciiControlChars.PrintBarcode);
					WriteByte(10);
                    WriteBytes(outputBytes);
					WriteByte(0);
				}
				break;
			}
		}
		
		/// <summary>
        /// Selects Width barcode.
		/// </summary>
		/// <param name="size">2 a 6 </param>
        public void SetBarcodeWidth(int size)
		{
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.SetBarcodeWidth);
            WriteByte((byte)size);
		}

        /// <summary>
        /// Selects Height barcode.
        /// </summary>
        /// <param name="size">1 a 255</param>
        public void SetBarcodeHeight(int size)
        {
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.SetBarcodeHeight);
            WriteByte((byte)size);
        }
		
		/// <summary>
		/// Sets the barcode left space.
		/// </summary>
		/// <param name='spacingDots'>
		/// Spacing dots.
		/// </param>
        [Obsolete("Cannot find in manual")]
        public void SetBarcodeLeftSpace(byte spacingDots)
        {
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(120);
            WriteByte(spacingDots);
        }

        public void PrintImage(string bmpFilename)
        {
            var data = Util.GetBitmapData(bmpFilename);
            var dots = data.Dots;
            var width = BitConverter.GetBytes(data.Width);

            // So we have our bitmap data sitting in a bit array called "dots."
            // This is one long array of 1s (black) and 0s (white) pixels arranged
            // as if we had scanned the bitmap from top to bottom, left to right.
            // The printer wants to see these arranged in bytes stacked three high.
            // So, essentially, we need to read 24 bits for x = 0, generate those
            // bytes, and send them to the printer, then keep increasing x. If our
            // image is more than 24 dots high, we have to send a second bit image
            // command.

            // Set the line spacing to 24 dots, the height of each "stripe" of the
            // image that we're drawing.
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SetLineSpacing);
            WriteByte((byte)24);

            // OK. So, starting from x = 0, read 24 bits down and send that data
            // to the printer.
            int offset = 0;

            while (offset < data.Height)
            {
                WriteByte(AsciiControlChars.Escape);
                WriteByte(AsciiControlChars.SelectBitImageMode);                // bit-image mode
                WriteByte(AsciiControlChars.SelectPrintMode);                   // 24-dot double-density
                WriteByte((byte)width[0]);                                      // width low byte
                WriteByte((byte)width[1]);                                      // width high byte

                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;

                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;

                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }

                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        WriteByte(slice);
                    }
                }

                offset += 24;
                WriteByte(AsciiControlChars.Newline);
            }

            // Restore the line spacing to the default of 30 dots.
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SetLineSpacing);
            WriteByte((byte)30);
        }
		
		/// <summary>
		/// Sets the printing parameters.
		/// </summary>
		/// <param name='maxPrintingDots'>
		/// Max printing dots (0-255), unit: (n+1)*8 dots, default: 7 (beceause (7+1)*8 = 64 dots)
		/// </param>
		/// <param name='heatingTime'>
		/// Heating time (3-255), unit: 10µs, default: 80 (800µs)
		/// </param>
		/// <param name='heatingInterval'>
		/// Heating interval (0-255), unit: 10µs, default: 2 (20µs)
		/// </param>
        //public void SetPrintingParameters(byte maxPrintingDots, byte heatingTime, byte heatingInterval)
        //{
        //    WriteByte(AsciiControlChars.Escape);
        //    WriteByte(55);	
        //    WriteByte(maxPrintingDots);
        //    WriteByte(heatingTime);				
        //    WriteByte(heatingInterval);
        //}
		
		/// <summary>
		/// Sets the printer offine.
		/// </summary>
		public void Sleep()
		{
            WriteByte(AsciiControlChars.Escape);
			WriteByte(61);
			WriteByte(0);
		}
		
		/// <summary>
		/// Sets the printer online.
		/// </summary>		
		public void WakeUp()
		{
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.SetPeripheralDevice);
			WriteByte(1);
		}

        //[Obsolete("Cannot find in manual")]

        public void Cut(bool full)
        {
            //Try to Get CutCommand from Config
            string printerThermalCutCommand = ConfigurationManager.AppSettings["PrinterThermalCutCommand"];

            // Send Config CutCommand to Cut Overload
            Cut(full, printerThermalCutCommand);
        }

        public void Cut(bool full, string configCutCommand)
        {
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.SelectCutModeAndCutPaper);

            //Use to convert
            //http://www.binaryhexconverter.com/decimal-to-hex-converter
            //http://www.binaryhexconverter.com/hex-to-decimal-converter
            //Ex.
            //Decimal Value (28) = Hexadecimal Value (1c)

            //Default CutCommand, used if dont have values from config or sent via parameter
            byte[] cutCommand = { 66, 0 };
            //string value = ASCIIEncoding.ASCII.GetString(cut);// { 66, 0 } => "B\0"
            //byte[] cutFromString = System.Text.Encoding.ASCII.GetBytes(value);

            if (configCutCommand != string.Empty)
            {
                if(configCutCommand != null)
                {
                    // Replace default cutCommand
                    //cutCommand = System.Text.Encoding.ASCII.GetBytes(printerThermalCutCommand);
                    cutCommand = configCutCommand.Split(new[] { ',' }).Select(s => Convert.ToByte(s, 16)).ToArray();
                }
                else
                {
                    _log.Error(String.Format("Error: invalid settings for PrinterThermalCutCommand: [{0}]", configCutCommand));
                }
            };

            //WriteByte(86);
            WriteBytes(cutCommand);

            //Custom TG2460H - FULL CUT
            //current
            //WriteByte((byte)28);//0x1C
            //WriteByte((byte)192);//0xC0
            //WriteByte((byte)52);//0x34

            //Other#1 - P12-USL, TM-T70 : Decimal 29 66 n : Acores
            //WriteByte((byte)66);0x42
            //WriteByte((byte)0);0x00

            //Other#2
            //WriteByte((byte)29);//0x1D
            //WriteByte((byte)248);//0xF8
            //WriteByte((byte)33);//0x1B

            //Other#3
            //WriteByte((byte)65);//0x41
            //WriteByte((byte)16);//0x10
            //WriteByte((byte)29);//0x1D
            //WriteByte((byte)248);//0xF8

            //Other#4 - Try for TM-T70 : Decimal 27 25 52 
            //WriteByte((byte)27);
            //WriteByte((byte)25);
            //WriteByte((byte)52);
        }

        /// <summary>
        /// FeedVerticalAndCut
        /// </summary>
        /// <param name="m"> 0 to 255</param>
        public void FeedVerticalAndCut(int m)
        {
            // Feed 3 vertical motion units and cut the paper with a 1 point cut
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte(AsciiControlChars.SelectCutModeAndCutPaper);
            WriteByte((byte)66);
            WriteByte((byte)m);
        }

        /// <summary>
        /// Prints the contents of the buffer and feeds n dots.
        /// </summary>
        /// <param name='dotsToFeed'>
        /// Number of dots to feed.
        /// </param>
        public void FeedDots(byte dotsToFeed)
        {
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.PrintAndFeedPaper);
            WriteByte(dotsToFeed);
        }

        public void SetEncoding(string encoding)
        {
            switch (encoding)
            {
                case "IBM437":
                    WriteByte(AsciiControlChars.Escape);
                    WriteByte(AsciiControlChars.SelectCharacterCodeTable);
                    WriteByte(0);
                    break;
                case "PC850":
                    WriteByte(AsciiControlChars.Escape);
                    WriteByte(AsciiControlChars.SelectCharacterCodeTable);
                    WriteByte(2);
                    break;
                case "PC860":
                    WriteByte(AsciiControlChars.Escape);
                    WriteByte(AsciiControlChars.SelectCharacterCodeTable);
                    WriteByte(3);
                    break;
            }
        }

        /// <summary>
        /// Output to connector realtime pulse
        /// </summary>
        /// <param name="connector">0 or 1</param>
        /// <param name="pulse">1 to 8</param>
        public void GeneratePulseRealtime(int connector, int pulse)
        {
            WriteByte(AsciiControlChars.DLE);
            WriteByte(AsciiControlChars.GeneratePulseRealtime);
            WriteByte((byte)1);
            WriteByte((byte)connector);
        }

        /// <summary>
        /// Output to connector pulse
        /// </summary>
        /// <param name="m">0, 1, 48, 49</param>
        /// <param name="t1">0,48</param>
        /// <param name="t2">1,49</param>
        public void GeneratePulse(int m, int t1, int t2)
        {
            WriteByte(AsciiControlChars.Escape);
            WriteByte(AsciiControlChars.GeneratePulse);
            WriteByte((byte)m);
            WriteByte((byte)t1);
            WriteByte((byte)t2);
        }

        /// <summary>
        /// Enable/disable panel button
        /// </summary>
        /// <param name="set"> LSB is 0 - enable; LSB is 1 - disable</param>
        public void SetPanelButton(int set)
        {
            WriteByte(AsciiControlChars.Escape);
            WriteByte(99);
            WriteByte(53);
            WriteByte((byte)set);
        }

        /// <summary>
        /// Setup upside-down print mode
        /// </summary>
        /// <param name="set">LSB is 0 - off; LSB is 1 - on</param>
        public void SetupUpsideDownPrinting(int set)
        {
            WriteByte(AsciiControlChars.Escape);
            WriteByte(123);
            WriteByte((byte)set);
        }

        /// <summary>
        /// When printing barcode select printing position of human readable characters
        /// </summary>
        /// <param name="set">0,1,2,3</param>
        public void SelectPrintingPositionHRIBarcode(int set)
        {
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte((byte)72);
            WriteByte((byte)set);
        }

        /// <summary>
        /// select font for printing barcode human readable characters
        /// </summary>
        /// <param name="set">0,1</param>
        public void SelectFontHRIBarcode(int set)
        {
            WriteByte(AsciiControlChars.GroupSeparator);
            WriteByte((byte)102);
            WriteByte((byte)set);
        }
		
		/// <summary>
		/// Returns a printing style.
		/// </summary>
		public enum PrintingStyle
		{
			/// <summary>
			/// White on black.
			/// </summary>
			Reverse = 1 << 1,
			/// <summary>
			/// Updown characters.
			/// </summary>
			Updown = 1 << 2,
			/// <summary>
			/// Bold characters.
			/// </summary>
			Bold = 1 << 3,
			/// <summary>
			/// Double height characters.
			/// </summary>
			DoubleHeight = 1 << 4,
			/// <summary>
			/// Double width characters.
			/// </summary>
			DoubleWidth = 1 << 5,
			/// <summary>
			/// Strikes text.
			/// </summary>
			DeleteLine = 1 << 6,
			/// <summary>
			/// Thin underline.
			/// </summary>
			Underline = 1 << 0,
			/// <summary>
			/// Thick underline.
			/// </summary>
			ThickUnderline = 1 << 7
		}
		
		/// <summary>
        /// Tests the value of a given bit.
        /// </summary>
        /// <param name="valueToTest">The value to test</param>
        /// <param name="testBit">The bit number to test</param>
        /// <returns></returns>
        static private bool _BitTest(byte valueToTest, int testBit)
        {
            return ((valueToTest & (byte)(1 << testBit)) == (byte)(1 << testBit));
        }
		
		/// <summary>
        /// Return the given value with its n bit set.
        /// </summary>
        /// <param name="originalValue">The value to return</param>
        /// <param name="bit">The bit number to set</param>
        /// <returns></returns>
        static private byte _BitSet(byte originalValue, byte bit)
        {
            return originalValue |= (byte)((byte)1 << bit);
        }

        /// <summary>
        /// Return the given value with its n bit cleared.
        /// </summary>
        /// <param name="originalValue">The value to return</param>
        /// <param name="bit">The bit number to clear</param>
        /// <returns></returns>
        static private byte _BitClear(byte originalValue, int bit)
        {
            return originalValue &= (byte)(~(1 << bit));
        }

        protected void WriteByte(byte valueToWrite)
        {
            byte[] tempArray = { valueToWrite };
            _binaryStream.Write(tempArray);
        }

        private void WriteBytes(byte[] valueToWrite)
        {
            _binaryStream.Write(valueToWrite);
        }

        public byte[] getByteArray()
        {
            _binaryStream.Flush();

            return _memStream.ToArray();
        }
	}
}

