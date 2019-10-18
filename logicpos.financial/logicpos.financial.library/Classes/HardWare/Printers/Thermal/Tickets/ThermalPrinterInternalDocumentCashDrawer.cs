using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.resources.Resources.Localization;
using System;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public class ThermalPrinterInternalDocumentCashDrawer : ThermalPrinterBaseInternalTemplate
    {
        private decimal _totalAmountInCashDrawer = 0.0m;
        private decimal _movementAmount = 0.0m;
        private string _movementDescription = string.Empty;
        
        public ThermalPrinterInternalDocumentCashDrawer(sys_configurationprinters pPrinter, string pTicketTitle, decimal pTotalAmountInCashDrawer)
            : this(pPrinter, pTicketTitle, pTotalAmountInCashDrawer, 0.0m) { }

        public ThermalPrinterInternalDocumentCashDrawer(sys_configurationprinters pPrinter, string pTicketTitle, decimal pTotalAmountInCashDrawer, decimal pMovementAmount)
            : this(pPrinter, pTicketTitle, pTotalAmountInCashDrawer, pMovementAmount, string.Empty) { }

        public ThermalPrinterInternalDocumentCashDrawer(sys_configurationprinters pPrinter, string pTicketTitle, decimal pTotalAmountInCashDrawer, decimal pMovementAmount, string pMovementDescription)
            : base(pPrinter)
        {
            _ticketTitle = pTicketTitle;
            _totalAmountInCashDrawer = pTotalAmountInCashDrawer;
            _movementAmount = pMovementAmount;
            _movementDescription = pMovementDescription;
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call Base Template PrintHeader
                base.PrintTitles();
                
                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                PrintDocumentDetails();

                //Reset to Left
                _thermalPrinterGeneric.SetAlignLeft();

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintDocumentDetails()
        {
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_cashdrawer"));
            _thermalPrinterGeneric.WriteLine(FrameworkUtils.DecimalToString(_totalAmountInCashDrawer), WriteLineTextMode.Big);
            _thermalPrinterGeneric.LineFeed();

            if (_movementAmount < 0.0m || _movementAmount > 0.0m)  {
                _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_movement_amount"));
                _thermalPrinterGeneric.WriteLine(FrameworkUtils.DecimalToString(_movementAmount), WriteLineTextMode.Big);
                _thermalPrinterGeneric.LineFeed();
            }

            string description = (_movementDescription != string.Empty) ? _movementDescription : "________________________________";
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_description"));
            _thermalPrinterGeneric.WriteLine(description);
        }
    }
}
