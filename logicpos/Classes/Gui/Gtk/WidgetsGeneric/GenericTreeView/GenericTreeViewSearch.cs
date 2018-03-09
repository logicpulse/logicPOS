using Gtk;
using logicpos.App;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    class GenericTreeViewSearch : Box
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private Window _sourceWindow;
        private TreeView _treeView;
        private List<GenericTreeViewColumnProperty> _columnProperties;
        private Boolean _isCaseSensitivity = false;
        //UI
        private EntryBoxValidation _entryBoxSearchCriteria;
        //WIP: private TouchButtonIconWithText _buttonSearchAdvanced;
        //Public Properties
        TreeModelFilter _listStoreModelFilter;
        public TreeModelFilter ListStoreModelFilter
        {
            get { return _listStoreModelFilter; }
            set
            {
                _listStoreModelFilter = value;
                //IMPORTANT NOTE: In GenericTreeView.Refresh() we ReCreate a new REFERENCE _listStoreModelFilter with new TreeModelFilter(_listStoreModel, null)
                //And lost VisibleFunc, this way in SETTER we ReAssign IT, this way we dont lost Filter
                _listStoreModelFilter.VisibleFunc = FilterTree;
            }
        }

        //Parameterless Constructor
        public GenericTreeViewSearch() { }
        public GenericTreeViewSearch(Window pSourceWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GenericTreeViewColumnProperty> pColumnProperties)
        {
            InitObject(pSourceWindow, pTreeView, pListStoreModelFilter, pColumnProperties);
        }

        public void InitObject(Window pSourceWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GenericTreeViewColumnProperty> pColumnProperties)
        {
            //ByPass Privileges, that dont use Filter, this way we Hide Search too
            if (pListStoreModelFilter != null)
            {
                //Parameters
                _sourceWindow = pSourceWindow;
                _treeView = pTreeView;
                _listStoreModelFilter = pListStoreModelFilter;

                //Init Model
                _listStoreModelFilter.VisibleFunc = new TreeModelFilterVisibleFunc(FilterTree);
                _columnProperties = pColumnProperties;
                //Init UI
                InitUI();
            }
        }

        private void InitUI()
        {
            //Settings
            String fontBaseDialogActionAreaButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogActionAreaButton"]);
            Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
            Color colorBaseDialogActionAreaButtonFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonFont"]);
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaBackOfficeNavigatorButton"]);
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon"]);
            //WIP: String fileIconSearchAdvanced = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_search_advanced.png");
            String regexAlfaNumericExtended = SettingsApp.RegexAlfaNumericExtended;

            //SearchCriteria
            _entryBoxSearchCriteria = new EntryBoxValidation(_sourceWindow, Resx.widget_generictreeviewsearch_search_label, KeyboardMode.AlfaNumeric, regexAlfaNumericExtended, true);
            //TODO:THEME
            _entryBoxSearchCriteria.WidthRequest = (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600) ? 150 : 250;

            //_entryBoxSearchCriteria.EntryValidation.Changed += delegate { ValidateDialog(); };

            //Initialize Buttons
            //WIP: _buttonSearchAdvanced = new TouchButtonIconWithText("touchButtonSearchAdvanced_DialogActionArea", colorBaseDialogActionAreaButtonBackground, Resx.widget_generictreeviewsearch_search_advanced, fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileIconSearchAdvanced, sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Width, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Height) { Sensitive = false };

            //Pack
            HBox hbox = new HBox(false, 0);
            hbox.PackStart(_entryBoxSearchCriteria, true, true, 0);
            //WIP: hbox.PackStart(_buttonSearchAdvanced, false, false, 0);

            //Final Pack
            PackStart(hbox);

            //Check if has Searchable Fields
            Sensitive = HasSearchableFields();

            //Events
            _entryBoxSearchCriteria.EntryValidation.Changed += _entryBoxSearchCriteria_Changed;
        }

        private bool HasSearchableFields()
        {
            foreach (GenericTreeViewColumnProperty column in _columnProperties)
            {
                if (column.Searchable.Equals(NullBoolean.True) || (column.Visible.Equals(true) && column.Searchable.Equals(NullBoolean.Null))) return true;
            }
            return false;
        }

        private void _entryBoxSearchCriteria_Changed(object sender, EventArgs e)
        {
            _listStoreModelFilter.Refilter();

            //Always set Cursor position to first record on filter, else we LOST Cursor, and it can Crash on Cursor dependent CRUD methods like UPDATE/DELETE
            TreeIter treeIter;
            _listStoreModelFilter.GetIterFirst(out treeIter);
            _treeView.SetCursor(_listStoreModelFilter.GetPath(treeIter), _treeView.Columns[0], false);
            _treeView.ScrollToCell(_listStoreModelFilter.GetPath(treeIter), _treeView.Columns[0], false, 0, 0);
        }

        public bool FilterTree(TreeModel model, TreeIter iter)
        {
            int i = 0;
            string fieldValue;
            bool result = false;
            string filter = _entryBoxSearchCriteria.EntryValidation.Text;

            //Filter is Disabled, return true
            if (_entryBoxSearchCriteria.EntryValidation.Text == string.Empty) return true;

            if (!_isCaseSensitivity) filter = filter.ToUpper();

            //Loop Columns and if Any of The searchable is a valid search, returns positive result (visible)
            foreach (GenericTreeViewColumnProperty column in _columnProperties)
            {
                if (column.Searchable.Equals(NullBoolean.True) || (column.Visible.Equals(true) && column.Searchable.Equals(NullBoolean.Null)))
                {
                    //Detect empty model values, come from "_treeIterModel = _listStoreModel.AppendValues(modelValues);", 
                    //somehow when we append to Model it Redirect to ModelFilter FilterFree, ODD Effect, we dont have assign FilterFree to Model
                    //This way we prevent NULL exceptions, bypassing FilterFree
                    //Check First Column (can be 0) if first row, and Second/Third Column for Null
                    //if (model.GetValue(iter, 0).Equals(0) && model.GetValue(iter, 1) == null && model.GetValue(iter, 2) == null) return true;
                    if (i > 0 && model.GetValue(iter, i) == null) return true;

                    try
                    {
                        fieldValue = model.GetValue(iter, i).ToString();
                        if (!_isCaseSensitivity) fieldValue = fieldValue.ToUpper();
                        if (fieldValue.IndexOf(filter) > -1) result = true;
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }
                }
                ++i;
            }

            //We dont have a positive search, send false(default) to hide model rec
            return result;
        }
    }
}
