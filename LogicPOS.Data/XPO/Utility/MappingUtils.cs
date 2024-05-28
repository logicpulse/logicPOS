using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.DTOs.Printing;

namespace LogicPOS.Data.XPO.Utility
{
    public static class MappingUtils
    {
        public static PrinterDto GetPrinterDto(sys_configurationprinters printer)
        {
            return new PrinterDto
            {
                Id = printer.Oid,
                Designation = printer.Designation,
                NetworkName = printer.NetworkName,
                Token = printer.PrinterType.Token
            };
        }
    }
}
