using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Extensions;
using System;
using System.Drawing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Utility;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    internal class GenericTreeViewNavigator<T1, T2> : Box
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private readonly Window _sourceWindow;
        private readonly GenericTreeView<T1, T2> _genericTreeView;
        private readonly GenericTreeViewNavigatorMode _navigatorMode;

        public HBox ExtraSlot { get; set; }

        public TouchButtonIconWithText ButtonFirstPage { get; set; }

        public TouchButtonIconWithText ButtonPrevPage { get; set; }

        public TouchButtonIconWithText ButtonNextPage { get; set; }

        public TouchButtonIconWithText ButtonLastPage { get; set; }

        public TouchButtonIconWithText ButtonPrevRecord { get; set; }

        public TouchButtonIconWithText ButtonNextRecord { get; set; }

        public TouchButtonIconWithText ButtonInsert { get; set; }

        public TouchButtonIconWithText ButtonView { get; set; }

        public TouchButtonIconWithText ButtonUpdate { get; set; }

        public TouchButtonIconWithText ButtonDelete { get; set; }

        public TouchButtonIconWithText ButtonRefresh { get; set; }

        //private GenericTreeViewSearch _genericTreeViewSearchWithButtons;
        public GenericTreeViewSearch TreeViewSearch { get; set; }

        //public GenericTreeViewSearch TreeViewSearchWithButtons
        //{
        //    get { return _genericTreeViewSearchWithButtons; }
        //    set { _genericTreeViewSearchWithButtons = value; }
        //}



        //public Properties
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentRecord { get; set; }
        public int TotalRecords { get; set; }

        //<T1 (XPCollection|DataTable) ,T2 (XPGuidObject|DataRow)>
        public GenericTreeViewNavigator(Window pSourceWindow, GenericTreeView<T1, T2> pGenericTreeView, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _genericTreeView = pGenericTreeView;
            _navigatorMode = pGenericTreeViewNavigatorMode;

            //Init
            CurrentPage = 1;

            this.HeightRequest = 60;

            //Init Navigator Buttons
            InitNavigator();
        }

        private void InitNavigator()
        {
            //Container For Search, ExtraButtons and Navigator Buttons
            HBox hboxNavigator = new HBox(false, 0);
            HBox hboxNavigatorButtons = new HBox(true, 0);

            string name = _genericTreeView.Toplevel.ToString();
            bool buttonMoreFilterVisible = false;
            if (name == "logicpos.Classes.Gui.Gtk.BackOffice.TreeViewDocumentFinanceMaster" || name == "logicpos.Classes.Gui.Gtk.BackOffice.TreeViewDocumentFinancePayment") { buttonMoreFilterVisible = true; }

            //Initialize GenericTreeViewSearch
            //_genericTreeViewSearch = new GenericTreeViewSearch(_sourceWindow, _genericTreeView.TreeView, _genericTreeView.ListStoreModelFilter, _genericTreeView.Columns);
            TreeViewSearch = new GenericTreeViewSearch(_sourceWindow, _genericTreeView.TreeView, _genericTreeView.ListStoreModelFilter, _genericTreeView.Columns, buttonMoreFilterVisible);
            // Help to Debug some Kind of Types
            //if (_genericTreeView.GetType().Equals(typeof(TreeViewConfigurationPreferenceParameter)))
            //{
            //    _logger.Debug($"BREAK {typeof(TreeViewConfigurationPreferenceParameter)}");
            //}

            //Initialize Buttons     
            ButtonPrevRecord = GetNewButton("touchButtonPrev_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_prev"), @"Icons/icon_pos_nav_prev.png");
            ButtonNextRecord = GetNewButton("touchButtonNext_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_record_next"), @"Icons/icon_pos_nav_next.png");
            ButtonInsert = GetNewButton("touchButtonInsert_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_insert"), @"Icons/icon_pos_nav_new.png");
            ButtonView = GetNewButton("touchButtonView_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_view"), @"Icons/icon_pos_nav_view.png");
            ButtonUpdate = GetNewButton("touchButtonUpdate_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_update"), @"Icons/icon_pos_nav_update.png");
            ButtonDelete = GetNewButton("touchButtonDelete_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_delete"), @"Icons/icon_pos_nav_delete.png");
            ButtonRefresh = GetNewButton("touchButtonRefresh_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_refresh"), @"Icons/icon_pos_nav_refresh.png");
            //_buttonClose = GetNewButton("touchButtonPosToolbarApplicationClose_Red", CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pos, @"Icons/icon_pos_toolbar_application_close.png");

            //Events
            //GenericTreeView : Shared
            ButtonPrevRecord.Clicked += delegate { _genericTreeView.PrevRecord(); };
            ButtonNextRecord.Clicked += delegate { _genericTreeView.NextRecord(); };
            //GenericTreeView Overrides : Must Be Override by IGenericTreeView
            ButtonInsert.Clicked += delegate { _genericTreeView.Insert(); };
            ButtonView.Clicked += delegate { _genericTreeView.Update(DialogMode.View); };
            ButtonUpdate.Clicked += delegate { _genericTreeView.Update(); };
            ButtonDelete.Clicked += delegate { _genericTreeView.Delete(); };
            ButtonRefresh.Clicked += delegate { _genericTreeView.Refresh(); };
            //_buttonClose.Clicked += delegate { GlobalApp.WindowBackOffice.Hide(); GlobalApp.WindowPos.ShowAll(); };

            //Pack Only if Default Navigator
            if (_navigatorMode == GenericTreeViewNavigatorMode.Default)
            {
                //Init ExtraSlot
                ExtraSlot = new HBox(false, 0);
                //Pack NavButtons
                hboxNavigatorButtons.PackStart(ButtonPrevRecord, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonNextRecord, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonInsert, false, false, 0);
                //OnlyPack View Button if is BackOffice BOBaseDialog, (Generic TreeView with CrudWidgetList, to Loop Fields and Assign Sensitive=True to All)
                if (_genericTreeView.DialogType != null && _genericTreeView.DialogType.BaseType == typeof(BOBaseDialog))
                {
                    hboxNavigatorButtons.PackStart(ButtonView, false, false, 0);
                }
                hboxNavigatorButtons.PackStart(ButtonUpdate, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonDelete, false, false, 0);
                hboxNavigatorButtons.PackStart(ButtonRefresh, false, false, 0);
                //hboxNav.PackStart(_buttonClose, false, false, 0);
                //Pack Final Hbox
                hboxNavigator.PackStart(TreeViewSearch, false, false, 0);
                //hboxNavigator.PackStart(_genericTreeViewSearchWithButtons, false, false, 0);
                hboxNavigator.PackStart(ExtraSlot, true, true, 0);
                hboxNavigator.PackStart(hboxNavigatorButtons, false, false, 0);
                this.PackStart(hboxNavigator);
            }
        }

        public TouchButtonIconWithText GetNewButton(string pId, string pLabel, string pIcon)
        {
            TouchButtonIconWithText result = null;
            try
            {
                string fileIcon = PathsSettings.ImagesFolderLocation + pIcon;
                string fontBaseDialogActionAreaButton = GeneralSettings.Settings["fontBaseDialogActionAreaButton"];
                Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
                Color colorBaseDialogActionAreaButtonFont = GeneralSettings.Settings["colorBaseDialogActionAreaButtonFont"].StringToColor();
                Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = logicpos.Utils.StringToSize(GeneralSettings.Settings["sizeBaseDialogActionAreaBackOfficeNavigatorButton"]);
                Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = logicpos.Utils.StringToSize(GeneralSettings.Settings["sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon"]);

                result = new TouchButtonIconWithText(pId, colorBaseDialogActionAreaButtonBackground, pLabel, fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileIcon, sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Width, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Height);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return (result);
        }

        public void UpdateButtons(TreeView pTreeView)
        {
            try
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
                if (pTreeView.Model.IterNChildren() > 0 && _genericTreeView.DataSourceRow != null)
                {
                    if (ButtonView != null && !ButtonView.Sensitive && _genericTreeView.AllowRecordView) ButtonView.Sensitive = true;
                    if (ButtonUpdate != null && !ButtonUpdate.Sensitive && _genericTreeView.AllowRecordUpdate) ButtonUpdate.Sensitive = true;
                    if (ButtonDelete != null && !ButtonDelete.Sensitive && _genericTreeView.AllowRecordDelete) ButtonDelete.Sensitive = true;
                }
                else
                {

                    if (ButtonView != null && ButtonView.Sensitive) ButtonView.Sensitive = false;
                    if (ButtonUpdate != null && ButtonUpdate.Sensitive) ButtonUpdate.Sensitive = false;
                    if (ButtonDelete != null && ButtonDelete.Sensitive) ButtonDelete.Sensitive = false;
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
