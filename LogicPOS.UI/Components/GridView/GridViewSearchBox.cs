using Gtk;
using logicpos;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components
{
    internal class GridViewSearchBox : Box
    {
        public bool flagMore = false;
        public bool flagFilter = false;
        private Window _sourceWindow;
        private TreeView _treeView;
        private List<GridViewColumnProperty> _columnProperties;
        private readonly bool _isCaseSensitivity = false;
        private EntryBoxValidation _entryBoxSearchCriteria;
        private TreeModelFilter _listStoreModelFilter;
        public TreeModelFilter ListStoreModelFilter
        {
            get { return _listStoreModelFilter; }
            set
            {
                _listStoreModelFilter = value;
                _listStoreModelFilter.VisibleFunc = FilterTree;
            }
        }

        public GridViewSearchBox() { }
        public GridViewSearchBox(Window parentWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GridViewColumnProperty> pColumnProperties)
        {
            InitObject(parentWindow, pTreeView, pListStoreModelFilter, pColumnProperties);
        }

        public GridViewSearchBox(Window parentWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GridViewColumnProperty> pColumnProperties, bool showFilterAndMoreButtons)
        {
            InitObject(parentWindow, pTreeView, pListStoreModelFilter, pColumnProperties, showFilterAndMoreButtons);
        }

        public void InitObject(Window parentWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GridViewColumnProperty> pColumnProperties)
        {
            InitObject(parentWindow, pTreeView, pListStoreModelFilter, pColumnProperties, false);
        }

        public void InitObject(Window parentWindow, TreeView pTreeView, TreeModelFilter pListStoreModelFilter, List<GridViewColumnProperty> pColumnProperties, bool showFilterAndMoreButtons)
        {
            //ByPass Privileges, that dont use Filter, this way we Hide Search too
            if (pListStoreModelFilter != null)
            {
                //Parameters
                _sourceWindow = parentWindow;
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
            string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
            Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
            Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = ExpressionEvaluatorExtended.sizePosToolbarButtonSizeDefault;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = ExpressionEvaluatorExtended.sizePosToolbarButtonIconSizeDefault;
            //WIP: String fileIconSearchAdvanced = SharedUtils.OSSlash(GeneralSettings.Path["images"] + @"Icons\icon_pos_search_advanced.png");

            string regexAlfaNumericExtended = RegexUtils.RegexAlfaNumericExtended;

            //SearchCriteria
            _entryBoxSearchCriteria = new EntryBoxValidation(_sourceWindow, GeneralUtils.GetResourceByName("widget_generictreeviewsearch_search_label"), KeyboardMode.AlfaNumeric, regexAlfaNumericExtended, false);
            //TODO:THEME
            _entryBoxSearchCriteria.WidthRequest = GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600 ? 150 : 250;

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

                IconButtonWithText buttonMore;
                IconButtonWithText buttonFilter;

                string fileActionMore = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_more.png";
                string fileActionFilter = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_filter.png";
                buttonMore = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "touchButtonSearchAdvanced_DialogActionArea",
                        BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                        Text = GeneralUtils.GetResourceByName("global_button_label_more"),
                        Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                        FontColor = colorBaseDialogActionAreaButtonFont,
                        Icon = fileActionMore,
                        IconSize = sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                        ButtonSize = sizeBaseDialogActionAreaBackOfficeNavigatorButton
                    })
                { Sensitive = true };

                buttonFilter = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "touchButtonSearchAdvanced_DialogActionArea",
                        BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                        Text = GeneralUtils.GetResourceByName("global_button_label_filter"),
                        Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                        FontColor = colorBaseDialogActionAreaButtonFont,
                        Icon = fileActionFilter,
                        IconSize = sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                        ButtonSize = sizeBaseDialogActionAreaBackOfficeNavigatorButton
                    })
                { Sensitive = true };

                hbox.PackStart(buttonMore, false, false, 0);
                hbox.PackStart(buttonFilter, false, false, 0);

                buttonMore.Clicked += _genericTreeViewSearch_ButtonMoreClicked;
                buttonFilter.Clicked += _genericTreeViewSearch_ButtonFilterClicked;
            }

        }

        private bool HasSearchableFields()
        {
            foreach (GridViewColumnProperty column in _columnProperties)
            {
                if (column.Searchable.Equals(NullBoolean.True) || column.Visible.Equals(true) && column.Searchable.Equals(NullBoolean.Null)) return true;
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

        public CustomButton Button { get; set; }
        public ResponseType Response { get; set; }
        public event EventHandler Clicked;
        private void ActionAreaButton_Clicked(object sender, EventArgs e)
        {
            //Send this and Not sender, to catch base object
            Clicked?.Invoke(this, e);
        }
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

            if (_entryBoxSearchCriteria.EntryValidation.Text == string.Empty) return true;

            if (!_isCaseSensitivity) filter = filter.ToUpper();

            foreach (GridViewColumnProperty column in _columnProperties)
            {
                if (column.Searchable.Equals(NullBoolean.True) || column.Visible.Equals(true) && column.Searchable.Equals(NullBoolean.Null))
                {

                    if (i > 0 && model.GetValue(iter, i) == null) return true;

                    fieldValue = model.GetValue(iter, i).ToString();
                    if (!_isCaseSensitivity) fieldValue = fieldValue.ToUpper();
                    if (fieldValue.IndexOf(filter) > -1) result = true;

                }
                ++i;
            }

            return result;
        }
        public bool Button_MoreResponse()
        {
            return flagMore;
        }
        public bool Button_FilterResponse()
        {
            return flagFilter;
        }
    }
}
