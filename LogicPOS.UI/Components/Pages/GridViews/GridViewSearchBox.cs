using Gtk;
using logicpos;
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
        public bool ShowMoreButton { get; set; } 
        public bool ShowFilterButton { get; set; } 

        private Window _parentWindow;
        private TreeView _gridView;
        private List<GridViewColumn> _columns;
        private EntryBoxValidation _entry;
        private TreeModelFilter _filter;
        public TreeModelFilter Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                _filter.VisibleFunc = FilterData;
            }
        }

        public GridViewSearchBox(Window parentWindow,
                                 TreeView gridView,
                                 TreeModelFilter filter,
                                 List<GridViewColumn> columns,
                                 bool showFilterAndMoreButtons)
        {
            InitObject(parentWindow,
                       gridView,
                       filter,
                       columns,
                       showFilterAndMoreButtons);
        }

        public void InitObject(Window parentWindow,
                               TreeView gridView,
                               TreeModelFilter filter,
                               List<GridViewColumn> columns,
                               bool showFilterAndMoreButtons)
        {

            if (filter == null)
            {
                return;
            }

            _parentWindow = parentWindow;
            _gridView = gridView;
            _filter = filter;
            _filter.VisibleFunc = new TreeModelFilterVisibleFunc(FilterData);
            _columns = columns;
            
            Design(showFilterAndMoreButtons);
        }

        private void Design(bool showFilterAndMoreButtons)
        {
            string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
            Color colorBaseDialogActionAreaButtonBackground = Color.Transparent;
            Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButton = ExpressionEvaluatorExtended.sizePosToolbarButtonSizeDefault;
            Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon = ExpressionEvaluatorExtended.sizePosToolbarButtonIconSizeDefault;
            string regexAlfaNumericExtended = RegexUtils.RegexAlfaNumericExtended;


            _entry = new EntryBoxValidation(_parentWindow,
                                            GeneralUtils.GetResourceByName("widget_generictreeviewsearch_search_label"),
                                            KeyboardMode.AlfaNumeric,
                                            regexAlfaNumericExtended,
                                            false);

            _entry.WidthRequest = GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600 ? 150 : 250;

            HBox horizontalBox = new HBox(false, 0);
            horizontalBox.PackStart(_entry, true, true, 0);
  
            PackStart(horizontalBox);

            Sensitive = HasSearchableFields();

            _entry.EntryValidation.Changed += TxtSearch_Changed;

            if (showFilterAndMoreButtons)
            {
                IconButtonWithText btnMore;
                IconButtonWithText btnFilter;

                string iconMore = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_more.png";
                string iconFilter = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_filter.png";

                btnMore = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "touchButtonSearchAdvanced_DialogActionArea",
                        BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                        Text = GeneralUtils.GetResourceByName("global_button_label_more"),
                        Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                        FontColor = colorBaseDialogActionAreaButtonFont,
                        Icon = iconMore,
                        IconSize = sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                        ButtonSize = sizeBaseDialogActionAreaBackOfficeNavigatorButton
                    })
                { Sensitive = true };

                btnFilter = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "touchButtonSearchAdvanced_DialogActionArea",
                        BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                        Text = GeneralUtils.GetResourceByName("global_button_label_filter"),
                        Font = ExpressionEvaluatorExtended.fontDocumentsSizeDefault,
                        FontColor = colorBaseDialogActionAreaButtonFont,
                        Icon = iconFilter,
                        IconSize = sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                        ButtonSize = sizeBaseDialogActionAreaBackOfficeNavigatorButton
                    })
                { Sensitive = true };

                horizontalBox.PackStart(btnMore, false, false, 0);
                horizontalBox.PackStart(btnFilter, false, false, 0);

                btnMore.Clicked += BtnMore_Clicked;
                btnFilter.Clicked += BtnFilter_Clicked;
            }

        }

        private bool HasSearchableFields()
        {
            foreach (GridViewColumn column in _columns)
            {
                if (column.Searchable.Equals(NullBoolean.True) || column.Visible.Equals(true) && column.Searchable.Equals(NullBoolean.Null)) return true;
            }
            return false;
        }

        private void TxtSearch_Changed(object sender, EventArgs e)
        {
            _filter.Refilter();

            TreeIter treeIter;
            _filter.GetIterFirst(out treeIter);
            _gridView.SetCursor(_filter.GetPath(treeIter), _gridView.Columns[0], false);
            _gridView.ScrollToCell(_filter.GetPath(treeIter), _gridView.Columns[0], false, 0, 0);

        }

        public CustomButton Button { get; set; }
        public ResponseType Response { get; set; }
        public event EventHandler Clicked;
        private void ActionAreaButton_Clicked(object sender, EventArgs e)
        {
            //Send this and Not sender, to catch base object
            Clicked?.Invoke(this, e);
        }
        public void BtnMore_Clicked(object sender, EventArgs e)
        {
            TreeIter iterator;
            _filter.GetIterFirst(out iterator);
            _gridView.SetCursor(_filter.GetPath(iterator), _gridView.Columns[0], false);
            ShowMoreButton = false;
        }
        public void BtnFilter_Clicked(object sender, EventArgs e)
        {
            ShowFilterButton = true;
            TreeIter treeIter;

            _filter.GetIterFirst(out treeIter);
            _gridView.SetCursor(_filter.GetPath(treeIter), _gridView.Columns[0], false);
            ShowFilterButton = false;
        }
        public bool FilterData(TreeModel model, TreeIter iterator)
        {
            int i = 0;
            string fieldValue;
            bool result = false;
            string filter = _entry.EntryValidation.Text;

            if (_entry.EntryValidation.Text == string.Empty) return true;

            filter = filter.ToUpper();

            foreach (GridViewColumn column in _columns)
            {
                if (column.Searchable.Equals(NullBoolean.True) || column.Visible.Equals(true) && column.Searchable.Equals(NullBoolean.Null))
                {

                    if (i > 0 && model.GetValue(iterator, i) == null) return true;

                    fieldValue = model.GetValue(iterator, i).ToString();
                    fieldValue = fieldValue.ToUpper();
                    if (fieldValue.IndexOf(filter) > -1) result = true;

                }
                ++i;
            }

            return result;
        }
    }
}
