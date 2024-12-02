using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsModal
    {
        private void AddEventHandlers()
        {
            BtnCompanyBillingReport.Button.Clicked += BtnCompanyBillingReport_Clicked;
		}

        private void BtnCompanyBillingReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
			modal.Run();
			modal.Destroy();
		}
    }
}
