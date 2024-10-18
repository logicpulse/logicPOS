using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    internal class PageNavigator<Tentity> : Box where Tentity : ApiEntity
    {
        private readonly Page<Tentity> _page;

        public HBox ExtraButtonSpace { get; set; }
        public PageSearchBox SearchBox { get; set; }

        #region Buttons
        public IconButtonWithText BtnPrevious { get; set; }
        public IconButtonWithText BtnNext { get; set; }
        public IconButtonWithText BtnInsert { get; set; }
        public IconButtonWithText BtnView { get; set; }
        public IconButtonWithText BtnUpdate { get; set; }
        public IconButtonWithText BtnDelete { get; set; }
        public IconButtonWithText BtnRefresh { get; set; }
        public IconButtonWithText BtnApply { get; set; }
        public HBox Bar { get; set; } = new HBox(false, 0);
        public HBox RightButtons { get; set; } = new HBox(true, 0);
        #endregion

        public int CurrentRecord { get; set; }
        public int TotalRecords { get; set; }

        public PageNavigator(Page<Tentity> page)
        {
            _page = page;
            HeightRequest = 60;

            InitializeButtons();
            InitializeSearchBox();
            InitializeExtraButtonSpace();

            Design();
        }

        private void Design()
        {
            RightButtons.PackStart(BtnPrevious, false, false, 0);
            RightButtons.PackStart(BtnNext, false, false, 0);
            RightButtons.PackStart(BtnInsert, false, false, 0);
            RightButtons.PackStart(BtnView, false, false, 0);
            RightButtons.PackStart(BtnUpdate, false, false, 0);
            RightButtons.PackStart(BtnDelete, false, false, 0);
            RightButtons.PackStart(BtnRefresh, false, false, 0);

            Bar.PackStart(SearchBox, false, false, 0);
            Bar.PackStart(ExtraButtonSpace, true, true, 0);
            Bar.PackStart(RightButtons, false, false, 0);

            PackStart(Bar);
        }

        private void InitializeSearchBox()
        {
            bool showFilterAndMoreButtons = true;

            SearchBox = new PageSearchBox(_page.SourceWindow, showFilterAndMoreButtons);

            SearchBox.TxtSearch.EntryValidation.Changed += delegate
            {
                _page.GridViewSettings.Filter.Refilter();
                Update();
            };
        }

        private void InitializeExtraButtonSpace()
        {
            ExtraButtonSpace = new HBox(false, 0);

            BtnApply = CreateButton("touchButtonApplyPrivileges_DialogActionArea",
                                    GeneralUtils.GetResourceByName("global_user_apply_privileges"),
                                    @"Icons/icon_pos_nav_refresh.png");

            BtnApply.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_USER_PRIVILEGES_APPLY");

            BtnApply.Clicked += delegate
            {
                GlobalApp.BackOffice.Menu.UpdateMenuPrivileges();
                GlobalApp.PosMainWindow.TicketList.UpdateSaleOptionsPanelButtons();
            };

            ExtraButtonSpace.PackStart(BtnApply, false, false, 0);
        }

        private void InitializeButtons()
        {
            BtnPrevious = CreateButton("touchButtonPrev_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_prev"), @"Icons/icon_pos_nav_prev.png");
            BtnNext = CreateButton("touchButtonNext_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_next"), @"Icons/icon_pos_nav_next.png");
            BtnInsert = CreateButton("touchButtonInsert_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_insert"), @"Icons/icon_pos_nav_new.png");
            BtnView = CreateButton("touchButtonView_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_view"), @"Icons/icon_pos_nav_view.png");
            BtnUpdate = CreateButton("touchButtonUpdate_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_update"), @"Icons/icon_pos_nav_update.png");
            BtnDelete = CreateButton("touchButtonDelete_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_delete"), @"Icons/icon_pos_nav_delete.png");
            BtnRefresh = CreateButton("touchButtonRefresh_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_refresh"), @"Icons/icon_pos_nav_refresh.png");

            AddButtonsEventHandlers();
        }

        public void Next()
        {
            if (_page.GridViewSettings.Path == null)
            {
                SelectSecondRecord();
                return;
            }

            _page.GridViewSettings.Path.Next();
            _page.GridView.SetCursor(_page.GridViewSettings.Path, null, false);
        }

        private void SelectSecondRecord()
        {
            var path = new TreePath();
            path.AppendIndex(1);
            _page.GridView.SetCursor(path, null, false);
        }

        public void Previous()
        {
            _page.GridViewSettings.Path.Prev();
            _page.GridView.SetCursor(_page.GridViewSettings.Path, null, false);
        }

        private void AddButtonsEventHandlers()
        {
            BtnPrevious.Clicked += delegate { Previous(); };
            BtnNext.Clicked += delegate { Next(); };
            BtnInsert.Clicked += delegate { _page.RunModal(EntityEditionModalMode.Insert); };
            BtnView.Clicked += delegate { _page.RunModal(EntityEditionModalMode.View); };
            BtnUpdate.Clicked += delegate { _page.RunModal(EntityEditionModalMode.Update); };
            BtnDelete.Clicked += delegate { _page.DeleteEntity(); };
            BtnRefresh.Clicked += delegate { _page.Refresh(); };
        }

        public IconButtonWithText CreateButton(string name,
                                               string label,
                                               string icon)
        {
            string fileIcon = PathsSettings.ImagesFolderLocation + icon;
            string font = AppSettings.Instance.fontBaseDialogActionAreaButton;
            Color bgColor = Color.Transparent;
            Color fontColor = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
            Size buttonSize = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButton;
            Size iconSize = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    BackgroundColor = bgColor,
                    Text = label,
                    Font = font,
                    FontColor = fontColor,
                    Icon = fileIcon,
                    IconSize = iconSize,
                    ButtonSize = buttonSize
                });
        }

        public void UpdateButtonsSensitivity()
        {
            UpdateBtnPrevious();
            UpdateBtnNext();
            UpdateBtnView();
            UpdateBtnUpdate();
            UpdateBtnDelete();
        }

        private void UpdateBtnDelete()
        {
            BtnDelete.Sensitive = _page.CanDeleteEntity && _page.SelectedEntity != null;
        }

        private void UpdateBtnUpdate()
        {
            BtnUpdate.Sensitive = _page.CanUpdateEntity && _page.SelectedEntity != null;
        }

        private void UpdateBtnView()
        {
            BtnView.Sensitive = _page.CanViewEntity && _page.SelectedEntity != null;
        }

        private void UpdateBtnNext()
        {
            if (CurrentRecord == TotalRecords || TotalRecords < 1)
            {
                if (BtnNext.Sensitive) BtnNext.Sensitive = false;
            }
            else
            {
                if (!BtnNext.Sensitive) BtnNext.Sensitive = true;
            }
        }

        private void UpdateBtnPrevious()
        {
            if (CurrentRecord == 0 || TotalRecords == 0)
            {
                if (BtnPrevious.Sensitive) BtnPrevious.Sensitive = false;
            }
            else
            {
                if (!BtnPrevious.Sensitive) BtnPrevious.Sensitive = true;
            }
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
