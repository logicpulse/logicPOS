using ESC_POS_USB_NET.Printer;
using LogicPOS.Api.Features.Company;
using LogicPOS.UI.Services;
using System.Drawing;

namespace LogicPOS.UI.Printing
{
    public abstract class ThermalPrinter
    {
        protected readonly Printer _printer;

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

            var bytes = System.Convert.FromBase64String(base64Logo);

            return new Bitmap(new System.IO.MemoryStream(bytes));
        }
    }
}
