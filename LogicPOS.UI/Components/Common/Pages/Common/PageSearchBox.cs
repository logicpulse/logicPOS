using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
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

        public PageSearchBox(Window parentWindow)
        {
            SourceWindow = parentWindow;
            Design();
        }

        private void Design()
        {
            TxtSearch = new EntryBoxValidation(SourceWindow,
                                               GeneralUtils.GetResourceByName("widget_generictreeviewsearch_search_label"),
                                               KeyboardMode.AlfaNumeric,
                                               RegularExpressions.AlfaNumericExtended,
                                               false);

            TxtSearch.WidthRequest = LogicPOSAppContext.ScreenSize.Width == 800 && LogicPOSAppContext.ScreenSize.Height == 600 ? 150 : 250;

            Bar.PackStart(TxtSearch, true, true, 0);



            BtnMore = CreateIconButton("touchButtonSearchAdvanced_DialogActionArea",
                                       GeneralUtils.GetResourceByName("global_button_label_more"),
                                       BtnMoreIcon);

            BtnFilter = CreateIconButton("touchButtonSearchAdvanced_DialogActionArea",
                                         GeneralUtils.GetResourceByName("global_button_label_filter"),
                                         BtnFilterIcon);

            Bar.PackStart(BtnMore, false, false, 0);
            Bar.PackStart(BtnFilter, false, false, 0);

            PackStart(Bar);
        }

        private IconButtonWithText CreateIconButton(string name, string text, string icon)
        {
            Size buttonSize = ExpressionEvaluatorExtended.sizePosToolbarButtonSizeDefault;
            Size buttonIconSize = ExpressionEvaluatorExtended.sizePosToolbarButtonIconSizeDefault;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    BackgroundColor = Color.Transparent,
                    Text = text,
                    Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                    FontColor = AppSettings.Instance.colorBaseDialogActionAreaButtonFont,
                    Icon = icon,
                    IconSize = buttonIconSize,
                    ButtonSize = buttonSize
                })
            { Sensitive = true };
        }
    }
}
