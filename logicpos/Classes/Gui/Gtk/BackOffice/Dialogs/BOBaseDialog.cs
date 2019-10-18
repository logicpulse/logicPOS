using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    abstract class BOBaseDialog : Dialog
    {
        //Log4Net
        protected static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected Members
        protected GenericTreeViewXPO _treeView = null;
        protected DialogMode _dialogMode;
        protected Notebook _notebook;
        protected HBox _hboxStatus;
        protected Dictionary<int, Widget> _panels = new Dictionary<int, Widget>();
        //Other Shared Protected Members
        protected int _widgetMaxWidth;
        //Protected Members, Defaults
        protected int _dialogPadding = 10;
        protected int _dialogLabelDistance = 20;
        protected System.Drawing.Size _sizefileChooserPreview = new System.Drawing.Size(37, 37);
        //Used to control rows on page  
        protected int _rowCurrent = 0;
        protected int _rowHeight = 0;
        //DEPRECATED : Protected ReadOnly Records, always initialized : Now use protections in TreeView Events
        protected List<Guid> _protectedRecords = new List<Guid>();
        protected bool _protectRecord = false;
        //VBox
        protected int _boxSpacing = 5;
        //Public Properties, to have access to/from TreeView EventHandlers
        protected XPGuidObject _dataSourceRow;
        public XPGuidObject DataSourceRow
        {
            get { return _dataSourceRow; }
            set { _dataSourceRow = value; }
        }
        protected GenericCRUDWidgetListXPO _crudWidgetList;
        public GenericCRUDWidgetListXPO CrudWidgetList
        {
            get { return _crudWidgetList; }
            set { _crudWidgetList = value; }
        }

        public BOBaseDialog(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pDataSourceRow)
            : base("", pSourceWindow, pFlags)
        {
            //Parameters
            _treeView = pTreeView;
            _dialogMode = pDialogMode;
            _dataSourceRow = pDataSourceRow;

            //TODO: try to prevent NULL Error
            //_dataSourceRow = GlobalFramework.SessionXpo.GetObjectByKey<XPGuidObject>(_dataSourceRow.Oid);
            //TODO: Validar se o erro de editar dá erro de acesso objecto eliminado.
            //APPEAR when we Try to ReEdit Terminal, after assign Printer
            //An exception of type 'System.NullReferenceException' occurred in logicpos.exe but was not handled in user code
            _dataSourceRow.Reload();

            //Defaults
            //Modal = true; //< Problems in Ubuntu, TitleBar Disapear
            WindowPosition = WindowPosition.CenterAlways;
            GrabFocus();
            SetSize(400, 400);
            _widgetMaxWidth = WidthRequest - (_dialogPadding * 2) - 16;

            //Grey Window : Luis|Muga
            //this.Decorated = false;
            //White Window : Mario
            this.Decorated = true;
            this.Resizable = false;
            this.WindowPosition = WindowPosition.Center;
            //Grey Window : Luis|Muga
            //this.ModifyBg(StateType.Normal, Utils.StringToGTKColor(GlobalFramework.Settings["colorBackOfficeContentBackground"]));

            //Accelerators
            AccelGroup accelGroup = new AccelGroup();
            AddAccelGroup(accelGroup);

            //Init WidgetList
            _crudWidgetList = new GenericCRUDWidgetListXPO(_dataSourceRow.Session);

            //Icon
            string fileImageAppIcon = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], SettingsApp.AppIcon));
            if (File.Exists(fileImageAppIcon)) Icon = Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));

            //Init StatusBar
            InitStatusBar();
            //InitButtons
            InitButtons();
            //InitUi
            InitUI();
        }

        protected void SetSize(int pWidth, int pHeight)
        {
            SetSizeRequest(pWidth, pHeight);
            _widgetMaxWidth = WidthRequest - (_dialogPadding * 2) - 16;
        }

        private void InitButtons()
        {
            //Settings
            String fontBaseDialogActionAreaButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogActionAreaButton"]);
            String tmpFileActionOK = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_ok.png");
            String tmpFileActionCancel = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");
            System.Drawing.Size sizeBaseDialogActionAreaButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaButtonIcon"]);  
            System.Drawing.Size sizeBaseDialogActionAreaButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaButton"]); 
            System.Drawing.Color colorBaseDialogActionAreaButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonBackground"]);
            System.Drawing.Color colorBaseDialogActionAreaButtonFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonFont"]);

            //TODO:THEME
            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                sizeBaseDialogActionAreaButton.Height -= 10;
                sizeBaseDialogActionAreaButtonIcon.Width -= 10;
                sizeBaseDialogActionAreaButtonIcon.Height -= 10;
            };

            TouchButtonIconWithText buttonOk = new TouchButtonIconWithText("touchButtonOk_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_ok"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, tmpFileActionOK, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);
            TouchButtonIconWithText buttonCancel = new TouchButtonIconWithText("touchButtonCancel_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_cancel"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, tmpFileActionCancel, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);

            //If DialogMode in View Mode, dont Show Ok Button
            if (_dialogMode != DialogMode.View)
            {
                this.AddActionWidget(buttonOk, ResponseType.Ok);
            }
            this.AddActionWidget(buttonCancel, ResponseType.Cancel);
        }

        private void InitUI()
        {
            //Init NoteBook
            _notebook = new Notebook();

            _notebook.BorderWidth = 3;

            //Pack
            //Grey Window : Luis|Muga
            //VBox.PackStart(_notebook, true, true, 5);
            //White Window : Mario
            VBox.PackStart(_notebook, true, true, 0);

            VBox.PackStart(_hboxStatus, false, false, 0);
            this.AddActionWidget(_hboxStatus, -1);
        }

        protected void InitNotes()
        {
            VBox vbox = new VBox(true, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            //Notes
            EntryMultiline entryMultiline = new EntryMultiline();
            entryMultiline.Value.Text = (DataSourceRow as XPGuidObject).Notes;
            //Remove ShadowType and Border
            //entryMultiline.ScrolledWindow.ShadowType = ShadowType.None;
            entryMultiline.ScrolledWindow.BorderWidth = 0;
            Label labelMultiline = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes"));
            vbox.PackStart(entryMultiline, true, true, 0);
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryMultiline, labelMultiline, DataSourceRow, "Notes", SettingsApp.RegexAlfaNumericExtended, false));

            //Append Tab
            _notebook.AppendPage(vbox, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes")));
        }

        protected override void OnResponse(ResponseType pResponse)
        {
            _crudWidgetList.ProcessDialogResponse(this, _dialogMode, pResponse);
        }

        private void InitStatusBar()
        {
            _hboxStatus = new HBox(true, 0);
            _hboxStatus.BorderWidth = 3;

            //UpdatedBy
            VBox vboxUpdatedBy = new VBox(true, 0);
            Label labelUpdatedBy = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_user_update"));
            Label labelUpdatedByValue = new Label(string.Empty);
            labelUpdatedBy.SetAlignment(0.0F, 0.5F);
            labelUpdatedByValue.SetAlignment(0.0F, 0.5F);
            //labelUpdatedBy.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
            //labelUpdatedByValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
            vboxUpdatedBy.PackStart(labelUpdatedBy);
            vboxUpdatedBy.PackStart(labelUpdatedByValue);

            //CreatedAt
            VBox vboxCreatedAt = new VBox(true, 0);
            Label labelCreatedAt = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_created"));
            Label labelCreatedAtValue = new Label(string.Empty);
            //labelCreatedAt.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
            //labelCreatedAtValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
            labelCreatedAt.SetAlignment(0.5F, 0.5F);
            labelCreatedAtValue.SetAlignment(0.5F, 0.5F);
            vboxCreatedAt.PackStart(labelCreatedAt);
            vboxCreatedAt.PackStart(labelCreatedAtValue);

            //UpdatedAt
            VBox vboxUpdatedAt = new VBox(true, 0);
            Label labelUpdatedAt = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_updated_for_base_dialog"));
            Label labelUpdatedAtValue = new Label(string.Empty);
            //labelUpdatedAt.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
            //labelUpdatedAtValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
            labelUpdatedAt.SetAlignment(1.0F, 0.5F);
            labelUpdatedAtValue.SetAlignment(1.0F, 0.5F);
            vboxUpdatedAt.PackStart(labelUpdatedAt);
            vboxUpdatedAt.PackStart(labelUpdatedAtValue);

            _hboxStatus.PackStart(vboxUpdatedBy);
            _hboxStatus.PackStart(vboxCreatedAt);
            _hboxStatus.PackStart(vboxUpdatedAt);

            _crudWidgetList.Add(new GenericCRUDWidgetXPO(labelUpdatedByValue, (_dataSourceRow as dynamic).UpdatedBy, "Name"));
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(labelCreatedAtValue, _dataSourceRow, "CreatedAt"));
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(labelUpdatedAtValue, _dataSourceRow, "UpdatedAt"));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DEPRECATED : Protected ReadOnly Records, always initialized : Now use protections in TreeView Events
        protected void ProtectComponents()
        {
            //Works on UPDATE & DELETE
            if (_dialogMode != DialogMode.Insert && _protectedRecords != null && _protectedRecords.Count > 0)
            {
                //Update Reference
                _protectRecord = _protectedRecords.Contains(_dataSourceRow.Oid);

                if (_protectRecord)
                {
                    //Protect Edits in Components
                    foreach (var item in _crudWidgetList)
                    {
                        //_log.Debug(String.Format("item: [{0}]", item));
                        item.Widget.Sensitive = false;
                    }
                }
            }
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // BO DEPRECATED : Muga Stuff
        /*
        protected void ResetRowManager()
        {
            _rowCurrent = 10;
            _rowHeight = 50;
        }

        protected void JumpRow()
        {
            JumpRow(1);
        }

        protected void JumpRow(int pFactor)
        {
            _rowCurrent += (_rowHeight / pFactor);
        }

        protected Fixed GetNewTabPage(string pLabel)
        {
            Fixed result = new Fixed();

            Label tmpNewLabel = new Label(pLabel);

            _panels.Add(_notebook.AppendPage(result, tmpNewLabel), result);

            ResetRowManager();
            return (result);
        }

        protected Entry AddRowItemText(Fixed pTab, string pCaption, string pFieldName, string pValidationRegex, bool pRequired, bool pFullWidth)
        {
            try
            {
                return AddRowItemText(pTab, pCaption, pFieldName, pValidationRegex, pRequired, pFullWidth, false);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        //Add item as rows
        protected Entry AddRowItemText(Fixed pTab, string pCaption, string pFieldName, string pValidationRegex, bool pRequired, bool pFullWidth, bool pReadOnly)
        {
            try
            {
                Label labelObject = new Label(pCaption);
                Entry entryObject = new Entry();

                if (pFullWidth)
                {
                    entryObject.WidthRequest = _widgetMaxWidth;
                }

                if (pReadOnly)
                {
                    entryObject.IsEditable = false;
                }

                pTab.Put(labelObject, _dialogPadding, _rowCurrent);
                pTab.Put(entryObject, _dialogPadding, _rowCurrent + _dialogLabelDistance);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryObject, labelObject, _dataSourceRow, pFieldName, pValidationRegex, pRequired));

                JumpRow();

                return entryObject;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        protected CheckButton AddRowItemCheckButton(Fixed pTab, string pCaption, string pFieldName)
        {
            try
            {
                CheckButton tmpCheckButton = new CheckButton(pCaption);

                pTab.Put(tmpCheckButton, _dialogPadding, _rowCurrent);

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(tmpCheckButton, _dataSourceRow, pFieldName));

                JumpRow(2);

                return tmpCheckButton;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        protected FileChooserButton AddRowItemFileSelect(Fixed pTab, string pCaption, FileFilter pFilters, string pFieldName, string pcurrentValue, bool pRequired, bool pFullWidth)
        {
            try
            {
                Label labelButtonIcon = new Label(pCaption);
                FileChooserButton fileChooserButtonIcon = new FileChooserButton(pCaption, FileChooserAction.Open);
                fileChooserButtonIcon.SetFilename(pcurrentValue);
                fileChooserButtonIcon.Filter = pFilters;

                if (pFullWidth)
                {
                    fileChooserButtonIcon.WidthRequest = _widgetMaxWidth;
                }

                pTab.Put(labelButtonIcon, _dialogPadding, _rowCurrent);
                pTab.Put(fileChooserButtonIcon, _dialogPadding, _rowCurrent + _dialogLabelDistance);

                _crudWidgetList.Add(new GenericCRUDWidgetXPO(fileChooserButtonIcon, labelButtonIcon, _dataSourceRow, pFieldName));

                JumpRow();

                return fileChooserButtonIcon;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        protected FileChooserButton AddRowItemImageSelect(Fixed pTab, string pCaption, FileFilter pFilters, string pFieldName, string pcurrentValue, bool pRequired, bool pFullWidth)
        {
            try
            {
                Label labelIcon = new Label(pCaption);
                FileChooserButton fileChooser = new FileChooserButton("", FileChooserAction.Open);
                fileChooser.SetFilename(pcurrentValue);

                Image imagePreviewButtonIcon = new Image()
                {
                    WidthRequest = _sizefileChooserPreview.Width,
                    HeightRequest = _sizefileChooserPreview.Height
                };

                Frame framePreviewButtonIcon = new Frame();
                framePreviewButtonIcon.Add(imagePreviewButtonIcon);
                fileChooser.Filter = Utils.GetFileFilterImages();
                fileChooser.WidthRequest = _widgetMaxWidth - imagePreviewButtonIcon.WidthRequest - 8;
                fileChooser.SelectionChanged += (sender, eventArgs) => imagePreviewButtonIcon.Pixbuf = Utils.ResizeAndCropFileToPixBuf((sender as FileChooserButton).Filename, new System.Drawing.Size(imagePreviewButtonIcon.WidthRequest, imagePreviewButtonIcon.HeightRequest));
                pTab.Put(labelIcon, _dialogPadding, _rowCurrent);
                pTab.Put(fileChooser, _dialogPadding, _rowCurrent + _dialogLabelDistance);
                pTab.Put(framePreviewButtonIcon, WidthRequest - _dialogPadding - imagePreviewButtonIcon.WidthRequest - 12, _rowCurrent);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(fileChooser, labelIcon, _dataSourceRow, pFieldName, string.Empty, pRequired));

                JumpRow();

                return fileChooser;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        protected XPOComboBox AddRowItemRelation(Fixed pTab, string pCaption, string pFieldName, Type pType, XPGuidObject pCurrentValue, string pFieldNameChildren, bool pFullWidth)
        {
            return (AddRowItemRelation(pTab, pCaption, pFieldName, pType, pCurrentValue, pFieldNameChildren, pFullWidth, false));
        }

        protected XPOComboBox AddRowItemRelation(Fixed pTab, string pCaption, string pFieldName, Type pType, XPGuidObject pCurrentValue, string pFieldNameChildren, bool pFullWidth, bool pRequired)
        {
            return (AddRowItemRelation(pTab, pCaption, pFieldName, pType, pCurrentValue, string.Empty, pFieldNameChildren, pFullWidth, pRequired, null));
        }

        protected XPOComboBox AddRowItemRelation(Fixed pTab, string pCaption, string pFieldName, Type pType, XPGuidObject pCurrentValue, string pFilter, string pFieldNameChildren, bool pFullWidth)
        {
            return (AddRowItemRelation(pTab, pCaption, pFieldName, pType, pCurrentValue, pFilter, pFieldNameChildren, pFullWidth, false, null));
        }

        protected XPOComboBox AddRowItemRelation(Fixed pTab, string pCaption, string pFieldName, Type pType, XPGuidObject pCurrentValue, string pFilter, string pFieldNameChildren, bool pFullWidth, bool pRequired, SortProperty[] pSortProperty)
        {
            string filterDefault = "(Disabled = 0 OR Disabled is NULL)";
            if (pFilter != string.Empty) filterDefault = string.Format("{0} AND {1}", filterDefault, pFilter);
            SortProperty[] sortProperty = (pSortProperty != null && pSortProperty.Length > 0) ? pSortProperty : null;

            Label labelObject = new Label(pCaption);
            XPOComboBox xpoComboBox = new XPOComboBox(_dataSourceRow.Session, pType, pCurrentValue, pFieldNameChildren, CriteriaOperator.Parse(filterDefault), sortProperty);

            if (pFullWidth)
            {
                xpoComboBox.WidthRequest = _widgetMaxWidth;
            }

            //Image imagePreviewButtonIcon = new Image()
            //{
            //    WidthRequest = _sizefileChooserPreview.Width,
            //    HeightRequest = _sizefileChooserPreview.Height
            //};
            //btn = new Button();

            //Frame framePreviewButtonIcon = new Frame();
            //framePreviewButtonIcon.Add(imagePreviewButtonIcon);
            //btn.Clicked += btn_Clicked;

            pTab.Put(labelObject, _dialogPadding, _rowCurrent);
            pTab.Put(xpoComboBox, _dialogPadding, _rowCurrent + _dialogLabelDistance);
            //pTab.Put(btn, WidthRequest - _dialogPadding - imagePreviewButtonIcon.WidthRequest - 12, _rowCurrent);

            //Commented by Mario: we always need RegexGuid here, else we have problemas on non Required Fields
            //string regExRule = (pRequired) ? SettingsApp.RegexGuid : string.Empty;
            //Fixed Non Required Fields
            string regExRule = SettingsApp.RegexGuid;
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(xpoComboBox, labelObject, _dataSourceRow, pFieldName, regExRule, pRequired));

            _rowCurrent += _rowHeight;

            return (xpoComboBox);
        }

        protected void AddToPositionItemLabel(int pX, int pY, Fixed pTab, string pCaption)
        {
            Label labelButtonIcon = new Label(pCaption);
            pTab.Put(labelButtonIcon, pX, pY);
        }

        protected void AddToPositionItemText(int pX, int pY, int pWidth, Fixed pTab, string pFieldName, string pValidationRegex, bool pRequired, bool pReadOnly)
        {
            Entry entryObject = new Entry();

            entryObject.WidthRequest = pWidth;

            if (pReadOnly)
            {
                entryObject.IsEditable = false;
            }

            pTab.Put(entryObject, pX, pY);
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryObject, _dataSourceRow, pFieldName, pValidationRegex, pRequired));
        }

        protected void AddToPositionItemCheckButton(int pX, int pY, Fixed pTab, string pFieldName)
        {
            CheckButton tmpCheckButton = new CheckButton(string.Empty);
            pTab.Put(tmpCheckButton, pX, pY);
            _crudWidgetList.Add(new GenericCRUDWidgetXPO(tmpCheckButton, _dataSourceRow, pFieldName));
        }

        //Commented: We cant Filter Combo after Render Model, this way we lost selected Item, and the selected item is always the last one, NOW we filter when generating Model
        //protected void FilterCombo(XPOComboBox sender, string pSQLFilter)
        //{
        //  CriteriaOperator criteria = CriteriaOperator.Parse(pSQLFilter);
        //  sender.UpdateModel(criteria);
        //}

        */
        //EO DEPRECATED
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}
