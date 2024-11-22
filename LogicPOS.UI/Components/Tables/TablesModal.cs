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
    public class TablesModal : Modal
    {
        #region Components
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnFilterAll { get; set; }
        private IconButtonWithText BtnFilterFree { get; set; }
        private IconButtonWithText BtnFilterOpen { get; set; }
        private IconButtonWithText BtnFilterReserved { get; set; }
        private IconButtonWithText BtnReservation { get; set; }
        private IconButtonWithText BtnViewOrders { get; set; }
        private IconButtonWithText BtnViewTables { get; set; }
        private IconButton BtnScrollPlacesPrevious { get; set; }
        private IconButton BtnScrollPlacesNext { get; set; }
        private IconButton BtnScrollTablesPrevious { get; set; }
        private IconButton BtnScrollTablesNext { get; set; }
        private PlacesMenu MenuPlaces { get; set; }
        private TablesMenu MenuTables { get; set; }
        #endregion

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

            MenuPlaces = new PlacesMenu(BtnScrollPlacesPrevious, BtnScrollPlacesNext);
            body.Put(MenuPlaces, 0, 0);

            MenuTables = new TablesMenu(BtnScrollTablesPrevious, BtnScrollTablesNext,MenuPlaces);
            MenuTables.TableSelected += MenuTables_TableSelected;
            body.Put(MenuTables, 143, 0);

            body.Put(CreatePlaceScrollersBox(), 0, 493 - AppSettings.Instance.sizePosTableButton.Height);
            body.Put(CreateTablesScrollersBox(), 690 - 130, 493 - AppSettings.Instance.sizePosTableButton.Height);
            return body;
        }

        private IconButtonWithText CreateButton(string name,
                                                string text,
                                                string icon,
                                                Color? bgColor = null)
        {
            var font = AppSettings.Instance.fontBaseDialogActionAreaButton;
            var fontColor = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
            var iconSize = AppSettings.Instance.sizeBaseDialogActionAreaButtonIcon;
            var buttonSize = AppSettings.Instance.sizeBaseDialogActionAreaButton;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    BackgroundColor = bgColor ?? Color.Transparent,
                    Text =text,
                    Font = font,
                    FontColor = fontColor,
                    Icon = icon,
                    IconSize = iconSize,
                    ButtonSize = buttonSize
                });
        }

        private void InitializeButtons()
        {
            BtnReservation = CreateButton("touchButtonTableReservation_DialogActionArea",
                                          GeneralUtils.GetResourceByName("pos_button_label_table_reservation"),
                                          PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_reservation.png",
                                          AppSettings.Instance.colorBaseDialogActionAreaButtonBackground);

            BtnFilterAll = CreateButton("touchButton_Green",
                                        GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_all"),
                                        PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_all.png");

            BtnFilterFree = CreateButton("touchButton_Green", GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_free"),
                                         PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_free.png");

            BtnFilterOpen = CreateButton("touchButton_Green", GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_open"),
                                         PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_open.png");

            BtnFilterReserved = CreateButton("touchButton_Green", GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_reserved"),
                                             PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_reserved.png");

            BtnViewOrders = CreateButton("touchButton_Red", GeneralUtils.GetResourceByName("dialog_orders_button_label_view_orders"),
                                         PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_retail_view_order.png");

            BtnViewTables = CreateButton("touchButton_Green", GeneralUtils.GetResourceByName("dialog_orders_button_label_view_tables"),
                                         PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_view_tables.png");

            InitializeScrollerButtons();
            AddEventHandlers();
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

        private void MenuTables_TableSelected(Api.Entities.Table table)
        {
            BtnReservation.Sensitive = (table.Status == TableStatus.Free || table.Status == TableStatus.Reserved);
            BtnOk.Sensitive = table.Status != TableStatus.Reserved;
        }

        private void BtnReservation_Clicked(object sender, EventArgs e)
        {
            if(MenuTables.SelectedTable == null)
            {
                return;
            }

            if(MenuTables.SelectedTable.Status == TableStatus.Free)
            {
                TablesService.ReserveTable(MenuTables.SelectedTable);
            }
            else if (MenuTables.SelectedTable.Status == TableStatus.Reserved)
            {
                TablesService.FreeTable(MenuTables.SelectedTable);
            }

            MenuTables.Refresh(MenuTables.Filter);
        }

        private void BtnFilterAll_Clicked(object sender, EventArgs e)
        {
            MenuPlaces.SelectedPlace = null;
            MenuTables.Refresh();
        }

        private void BtnFilterReserved_Clicked(object sender, EventArgs e)
        {
            MenuPlaces.SelectedPlace = null;
            MenuTables.Refresh(TableStatus.Reserved);
        }

        private void BtnFilterOpen_Clicked(object sender, EventArgs e)
        {
            MenuPlaces.SelectedPlace = null;
            MenuTables.Refresh(TableStatus.Open);
        }

        private void BtnFilterFree_Clicked(object sender, EventArgs e)
        {
            MenuPlaces.SelectedPlace = null;
            MenuTables.Refresh(TableStatus.Free);
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if(MenuTables.SelectedTable != null)
            {
                SaleContext.SetCurrentTable(MenuTables.SelectedTable);
            }
        }

        private void InitializeScrollerButtons()
        {
            InitializePlacesScrollersButtons();
            InitializeTablesScrollersButtons();
        }

        private void InitializePlacesScrollersButtons()
        {
            BtnScrollPlacesPrevious = new IconButton(
              new ButtonSettings
              {
                  BackgroundColor = Color.White,
                  Icon = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_left.png",
                  IconSize = new Size(62, 31),
                  ButtonSize = AppSettings.Instance.sizePosSmallButtonScroller
              });

            BtnScrollPlacesNext = new IconButton(
               new ButtonSettings
               {
                   BackgroundColor = Color.White,
                   Icon = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_right.png",
                   IconSize = new Size(62, 31),
                   ButtonSize = AppSettings.Instance.sizePosSmallButtonScroller
               });

            BtnScrollPlacesPrevious.Relief = ReliefStyle.None;
            BtnScrollPlacesPrevious.BorderWidth = 0;
            BtnScrollPlacesPrevious.CanFocus = false;

            BtnScrollPlacesNext.Relief = ReliefStyle.None;
            BtnScrollPlacesNext.BorderWidth = 0;
            BtnScrollPlacesNext.CanFocus = false;
        }

        private void InitializeTablesScrollersButtons()
        {
            BtnScrollTablesPrevious = new IconButton(
              new ButtonSettings
              {
                  BackgroundColor = Color.White,
                  Icon = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_left.png",
                  IconSize = new Size(62, 31),
                  ButtonSize = AppSettings.Instance.sizePosSmallButtonScroller
              });

            BtnScrollTablesNext = new IconButton(
               new ButtonSettings
               {
                   BackgroundColor = Color.White,
                   Icon = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_right.png",
                   IconSize = new Size(62, 31),
                   ButtonSize = AppSettings.Instance.sizePosSmallButtonScroller
               });

            BtnScrollTablesPrevious.Relief = ReliefStyle.None;
            BtnScrollTablesPrevious.BorderWidth = 0;
            BtnScrollTablesPrevious.CanFocus = false;

            BtnScrollTablesNext.Relief = ReliefStyle.None;
            BtnScrollTablesNext.BorderWidth = 0;
            BtnScrollTablesNext.CanFocus = false;
        }

        private HBox CreatePlaceScrollersBox()
        {
            HBox box = new HBox(true, 0);
            box.PackStart(BtnScrollPlacesPrevious);
            box.PackStart(BtnScrollPlacesNext);
            return box;
        }

        private HBox CreateTablesScrollersBox()
        {
            HBox box = new HBox(true, 0);
            box.PackStart(BtnScrollTablesPrevious);
            box.PackStart(BtnScrollTablesNext);
            return box;
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
