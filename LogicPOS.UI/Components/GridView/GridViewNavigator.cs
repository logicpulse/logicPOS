using Gtk;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.Utility;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    internal class GridViewNavigator<T1, T2> : Box
    {
        private readonly Window _sourceWindow;
        private readonly GridView<T1, T2> _gridView;
        private readonly GridViewNavigatorMode _navigatorMode;

        public HBox ExtraSlot { get; set; }

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

        public GridViewSearchBox TreeViewSearch { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentRecord { get; set; }
        public int TotalRecords { get; set; }


        public GridViewNavigator(Window parentWindow, GridView<T1, T2> pGenericTreeView, GridViewNavigatorMode navigatorMode)
        {
            _sourceWindow = parentWindow;
            _gridView = pGenericTreeView;
            _navigatorMode = navigatorMode;

            CurrentPage = 1;

            this.HeightRequest = 60;

            InitNavigator();
        }

        private void InitNavigator()
        {
            HBox hboxNavigator = new HBox(false, 0);
            HBox hboxNavigatorButtons = new HBox(true, 0);

            string name = _gridView.Toplevel.ToString();
            bool buttonMoreFilterVisible = false;
            if (name == "logicpos.Classes.Gui.Gtk.BackOffice.TreeViewDocumentFinanceMaster" || name == "logicpos.Classes.Gui.Gtk.BackOffice.TreeViewDocumentFinancePayment") { buttonMoreFilterVisible = true; }


            TreeViewSearch = new GridViewSearchBox(_sourceWindow, _gridView.TreeView, _gridView.ListStoreModelFilter, _gridView.Columns, buttonMoreFilterVisible);

            ButtonPrevRecord = GetNewButton("touchButtonPrev_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_prev"), @"Icons/icon_pos_nav_prev.png");
            ButtonNextRecord = GetNewButton("touchButtonNext_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_next"), @"Icons/icon_pos_nav_next.png");
            ButtonInsert = GetNewButton("touchButtonInsert_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_insert"), @"Icons/icon_pos_nav_new.png");
            ButtonView = GetNewButton("touchButtonView_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_view"), @"Icons/icon_pos_nav_view.png");
            ButtonUpdate = GetNewButton("touchButtonUpdate_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_update"), @"Icons/icon_pos_nav_update.png");
            ButtonDelete = GetNewButton("touchButtonDelete_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_delete"), @"Icons/icon_pos_nav_delete.png");
            ButtonRefresh = GetNewButton("touchButtonRefresh_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_refresh"), @"Icons/icon_pos_nav_refresh.png");

            ButtonPrevRecord.Clicked += delegate { _gridView.PrevRecord(); };
            ButtonNextRecord.Clicked += delegate { _gridView.NextRecord(); };

            ButtonInsert.Clicked += delegate { _gridView.Insert(); };
            ButtonView.Clicked += delegate { _gridView.Update(DialogMode.View); };
            ButtonUpdate.Clicked += delegate { _gridView.Update(); };
            ButtonDelete.Clicked += delegate { _gridView.Delete(); };
            ButtonRefresh.Clicked += delegate { _gridView.Refresh(); };

            if (_navigatorMode == GridViewNavigatorMode.Default)
            {
                ExtraSlot = new HBox(false, 0);

                hboxNavigatorButtons.PackStart(ButtonPrevRecord, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonNextRecord, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonInsert, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonView, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonUpdate, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonDelete, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonRefresh, false, false, 0);

                hboxNavigator.PackStart(TreeViewSearch, false, false, 0);

                hboxNavigator.PackStart(ExtraSlot, true, true, 0);
                hboxNavigator.PackStart(hboxNavigatorButtons, false, false, 0);
                this.PackStart(hboxNavigator);
            }
        }

        public IconButtonWithText GetNewButton(string pId, string pLabel, string pIcon)
        {
            IconButtonWithText result = null;

            string fileIcon = PathsSettings.ImagesFolderLocation + pIcon;
            string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
            Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
            Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButton;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon;

            result = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = pId,
                    BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                    Text = pLabel,
                    Font = fontBaseDialogActionAreaButton,
                    FontColor = colorBaseDialogActionAreaButtonFont,
                    Icon = fileIcon,
                    IconSize = sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                    ButtonSize = sizeBaseDialogActionAreaBackOfficeNavigatorButton
                });

            return (result);
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
            if (gridView.Model.IterNChildren() > 0 && _gridView.Entity != null)
            {
                if (ButtonView != null && !ButtonView.Sensitive && _gridView.AllowRecordView) ButtonView.Sensitive = true;
                if (ButtonUpdate != null && !ButtonUpdate.Sensitive && _gridView.AllowRecordUpdate) ButtonUpdate.Sensitive = true;
                if (ButtonDelete != null && !ButtonDelete.Sensitive && _gridView.AllowRecordDelete) ButtonDelete.Sensitive = true;
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
