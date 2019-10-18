using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogTemplate : BOBaseDialog
    {
        public DialogTemplate(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_template"));
            SetSizeRequest(400, 600);

            InitUI();
            InitNotes();

            //Deprecated : Now Use protections in TreeView, BeforeUpdate|BeforeDelete
            //Get Protected Records from TreeView, and Protect it after InitUI()
            //_protectedRecords = TreeViewTemplate.ProtectedRecords;
            //Disable All UI Components (Sensitive False)
            //ProtectComponents();

            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Get XpoObject Reference from DataSourceRow
                //Template dataSourceRow = (DataSourceRow as Template);

                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumericExtended, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                /* Tab2 Sample

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Notes
                Entry entryNotes = new Entry();
                BOWidgetBox boxNotes = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes"), entryNotes);
                vboxTab2.PackStart(boxNotes, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxNotes, _dataSourceRow, "Notes", SettingsApp.RegexAlfa, false));

                //CreatedAt
                Entry entryCreatedAt = new Entry() { WidthRequest = 250 };
                BOWidgetBox boxCreatedAt = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), entryCreatedAt);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCreatedAt, _dataSourceRow, "CreatedAt", SettingsApp.RegexDateTime, false));

                //UpdatedAt
                Entry entryUpdatedAt = new Entry();
                BOWidgetBox boxUpdatedAt = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), entryUpdatedAt);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxUpdatedAt, _dataSourceRow, "UpdatedAt", SettingsApp.RegexDateTime, false));

                //Hbox CreatedAt and UpdatedAt
                HBox hboxCreatedAtAndUpdatedAt = new HBox(false, _boxSpacing);
                hboxCreatedAtAndUpdatedAt.PackStart(boxCreatedAt, true, true, 0);
                hboxCreatedAtAndUpdatedAt.PackStart(boxUpdatedAt, true, true, 0);
                vboxTab2.PackStart(hboxCreatedAtAndUpdatedAt, false, false, 0);

                //CreatedBy
                XPOComboBox xpoComboBoxCreatedBy = new XPOComboBox(DataSourceRow.Session, typeof(sys_userdetail), (DataSourceRow as fin_articleclass).CreatedBy, "Name") { WidthRequest = 250 };
                BOWidgetBox boxCreatedBy = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_user, xpoComboBoxCreatedBy);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCreatedBy, DataSourceRow, "CreatedBy", SettingsApp.RegexGuid, true));

                //CreatedWhere
                XPOComboBox xpoComboBoxCreatedWhere = new XPOComboBox(DataSourceRow.Session, typeof(pos_configurationplaceterminal), (DataSourceRow as fin_articleclass).CreatedWhere, "Designation");
                BOWidgetBox boxCreatedWhere = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_terminal, xpoComboBoxCreatedWhere);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCreatedWhere, DataSourceRow, "CreatedWhere", SettingsApp.RegexGuid, true));
                
                //Hbox CreatedBy and CreatedWhere
                HBox hboxCreatedByAndCreatedWhere = new HBox(false, _boxSpacing);
                hboxCreatedByAndCreatedWhere.PackStart(boxCreatedBy, true, true, 0);
                hboxCreatedByAndCreatedWhere.PackStart(boxCreatedWhere, true, true, 0);
                vboxTab2.PackStart(hboxCreatedByAndCreatedWhere, false, false, 0);
                
                //ButtonImage
                FileChooserButton fileChooserButtonImage = new FileChooserButton(string.Empty, FileChooserAction.Open) { HeightRequest = 23 };
                Image fileChooserImagePreviewButtonImage = new Image() { WidthRequest = _sizefileChooserPreview.Width, HeightRequest = _sizefileChooserPreview.Height };
                Frame fileChooserFrameImagePreviewButtonImage = new Frame();
                fileChooserFrameImagePreviewButtonImage.ShadowType = ShadowType.None;
                fileChooserFrameImagePreviewButtonImage.Add(fileChooserImagePreviewButtonImage);
                fileChooserButtonImage.SetFilename(((fin_articlefamily)DataSourceRow).ButtonImage);
                fileChooserButtonImage.Filter = Utils.GetFileFilterImages();
                fileChooserButtonImage.SelectionChanged += (sender, eventArgs) => fileChooserImagePreviewButtonImage.Pixbuf = Utils.ResizeAndCropFileToPixBuf((sender as FileChooserButton).Filename, new System.Drawing.Size(fileChooserImagePreviewButtonImage.WidthRequest, fileChooserImagePreviewButtonImage.HeightRequest));
                BOWidgetBox boxfileChooserButtonImage = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_image, fileChooserButtonImage);
                HBox hboxfileChooserAndimagePreviewButtonImage = new HBox(false, _boxSpacing);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(boxfileChooserButtonImage, true, true, 0);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(fileChooserFrameImagePreviewButtonImage, false, false, 0);
                vboxTab1.PackStart(hboxfileChooserAndimagePreviewButtonImage, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxfileChooserButtonImage, _dataSourceRow, "ButtonImage", string.Empty, true));

                //Notes
                EntryMultiline entryMultilineNotes = new EntryMultiline();
                entryMultilineNotes.Value.Text = (DataSourceRow as fin_article).Notes;
                Label labelMultilineNotes = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes);
                vboxTab4.PackStart(entryMultilineNotes, true, true, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(entryMultilineNotes, labelMultilineNotes, DataSourceRow, "Notes", SettingsApp.RegexAlfaNumericExtended, false));
                 
                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes));
                */

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                /* Events Sample
                
                //Capture Events
                //_crudWidgetList.BeforeUpdate += delegate { ShowEventOutput(); };
                */

            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        /* Events Sample

        private void ShowEventOutput()
        {
            GenericCRUDWidgetXPO genericCRUDWidgetXPO = (this._crudWidgetList.GetFieldWidget("Designation") as GenericCRUDWidgetXPO);

            _log.Debug(string.Format("Field: [{0}]", genericCRUDWidgetXPO.FieldName));
            _log.Debug(string.Format("FieldType: [{0}]", genericCRUDWidgetXPO.FieldType));
            _log.Debug(string.Format("Label.Text: [{0}]", genericCRUDWidgetXPO.Label.Text));
            _log.Debug(string.Format("Source: [{0}]", genericCRUDWidgetXPO.DataSourceRow));
            _log.Debug(string.Format("Source.Designation: [{0}]", (genericCRUDWidgetXPO.DataSourceRow as fin_articlefamily).Designation));
            _log.Debug(string.Format("Widget.Value: [{0}]", (genericCRUDWidgetXPO.Widget as Entry).Text));
        }
        */
    }
}

