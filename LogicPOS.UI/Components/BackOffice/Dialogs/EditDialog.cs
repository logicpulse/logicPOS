using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.InputFieds;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal abstract class EditDialog : Dialog
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected Members
        protected XpoGridView _treeView = null;
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

        public Entity Entity { get; set; }

        public GenericCRUDWidgetListXPO InputFields { get; set; }

        public ICollection<fin_articlecomposition> _articlecompositions;


        public IconButtonWithText ButtonOk { get; set; }
  
        protected IconButtonWithText _buttonCancel;
        public IconButtonWithText buttonCancel
        {
            get { return _buttonCancel; }
            set { _buttonCancel = value; }
        }

        public EditDialog(Window parentWindow,
                            XpoGridView pTreeView,
                            DialogFlags pFlags,
                            DialogMode pDialogMode,
                            Entity entity)
            : base("", parentWindow, pFlags)
        {
            //Parameters
            _treeView = pTreeView;
            _dialogMode = pDialogMode;
            if (entity != null) Entity = entity;


            if (entity != null) Entity.Reload();

            WindowPosition = WindowPosition.CenterAlways;
            GrabFocus();
            SetSize(400, 400);
            _widgetMaxWidth = WidthRequest - (_dialogPadding * 2) - 16;


            this.Decorated = true;
            this.Resizable = false;
            this.WindowPosition = WindowPosition.Center;

            //Accelerators
            AccelGroup accelGroup = new AccelGroup();
            AddAccelGroup(accelGroup);

            //Init WidgetList
            if (entity != null) InputFields = new GenericCRUDWidgetListXPO(Entity.Session);

            //Ico
            string fileImageAppIcon = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, POSSettings.AppIcon);
            if (File.Exists(fileImageAppIcon)) Icon = logicpos.Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));

            InitStatusBar();
            InitButtons();
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
            string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
            string tmpFileActionOK = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
            string tmpFileActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
            System.Drawing.Size sizeBaseDialogActionAreaButtonIcon = AppSettings.Instance.sizeBaseDialogActionAreaButtonIcon;
            System.Drawing.Size sizeBaseDialogActionAreaButton = AppSettings.Instance.sizeBaseDialogActionAreaButton;
            System.Drawing.Color colorBaseDialogActionAreaButtonBackground = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground;
            System.Drawing.Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;

            //TODO:THEME
            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                sizeBaseDialogActionAreaButton.Height -= 10;
                sizeBaseDialogActionAreaButtonIcon.Width -= 10;
                sizeBaseDialogActionAreaButtonIcon.Height -= 10;
            };

            ButtonOk = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonOk_DialogActionArea",
                    BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_button_label_ok"),
                    Font = fontBaseDialogActionAreaButton,
                    FontColor = colorBaseDialogActionAreaButtonFont,
                    Icon = tmpFileActionOK,
                    IconSize = sizeBaseDialogActionAreaButtonIcon,
                    ButtonSize = new System.Drawing.Size(
                        sizeBaseDialogActionAreaButton.Width,
                        sizeBaseDialogActionAreaButton.Height)
                });

            _buttonCancel = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonCancel_DialogActionArea",
                    BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_button_label_cancel"),
                    Font = fontBaseDialogActionAreaButton,
                    FontColor = colorBaseDialogActionAreaButtonFont,
                    Icon = tmpFileActionCancel,
                    IconSize = sizeBaseDialogActionAreaButtonIcon,
                    ButtonSize = new System.Drawing.Size(
                        sizeBaseDialogActionAreaButton.Width,
                        sizeBaseDialogActionAreaButton.Height)
                });

            //If DialogMode in View Mode, dont Show Ok Button
            if (_dialogMode != DialogMode.View)
            {
                this.AddActionWidget(ButtonOk, ResponseType.Ok);
            }

            this.AddActionWidget(_buttonCancel, ResponseType.Cancel);
        }

        private void InitUI()
        {
            //Init NoteBook
            _notebook = new Notebook();

            _notebook.BorderWidth = 3;

            VBox.PackStart(_notebook, true, true, 0);

            VBox.PackStart(_hboxStatus, false, false, 0);
            this.AddActionWidget(_hboxStatus, -1);
        }

        protected void InitNotes()
        {
            VBox vbox = new VBox(true, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            //Notes
            MultilineTextBox entryMultiline = new MultilineTextBox();
            entryMultiline.Value.Text = (Entity as Entity).Notes;
            //Remove ShadowType and Border
            //entryMultiline.ScrolledWindow.ShadowType = ShadowType.None;
            entryMultiline.ScrolledWindow.BorderWidth = 0;
            Label labelMultiline = new Label(GeneralUtils.GetResourceByName("global_notes"));
            vbox.PackStart(entryMultiline, true, true, 0);
            InputFields.Add(new GenericCRUDWidgetXPO(entryMultiline, labelMultiline, Entity, "Notes", RegexUtils.RegexAlfaNumericExtended, false));

            //Append Tab
            _notebook.AppendPage(vbox, new Label(GeneralUtils.GetResourceByName("global_notes")));
        }

        public virtual void OnButtonOkClicked()
        {
            SimpleAlerts.Information()
                .WithParent(this)
                .WithTitle("Integração da API")
                .WithMessage("Botão OK clicado")
                .Show();
        }


        protected override void OnResponse(ResponseType responseType)
        {
            if(responseType == ResponseType.Ok)
            {
                OnButtonOkClicked();
            }

            if (InputFields != null) InputFields.ProcessDialogResponse(this, _dialogMode, responseType);
            //Artigos Compostos [IN:016522]


            if (this.GetType() == (typeof(DialogArticle)))
            {
                try
                {
                    if (Entity.GetType() == (typeof(fin_article)))
                    {
                        //Restore Objects before editing if cancel or delete  
                        if ((Entity as fin_article).IsComposed)
                        {
                            if (responseType == ResponseType.Cancel || responseType == ResponseType.DeleteEvent)
                            {
                                for (int i = (Entity as fin_article).ArticleComposition.Count; i > 0; i--)
                                {
                                    var aux = (Entity as fin_article).ArticleComposition[i - 1];
                                    (Entity as fin_article).ArticleComposition.Remove(aux);
                                }
                                foreach (var item in _articlecompositions)
                                {
                                    (Entity as fin_article).ArticleComposition.Add(item);

                                }
                            }
                        }

                        //Process stocks
                        //Gestão de Stocks - Ajuste de Stock diretamente no Artigo (BackOffice) [IN:016530]
                        try
                        {
                            if (responseType == ResponseType.Ok || responseType == ResponseType.Apply)
                            {
                                string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", (Entity as fin_article).Oid);
                                var getArticleStock = Convert.ToDecimal(Entity.Session.ExecuteScalar(stockQuery));
                                if (Convert.ToDecimal(getArticleStock.ToString()) != (Entity as fin_article).Accounting)
                                {
                                    var own_customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), XPOSettings.XpoOidUserRecord);
                                    if (own_customer != null)
                                    {
                                        if (string.IsNullOrEmpty(own_customer.Name))
                                        {
                                            //update owner customer for internal stock moviments
                                            own_customer.FiscalNumber = GeneralSettings.PreferenceParameters["COMPANY_FISCALNUMBER"];
                                            own_customer.Name = GeneralSettings.PreferenceParameters["COMPANY_NAME"];
                                            own_customer.Save();
                                        }
                                    }
                                    if ((Entity as fin_article).Accounting > getArticleStock)
                                    {
                                        decimal quantity = (Entity as fin_article).Accounting - getArticleStock;
                                        ProcessArticleStock.Add(ProcessArticleStockMode.In, own_customer, 1, DateTime.Now, GeneralUtils.GetResourceByName("global_internal_document_footer1"), (Entity as fin_article), quantity, GeneralUtils.GetResourceByName("global_internal_document_footer1"));
                                    }
                                    else
                                    {
                                        decimal quantity = getArticleStock - (Entity as fin_article).Accounting;
                                        ProcessArticleStock.Add(ProcessArticleStockMode.Out, own_customer, 1, DateTime.Now, GeneralUtils.GetResourceByName("global_internal_document_footer1"), (Entity as fin_article), quantity, GeneralUtils.GetResourceByName("global_internal_document_footer1"));
                                    }

                                }
                            }
                        }
                        //New article
                        catch
                        {
                            ProcessArticleStock.Add(ProcessArticleStockMode.In, null, 1, DateTime.Now, GeneralUtils.GetResourceByName("global_internal_document_footer1"), (Entity as fin_article), (Entity as fin_article).Accounting, GeneralUtils.GetResourceByName("global_internal_document_footer1"));
                        }

                    }
                    //Delete Articles compositions with deleted Parents
                    string sqlDelete = string.Format("DELETE FROM [fin_articlecomposition] WHERE [Article] IS NULL;");
                    XPOSettings.Session.ExecuteQuery(sqlDelete);
                    _logger.Debug("Delete() :: articles composition with null parents'" + "'  ");

                    Entity.Reload();
                }
                catch (Exception ex)
                {
                    _logger.Error("error Delete() :: articles composition with null parents '" + "' : " + ex.Message, ex);
                }
            }
        }

        private void InitStatusBar()
        {
            if (InputFields != null)
            {
                _hboxStatus = new HBox(true, 0);
                _hboxStatus.BorderWidth = 3;

                //UpdatedBy
                VBox vboxUpdatedBy = new VBox(true, 0);
                Label labelUpdatedBy = new Label(GeneralUtils.GetResourceByName("global_record_user_update"));
                Label labelUpdatedByValue = new Label(string.Empty);
                labelUpdatedBy.SetAlignment(0.0F, 0.5F);
                labelUpdatedByValue.SetAlignment(0.0F, 0.5F);
                //labelUpdatedBy.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                //labelUpdatedByValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                vboxUpdatedBy.PackStart(labelUpdatedBy);
                vboxUpdatedBy.PackStart(labelUpdatedByValue);

                //CreatedAt
                VBox vboxCreatedAt = new VBox(true, 0);
                Label labelCreatedAt = new Label(GeneralUtils.GetResourceByName("global_record_date_created"));
                Label labelCreatedAtValue = new Label(string.Empty);
                //labelCreatedAt.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                //labelCreatedAtValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(System.Drawing.Color.White));
                labelCreatedAt.SetAlignment(0.5F, 0.5F);
                labelCreatedAtValue.SetAlignment(0.5F, 0.5F);
                vboxCreatedAt.PackStart(labelCreatedAt);
                vboxCreatedAt.PackStart(labelCreatedAtValue);

                //UpdatedAt
                VBox vboxUpdatedAt = new VBox(true, 0);
                Label labelUpdatedAt = new Label(GeneralUtils.GetResourceByName("global_record_date_updated_for_base_dialog"));
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

                InputFields.Add(new GenericCRUDWidgetXPO(labelUpdatedByValue, (Entity as dynamic).UpdatedBy, "Name"));
                InputFields.Add(new GenericCRUDWidgetXPO(labelCreatedAtValue, Entity, "CreatedAt"));
                InputFields.Add(new GenericCRUDWidgetXPO(labelUpdatedAtValue, Entity, "UpdatedAt"));
            }
        }

        protected void ProtectComponents()
        {
            //Works on UPDATE & DELETE
            if (_dialogMode != DialogMode.Insert && _protectedRecords != null && _protectedRecords.Count > 0)
            {
                //Update Reference
                _protectRecord = _protectedRecords.Contains(Entity.Oid);

                if (_protectRecord)
                {
                    foreach (var item in InputFields)
                    {
                        item.Widget.Sensitive = false;
                    }
                }
            }
        }
    }
}
