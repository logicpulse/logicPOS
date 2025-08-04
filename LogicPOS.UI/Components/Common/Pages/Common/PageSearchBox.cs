using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    public class PageSearchBox : Box
    {
        private Window SourceWindow { get; set; }
        internal EntryBoxValidation TxtSearch { get; set; }
        public string SearchText => TxtSearch.EntryValidation.Text;
        public HBox Bar { get; set; } = new HBox(false, 0);
        public IconButtonWithText BtnMore { get; set; }
        public IconButtonWithText BtnFilter { get; set; }
        public string BtnMoreIcon => AppSettings.Paths.Images + @"Icons\icon_pos_more.png";
        public string BtnFilterIcon => AppSettings.Paths.Images + @"Icons\icon_pos_filter.png";

        public PageSearchBox(Window parentWindow)
        {
            SourceWindow = parentWindow;
            Intitialize();
        }

        private void Intitialize()
        {
            TxtSearch = new EntryBoxValidation(SourceWindow,
                                               GeneralUtils.GetResourceByName("widget_generictreeviewsearch_search_label"),
                                               KeyboardMode.AlfaNumeric,
                                               RegularExpressions.AlfaNumericExtended,
                                               false);

            TxtSearch.WidthRequest = AppSettings.Instance.AppScreenSize.Width == 800 && AppSettings.Instance.AppScreenSize.Height == 600 ? 150 : 250;
            TxtSearch.WidthRequest = AppSettings.Instance.AppScreenSize.Width <= 1024? 150 : 250;

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
                    FontColor = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont,
                    Icon = icon,
                    IconSize = buttonIconSize,
                    ButtonSize = buttonSize
                })
            { Sensitive = true };
        }
    }
}
