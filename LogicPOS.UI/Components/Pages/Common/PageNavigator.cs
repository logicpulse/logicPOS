using Gtk;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    internal class PageNavigator : Box
    {
        private readonly Window _parentWindow;
        private readonly Page _page;
        private readonly GridViewNavigatorMode _mode;

        public HBox ExtraButtonSpace { get; set; }

        public IconButtonWithText ButtonFirstPage { get; set; }

        public IconButtonWithText ButtonPrevPage { get; set; }

        public IconButtonWithText ButtonNextPage { get; set; }

        public IconButtonWithText ButtonLastPage { get; set; }

        public IconButtonWithText ButtonPrevRecord { get; set; }

        public IconButtonWithText ButtonNextRecord { get; set; }

        public IconButtonWithText ButtonInsert { get; set; }

        public IconButtonWithText ButtonView { get; set; }

        public IconButtonWithText ButtonUpdate { get; set; }

        public IconButtonWithText ButtonDelete { get; set; }

        public IconButtonWithText ButtonRefresh { get; set; }

        public PageSearchBox SearchBox { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentRecord { get; set; }
        public int TotalRecords { get; set; }


        public PageNavigator(
            Window parent,
            Page page,
            GridViewNavigatorMode mode)
        {
            _parentWindow = parent;
            _page = page;
            _mode = mode;

            CurrentPage = 1;
            this.HeightRequest = 60;
            Initialize();
        }

        private void Initialize()
        {
            HBox navigatorComponent = new HBox(false, 0);
            HBox buttonsComponent = new HBox(true, 0);

            string name = _page.Toplevel.ToString();
            bool showFilterAndMoreButtons = false;

            if (name == "logicpos.Classes.Gui.Gtk.BackOffice.TreeViewDocumentFinanceMaster" ||
                name == "logicpos.Classes.Gui.Gtk.BackOffice.TreeViewDocumentFinancePayment")
            {
                showFilterAndMoreButtons = true;
            }

            SearchBox = new PageSearchBox(_parentWindow, showFilterAndMoreButtons);

            InitializeButtons();

            if (_mode == GridViewNavigatorMode.Default)
            {
                ExtraButtonSpace = new HBox(false, 0);

                buttonsComponent.PackStart(ButtonPrevRecord, false, false, 0);
                buttonsComponent.PackStart(ButtonNextRecord, false, false, 0);
                buttonsComponent.PackStart(ButtonInsert, false, false, 0);
                buttonsComponent.PackStart(ButtonView, false, false, 0);
                buttonsComponent.PackStart(ButtonUpdate, false, false, 0);
                buttonsComponent.PackStart(ButtonDelete, false, false, 0);
                buttonsComponent.PackStart(ButtonRefresh, false, false, 0);

                navigatorComponent.PackStart(SearchBox, false, false, 0);

                navigatorComponent.PackStart(ExtraButtonSpace, true, true, 0);
                navigatorComponent.PackStart(buttonsComponent, false, false, 0);
                PackStart(navigatorComponent);
            }
        }

        private void InitializeButtons()
        {
            ButtonPrevRecord = CreateButton("touchButtonPrev_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_prev"), @"Icons/icon_pos_nav_prev.png");
            ButtonNextRecord = CreateButton("touchButtonNext_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_next"), @"Icons/icon_pos_nav_next.png");
            ButtonInsert = CreateButton("touchButtonInsert_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_insert"), @"Icons/icon_pos_nav_new.png");
            ButtonView = CreateButton("touchButtonView_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_view"), @"Icons/icon_pos_nav_view.png");
            ButtonUpdate = CreateButton("touchButtonUpdate_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_update"), @"Icons/icon_pos_nav_update.png");
            ButtonDelete = CreateButton("touchButtonDelete_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_delete"), @"Icons/icon_pos_nav_delete.png");
            ButtonRefresh = CreateButton("touchButtonRefresh_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_refresh"), @"Icons/icon_pos_nav_refresh.png");

            AddButtonsEventHandlers();
        }

        private void AddButtonsEventHandlers()
        {
            ButtonPrevRecord.Clicked += delegate { _page.Previous(); };
            ButtonNextRecord.Clicked += delegate { _page.Next(); };

            ButtonInsert.Clicked += delegate { _page.Insert(); };
            ButtonView.Clicked += delegate { _page.Update(DialogMode.View); };
            ButtonUpdate.Clicked += delegate { _page.Update(); };
            ButtonDelete.Clicked += delegate { _page.Delete(); };
            ButtonRefresh.Clicked += delegate { _page.Refresh(); };
        }

        public IconButtonWithText CreateButton(string name,
                                               string label,
                                               string icon)
        {
            string fileIcon = PathsSettings.ImagesFolderLocation + icon;
            string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
            Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
            Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButton;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                    Text = label,
                    Font = fontBaseDialogActionAreaButton,
                    FontColor = colorBaseDialogActionAreaButtonFont,
                    Icon = fileIcon,
                    IconSize = sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                    ButtonSize = sizeBaseDialogActionAreaBackOfficeNavigatorButton
                });
        }

        public void UpdateButtons(TreeView gridView)
        {
            //FirstPage/PrevPage
            if (CurrentPage == 1 || TotalRecords == 0)
            {
                if (ButtonFirstPage != null && ButtonFirstPage.Sensitive) ButtonFirstPage.Sensitive = false;
                if (ButtonPrevPage != null && ButtonPrevPage.Sensitive) ButtonPrevPage.Sensitive = false;
            }
            else
            {
                if (ButtonFirstPage != null && !ButtonFirstPage.Sensitive) ButtonFirstPage.Sensitive = true;
                if (ButtonPrevPage != null && !ButtonPrevPage.Sensitive) ButtonPrevPage.Sensitive = true;
            };
            //NextPage/LastPage
            if (CurrentPage == TotalPages || TotalRecords == 0)
            {
                if (ButtonNextPage != null && ButtonNextPage.Sensitive) ButtonNextPage.Sensitive = false;
                if (ButtonLastPage != null && ButtonLastPage.Sensitive) ButtonLastPage.Sensitive = false;
            }
            else
            {
                if (ButtonNextPage != null && !ButtonNextPage.Sensitive) ButtonNextPage.Sensitive = true;
                if (ButtonLastPage != null && !ButtonLastPage.Sensitive) ButtonLastPage.Sensitive = true;
            }
            //PrevRecord
            if (CurrentRecord == 0 || TotalRecords == 0)
            {
                if (ButtonPrevRecord != null && ButtonPrevRecord.Sensitive) ButtonPrevRecord.Sensitive = false;
            }
            else
            {
                if (ButtonPrevRecord != null && !ButtonPrevRecord.Sensitive) ButtonPrevRecord.Sensitive = true;
            };
            //NextRecord
            if (CurrentRecord == TotalRecords || TotalRecords < 1 || TotalRecords == 0)
            {
                if (ButtonNextRecord != null && ButtonNextRecord.Sensitive) ButtonNextRecord.Sensitive = false;
            }
            else
            {
                if (ButtonNextRecord != null && !ButtonNextRecord.Sensitive) ButtonNextRecord.Sensitive = true;
            };

            //View/Update/Delete
            if (gridView.Model.IterNChildren() > 0 && _page.SelectedEntity != null)
            {
                if (ButtonView != null && !ButtonView.Sensitive && _page.AllowRecordView) ButtonView.Sensitive = true;
                if (ButtonUpdate != null && !ButtonUpdate.Sensitive && _page.AllowRecordUpdate) ButtonUpdate.Sensitive = true;
                if (ButtonDelete != null && !ButtonDelete.Sensitive && _page.AllowRecordDelete) ButtonDelete.Sensitive = true;
            }
            else
            {

                if (ButtonView != null && ButtonView.Sensitive) ButtonView.Sensitive = false;
                if (ButtonUpdate != null && ButtonUpdate.Sensitive) ButtonUpdate.Sensitive = false;
                if (ButtonDelete != null && ButtonDelete.Sensitive) ButtonDelete.Sensitive = false;
            };

        }
    }
}
