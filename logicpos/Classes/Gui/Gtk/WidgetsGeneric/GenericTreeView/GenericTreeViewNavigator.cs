using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    class GenericTreeViewNavigator<T1, T2> : Box
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private Window _sourceWindow;
        private GenericTreeView<T1, T2> _genericTreeView;
        private GenericTreeViewNavigatorMode _navigatorMode;

        //Public Properties
        private HBox _hboxExtraSlot;
        public HBox ExtraSlot
        {
            get { return _hboxExtraSlot; }
            set { _hboxExtraSlot = value; }
        }

        private TouchButtonIconWithText _buttonFirstPage;
        public TouchButtonIconWithText ButtonFirstPage
        {
            get { return _buttonFirstPage; }
            set { _buttonFirstPage = value; }
        }

        private TouchButtonIconWithText _buttonPrevPage;
        public TouchButtonIconWithText ButtonPrevPage
        {
            get { return _buttonPrevPage; }
            set { _buttonPrevPage = value; }
        }

        private TouchButtonIconWithText _buttonNextPage;
        public TouchButtonIconWithText ButtonNextPage
        {
            get { return _buttonNextPage; }
            set { _buttonNextPage = value; }
        }

        private TouchButtonIconWithText _buttonLastPage;
        public TouchButtonIconWithText ButtonLastPage
        {
            get { return _buttonLastPage; }
            set { _buttonLastPage = value; }
        }

        private TouchButtonIconWithText _buttonPrevRecord;
        public TouchButtonIconWithText ButtonPrevRecord
        {
            get { return _buttonPrevRecord; }
            set { _buttonPrevRecord = value; }
        }

        private TouchButtonIconWithText _buttonNextRecord;
        public TouchButtonIconWithText ButtonNextRecord
        {
            get { return _buttonNextRecord; }
            set { _buttonNextRecord = value; }
        }

        private TouchButtonIconWithText _buttonInsert;
        public TouchButtonIconWithText ButtonInsert
        {
            get { return _buttonInsert; }
            set { _buttonInsert = value; }
        }

        private TouchButtonIconWithText _buttonView;
        public TouchButtonIconWithText ButtonView
        {
            get { return _buttonView; }
            set { _buttonView = value; }
        }

        private TouchButtonIconWithText _buttonUpdate;
        public TouchButtonIconWithText ButtonUpdate
        {
            get { return _buttonUpdate; }
            set { _buttonUpdate = value; }
        }

        private TouchButtonIconWithText _buttonDelete;
        public TouchButtonIconWithText ButtonDelete
        {
            get { return _buttonDelete; }
            set { _buttonDelete = value; }
        }

        private TouchButtonIconWithText _buttonRefresh;
        public TouchButtonIconWithText ButtonRefresh
        {
            get { return _buttonRefresh; }
            set { _buttonRefresh = value; }
        }

        //private TouchButtonIconWithText _buttonClose;
        //public TouchButtonIconWithText ButtonClose
        //{
        //  get { return _buttonClose; }
        //  set { _buttonClose = value; }
        //}

        private GenericTreeViewSearch _genericTreeViewSearch;
        //private GenericTreeViewSearch _genericTreeViewSearchWithButtons;
        public GenericTreeViewSearch TreeViewSearch
        {
            get { return _genericTreeViewSearch; }
            set { _genericTreeViewSearch = value; }
        }

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
            _genericTreeViewSearch = new GenericTreeViewSearch(_sourceWindow, _genericTreeView.TreeView, _genericTreeView.ListStoreModelFilter, _genericTreeView.Columns, buttonMoreFilterVisible);
            // Help to Debug some Kind of Types
            //if (_genericTreeView.GetType().Equals(typeof(TreeViewConfigurationPreferenceParameter)))
            //{
            //    _log.Debug($"BREAK {typeof(TreeViewConfigurationPreferenceParameter)}");
            //}

            //Initialize Buttons     
            _buttonPrevRecord = GetNewButton("touchButtonPrev_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_record_prev"), @"Icons/icon_pos_nav_prev.png");
            _buttonNextRecord = GetNewButton("touchButtonNext_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_record_next"), @"Icons/icon_pos_nav_next.png");
            _buttonInsert = GetNewButton("touchButtonInsert_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_insert"), @"Icons/icon_pos_nav_new.png");
            _buttonView = GetNewButton("touchButtonView_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_view"), @"Icons/icon_pos_nav_view.png");
            _buttonUpdate = GetNewButton("touchButtonUpdate_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_update"), @"Icons/icon_pos_nav_update.png");
            _buttonDelete = GetNewButton("touchButtonDelete_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_delete"), @"Icons/icon_pos_nav_delete.png");
            _buttonRefresh = GetNewButton("touchButtonRefresh_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_refresh"), @"Icons/icon_pos_nav_refresh.png");
            //_buttonClose = GetNewButton("touchButtonPosToolbarApplicationClose_Red", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_pos, @"Icons/icon_pos_toolbar_application_close.png");

            //Events
            //GenericTreeView : Shared
            _buttonPrevRecord.Clicked += delegate { _genericTreeView.PrevRecord(); };
            _buttonNextRecord.Clicked += delegate { _genericTreeView.NextRecord(); };
            //GenericTreeView Overrides : Must Be Override by IGenericTreeView
            _buttonInsert.Clicked += delegate { _genericTreeView.Insert(); };
            _buttonView.Clicked += delegate { _genericTreeView.Update(DialogMode.View); };
            _buttonUpdate.Clicked += delegate { _genericTreeView.Update(); };
            _buttonDelete.Clicked += delegate { _genericTreeView.Delete(); };
            _buttonRefresh.Clicked += delegate { _genericTreeView.Refresh(); };
            //_buttonClose.Clicked += delegate { GlobalApp.WindowBackOffice.Hide(); GlobalApp.WindowPos.ShowAll(); };

            //Pack Only if Default Navigator
            if (_navigatorMode == GenericTreeViewNavigatorMode.Default)
            {
                //Init ExtraSlot
                _hboxExtraSlot = new HBox(false, 0);
                //Pack NavButtons
                hboxNavigatorButtons.PackStart(_buttonPrevRecord, false, false, 0);
                hboxNavigatorButtons.PackStart(_buttonNextRecord, false, false, 0);
                hboxNavigatorButtons.PackStart(_buttonInsert, false, false, 0);
                //OnlyPack View Button if is BackOffice BOBaseDialog, (Generic TreeView with CrudWidgetList, to Loop Fields and Assign Sensitive=True to All)
                if (_genericTreeView.DialogType != null && _genericTreeView.DialogType.BaseType == typeof(BOBaseDialog))
                {
                    hboxNavigatorButtons.PackStart(_buttonView, false, false, 0);
                }
                hboxNavigatorButtons.PackStart(_buttonUpdate, false, false, 0);
                hboxNavigatorButtons.PackStart(_buttonDelete, false, false, 0);
                hboxNavigatorButtons.PackStart(_buttonRefresh, false, false, 0);
                //hboxNav.PackStart(_buttonClose, false, false, 0);
                //Pack Final Hbox
                hboxNavigator.PackStart(_genericTreeViewSearch, false, false, 0);
                //hboxNavigator.PackStart(_genericTreeViewSearchWithButtons, false, false, 0);
                hboxNavigator.PackStart(_hboxExtraSlot, true, true, 0);
                hboxNavigator.PackStart(hboxNavigatorButtons, false, false, 0);
                this.PackStart(hboxNavigator);
            }
        }

        public TouchButtonIconWithText GetNewButton(string pId, string pLabel, string pIcon)
        {
            TouchButtonIconWithText result = null;
            try
            {
                String fileIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + pIcon);
                String fontBaseDialogActionAreaButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogActionAreaButton"]);
                Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
                Color colorBaseDialogActionAreaButtonFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonFont"]);
                Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaBackOfficeNavigatorButton"]);
                Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon"]);

                result = new TouchButtonIconWithText(pId, colorBaseDialogActionAreaButtonBackground, pLabel, fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileIcon, sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Width, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Height);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                    if (_buttonFirstPage != null && _buttonFirstPage.Sensitive) _buttonFirstPage.Sensitive = false;
                    if (_buttonPrevPage != null && _buttonPrevPage.Sensitive) _buttonPrevPage.Sensitive = false;
                }
                else
                {
                    if (_buttonFirstPage != null && !_buttonFirstPage.Sensitive) _buttonFirstPage.Sensitive = true;
                    if (_buttonPrevPage != null && !_buttonPrevPage.Sensitive) _buttonPrevPage.Sensitive = true;
                };
                //NextPage/LastPage
                if (CurrentPage == TotalPages || TotalRecords == 0)
                {
                    if (_buttonNextPage != null && _buttonNextPage.Sensitive) _buttonNextPage.Sensitive = false;
                    if (_buttonLastPage != null && _buttonLastPage.Sensitive) _buttonLastPage.Sensitive = false;
                }
                else
                {
                    if (_buttonNextPage != null && !_buttonNextPage.Sensitive) _buttonNextPage.Sensitive = true;
                    if (_buttonLastPage != null && !_buttonLastPage.Sensitive) _buttonLastPage.Sensitive = true;
                }
                //PrevRecord
                if (CurrentRecord == 0 || TotalRecords == 0)
                {
                    if (_buttonPrevRecord != null && _buttonPrevRecord.Sensitive) _buttonPrevRecord.Sensitive = false;
                }
                else
                {
                    if (_buttonPrevRecord != null && !_buttonPrevRecord.Sensitive) _buttonPrevRecord.Sensitive = true;
                };
                //NextRecord
                if (CurrentRecord == TotalRecords || TotalRecords < 1 || TotalRecords == 0)
                {
                    if (_buttonNextRecord != null && _buttonNextRecord.Sensitive) _buttonNextRecord.Sensitive = false;
                }
                else
                {
                    if (_buttonNextRecord != null && !_buttonNextRecord.Sensitive) _buttonNextRecord.Sensitive = true;
                };

                //View/Update/Delete
                if (pTreeView.Model.IterNChildren() > 0 && _genericTreeView.DataSourceRow != null)
                {
                    if (_buttonView != null && !_buttonView.Sensitive && _genericTreeView.AllowRecordView) _buttonView.Sensitive = true;
                    if (_buttonUpdate != null && !_buttonUpdate.Sensitive && _genericTreeView.AllowRecordUpdate) _buttonUpdate.Sensitive = true;
                    if (_buttonDelete != null && !_buttonDelete.Sensitive && _genericTreeView.AllowRecordDelete) _buttonDelete.Sensitive = true;
                }
                else
                {
                    
                    if (_buttonView != null && _buttonView.Sensitive) _buttonView.Sensitive = false;
                    if (_buttonUpdate != null && _buttonUpdate.Sensitive) _buttonUpdate.Sensitive = false;
                    if (_buttonDelete != null && _buttonDelete.Sensitive) _buttonDelete.Sensitive = false;
                };
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
