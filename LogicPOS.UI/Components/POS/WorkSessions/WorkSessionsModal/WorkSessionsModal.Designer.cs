using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WorkSessionsModal
    {
       private IconButtonWithText CreateButton(string name,
                                             string label,
                                             string icon)
        {
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    Text = label,
                    Font = AppSettings.Instance.FontBaseDialogActionAreaButton,
                    FontColor = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont,
                    Icon = AppSettings.Paths.Images + icon,
                    IconSize = AppSettings.Instance.SizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                    ButtonSize = AppSettings.Instance.SizeBaseDialogActionAreaBackOfficeNavigatorButton
                });
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
            AddButtonsEventHandlers();

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();

            actionAreaButtons.Add(new ActionAreaButton(BtnPrintDay, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnClose, ResponseType.Close));

            return actionAreaButtons;
        }

        protected override Widget CreateLeftContent()
        {
            HBox box = new HBox(false, 0);

            TxtSearch = new TextBox(this,
                                        LocalizedString.Instance["widget_generictreeviewsearch_search_label"],
                                        isRequired: false,
                                        isValidatable: false,
                                        includeKeyBoardButton: true,
                                        includeSelectButton: false);

            TxtSearch.Component.WidthRequest = AppSettings.Instance.AppScreenSize.Width == 800 && AppSettings.Instance.AppScreenSize.Height == 600 ? 150 : 250;

            box.PackStart(TxtSearch.Component, false, false, 0);
            box.PackStart(BtnFilter, false, false, 0);

            TxtSearch.Entry.Changed += delegate { Page.Navigator.SearchBox.TxtSearch.EntryValidation.Text = TxtSearch.Text; };

            return box;
        }

        protected override Widget CreateBody()
        {
            var pageOptions = PageOptions.SelectionPageOptions;
           
            var page = new WorkSessionsPage(this, pageOptions);
            page.SetSizeRequest(WindowSettings.Size.Width - 14, WindowSettings.Size.Height - 124);
            Fixed fixedContent = new Fixed();
            fixedContent.Put(page, 0, 0);
            Page = page;
            return fixedContent;
        }

        private void InitializeButtons()
        {

            BtnFilter = CreateButton("touchButtonSearchAdvanced_DialogActionArea",
                                    LocalizedString.Instance["global_button_label_filter"],
                                    @"Icons\icon_pos_filter.png");
        }
    }
}
