namespace LogicPOS.DTOs.Printing
{
    public class PrintOrderDetailDto
    {
        public string Designation { get; set; }
        public decimal Quantity { get; set; }
        public string UnitMeasure { get; set; }

        public PrintingPrinterDto ArticlePrinter { get; set; }
    }
}
