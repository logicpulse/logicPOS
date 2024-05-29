namespace LogicPOS.DTOs.Printing
{
    public struct ThermalPrinterOpenDrawerValues
    {
       public int M { get; set; }
       public int T1 { get; set; }
       public int T2 { get; set; }

       public bool IsDefault => M == 0 && T1 == 0 && T2 == 0;
    }
}
