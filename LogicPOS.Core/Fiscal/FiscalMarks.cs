namespace LogicPOS.Core.Fiscal;

public static class FiscalMarks
{
    public static FiscalPrint Read(IFiscalModule? module, FiscalDocument document)
    {
        if (module is not { IsAvailable: true })
        {
            return new FiscalPrint();
        }

        return module.DescribePrint(document);
    }
}
