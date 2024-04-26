using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.Extensions;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    internal class GenericTreeViewSearch : Box
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool flagMore = false;
        public bool flagFilter = false;

        //private ResponseType _responseTypeLoadMoreDocuments = (ResponseType)80;

        //Private Members
        private Window _sourceWindow;
        private TreeView _treeView;
        private List<GenericTreeViewColumnProperty> _columnProperties;
        private readonly bool _isCaseSensitivity = false;
        //UI
        private EntryBoxValidation _entryBoxSearchCriteria;

        //WIP: private TouchButtonIconWithText _buttonSearchAdvanced;


        //Public Properties
        private TreeModelFilter _listStoreModelFilter;
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

        public GenericTreeViewSearch(Window pSourceWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GenericTreeViewColumnProperty> pColumnProperties, bool showFilterAndMoreButtons)
        {
            InitObject(pSourceWindow, pTreeView, pListStoreModelFilter, pColumnProperties, showFilterAndMoreButtons);
        }

        public void InitObject(Window pSourceWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GenericTreeViewColumnProperty> pColumnProperties)
        {
            InitObject(pSourceWindow, pTreeView, pListStoreModelFilter, pColumnProperties, false);
        }

        public void InitObject(Window pSourceWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GenericTreeViewColumnProperty> pColumnProperties, bool showFilterAndMoreButtons)
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
                InitUI(showFilterAndMoreButtons);
            }
        }

        private void InitUI(bool showFilterAndMoreButtons)
        {
            //Settings
            string fontBaseDialogActionAreaButton = SharedUtils.OSSlash(DataLayerFramework.Settings["fontBaseDialogActionAreaButton"]);
            Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
            Color colorBaseDialogActionAreaButtonFont = DataLayerFramework.Settings["colorBaseDialogActionAreaButtonFont"].StringToColor();
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = ExpressionEvaluatorExtended.sizePosToolbarButtonSizeDefault;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = ExpressionEvaluatorExtended.sizePosToolbarButtonIconSizeDefault;
            //WIP: String fileIconSearchAdvanced = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\icon_pos_search_advanced.png");

            string regexAlfaNumericExtended = SharedSettings.RegexAlfaNumericExtended;

            //SearchCriteria
            _entryBoxSearchCriteria = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewsearch_search_label"), KeyboardMode.AlfaNumeric, regexAlfaNumericExtended, false);
            //TODO:THEME
            _entryBoxSearchCriteria.WidthRequest = (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600) ? 150 : 250;

            //_entryBoxSearchCriteria.EntryValidation.Changed += delegate { ValidateDialog(); };

            //Initialize Buttons
            //WIP: _buttonSearchAdvanced = new TouchButtonIconWithText("touchButtonSearchAdvanced_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewsearch_search_advanced, fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileIconSearchAdvanced, sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Width, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Height) { Sensitive = false };



            //TouchButtonIconWithText _buttonFilter = TouchButtonIconWithText(ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Filter, "touchButtonMore_Green", string.Format(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_button_label_filter, SettingsApp.PaginationRowsPerPage), fileActionFilter);

            //ActionAreaButtons actionAreaButtons = new ActionAreaButtons();

            //actionAreaButtons.Add(new ActionAreaButton(_buttonMore, _responseTypeLoadMoreDocuments));


            //Pack
            HBox hbox = new HBox(false, 0);
            hbox.PackStart(_entryBoxSearchCriteria, true, true, 0);
            //WIP:hbox.PackStart(_buttonSearchAdvanced, false, false, 0);

            //Final Pack
            PackStart(hbox);

            //Check if has Searchable Fields
            Sensitive = HasSearchableFields();

            //Events
            _entryBoxSearchCriteria.EntryValidation.Changed += _entryBoxSearchCriteria_Changed;

            //if ("Base Dialog Window".Equals(_sourceWindow.Title))
            if (showFilterAndMoreButtons)
            {

                TouchButtonIconWithText buttonMore;
                TouchButtonIconWithText buttonFilter;

                string fileActionMore = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\icon_pos_more.png");
                string fileActionFilter = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\icon_pos_filter.png");
                buttonMore = new TouchButtonIconWithText("touchButtonSearchAdvanced_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_button_label_more"), ExpressionEvaluatorExtended.fontDocumentsSizeDefault, colorBaseDialogActionAreaButtonFont, fileActionMore, sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Width, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Height) { Sensitive = true };
                buttonFilter = new TouchButtonIconWithText("touchButtonSearchAdvanced_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_button_label_filter"), ExpressionEvaluatorExtended.fontDocumentsSizeDefault, colorBaseDialogActionAreaButtonFont, fileActionFilter, sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Width, sizeBaseDialogActionAreaBackOfficeNavigatorButton.Height) { Sensitive = true };

                hbox.PackStart(buttonMore, false, false, 0);
                hbox.PackStart(buttonFilter, false, false, 0);

                buttonMore.Clicked += _genericTreeViewSearch_ButtonMoreClicked;
                buttonFilter.Clicked += _genericTreeViewSearch_ButtonFilterClicked;
            }

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

        public TouchButtonBase Button { get; set; }
        public ResponseType Response { get; set; }

        //Events
        public event EventHandler Clicked;

        private void ActionAreaButton_Clicked(object sender, EventArgs e)
        {
            //Send this and Not sender, to catch base object
            Clicked?.Invoke(this, e);
        }

        /// <summary>
        /// Button More Clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _genericTreeViewSearch_ButtonMoreClicked(object sender, EventArgs e)
        {

            flagMore = true;
            Button_MoreResponse();
            TreeIter treeIter;

            _listStoreModelFilter.GetIterFirst(out treeIter);
            _treeView.SetCursor(_listStoreModelFilter.GetPath(treeIter), _treeView.Columns[0], false);
            //_treeView.ScrollToCell(_listStoreModelFilter.GetPath(treeIter), _treeView.Columns[0], false, 0, 0);
            flagMore = false;
            Button_MoreResponse();
        }

        /// <summary>
        /// Button Filter Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _genericTreeViewSearch_ButtonFilterClicked(object sender, EventArgs e)
        {

            flagFilter = true;
            Button_FilterResponse();
            TreeIter treeIter;

            _listStoreModelFilter.GetIterFirst(out treeIter);
            _treeView.SetCursor(_listStoreModelFilter.GetPath(treeIter), _treeView.Columns[0], false);
            //_treeView.ScrollToCell(_listStoreModelFilter.GetPath(treeIter), _treeView.Columns[0], false, 0, 0);
            flagFilter = false;
            Button_MoreResponse();
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
                        _logger.Error(ex.Message, ex);
                    }
                }
                ++i;
            }

            //We dont have a positive search, send false(default) to hide model rec
            return result;
        }
        /// <summary>
        /// Activate button more results
        /// </summary>
        /// <returns></returns>
        public bool Button_MoreResponse()
        {
            return this.flagMore;
        }
        /// <summary>
        /// Activate button filter results
        /// </summary>
        /// <returns></returns>
        public bool Button_FilterResponse()
        {
            return this.flagFilter;
        }


    }
}
