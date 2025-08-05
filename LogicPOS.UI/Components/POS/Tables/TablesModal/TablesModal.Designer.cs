using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.POS
{
    public partial class TablesModal
    {

        protected override Widget CreateBody()
        {
            var body = new Fixed();

            MenuPlaces = new PlacesMenu(BtnScrollPlacesPrevious, BtnScrollPlacesNext, this);
            body.Put(MenuPlaces, 0, 0);

            MenuTables = new TablesMenu(BtnScrollTablesPrevious,
                                        BtnScrollTablesNext,
                                        MenuPlaces,
                                        this);

            MenuTables.OnEntitySelected += MenuTables_TableSelected;
            body.Put(MenuTables, 143, 0);

            body.Put(CreatePlaceScrollersBox(), 0, 493 - AppSettings.Instance.SizePosTableButton.Height);
            body.Put(CreateTablesScrollersBox(), 690 - 130, 493 - AppSettings.Instance.SizePosTableButton.Height);
            return body;
        }

        private void InitializeButtons()
        {
            BtnReservation = CreateButton("touchButtonTableReservation_DialogActionArea",
                                          GeneralUtils.GetResourceByName("pos_button_label_table_reservation"),
                                          AppSettings.Paths.Images + @"Icons\icon_pos_table_reservation.png",
                                          AppSettings.Instance.ColorBaseDialogActionAreaButtonBackground);

            BtnFilterAll = CreateButton("touchButton_Green",
                                        GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_all"),
                                        AppSettings.Paths.Images + @"Icons\icon_pos_table_filter_all.png");

            BtnFilterFree = CreateButton("touchButton_Green", GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_free"),
                                         AppSettings.Paths.Images + @"Icons\icon_pos_table_filter_free.png");

            BtnFilterOpen = CreateButton("touchButton_Green", GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_open"),
                                         AppSettings.Paths.Images + @"Icons\icon_pos_table_filter_open.png");

            BtnFilterReserved = CreateButton("touchButton_Green", GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_reserved"),
                                             AppSettings.Paths.Images + @"Icons\icon_pos_table_filter_reserved.png");

            BtnViewOrders = CreateButton("touchButton_Red", GeneralUtils.GetResourceByName("dialog_orders_button_label_view_orders"),
                                         AppSettings.Paths.Images + @"Icons\icon_pos_retail_view_order.png");

            BtnViewTables = CreateButton("touchButton_Green", GeneralUtils.GetResourceByName("dialog_orders_button_label_view_tables"),
                                         AppSettings.Paths.Images + @"Icons\icon_pos_table_view_tables.png");

            InitializeScrollerButtons();
            AddEventHandlers();
        }

        private IconButtonWithText CreateButton(string name,
                                               string text,
                                               string icon,
                                               Color? bgColor = null)
        {
            var font = AppSettings.Instance.FontBaseDialogActionAreaButton;
            var fontColor = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont;
            var iconSize = AppSettings.Instance.SizeBaseDialogActionAreaButtonIcon;
            var buttonSize = AppSettings.Instance.SizeBaseDialogActionAreaButton;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    BackgroundColor = bgColor ?? Color.Transparent,
                    Text = text,
                    Font = font,
                    FontColor = fontColor,
                    Icon = icon,
                    IconSize = iconSize,
                    ButtonSize = buttonSize
                });
        }

        private void InitializeTablesScrollersButtons()
        {
            BtnScrollTablesPrevious = new IconButton(
              new ButtonSettings
              {
                  BackgroundColor = Color.White,
                  Icon = AppSettings.Paths.Images + @"Buttons\Pos\button_subfamily_article_scroll_left.png",
                  IconSize = new Size(62, 31),
                  ButtonSize = AppSettings.Instance.SizePosSmallButtonScroller
              });

            BtnScrollTablesNext = new IconButton(
               new ButtonSettings
               {
                   BackgroundColor = Color.White,
                   Icon = AppSettings.Paths.Images + @"Buttons\Pos\button_subfamily_article_scroll_right.png",
                   IconSize = new Size(62, 31),
                   ButtonSize = AppSettings.Instance.SizePosSmallButtonScroller
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
        
        private void InitializePlacesScrollersButtons()
        {
            BtnScrollPlacesPrevious = new IconButton(
              new ButtonSettings
              {
                  BackgroundColor = Color.White,
                  Icon = AppSettings.Paths.Images + @"Buttons\Pos\button_subfamily_article_scroll_left.png",
                  IconSize = new Size(62, 31),
                  ButtonSize = AppSettings.Instance.SizePosSmallButtonScroller
              });

            BtnScrollPlacesNext = new IconButton(
               new ButtonSettings
               {
                   BackgroundColor = Color.White,
                   Icon = AppSettings.Paths.Images + @"Buttons\Pos\button_subfamily_article_scroll_right.png",
                   IconSize = new Size(62, 31),
                   ButtonSize = AppSettings.Instance.SizePosSmallButtonScroller
               });

            BtnScrollPlacesPrevious.Relief = ReliefStyle.None;
            BtnScrollPlacesPrevious.BorderWidth = 0;
            BtnScrollPlacesPrevious.CanFocus = false;

            BtnScrollPlacesNext.Relief = ReliefStyle.None;
            BtnScrollPlacesNext.BorderWidth = 0;
            BtnScrollPlacesNext.CanFocus = false;
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
    }
}
