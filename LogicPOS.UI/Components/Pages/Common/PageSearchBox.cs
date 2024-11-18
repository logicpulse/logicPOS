using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    public class PageSearchBox : Box
    {
        private Window SourceWindow { get; set; }
        public bool ShowMoreButton { get; set; }
        public bool ShowFilterButton { get; set; }
        internal EntryBoxValidation TxtSearch { get; set; }
        public string SearchText => TxtSearch.EntryValidation.Text;
        public HBox Bar { get; set; } = new HBox(false, 0);
        public IconButtonWithText BtnMore { get; set; }
        public IconButtonWithText BtnFilter { get; set; }
        public string BtnMoreIcon => PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_more.png";
        public string BtnFilterIcon => PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_filter.png";

        public PageSearchBox(Window parentWindow, bool showFilterAndMoreButtons)
        {
            SourceWindow = parentWindow;
            Design(showFilterAndMoreButtons);
        }

        private void Design(bool showFilterAndMoreButtons)
        {
           
            string regexAlfaNumericExtended = RegularExpressions.AlfaNumericExtended;


            TxtSearch = new EntryBoxValidation(SourceWindow,
                                            GeneralUtils.GetResourceByName("widget_generictreeviewsearch_search_label"),
                                            KeyboardMode.AlfaNumeric,
                                            regexAlfaNumericExtended,
                                            false);

            TxtSearch.WidthRequest = LogicPOSAppContext.ScreenSize.Width == 800 && LogicPOSAppContext.ScreenSize.Height == 600 ? 150 : 250;

            Bar.PackStart(TxtSearch, true, true, 0);

            PackStart(Bar);


            if (showFilterAndMoreButtons)
            {
                Size buttonSize = ExpressionEvaluatorExtended.sizePosToolbarButtonSizeDefault;
                Size buttonIconSize = ExpressionEvaluatorExtended.sizePosToolbarButtonIconSizeDefault;

                BtnMore = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "touchButtonSearchAdvanced_DialogActionArea",
                        BackgroundColor = Color.Transparent,
                        Text = GeneralUtils.GetResourceByName("global_button_label_more"),
                        Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                        FontColor = AppSettings.Instance.colorBaseDialogActionAreaButtonFont,
                        Icon = BtnMoreIcon,
                        IconSize = buttonIconSize,
                        ButtonSize = buttonSize
                    })
                { Sensitive = true };

                BtnFilter = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "touchButtonSearchAdvanced_DialogActionArea",
                        BackgroundColor = Color.Transparent,
                        Text = GeneralUtils.GetResourceByName("global_button_label_filter"),
                        Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                        FontColor = AppSettings.Instance.colorBaseDialogActionAreaButtonFont,
                        Icon = BtnFilterIcon,
                        IconSize = buttonIconSize,
                        ButtonSize = buttonSize
                    })
                { Sensitive = true };

                Bar.PackStart(BtnMore, false, false, 0);
                Bar.PackStart(BtnFilter, false, false, 0);

                BtnMore.Clicked += BtnMore_Clicked;
                BtnMore.Clicked += BtnFilter_Clicked;
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
