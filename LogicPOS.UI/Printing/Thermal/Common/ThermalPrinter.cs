using ESC_POS_USB_NET.Printer;
using LogicPOS.Api.Features.Company;
using LogicPOS.UI.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogicPOS.UI.Printing
{
    public abstract class ThermalPrinter
    {
        protected readonly Printer _printer;
        protected ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public ThermalPrinter(Printer printer)
        {
            _printer = printer;
        }

        public abstract void Print();

        protected CompanyInformations GetCompanyInformations()
        {
            return PreferenceParametersService.CompanyInformations;
        }

        protected Bitmap GetCompanyLogo()
        {
            var base64Logo = GetCompanyInformations().LogoBmp;

            if (string.IsNullOrEmpty(base64Logo))
            {
                return null;
            }
            if (IsBase64String(base64Logo))
            {
                var bytes = System.Convert.FromBase64String(base64Logo);
                return new Bitmap(new System.IO.MemoryStream(bytes));
            }
            else
            {
                return new Bitmap(base64Logo);
            }
        }

        public string DecimalToMoneyString(decimal valor)
        {
            string texto = valor.ToString("F2", CultureInfo.InvariantCulture);
            string[] partes = texto.Split('.');

            string parteInteira = partes[0];
            string parteDecimal = partes.Length > 1 ? partes[1] : "00";
            string parteInteiraFormatada = Regex.Replace(parteInteira, @"(\d)(?=(\d{3})+(?!\d))", "$1.");

            return $"{parteInteiraFormatada},{parteDecimal}";
        }

        public bool IsBase64String(string base64)
        {

            base64 = base64.Trim();
            return (base64.Length % 4 == 0) &&
                   !base64.Contains(" ") &&
                   base64.All(c => char.IsLetterOrDigit(c) ||
                                    c == '+' ||
                                    c == '/' ||
                                    c == '=');
        }

        protected void PrintHeader()
        {
            var companyInformations = GetCompanyInformations();
            var logo = GetCompanyLogo();
            
            _printer.AlignCenter();
            _printer.DoubleWidth2();
            if (logo != null)
            {
                _printer.Image(logo);
            }
            else
            {
                if (string.IsNullOrEmpty(companyInformations.BusinessName))
                {
                    companyInformations.BusinessName = companyInformations.Name;
                }
                _printer.Append(companyInformations.BusinessName);

            }
            _printer.NormalWidth();
        }
    }
}
