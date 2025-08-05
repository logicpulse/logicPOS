using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.POS
{
    public partial class TablesModal : Modal
    {
        public TablesModal(Window parent) : base(parent,
                                                 GeneralUtils.GetResourceByName("window_title_dialog_orders"),
                                                 new Size(720, 580),
                                                 AppSettings.Paths.Images + @"Icons\Windows\icon_window_tables_retail.png")
        {
            BtnViewTables.Visible = false;
        }

      
        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnFilterFree.Clicked += BtnFilterFree_Clicked;
            BtnFilterOpen.Clicked += BtnFilterOpen_Clicked;
            BtnFilterReserved.Clicked += BtnFilterReserved_Clicked;
            BtnFilterAll.Clicked += BtnFilterAll_Clicked;
            BtnReservation.Clicked += BtnReservation_Clicked;
        }

        private void InitializeScrollerButtons()
        {
            InitializePlacesScrollersButtons();
            InitializeTablesScrollersButtons();
        }

        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Ok && response != ResponseType.Cancel)
            {
                Run();
                return;
            }

            base.OnResponse(response);
        }
    }
}
