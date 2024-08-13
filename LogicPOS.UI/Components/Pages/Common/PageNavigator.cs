using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    internal class PageNavigator : Box
    {
        private readonly Page _page;

        public HBox ExtraButtonSpace { get; set; }
        public PageSearchBox SearchBox { get; set; }

        #region Buttons
        public IconButtonWithText BtnPrevious { get; set; }
        public IconButtonWithText ButtonNextRecord { get; set; }
        public IconButtonWithText ButtonInsert { get; set; }
        public IconButtonWithText ButtonView { get; set; }
        public IconButtonWithText ButtonUpdate { get; set; }
        public IconButtonWithText ButtonDelete { get; set; }
        public IconButtonWithText ButtonRefresh { get; set; }
        #endregion

     
        public int CurrentRecord { get; set; }
        public int TotalRecords { get; set; }


        public PageNavigator(Page page)
        {
            _page = page;
            HeightRequest = 60;

            Initialize();
        }

        private void Initialize()
        {
            HBox navigatorComponent = new HBox(false, 0);
            HBox buttonsComponent = new HBox(true, 0);

            InitializeButtons();

            buttonsComponent.PackStart(BtnPrevious, false, false, 0);
            buttonsComponent.PackStart(ButtonNextRecord, false, false, 0);
            buttonsComponent.PackStart(ButtonInsert, false, false, 0);
            buttonsComponent.PackStart(ButtonView, false, false, 0);
            buttonsComponent.PackStart(ButtonUpdate, false, false, 0);
            buttonsComponent.PackStart(ButtonDelete, false, false, 0);
            buttonsComponent.PackStart(ButtonRefresh, false, false, 0);

            InitializeSearchBox();
            navigatorComponent.PackStart(SearchBox, false, false, 0);

            InitializeExtraButtonSpace();
            navigatorComponent.PackStart(ExtraButtonSpace, true, true, 0);

            navigatorComponent.PackStart(buttonsComponent, false, false, 0);

            PackStart(navigatorComponent);

        }

        private void InitializeSearchBox()
        {
            bool showFilterAndMoreButtons = false;

            if ((_page as object) is logicpos.Classes.Gui.Gtk.BackOffice.TreeViewDocumentFinanceMaster ||
                (_page as object) is logicpos.Classes.Gui.Gtk.BackOffice.TreeViewDocumentFinancePayment)
            {
                showFilterAndMoreButtons = true;
            }

            SearchBox = new PageSearchBox(_page.PageParentWindow, showFilterAndMoreButtons);

            SearchBox.TxtSearch.EntryValidation.Changed += delegate
            {
                _page.GridViewSettings.Filter.Refilter();
                Update();
            };
        }

        private void InitializeExtraButtonSpace()
        {
            ExtraButtonSpace = new HBox(false, 0);

            IconButtonWithText buttonApplyPrivileges = CreateButton("touchButtonApplyPrivileges_DialogActionArea",
                                                                    GeneralUtils.GetResourceByName("global_user_apply_privileges"),
                                                                    @"Icons/icon_pos_nav_refresh.png");

            buttonApplyPrivileges.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_USER_PRIVILEGES_APPLY");

            buttonApplyPrivileges.Clicked += delegate
            {
                GlobalApp.BackOfficeMainWindow.Accordion.UpdateMenuPrivileges();
                GlobalApp.PosMainWindow.TicketList.UpdateTicketListButtons();
            };

            ExtraButtonSpace.PackStart(buttonApplyPrivileges, false, false, 0);
        }

        private void InitializeButtons()
        {
            BtnPrevious = CreateButton("touchButtonPrev_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_prev"), @"Icons/icon_pos_nav_prev.png");
            ButtonNextRecord = CreateButton("touchButtonNext_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_next"), @"Icons/icon_pos_nav_next.png");
            ButtonInsert = CreateButton("touchButtonInsert_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_insert"), @"Icons/icon_pos_nav_new.png");
            ButtonView = CreateButton("touchButtonView_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_view"), @"Icons/icon_pos_nav_view.png");
            ButtonUpdate = CreateButton("touchButtonUpdate_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_update"), @"Icons/icon_pos_nav_update.png");
            ButtonDelete = CreateButton("touchButtonDelete_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_delete"), @"Icons/icon_pos_nav_delete.png");
            ButtonRefresh = CreateButton("touchButtonRefresh_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_refresh"), @"Icons/icon_pos_nav_refresh.png");

            AddButtonsEventHandlers();
        }

        public void Next()
        {
            _page.GridViewSettings.Path.Next();
            _page.GridView.SetCursor(_page.GridViewSettings.Path, null, false);
        }
        
        public void Previous()
        {
            _page.GridViewSettings.Path.Prev();
            _page.GridView.SetCursor(_page.GridViewSettings.Path, null, false);
        }
        
        private void AddButtonsEventHandlers()
        {
            BtnPrevious.Clicked += delegate { Previous(); };
            ButtonNextRecord.Clicked += delegate { Next(); };

            ButtonInsert.Clicked += delegate { _page.InsertEntity(); };
            ButtonView.Clicked += delegate { _page.ViewEntity(); };
            ButtonUpdate.Clicked += delegate { _page.UpdateEntity(); };
            ButtonDelete.Clicked += delegate { _page.DeleteEntity(); };
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

        public void UpdateButtonsSensitivity()
        {
            if (CurrentRecord == 0 || TotalRecords == 0)
            {
                if (BtnPrevious.Sensitive) BtnPrevious.Sensitive = false;
            }
            else
            {
                if (!BtnPrevious.Sensitive) BtnPrevious.Sensitive = true;
            }

            if (CurrentRecord == TotalRecords || TotalRecords < 1)
            {
                if (ButtonNextRecord.Sensitive) ButtonNextRecord.Sensitive = false;
            }
            else
            {
                if (!ButtonNextRecord.Sensitive) ButtonNextRecord.Sensitive = true;
            }

            if (_page.GridView.Model.IterNChildren() > 0 && _page.SelectedEntity != null)
            {
                if (!ButtonView.Sensitive && _page.CanViewEntity) ButtonView.Sensitive = true;
                if (!ButtonUpdate.Sensitive && _page.CanUpdateEntity) ButtonUpdate.Sensitive = true;
                if (!ButtonDelete.Sensitive && _page.CanDeleteEntity) ButtonDelete.Sensitive = true;
            }
            else
            {

                if (ButtonView != null && ButtonView.Sensitive) ButtonView.Sensitive = false;
                if (ButtonUpdate != null && ButtonUpdate.Sensitive) ButtonUpdate.Sensitive = false;
                if (ButtonDelete != null && ButtonDelete.Sensitive) ButtonDelete.Sensitive = false;
            };
        }

        public void Update()
        {
            if (_page.GridView.Model == null)
            {
                TotalRecords = 0;
            }
            else
            {
                TotalRecords = _page.GridView.Model.IterNChildren() - 1;
            }
;
            UpdateButtonsSensitivity();
        }
    }
}
