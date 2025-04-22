using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
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
    }
}
