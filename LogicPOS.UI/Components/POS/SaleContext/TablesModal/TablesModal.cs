using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Components.Modals.Common;
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

        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();

            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnFilterAll, ResponseType.None),
                new ActionAreaButton(BtnFilterFree, ResponseType.None),
                new ActionAreaButton(BtnFilterOpen, ResponseType.None),
                new ActionAreaButton(BtnFilterReserved, ResponseType.None),
                new ActionAreaButton(BtnViewOrders, ResponseType.None),
                new ActionAreaButton(BtnViewTables, ResponseType.None),
                new ActionAreaButton(BtnReservation, ResponseType.None),
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            var body = new Fixed();

            MenuPlaces = new PlacesMenu(BtnScrollPlacesPrevious, BtnScrollPlacesNext);
            body.Put(MenuPlaces, 0, 0);

            MenuTables = new TablesMenu(BtnScrollTablesPrevious, BtnScrollTablesNext,MenuPlaces);
            body.Put(MenuTables, 143, 0);

            body.Put(CreatePlaceScrollersBox(), 0, 493 - AppSettings.Instance.sizePosTableButton.Height);
            body.Put(CreateTablesScrollersBox(), 690 - 130, 493 - AppSettings.Instance.sizePosTableButton.Height);
            return body;
        }

        private IconButtonWithText CreateButton(string text,
                                                string icon)
        {
            var bgColor = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground;
            var font = AppSettings.Instance.fontBaseDialogActionAreaButton;
            var fontColor = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
            var iconSize = AppSettings.Instance.sizeBaseDialogActionAreaButtonIcon;
            var buttonSize = AppSettings.Instance.sizeBaseDialogActionAreaButton;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    BackgroundColor = bgColor,
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
            BtnReservation = CreateButton(GeneralUtils.GetResourceByName("pos_button_label_table_reservation"),
                                          PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_reservation.png");

            BtnFilterAll = CreateButton(GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_all"),
                                        PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_all.png");

            BtnFilterFree = CreateButton(GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_free"),
                                         PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_free.png");

            BtnFilterOpen = CreateButton(GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_open"),
                                         PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_open.png");

            BtnFilterReserved = CreateButton(GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_reserved"),
                                             PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_reserved.png");

            BtnViewOrders = CreateButton(GeneralUtils.GetResourceByName("dialog_orders_button_label_view_orders"),
                                         PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_retail_view_order.png");

            BtnViewTables = CreateButton(GeneralUtils.GetResourceByName("dialog_orders_button_label_view_tables"),
                                         PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_view_tables.png");

            InitializeScrollerButtons();
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
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
    }
}
