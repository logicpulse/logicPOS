using Gtk;
using logicpos;
using logicpos.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    public class PageSearchBox : Box
    {
        private Window _parentWindow;
        public bool ShowMoreButton { get; set; }
        public bool ShowFilterButton { get; set; }
        internal EntryBoxValidation TxtSearch { get; set; }
        public string SearchText => TxtSearch.EntryValidation.Text;
  

        public PageSearchBox(Window parentWindow, bool showFilterAndMoreButtons)
        {
            _parentWindow = parentWindow;
            Design(showFilterAndMoreButtons);
        }
        private void Design(bool showFilterAndMoreButtons)
        {
            string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
            Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
            Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = ExpressionEvaluatorExtended.sizePosToolbarButtonSizeDefault;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = ExpressionEvaluatorExtended.sizePosToolbarButtonIconSizeDefault;
            string regexAlfaNumericExtended = RegexUtils.RegexAlfaNumericExtended;


            TxtSearch = new EntryBoxValidation(_parentWindow,
                                            GeneralUtils.GetResourceByName("widget_generictreeviewsearch_search_label"),
                                            KeyboardMode.AlfaNumeric,
                                            regexAlfaNumericExtended,
                                            false);

            TxtSearch.WidthRequest = GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600 ? 150 : 250;

            HBox horizontalBox = new HBox(false, 0);
            horizontalBox.PackStart(TxtSearch, true, true, 0);

            PackStart(horizontalBox);


            if (showFilterAndMoreButtons)
            {
                IconButtonWithText btnMore;
                IconButtonWithText btnFilter;

                string iconMore = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_more.png";
                string iconFilter = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_filter.png";

                btnMore = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "touchButtonSearchAdvanced_DialogActionArea",
                        BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                        Text = GeneralUtils.GetResourceByName("global_button_label_more"),
                        Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                        FontColor = colorBaseDialogActionAreaButtonFont,
                        Icon = iconMore,
                        IconSize = sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                        ButtonSize = sizeBaseDialogActionAreaBackOfficeNavigatorButton
                    })
                { Sensitive = true };

                btnFilter = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "touchButtonSearchAdvanced_DialogActionArea",
                        BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                        Text = GeneralUtils.GetResourceByName("global_button_label_filter"),
                        Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                        FontColor = colorBaseDialogActionAreaButtonFont,
                        Icon = iconFilter,
                        IconSize = sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                        ButtonSize = sizeBaseDialogActionAreaBackOfficeNavigatorButton
                    })
                { Sensitive = true };

                horizontalBox.PackStart(btnMore, false, false, 0);
                horizontalBox.PackStart(btnFilter, false, false, 0);

                btnMore.Clicked += BtnMore_Clicked;
                btnFilter.Clicked += BtnFilter_Clicked;
            }

        }

        public event EventHandler Clicked;
        public void BtnMore_Clicked(object sender, EventArgs e)
        {
            //TreeIter iterator;
            //_filter.GetIterFirst(out iterator);
            //_gridView.SetCursor(_filter.GetPath(iterator), _gridView.Columns[0], false);
            ShowMoreButton = false;
        }
        public void BtnFilter_Clicked(object sender, EventArgs e)
        {
            //ShowFilterButton = true;
            //TreeIter treeIter;

            //_filter.GetIterFirst(out treeIter);
            //_gridView.SetCursor(_filter.GetPath(treeIter), _gridView.Columns[0], false);
            ShowFilterButton = false;
        }

    }
}
