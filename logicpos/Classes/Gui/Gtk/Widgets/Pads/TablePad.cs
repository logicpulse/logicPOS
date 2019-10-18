using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class TablePad : Table
    {
        //Log4Net
        protected log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Settings
        private int _posBaseButtonScrollerHeight = Convert.ToInt32(GlobalFramework.Settings["posBaseButtonScrollerHeight"]);
        private int _posBaseButtonMaxCharsPerLabel = Convert.ToInt16(GlobalFramework.Settings["posBaseButtonMaxCharsPerLabel"]);
        protected int _fontPosBaseButtonSize = Convert.ToInt16(GlobalFramework.Settings["fontPosBaseButtonSize"]);
        //Paths/Files
        protected string _fileBaseButtonOverlay = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Buttons\Pos\button_overlay.png");
        //TouchButton List        
        private List<TouchButtonBase> _listButtons;
        //TouchButton Properties
        protected string _strButtonName;
        protected string _strButtonLabel;
        protected string _strButtonImage;
        //Mode
        private bool _toggleMode;
        //Initial Active Button Guid, From Parameters
        Guid _initialActiveButtonOid;
        //Pagging
        private int _totalItems;
        private int _totalPages;
        private int _currentPage;
        private int _itemsPerPage;
        //Buttons Scrollers
        private TouchButtonBase _buttonScrollPrev;
        private TouchButtonBase _buttonScrollNext;
        //Constructor Parameters
        private uint _rows;
        private uint _columns;
        private String _buttonNamePrefix;
        protected Color _colorButton;
        protected int _buttonWidth;
        protected int _buttonHeight;
        //DataSet
        protected Dictionary<string, int> _fieldIndex;
        protected SelectStatementResultRow _resultRow;
        //Public Properties
        private Window _sourceWindow;
        public Window SourceWindow
        {
            get { return (_sourceWindow); }
            set { _sourceWindow = value; }
        }
        private Guid _selectedButtonOid;
        public Guid SelectedButtonOid
        {
            get { return _selectedButtonOid; }
            set { _selectedButtonOid = value; }
        }
        private TouchButtonBase _selectedButton;
        public TouchButtonBase SelectedButton
        {
            get { return _selectedButton; }
            set { _selectedButton = value; }
        }
        private String _sql;
        public String Sql
        {
            get { return _sql; }
            set
            {
                _sql = value;
                UpdateSql();
            }
        }
        private String _filter;
        public String Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                UpdateSql();
            }
        }
        private String _order;
        public String Order
        {
            get { return _order; }
            set { _order = value; }
        }

        //Declare public Button Event, to Expose Button Clicks
        public event EventHandler Clicked;

        public TablePad(String pSql, String pOrder, String pFilter, Guid pActiveButtonOid, bool pToggleMode, uint pRows, uint pColumns, String pButtonNamePrefix, Color pColorButton, int pButtonWidth, int pButtonHeight, TouchButtonBase buttonPrev, TouchButtonBase buttonNext)
            : base(pRows, pColumns, true)
        {
            //_log.Debug(string.Format("TablePad():{0}pSql: [{1}]{0}pOrder: [{2}]{0}pFilter: [{3}]", Environment.NewLine, pSql, pOrder, pFilter));

            //Assign parameters to fields/properties
            _sql = pSql;
            _order = " " + pOrder;
            _filter = " " + pFilter;
            _initialActiveButtonOid = pActiveButtonOid;
            _toggleMode = pToggleMode;
            _rows = pRows;
            _columns = pColumns;
            _buttonNamePrefix = pButtonNamePrefix;
            _colorButton = pColorButton;
            _buttonWidth = pButtonWidth;
            _buttonHeight = pButtonHeight;
            _buttonScrollPrev = buttonPrev;
            _buttonScrollNext = buttonNext;
            _buttonScrollPrev.Sensitive = false;
            _buttonScrollNext.Sensitive = false;

            //Create List
            _listButtons = new List<TouchButtonBase>();

            //Signals/events
            _buttonScrollPrev.Clicked += _buttonScrollPrev_Clicked;
            _buttonScrollNext.Clicked += _buttonScrollNext_Clicked;

            UpdateSql();
        }

        public void UpdateSql()
        {
            try
            {
                bool useImageOverlay = Convert.ToBoolean(GlobalFramework.Settings["useImageOverlay"]);
                if (!useImageOverlay) _fileBaseButtonOverlay = null;

                //When update always set page 1, start page
                _currentPage = 1;
                //Store Total Childs 
                bool _hasChilds = false;

                //Local Vars
                String executeSql;
                TouchButtonBase buttonCurrent = null;

                //Reset CurrentButtonOid, Or Assign it to initialActiveButtonOid if Defined in TablePad Constructor
                _selectedButtonOid = (_initialActiveButtonOid != Guid.Empty) ? _initialActiveButtonOid : Guid.Empty;

                //Prepare executeSql for first time
                if (_filter != string.Empty) { executeSql = _sql + _filter; } else { executeSql = _sql; };
                if (_order != string.Empty) { executeSql += _order; };
                executeSql = string.Format("{0};", FrameworkUtils.RemoveCarriageReturnAndExtraWhiteSpaces(executeSql));
                //_log.Debug(string.Format("TablePad(): executeSql: [{0}]", executeSql));

                //Always clear listItems
                if (_listButtons.Count > 0) _listButtons.Clear();

                //Debug
                SelectedData xpoSelectedData = GlobalFramework.SessionXpo.ExecuteQueryWithMetadata(executeSql);

                SelectStatementResultRow[] selectStatementResultMeta = xpoSelectedData.ResultSet[0].Rows;
                SelectStatementResultRow[] selectStatementResultData = xpoSelectedData.ResultSet[1].Rows;
                //    foreach (SelectStatementResultRow row in selectStatementResultMeta)
                //    {
                //        _log.Debug(string.Format("UpdateSql(): {0}\t{1}\t{2}", row.Values[0], row.Values[1], row.Values[2]));
                //    }

                // Detect Encrypted Model
                if (GlobalFramework.PluginSoftwareVendor != null && executeSql.ToLower().Contains(nameof(sys_userdetail).ToLower()))
                {
                    // Inject nonPropertyFields that are outside of attributes Scope and are required to exists to be decrypted
                    string[] nonPropertyFields = { "label" };
                    // Unencrypt selectStatementResultData encrypted properties
                    selectStatementResultData = XPGuidObject.DecryptSelectStatementResults(typeof(sys_userdetail), selectStatementResultMeta, selectStatementResultData, nonPropertyFields);
                }

                //Create a FieldIndex to Get Values From FieldNames
                int i = 0;
                _fieldIndex = new Dictionary<string, int>();
                foreach (SelectStatementResultRow field in selectStatementResultMeta)
                {
                    _fieldIndex.Add(field.Values[0].ToString(), i++);
                }

                if (selectStatementResultData != null && selectStatementResultData.LongLength > 0)
                {
                    //check existance of required fields 
                    if (selectStatementResultMeta[_fieldIndex["id"]].Values[0].ToString() == "id"
                      && selectStatementResultMeta[_fieldIndex["name"]].Values[0].ToString() == "name"
                      && selectStatementResultMeta[_fieldIndex["label"]].Values[0].ToString() == "label"
                      && selectStatementResultMeta[_fieldIndex["image"]].Values[0].ToString() == "image")
                    {
                        foreach (SelectStatementResultRow row in selectStatementResultData)
                        {
                            //Create the protected reference to current row
                            _resultRow = row;

                            //Assign first id to TablePad
                            if (_selectedButtonOid == Guid.Empty)
                            {
                                _selectedButtonOid = new Guid(_resultRow.Values[_fieldIndex["id"]].ToString());
                                //If not defined _initialActiveButtonOid, by default the _initialActiveButtonOid will be the first button 
                                if (_initialActiveButtonOid == Guid.Empty)
                                {
                                    _initialActiveButtonOid = _selectedButtonOid;
                                }
                            };

                            _strButtonName = string.Format("{0}_{1}", _buttonNamePrefix, _resultRow.Values[_fieldIndex["id"]].ToString());
                            _strButtonLabel = (_resultRow.Values[_fieldIndex["label"]] != null && _resultRow.Values[_fieldIndex["label"]].ToString() != string.Empty) ? _resultRow.Values[_fieldIndex["label"]].ToString() : _resultRow.Values[_fieldIndex["name"]].ToString();
                            _strButtonImage = (_resultRow.Values[_fieldIndex["image"]] != null && _resultRow.Values[_fieldIndex["image"]].ToString() != string.Empty) ? FrameworkUtils.OSSlash(_resultRow.Values[_fieldIndex["image"]].ToString()) : "";
                            if (_strButtonLabel.Length > _posBaseButtonMaxCharsPerLabel) { _strButtonLabel = _strButtonLabel.Substring(0, _posBaseButtonMaxCharsPerLabel) + "."; };

                            //Initialize Button
                            buttonCurrent = InitializeButton();

                            //Add Current Button to List
                            _listButtons.Add(buttonCurrent);

                            buttonCurrent.CurrentButtonOid = new Guid(_resultRow.Values[_fieldIndex["id"]].ToString());
                            //Disable Current Active Button, and turn it the selected
                            if (new Guid(_resultRow.Values[_fieldIndex["id"]].ToString()) == _initialActiveButtonOid)
                            {
                                if (_toggleMode) buttonCurrent.Sensitive = false;
                                //Always assign selected reference to SelectedButton
                                _selectedButton = buttonCurrent;
                            }

                            //Childs
                            if (_fieldIndex.ContainsKey("childs"))
                            {
                                _hasChilds = (Convert.ToInt16(_resultRow.Values[_fieldIndex["childs"]]) > 0) ? true : false;
                            }
                            else
                            {
                                _hasChilds = true;
                            };

                            if (_hasChilds)
                            {
                                //Assign to public Event, to be Exposed to initialized object
                                buttonCurrent.Clicked += TablePadChildButton_Clicked;
                                //Add Childs to Button Label
                                //TODO: WorkInProgress
                                //_log.Debug(string.Format("UpdateSql(): _reader.FieldCount [{0}]", _reader.FieldCount));
                                //if(_reader.FieldCount >= 5 && !_reader.IsDBNull(4)) buttonCurrent.Label += string.Format(" ({0})", _reader.GetInt16("childs"));
                            }
                            else
                            {
                                //Disable Button
                                buttonCurrent.Sensitive = false;
                            };
                        };

                        //prepare pagging
                        _totalItems = _listButtons.Count;
                        _itemsPerPage = Convert.ToInt16(_rows * _columns);
                        _totalPages = (int)Math.Ceiling((float)_totalItems / (float)_itemsPerPage);

                        //Debug
                        //_log.Debug(string.Format("UpdateSql(): totalItems: [{0}], itemsPerPage: [{1}], totalPages: [{2}]", _totalItems, _itemsPerPage, _totalPages));

                        Update();
                    }
                    else
                    {
                        Utils.ShowMessageTouch(GlobalApp.WindowPos, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), "TablePad: Cant create TablePad, invalid query! You must supply mandatory fields name in Sql (id, name, label and image)!");
                    };
                }
                else
                {
                    //Always update buttons, even if result in empty query
                    Update();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public void Update()
        {
            //Remove Old Buttons
            int childrenLength = this.Children.Length;
            for (int i = 0; i < childrenLength; i++)
            {
                //_log.Debug("Update(): TablePad Remove:" + this.Children[0].Name + "["+ i + "]");
                this.Remove(this.Children[0]);
            }

            int startItem = 0, endItem = 0;
            if (_listButtons.Count > 0)
            {
                //Add Items
                uint currRow = 0, currColumn = 0;
                startItem = (_currentPage * _itemsPerPage) - _itemsPerPage;
                endItem = startItem + _itemsPerPage - 1;
                for (int i = startItem; i <= endItem; i++)
                {
                    if (i < _totalItems)
                    {
                        //_log.Debug(string.Format("Update(): TablePad Add: [{0}] to row/column[{1}/{2}]", i, currRow, currColumn));
                        this.Attach(_listButtons[i], currColumn, currColumn + 1, currRow, currRow + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
                    }

                    if (currColumn == this.NColumns - 1)
                    {
                        ++currRow;
                        currColumn = 0;
                    }
                    else
                    {
                        ++currColumn;
                    }
                }
            }

            //Button State
            if (_currentPage == 1)
            {
                _buttonScrollPrev.Sensitive = false;
            }
            else
            {
                _buttonScrollPrev.Sensitive = true;
            };

            if (_currentPage == _totalPages)
            {
                _buttonScrollNext.Sensitive = false;
            }
            else
            {
                if (_totalPages > 1) _buttonScrollNext.Sensitive = true;
            };

            //Debug
            //_log.Debug(
            //  "Update()" +
            //  ": startItem=" + startItem +
            //  ", endItem=" + endItem +
            //  ", _currentPage=" + _currentPage +
            //  ", _totalPages=" + _totalPages +
            //  ", _totalItems=" + _totalItems +
            //  ", _itemsPerPage=" + _itemsPerPage
            //);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Protected Methods - To Override 

        public virtual TouchButtonBase InitializeButton()
        {
            return new TouchButtonImage(_strButtonName, _colorButton, _strButtonLabel, _fontPosBaseButtonSize, _strButtonImage, _fileBaseButtonOverlay, _buttonWidth, _buttonHeight);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void _buttonScrollPrev_Clicked(object obj, EventArgs args)
        {
            TouchButtonBase button = (TouchButtonBase)obj;
            _currentPage = _currentPage - 1;
            Update();
        }

        private void _buttonScrollNext_Clicked(object obj, EventArgs args)
        {
            TouchButtonBase button = (TouchButtonBase)obj;
            _currentPage = _currentPage + 1;
            Update();
        }

        //Redirect to public Clicked Event
        private void TablePadChildButton_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            //Restore old selected button Color
            if (_toggleMode && _selectedButton != null) _selectedButton.Sensitive = true;
            //Change New Selected Button Reference
            _selectedButton = button;
            if (_toggleMode) _selectedButton.Sensitive = false;

            if (Clicked != null) Clicked(sender, e);
        }

        internal void Refresh()
        {
            _listButtons = new List<TouchButtonBase>();
            UpdateSql();
        }
    }
}
