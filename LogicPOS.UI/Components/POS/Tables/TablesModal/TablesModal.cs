using Gtk;
using LogicPOS.Api.Enums;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.POS
{
    public partial class TablesModal : Modal
    {
        public TablesModal(Window parent) : base(parent,
                                                 GeneralUtils.GetResourceByName("window_title_dialog_orders"),
                                                 new Size(720, 580),
                                                 PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_tables_retail.png")
        {
            BtnViewTables.Visible = false;
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
            var actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(BtnFilterAll, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnFilterFree, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnFilterOpen, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnFilterReserved, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnViewOrders, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnViewTables, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnReservation, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnCancel, ResponseType.Cancel));
            return actionAreaButtons;
        }

        protected override Widget CreateBody()
        {
            var body = new Fixed();

            MenuPlaces = new PlacesMenu(BtnScrollPlacesPrevious, BtnScrollPlacesNext,this);
            body.Put(MenuPlaces, 0, 0);

            MenuTables = new TablesMenu(BtnScrollTablesPrevious,
                                        BtnScrollTablesNext,
                                        MenuPlaces,
                                        this);

            MenuTables.OnEntitySelected += MenuTables_TableSelected;
            body.Put(MenuTables, 143, 0);

            body.Put(CreatePlaceScrollersBox(), 0, 493 - AppSettings.Instance.sizePosTableButton.Height);
            body.Put(CreateTablesScrollersBox(), 690 - 130, 493 - AppSettings.Instance.sizePosTableButton.Height);
            return body;
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
