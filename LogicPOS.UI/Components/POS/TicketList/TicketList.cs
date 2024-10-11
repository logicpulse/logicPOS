using Gtk;
using logicpos;
using logicpos.App;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Others;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.Orders;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components
{
    public partial class TicketList : Box
    {
        public SaleOptionsPanel SaleOptionsPanel { get; }
        private Color TicketModeBackgroundColor => AppSettings.Instance.colorPosTicketListModeTicketBackground;
        private Color DocumentModeBackgroundColor => AppSettings.Instance.colorPosTicketListModeOrderMainBackground;
        private Color EditModeEditBackgroundColor => AppSettings.Instance.colorPosTicketListModeEditBackground;
        private Guid CurrentDocumentId { get; set; }
        private int CurrentTickedId { get; set; }
        private Guid CurrentDetailArticleId { get; set; }
        private int SelectedIndex { get; set; } = -1;
        private int TotalItems { get; set; } = 0;
        private int TotalItemsTicketListMode { get; set; } = 0;
        public OrderDetail CurrentOrderDetail { get; set; }
        private ArticleBag ArticleBag { get; set; }
        private fin_article CurrentDetailArticle { get; set; }

        private TreeIter _treeIter;
        private TreePath TreePath { get; set; }
        private TreeView TreeView { get; set; }
        private Label LabelTotal { get; set; }
        private Label LabelTotalLabel { get; set; }

        public POSMainWindow SourceWindow { get; set; }
        public ListStore ListStore { get; set; }
        internal TicketListMode ListMode { get; set; } = TicketListMode.Ticket;

        private IconButtonWithText _toolbarApplicationClose;
        public IconButtonWithText ToolbarApplicationClose
        {
            set { _toolbarApplicationClose = value; }
        }

        private IconButtonWithText _toolbarLogoutUser;
        public IconButtonWithText ToolbarLogoutUser
        {
            set { _toolbarLogoutUser = value; }
        }

        private IconButtonWithText _toolbarShowSystemDialog;
        public IconButtonWithText ToolbarShowSystemDialog
        {
            set { _toolbarShowSystemDialog = value; }
        }

        private IconButtonWithText _toolbarShowChangeUserDialog;
        public IconButtonWithText ToolbarShowChangeUserDialog
        {
            set { _toolbarShowChangeUserDialog = value; }
        }

        private IconButtonWithText _toolbarBackOffice;
        public IconButtonWithText ToolbarBackOffice
        {
            set { _toolbarBackOffice = value; }
        }

        private IconButtonWithText _toolbarReports;
        public IconButtonWithText ToolbarReports
        {
            set { _toolbarReports = value; }
        }

        private IconButtonWithText _toolbarCashDrawer;
        public IconButtonWithText ToolbarCashDrawer
        {
            set { _toolbarCashDrawer = value; }
        }

        private IconButtonWithText _toolbarFinanceDocuments;
        public IconButtonWithText ToolbarFinanceDocuments
        {
            set { _toolbarFinanceDocuments = value; }
        }

        private IconButtonWithText _toolbarNewFinanceDocument;
        public IconButtonWithText ToolbarNewFinanceDocument
        {
            set { _toolbarNewFinanceDocument = value; }
        }

        public TicketList(dynamic theme, SaleOptionsPanel panel)
        {
            SaleOptionsPanel = panel;
            InitUI(theme);
            ConfigureWeighingBalance();
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            SaleOptionsPanel.BtnSelectTable.Clicked += BtnSelectTable_Clicked;
        }

        private void ConfigureWeighingBalance()
        {
            if (GlobalApp.WeighingBalance != null && GlobalApp.WeighingBalance.IsPortOpen())
            {
                GlobalApp.WeighingBalance.ComPort().DataReceived += WeighingBalance_DataReceived;
            }
        }

        private void InitUI(dynamic pThemeObject)
        {
            this.BorderWidth = 10;

            Gdk.Color eventBoxTotalBackgroundColor = (pThemeObject.EventBoxTotal.BackgroundColor as string).StringToGdkColor();
            Pango.FontDescription columnsFontTitle = Pango.FontDescription.FromString(pThemeObject.Columns.FontTitle);
            Pango.FontDescription columnsFontData = Pango.FontDescription.FromString(pThemeObject.Columns.FontData);
            Pango.FontDescription labelLabelTotalFont = Pango.FontDescription.FromString(pThemeObject.EventBoxTotal.LabelLabelTotal.Font);
            Gdk.Color labelLabelTotalFontColor = (pThemeObject.EventBoxTotal.LabelLabelTotal.FontColor as string).StringToGdkColor();
            float labelLabelTotalAlignmentX = Convert.ToSingle(pThemeObject.EventBoxTotal.LabelLabelTotal.AlignmentX);
            Pango.FontDescription labelTotalFont = Pango.FontDescription.FromString(pThemeObject.EventBoxTotal.LabelTotal.Font);
            Gdk.Color labelTotalFontColor = (pThemeObject.EventBoxTotal.LabelTotal.FontColor as string).StringToGdkColor();
            float labelTotalAlignmentX = Convert.ToSingle(pThemeObject.EventBoxTotal.LabelTotal.AlignmentX);

            int columnDesignationWidth = Convert.ToInt16(pThemeObject.Columns.DesignationWidth);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Always);

            LabelTotalLabel = new Label(GeneralUtils.GetResourceByName("global_total_ticket"));
            LabelTotalLabel.ModifyFont(labelLabelTotalFont);
            LabelTotalLabel.ModifyFg(StateType.Normal, labelLabelTotalFontColor);
            LabelTotalLabel.SetAlignment(labelLabelTotalAlignmentX, 0.0F);

            LabelTotal = new Label();
            LabelTotal.ModifyFont(labelTotalFont);
            LabelTotal.ModifyFg(StateType.Normal, labelTotalFontColor);
            LabelTotal.SetAlignment(labelTotalAlignmentX, 0.0F);
            LabelTotal.Text = DataConversionUtils.DecimalToStringCurrency(0, XPOSettings.ConfigurationSystemCurrency.Acronym);

            HBox hboxTotal = new HBox(false, 4);
            hboxTotal.PackStart(LabelTotalLabel, true, true, 5);
            hboxTotal.PackStart(LabelTotal, false, false, 5);

            EventBox eventBoxTotal = new EventBox() { BorderWidth = 0 };
            eventBoxTotal.ModifyBg(StateType.Normal, eventBoxTotalBackgroundColor);
            eventBoxTotal.Add(hboxTotal);

            ListStore = new ListStore(typeof(Guid),
                                      typeof(string),
                                      typeof(string),
                                      typeof(string),
                                      typeof(string),
                                      typeof(string),
                                      typeof(string),
                                      typeof(ArticleBagKey));

            TreeView = new TreeView(ListStore) { RulesHint = false, CanFocus = false };
            InitColumns(TreeView, columnsFontTitle, columnsFontData, columnDesignationWidth);
            TreeView.CursorChanged += TreeView_CursorChanged;
            ListStore.RowInserted += RowInserted;

            if (POSSession.CurrentSession.OrderMains.Count > 0) UpdateModel();
            scrolledWindow.Add(TreeView);

            UpdateArticleBag();

            VBox vbox = new VBox(false, 0);
            vbox.PackStart(scrolledWindow, true, true, 0);
            vbox.PackStart(eventBoxTotal, false, false, 2);

            PackStart(vbox, true, true, 0);
        }

        private void InitColumns(TreeView pTreeView,
                                 Pango.FontDescription pColumnTitleFontDesc,
                                 Pango.FontDescription pColumnDataFontDesc,
                                 int pWidthDesignation)
        {
            CellRendererText rendererText;
            TreeViewColumn column;
            int sharedWidth = 65;

            int widthDesignation = pWidthDesignation - 10;

            Pango.FontDescription fontDescTitle = pColumnTitleFontDesc;
            Pango.FontDescription fontDesc = pColumnDataFontDesc;

            Label labelDesignation = new Label(GeneralUtils.GetResourceByName("pos_ticketlist_label_designation")) { Visible = true };
            Label labelPrice = new Label(GeneralUtils.GetResourceByName("pos_ticketlist_label_price")) { Visible = true };
            Label labelQuantity = new Label(GeneralUtils.GetResourceByName("pos_ticketlist_label_quantity")) { Visible = true };
            Label labelDiscount = new Label(GeneralUtils.GetResourceByName("pos_ticketlist_label_discount")) { Visible = true };
            Label labelVat = new Label(GeneralUtils.GetResourceByName("pos_ticketlist_label_vat")) { Visible = true };
            Label labelTotal = new Label(GeneralUtils.GetResourceByName("pos_ticketlist_label_total")) { Visible = true };
            labelDesignation.ModifyFont(fontDescTitle);
            labelPrice.ModifyFont(fontDescTitle);
            labelQuantity.ModifyFont(fontDescTitle);
            labelDiscount.ModifyFont(fontDescTitle);
            labelVat.ModifyFont(fontDescTitle);
            labelTotal.ModifyFont(fontDescTitle);

            //TicketListColumns.ArticleId
            rendererText = new CellRendererText() { FontDesc = fontDesc };
            column = new TreeViewColumn("Index", rendererText, "text", 0)
            {
                Visible = false
            };
            pTreeView.AppendColumn(column);

            //TicketListColumns.Designation
            rendererText = new CellRendererText() { FontDesc = fontDesc };
            column = new TreeViewColumn(null, rendererText, "text", TicketListColumns.Designation)
            {
                Widget = labelDesignation,
                MinWidth = widthDesignation,
                MaxWidth = widthDesignation
            };
            pTreeView.AppendColumn(column);

            //TicketListColumns.Price
            rendererText = new CellRendererText() { FontDesc = fontDesc, Xalign = 1.0F };
            column = new TreeViewColumn(null, rendererText, "text", TicketListColumns.Price)
            {
                Widget = labelPrice,
                Alignment = 1.0F,
                MinWidth = sharedWidth,
                MaxWidth = sharedWidth
            };
            pTreeView.AppendColumn(column);

            //TicketListColumns.Quantity
            rendererText = new CellRendererText() { FontDesc = fontDesc, Xalign = 1.0F };
            column = new TreeViewColumn(null, rendererText, "text", TicketListColumns.Quantity)
            {
                Widget = labelQuantity,
                Alignment = 1.0F,
                MinWidth = sharedWidth - 10,
                MaxWidth = sharedWidth - 10
            };
            pTreeView.AppendColumn(column);

            //TicketListColumns.Discount
            rendererText = new CellRendererText() { FontDesc = fontDesc, Xalign = 1.0F };
            column = new TreeViewColumn(null, rendererText, "text", TicketListColumns.Discount)
            {
                Widget = labelDiscount,
                Alignment = 1.0F,
                MinWidth = sharedWidth,
                MaxWidth = sharedWidth,
                Visible = false
            };
            pTreeView.AppendColumn(column);

            //TicketListColumns.Vat
            rendererText = new CellRendererText() { FontDesc = fontDesc, Xalign = 1.0F };
            column = new TreeViewColumn(null, rendererText, "text", TicketListColumns.Vat)
            {
                Widget = labelVat,
                Alignment = 1.0F,
                MinWidth = sharedWidth,
                MaxWidth = sharedWidth,
                Visible = false
            };
            pTreeView.AppendColumn(column);

            //TicketListColumns.Total
            rendererText = new CellRendererText() { FontDesc = fontDesc, Xalign = 1.0F };
            column = new TreeViewColumn(null, rendererText, "text", TicketListColumns.Total)
            {
                Widget = labelTotal,
                Alignment = 1.0F,
                MinWidth = sharedWidth + 10,
                MaxWidth = sharedWidth + 10
            };
            pTreeView.AppendColumn(column);
        }

        public void UpdateModel()
        {
            //Always Init a New Model, and Init Values
            ListStore.Clear();
            SelectedIndex = -1;
            TotalItems = 0;

            //Init Values from SessionApp
            CurrentDocumentId = POSSession.CurrentSession.CurrentOrderMainId;
            CurrentTickedId = POSSession.CurrentSession.OrderMains[CurrentDocumentId].CurrentTicketId;
            CurrentOrderDetail = POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails;

            //Change BackGround Color ListMode
            Gdk.Color colorListMode = (ListMode == TicketListMode.Ticket) ? colorListMode = TicketModeBackgroundColor.ToGdkColor() : colorListMode = DocumentModeBackgroundColor.ToGdkColor();
            TreeView.ModifyBase(StateType.Normal, colorListMode);

            //Ticket Mode
            if (ListMode == TicketListMode.Ticket)
            {
                //Reset TicketList Mode, only if is in TicketList Mode, else keep unchanged
                TotalItemsTicketListMode = 0;

                //Start Append Items to Model
                foreach (OrderDetailLine line in CurrentOrderDetail.Lines)
                {
                    if (line.Properties.Quantity > 0)
                    {
                        ListStore.AppendValues(
                          line.ArticleOid,
                          line.Designation,
                          DataConversionUtils.DecimalToString(line.Properties.PriceFinal),
                          DataConversionUtils.DecimalToString(line.Properties.Quantity),
                          line.Properties.DiscountArticle.ToString(),
                          line.Properties.Vat.ToString(),
                          DataConversionUtils.DecimalToString(line.Properties.TotalFinal)
                        );
                        //Store Current TreeIter in Session (Non Public)
                        line.TreeIter = _treeIter;
                    }

                }
            }
            //OrderMain Mode
            else
            {
                //Update Global Article Bag, from DB
                UpdateArticleBag();

                //Reset listIndex to store Index position in Dictionary
                int listIndex = 0;

                //Loop ArticleBag, and create new FinanceDetail Document lines
                foreach (var item in ArticleBag)
                {
                    ArticleBagKey articleBagKey = new ArticleBagKey(item.Key.ArticleId, item.Key.Designation, item.Key.Price, item.Key.Discount, item.Key.Vat);

                    //Detect and Assign VatExemptionReason to ArticleBak Key
                    if (item.Key.VatExemptionReasonId != null && item.Key.VatExemptionReasonId != Guid.Empty)
                    {
                        articleBagKey.VatExemptionReasonId = item.Key.VatExemptionReasonId;
                    }
                    if (item.Value.Quantity > 0)
                    {
                        ListStore.AppendValues(
                        item.Key.ArticleId,
                        item.Key.Designation,
                        DataConversionUtils.DecimalToString(item.Value.PriceFinal),
                        DataConversionUtils.DecimalToString(item.Value.Quantity),
                        item.Key.Discount.ToString(),
                        item.Key.Vat.ToString(),
                        DataConversionUtils.DecimalToString(item.Value.TotalFinal),
                        //Add ArticleBag Key to Model
                        articleBagKey
                        );
                        //Store Current TreeIter in Session (Non Public)
                        item.Value.TreeIter = _treeIter;
                        //Assign / Increment listIndex
                        item.Value.ListIndex = listIndex++;
                    }


                }
            }

            //Required Always Start in Last Record, After initialize TreeView Model
            TreeView.SetCursor(TreePath, null, false);
            TreeView.ScrollToCell(TreePath, TreeView.Columns[0], false, 0, 0);

            //Init _currentDetailArticleOid, If Model is Empty the Value 0 is Assigned
            if (ListStore.Data.Count > 0) CurrentDetailArticleId = (Guid)ListStore.GetValue(_treeIter, 0);

            UpdateSaleOptionsPanelButtons();
            UpdateTicketListTotal();

        }

        public void TicketDecrease()
        {
            if (ListMode == TicketListMode.OrderMain)
            {
                OrderDetailLine newLine;
                CurrentDocumentId = POSSession.CurrentSession.CurrentOrderMainId;
                CurrentTickedId = POSSession.CurrentSession.OrderMains[CurrentDocumentId].CurrentTicketId;
                CurrentOrderDetail = POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails;
                int currentTicketAux = CurrentTickedId;
                bool newArticleLine = true;

                if (POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines.Count == 0)
                {
                    POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails = new OrderDetail();

                    if (CurrentDetailArticle != null)
                    {
                        newLine = new OrderDetailLine(CurrentDetailArticle.Oid, CurrentDetailArticle.Designation,
                            ArticleUtils.GetArticlePrice(XPOUtility.GetEntityById<fin_article>(CurrentDetailArticle.Oid),
                            (AppOperationModeSettings.AppMode == AppOperationMode.Retail) ? TaxSellType.TakeAway : TaxSellType.Normal));

                        newLine.Properties.PriceNet = Convert.ToDecimal((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Price));
                        newLine.Properties.TotalFinal = Convert.ToDecimal((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Total)) - Convert.ToDecimal((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Price));

                        newLine.Properties.Quantity = -1;

                        POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines.Add(newLine);
                    }
                }
                else
                {
                    if (CurrentDetailArticle != null)
                    {
                        for (int i = 0; i < POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines.Count; i++)
                        {
                            if (POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].ArticleOid == CurrentDetailArticle.Oid &&
                                POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].Properties.PriceNet == POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].Properties.PriceNet)
                            {
                                POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].Properties.Quantity = -1;
                                POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].Properties.PriceNet = POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].Properties.PriceFinal;
                                POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].Properties.TotalFinal = (POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].Properties.TotalFinal
                                                                                                                                                                       - (POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines[i].Properties.PriceNet));
                                newArticleLine = false;
                            }
                        }
                        if (newArticleLine)
                        {

                            if (CurrentDetailArticle != null)
                            {

                                newLine = new OrderDetailLine(CurrentDetailArticle.Oid, CurrentDetailArticle.Designation,
                                    ArticleUtils.GetArticlePrice(XPOUtility.GetEntityById<fin_article>(CurrentDetailArticle.Oid),
                                    (AppOperationModeSettings.AppMode == AppOperationMode.Retail) ? TaxSellType.TakeAway : TaxSellType.Normal));

                                newLine.Properties.PriceNet = Convert.ToDecimal((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Price));
                                newLine.Properties.TotalFinal = Convert.ToDecimal((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Total)) - Convert.ToDecimal((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Price));

                                newLine.Properties.Quantity = -1;
                                POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails.Lines.Add(newLine);
                            }
                        }
                    }
                }


                POSSession.CurrentSession.OrderMains[CurrentDocumentId].FinishOrder(XPOSettings.Session, false, true);
                SaleOptionsPanel.BtnFinishOrder.Sensitive = true;
            }

        }

        public void InsertOrUpdate(string pArticleEAN)
        {
            //public static XPGuidObject GetXPGuidObjectFromCriteria(Session pSession, Type pXPGuidObjectType, string pCriteriaFilter)
            string sql = string.Format("SELECT Oid FROM fin_article WHERE BarCode = '{0}';", pArticleEAN);
            Guid articleGuid = XPOUtility.GetGuidFromQuery(sql);

            if (articleGuid != Guid.Empty)
            {
                InsertOrUpdate(articleGuid);
            }
            else
            {
                sql = string.Format("SELECT Oid FROM fin_article WHERE Code = '{0}';", pArticleEAN);
                articleGuid = XPOUtility.GetGuidFromQuery(sql);
                if (articleGuid != Guid.Empty)
                {
                    InsertOrUpdate(articleGuid);
                }
                else
                {
                    string message = GeneralUtils.GetResourceByName("global_invalid_code");
                    logicpos.Utils.ShowMessageBox(SourceWindow, DialogFlags.DestroyWithParent, new Size(400, 300), MessageType.Error, ButtonsType.Ok, "Código Inválido", message);
                    return;
                }
            }
        }

        public void ArticleNotFound()
        {
            string message = GeneralUtils.GetResourceByName("global_invalid_code");
            logicpos.Utils.ShowMessageBox(SourceWindow, DialogFlags.DestroyWithParent, new Size(400, 300), MessageType.Error, ButtonsType.Ok, "Código Inválido", message);
            return;
        }

        public void WsNotFound()
        {
            string message = string.Format("O Web service não se encontra em funcionamento");
            logicpos.Utils.ShowMessageBox(SourceWindow, DialogFlags.DestroyWithParent, new Size(400, 300), MessageType.Error, ButtonsType.Ok, "Web Service não encontrado", message);
            return;
        }


        public void InsertOrUpdate(Guid pArticleOid)
        {
            InsertOrUpdate(pArticleOid, new ParkingTicketResult());
        }

        public void InsertOrUpdate(Guid pArticleOid, ParkingTicketResult parkingTicketResult)
        {
            bool requireToChooseVatExemptionReason = true;

            requireToChooseVatExemptionReason = AppSettings.Instance.requireToChooseVatExemptionReason;

            fin_article article = XPOUtility.GetEntityById<fin_article>(pArticleOid);

            article.Reload();

            string originalDesignation = article.Designation;

            string ean = parkingTicketResult.Ean;
            bool isAppUseParkingTicketModule = GeneralSettings.AppUseParkingTicketModule && !string.IsNullOrEmpty(ean);

            if (isAppUseParkingTicketModule)
            {
                if (parkingTicketResult.Ean.Length == 13)
                {
                    string designation = article.Designation;

                    int index = designation.LastIndexOf("[");
                    if (index > 0)
                        designation = designation.Substring(0, index);
                    article.Designation = $"{article.Designation} [{parkingTicketResult.Minutes} min.] [{parkingTicketResult.Ean}]";
                    article.Notes = $"[{ean}] {parkingTicketResult.Description}";
                    article.DefaultQuantity = Convert.ToInt32(parkingTicketResult.Quantity);
                    GlobalApp.PosMainWindow.UpdateWorkSessionUI();
                }
                else
                {
                    string designation = article.Designation;

                    int index = designation.LastIndexOf("[");
                    if (index > 0)
                        designation = designation.Substring(0, index);

                    article.Designation = $"{designation}[{parkingTicketResult.Ean}]";
                    article.Notes = $"[{ean}]";
                    article.DefaultQuantity = Convert.ToInt32(parkingTicketResult.Quantity);
                }

                SelectedIndex = -1;
            }
            else
            {
                SelectedIndex = CurrentOrderDetail.Lines.FindIndex(item => item.ArticleOid == pArticleOid);
            };

            decimal defaultQuantity = GetArticleDefaultQuantity(pArticleOid);
            if (GeneralSettings.AppUseParkingTicketModule) { defaultQuantity = article.DefaultQuantity; if (defaultQuantity == 0) defaultQuantity = 1; }
            else { defaultQuantity = GetArticleDefaultQuantity(pArticleOid); }
            decimal price = 0.0m;

            if (requireToChooseVatExemptionReason && article.VatDirectSelling.Oid == InvoiceSettings.XpoOidConfigurationVatRateDutyFree && article.VatExemptionReason == null)
            {
                logicpos.Utils.ShowMessageBox(SourceWindow, DialogFlags.DestroyWithParent, new Size(400, 300), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("window_title_dialog_vatrate_free_article_detected"), GeneralUtils.GetResourceByName("dialog_message_article_without_vat_exception_reason_detected"));
                return;
            }

            if (parkingTicketResult.AlreadyExit)
            {
                string message = string.Format("Numero do ticket: {0}\n\n{1}\n\nData de Saida: {2}", parkingTicketResult.Ean, GeneralUtils.GetResourceByName("dialog_message_article_already_exited"), parkingTicketResult.DateExits);
                logicpos.Utils.ShowMessageBox(SourceWindow, DialogFlags.DestroyWithParent, new Size(450, 350), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("window_title_dialog_already_exited"), message);
                return;
            }

            else if (parkingTicketResult.AlreadyPaid)
            {
                string message = string.Format("Numero do ticket: {0}\n\n{1}\nData de pagamento: {2}\n\nPode sair até: {3} ", parkingTicketResult.Ean, GeneralUtils.GetResourceByName("dialog_message_article_already_paid"), parkingTicketResult.DatePaid, parkingTicketResult.DateTolerance);
                logicpos.Utils.ShowMessageBox(SourceWindow, DialogFlags.DestroyWithParent, new Size(450, 350), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("window_title_dialog_already_paid"), message);
                return;
            }

            if (SelectedIndex != -1 && GeneralSettings.AppUseParkingTicketModule && isAppUseParkingTicketModule)
            {
                CurrentOrderDetail.Update(SelectedIndex, CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity += defaultQuantity);

                _treeIter = (TreeIter)CurrentOrderDetail.Lines[SelectedIndex].TreeIter;
                ListStore.SetValue(_treeIter, (int)TicketListColumns.Quantity, DataConversionUtils.DecimalToString(CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity));
                ListStore.SetValue(_treeIter, (int)TicketListColumns.Total, DataConversionUtils.DecimalToString(CurrentOrderDetail.Lines[SelectedIndex].Properties.TotalFinal));
            }
            else
            {
                bool showMessage;
                if (logicpos.Utils.CheckStocks())
                {
                    if (!logicpos.Utils.ShowMessageMinimumStock(SourceWindow, pArticleOid, Convert.ToDecimal(ListStore.GetValue(_treeIter, (int)TicketListColumns.Quantity)) + defaultQuantity, out showMessage))
                    {
                        if (showMessage) return;
                    }
                }
                OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                pos_configurationplace configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), currentOrderMain.Table.PlaceId);
                fin_articletype articletype = (fin_articletype)XPOSettings.Session.GetObjectByKey(typeof(fin_articletype), article.Type.Oid);

                if (configurationPlace == null) { configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), POSSettings.XpoOidConfigurationPlaceTableDefaultOpenTable); }
                TaxSellType taxSellType = (AppOperationModeSettings.AppMode == AppOperationMode.Retail || configurationPlace.MovementType.VatDirectSelling) ? TaxSellType.TakeAway : TaxSellType.Normal;
                decimal priceTax = (taxSellType == TaxSellType.Normal) ? article.VatOnTable.Value : article.VatDirectSelling.Value;

                PriceProperties priceProperties = ArticleUtils.GetArticlePrice(article, taxSellType);
                price = priceProperties.PriceFinal;
                PricePropertiesSourceMode sourceMode;

                if (isAppUseParkingTicketModule)
                {
                    sourceMode = PricePropertiesSourceMode.FromTotalFinal;
                    price = parkingTicketResult.Price;
                    string message = string.Empty;
                    if (parkingTicketResult.Ean.Length == 13)
                    {
                        message = string.Format(" Numero do ticket: {0}\n\n Data de Emissão: {1}\n Duração: {2} minuto(s)\n Descrição: {3}\n", parkingTicketResult.Ean, parkingTicketResult.Date, parkingTicketResult.Minutes, parkingTicketResult.Description);
                        logicpos.Utils.ShowMessageBox(SourceWindow, DialogFlags.DestroyWithParent, new Size(480, 350), MessageType.Info, ButtonsType.Ok, "Ticket Details", message);
                    }
                    else
                    {
                        message = string.Format(" Numero do cartão: {0} \n", parkingTicketResult.Ean);
                        logicpos.Utils.ShowMessageBox(SourceWindow, DialogFlags.DestroyWithParent, new Size(480, 350), MessageType.Info, ButtonsType.Ok, "Ticket Details", message);
                    }

                }
                else if (!articletype.HavePrice)
                {
                    sourceMode = PricePropertiesSourceMode.FromPriceNet;
                    price = 0.00m;
                    article.Designation = string.Format(articletype.Designation + " : " + originalDesignation);
                }
                else if (price <= 0.0m || article.PVPVariable == true)
                {
                    MoneyPadResult result = PosMoneyPadDialog.RequestDecimalValue(SourceWindow, GeneralUtils.GetResourceByName("window_title_dialog_moneypad_product_price"), price);
                    if (result.Response == ResponseType.Cancel) return;
                    sourceMode = PricePropertiesSourceMode.FromTotalFinal;
                    price = result.Value;
                }
                else
                {
                    sourceMode = PricePropertiesSourceMode.FromPriceNet;
                    price = priceProperties.PriceNet;
                }

                if (price <= 0.0m && articletype.HavePrice)
                {
                    throw new Exception("defaultPrice <= 0.0m");
                }

                priceProperties = PriceProperties.GetPriceProperties(
                  sourceMode,
                  article.PriceWithVat,
                  price,
                  1.0m,
                  article.Discount,
                  POSSession.GetGlobalDiscount(),
                  priceTax
                );

                if (article.VatExemptionReason != null) priceProperties.VatExemptionReason = article.VatExemptionReason.Oid;
                int i = 0;
                foreach (var item in CurrentOrderDetail.Lines)
                {
                    if (item.ArticleOid == pArticleOid && item.Properties.PriceFinal == priceProperties.PriceFinal)
                    {
                        SelectedIndex = i;
                        break;
                    }
                    i++;
                }

                bool itemInTicket = false;
                decimal newQuantity = 0.0m;
                decimal newTotalPrice = 0.0m;

                ListStore.GetIterFirst(out _treeIter);
                for (int itemPosition = 0; itemPosition < ListStore.IterNChildren(); itemPosition++)
                {
                    if ((Guid)(ListStore.GetValue(_treeIter, (int)TicketListColumns.ArticleId)) == pArticleOid &&
                            Convert.ToDecimal(ListStore.GetValue(_treeIter, (int)TicketListColumns.Price)) == Math.Round(priceProperties.PriceFinal, CultureSettings.DecimalRoundTo))
                    {

                        SelectedIndex = itemPosition;

                        //Update TreeView Model Price and quantity
                        newQuantity = Convert.ToDecimal(ListStore.GetValue(_treeIter, (int)TicketListColumns.Quantity)) + defaultQuantity;
                        newTotalPrice = newQuantity * Convert.ToDecimal(CurrentOrderDetail.Lines[SelectedIndex].Properties.PriceFinal);

                        //Update orderDetails 
                        CurrentOrderDetail.Update(SelectedIndex, newQuantity, Convert.ToDecimal(CurrentOrderDetail.Lines[SelectedIndex].Properties.PriceUser));


                        //_listStoreModel.SetValue(_treeIter, (int)TicketListColumns.Price, LogicPOS.Utility.DataConversionUtils.DecimalToString(newPrice));
                        ListStore.SetValue(_treeIter, (int)TicketListColumns.Quantity, DataConversionUtils.DecimalToString(newQuantity));

                        //Update Total
                        ListStore.SetValue(_treeIter, (int)TicketListColumns.Total, DataConversionUtils.DecimalToString(newTotalPrice));

                        CurrentOrderDetail.Lines[SelectedIndex].TreeIter = _treeIter;

                        itemInTicket = true;

                        break;
                    }
                    ListStore.IterNext(ref _treeIter);
                }
                //Update Total Items Member
                TotalItems = ListStore.IterNChildren();

                if (!itemInTicket)
                {
                    //Insert into orderDetails SessionApp
                    CurrentOrderDetail.Insert(article.Oid, article.Designation, priceProperties);

                    //Insert into TreeView Model : [0]ArticleId,[1]Designation,[2]Price,[3]Quantity,[4]Discount,[5]Vat,[6]Total
                    object[] columnValues = new object[Enum.GetNames(typeof(TicketListColumns)).Length];
                    columnValues[0] = article.Oid;
                    columnValues[1] = article.Designation;
                    columnValues[2] = DataConversionUtils.DecimalToString(CurrentOrderDetail.Lines[CurrentOrderDetail.Lines.Count - 1].Properties.PriceFinal);
                    columnValues[3] = DataConversionUtils.DecimalToString(defaultQuantity);
                    columnValues[4] = CurrentOrderDetail.Lines[CurrentOrderDetail.Lines.Count - 1].Properties.DiscountArticle.ToString();
                    columnValues[5] = CurrentOrderDetail.Lines[CurrentOrderDetail.Lines.Count - 1].Properties.Vat.ToString();
                    columnValues[6] = DataConversionUtils.DecimalToString(CurrentOrderDetail.Lines[CurrentOrderDetail.Lines.Count - 1].Properties.TotalFinal);

                    //Assign Global TreeIter
                    _treeIter = ListStore.AppendValues(columnValues);

                    //Assign SessionApp TreeIter, get ArticleId with LINQ
                    SelectedIndex = CurrentOrderDetail.Lines.FindIndex(item => item.ArticleOid == pArticleOid);
                    CurrentOrderDetail.Lines[SelectedIndex].TreeIter = _treeIter;
                    if (GeneralSettings.AppUseParkingTicketModule) { CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity = defaultQuantity; }
                }
            };

            //Shared SetCursor
            TreeView.SetCursor(ListStore.GetPath(_treeIter), null, false);
            TreeView.ScrollToCell(ListStore.GetPath(_treeIter), null, false, 0, 0);

            //Update Total
            UpdateTicketListTotal();

        }

        public void ChangeQuantity(decimal pQuantity)
        {
            SelectedIndex = CurrentOrderDetail.Lines.FindIndex(item => item.ArticleOid == (Guid)ListStore.GetValue(_treeIter,
                    (int)TicketListColumns.ArticleId));

            decimal oldValueQnt = CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity;

            if (oldValueQnt != pQuantity)
            {
                CurrentOrderDetail.Update(SelectedIndex, pQuantity);
                ListStore.SetValue(_treeIter, (int)TicketListColumns.Quantity, DataConversionUtils.DecimalToString(pQuantity));
                CurrentOrderDetail.Lines[CurrentOrderDetail.Lines.Count - 1].Properties.Quantity = pQuantity;
                ListStore.SetValue(_treeIter, (int)TicketListColumns.Total, DataConversionUtils.DecimalToString(CurrentOrderDetail.Lines[SelectedIndex].Properties.TotalFinal));
                UpdateTicketListTotal();
                if (GeneralSettings.AppUseParkingTicketModule) CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity = pQuantity; ;
            }
        }

        public void DeleteItem_Event(TicketListDeleteMode pMode)
        {
            if (ListMode == TicketListMode.Ticket)
            {
                if (pMode == TicketListDeleteMode.Decrease)
                {
                    DeleteItem_ListModeTicket();
                }
                else if (pMode == TicketListDeleteMode.Delete)
                {
                    CurrentOrderDetail.Update(SelectedIndex, CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity -= CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity);

                    if (CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity <= 0)
                    {
                        CurrentOrderDetail.Delete(SelectedIndex);

                        ListStore.Remove(ref _treeIter);

                        TotalItems = ListStore.IterNChildren();

                        if (ListMode == TicketListMode.Ticket) TotalItemsTicketListMode = ListStore.IterNChildren();

                        Previous();

                        UpdateModel();

                        UpdateSaleOptionsPanelButtons();
                    }
                }
                else
                {
                    DeleteItem_ListModeOrderMain();
                }

            }
            else if (ListMode == TicketListMode.OrderMain)
            {
                DeleteItem_ListModeOrderMain();
            }

        }

        public void DeleteItem_ListModeTicket()
        {
            CurrentDocumentId = POSSession.CurrentSession.CurrentOrderMainId;
            CurrentTickedId = POSSession.CurrentSession.OrderMains[CurrentDocumentId].CurrentTicketId;
            CurrentOrderDetail = POSSession.CurrentSession.OrderMains[CurrentDocumentId].OrderTickets[CurrentTickedId].OrderDetails;
            decimal defaultQuantity = GetArticleDefaultQuantity(CurrentDetailArticleId);
            DeleteItem_ListModeTicket(defaultQuantity);
        }

        public void DeleteItem_ListModeTicket(decimal pRemoveQuantity)
        {
            var quantity = ((string)ListStore.GetValue(_treeIter, (int)TicketListColumns.Quantity)).Replace('.', ',');

            decimal totalQtd = Convert.ToDecimal(quantity) - pRemoveQuantity;

            CurrentOrderDetail.Update(SelectedIndex, totalQtd);

            if (CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity <= 0)
            {
                CurrentOrderDetail.Delete(SelectedIndex);
                ListStore.Remove(ref _treeIter);
                TotalItems = ListStore.IterNChildren();

                if (ListMode == TicketListMode.Ticket) TotalItemsTicketListMode = ListStore.IterNChildren();

                Previous();
                UpdateSaleOptionsPanelButtons();
            }
            else
            {
                ListStore.SetValue(_treeIter, (int)TicketListColumns.Quantity, DataConversionUtils.DecimalToString(CurrentOrderDetail.Lines[SelectedIndex].Properties.Quantity));
                ListStore.SetValue(_treeIter, (int)TicketListColumns.Total, DataConversionUtils.DecimalToString(CurrentOrderDetail.Lines[SelectedIndex].Properties.TotalFinal));
            }

            UpdateTicketListTotal();
        }

        public void DeleteItem_ListModeOrderMain()
        {
            decimal defaultQuantity = GetArticleDefaultQuantity(CurrentDetailArticleId);
            DeleteItem_ListModeOrderMain(defaultQuantity);
            UpdateArticleBag();
        }

        public void DeleteItem_ListModeOrderMain(decimal pRemoveQuantity)
        {
            bool canDeleteItem = true;

            ArticleBagKey articleBagKey = (ArticleBagKey)ListStore.GetValue(_treeIter, 7);

            if (canDeleteItem)
            {
                decimal currentTotalQuantity = ArticleBag.DeleteFromDocumentOrder(articleBagKey, pRemoveQuantity);

                decimal currentTotalFinal = currentTotalQuantity * ArticleBag[articleBagKey].PriceFinal;

                TicketDecrease();

                if (currentTotalQuantity <= 0)
                {
                    ListStore.Remove(ref _treeIter);

                    TotalItems = ListStore.IterNChildren();

                    Previous();

                    UpdateSaleOptionsPanelButtons();
                }
                else
                {
                    ListStore.SetValue(_treeIter, (int)TicketListColumns.Quantity, DataConversionUtils.DecimalToString(currentTotalQuantity));

                    ListStore.SetValue(_treeIter, (int)TicketListColumns.Total, DataConversionUtils.DecimalToString(currentTotalFinal));
                }

                UpdateOrderStatusBar();
            }
        }

        public void Previous()
        {
            if (TreePath is null) return;
            TreePath.Prev();
            TreeView.SetCursor(TreePath, null, false);

        }

        public void Next()
        {
            if (TreePath is null) return;
            TreePath.Next();
            TreeView.SetCursor(TreePath, null, false);

        }

        public void UpdateArticleBag()
        {

            if (POSSession.CurrentSession.CurrentOrderMainId != Guid.Empty && POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId))
            {
                ArticleBag = ArticleBag.TicketOrderToArticleBag(POSSession.CurrentSession.OrderMains[CurrentDocumentId]);
            }
        }

        public void UpdateSaleOptionsPanelButtons()
        {
            if (TotalItems == 0)
            {
                if (SaleOptionsPanel.BtnIncrease != null && SaleOptionsPanel.BtnIncrease.Sensitive) SaleOptionsPanel.BtnIncrease.Sensitive = false;
                if (SaleOptionsPanel.BtnDecrease != null && SaleOptionsPanel.BtnDecrease.Sensitive) SaleOptionsPanel.BtnDecrease.Sensitive = false;
                if (SaleOptionsPanel.BtnDelete != null && SaleOptionsPanel.BtnDelete.Sensitive) SaleOptionsPanel.BtnDelete.Sensitive = false;
                if (SaleOptionsPanel.BtnQuantity != null && SaleOptionsPanel.BtnQuantity.Sensitive) SaleOptionsPanel.BtnQuantity.Sensitive = false;
                if (SaleOptionsPanel.BtnPrice != null && SaleOptionsPanel.BtnPrice.Sensitive) SaleOptionsPanel.BtnPrice.Sensitive = false;
                if (SaleOptionsPanel.BtnWeight != null && SaleOptionsPanel.BtnWeight.Sensitive) SaleOptionsPanel.BtnWeight.Sensitive = false;
                if (SaleOptionsPanel.BtnSplitAccount != null && SaleOptionsPanel.BtnSplitAccount.Sensitive) SaleOptionsPanel.BtnSplitAccount.Sensitive = false;
            }
            else
            {
                if (ListMode == TicketListMode.Ticket && SelectedIndex > -1)
                {
                    if (SaleOptionsPanel.BtnIncrease != null && !SaleOptionsPanel.BtnIncrease.Sensitive) SaleOptionsPanel.BtnIncrease.Sensitive = true;
                    if (SaleOptionsPanel.BtnDecrease != null && !SaleOptionsPanel.BtnDecrease.Sensitive) SaleOptionsPanel.BtnDecrease.Sensitive = true;

                    if (SaleOptionsPanel.BtnDelete != null && SelectedIndex > -1) SaleOptionsPanel.BtnDelete.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("TICKETLIST_DELETE");
                    if (SaleOptionsPanel.BtnPrice != null && !SaleOptionsPanel.BtnPrice.Sensitive) SaleOptionsPanel.BtnPrice.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("TICKETLIST_CHANGE_PRICE");
                    if (SaleOptionsPanel.BtnQuantity != null && !SaleOptionsPanel.BtnQuantity.Sensitive) SaleOptionsPanel.BtnQuantity.Sensitive = true;
                    if (SaleOptionsPanel.BtnWeight != null) SaleOptionsPanel.BtnWeight.Sensitive = (GlobalApp.WeighingBalance != null && GlobalApp.WeighingBalance.IsPortOpen() && CurrentDetailArticle.UseWeighingBalance);
                    if (SaleOptionsPanel.BtnSplitAccount != null && !SaleOptionsPanel.BtnSplitAccount.Sensitive) SaleOptionsPanel.BtnSplitAccount.Sensitive = (ArticleBag.Count > 1 && ArticleBag.TotalFinal > 0.00m);
                }
                else
                {
                    if (SaleOptionsPanel.BtnIncrease != null && SaleOptionsPanel.BtnIncrease.Sensitive) SaleOptionsPanel.BtnIncrease.Sensitive = false;
                    if (SaleOptionsPanel.BtnDecrease != null && SaleOptionsPanel.BtnDecrease.Sensitive) SaleOptionsPanel.BtnDecrease.Sensitive = false;

                    if (SaleOptionsPanel.BtnDelete != null && SelectedIndex > -1) SaleOptionsPanel.BtnDelete.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("TICKETLIST_DELETE");
                    if (SaleOptionsPanel.BtnPrice != null && SaleOptionsPanel.BtnPrice.Sensitive) SaleOptionsPanel.BtnPrice.Sensitive = false;
                    if (SaleOptionsPanel.BtnQuantity != null && SaleOptionsPanel.BtnQuantity.Sensitive) SaleOptionsPanel.BtnQuantity.Sensitive = false;
                    if (SaleOptionsPanel.BtnWeight != null && SaleOptionsPanel.BtnWeight.Sensitive) SaleOptionsPanel.BtnWeight.Sensitive = false;
                    if (SaleOptionsPanel.BtnSplitAccount != null && SaleOptionsPanel.BtnSplitAccount.Sensitive) SaleOptionsPanel.BtnSplitAccount.Sensitive = (ArticleBag.Count > 1 && ArticleBag.TotalFinal > 0.00m); ;
                }
            };

            if (TotalItemsTicketListMode == 0)
            {
                if (SaleOptionsPanel.BtnFinishOrder != null && SaleOptionsPanel.BtnFinishOrder.Sensitive) SaleOptionsPanel.BtnFinishOrder.Sensitive = false;


                if (_toolbarBackOffice != null) _toolbarBackOffice.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_ACCESS");
                if (_toolbarReports != null) _toolbarReports.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("REPORT_ACCESS");
                if (_toolbarShowSystemDialog != null && !_toolbarShowSystemDialog.Sensitive) _toolbarShowSystemDialog.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("SYSTEM_ACCESS");
                if (_toolbarLogoutUser != null && !_toolbarLogoutUser.Sensitive) _toolbarLogoutUser.Sensitive = true;
                if (_toolbarShowChangeUserDialog != null && !_toolbarShowChangeUserDialog.Sensitive) _toolbarShowChangeUserDialog.Sensitive = true;
                if (_toolbarCashDrawer != null) _toolbarCashDrawer.Sensitive = (GeneralSettings.LoggedUserHasPermissionTo("WORKSESSION_ALL"));
                if (_toolbarFinanceDocuments != null && !_toolbarFinanceDocuments.Sensitive) _toolbarFinanceDocuments.Sensitive = true;

                if (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
                {
                    if (_toolbarNewFinanceDocument != null && !_toolbarNewFinanceDocument.Sensitive) _toolbarNewFinanceDocument.Sensitive = true;
                }
            }
            else
            {
                if (SaleOptionsPanel.BtnFinishOrder != null && !SaleOptionsPanel.BtnFinishOrder.Sensitive) SaleOptionsPanel.BtnFinishOrder.Sensitive = true;

                if (_toolbarBackOffice != null && _toolbarBackOffice.Sensitive) _toolbarBackOffice.Sensitive = false;
                if (_toolbarReports != null && _toolbarReports.Sensitive) _toolbarReports.Sensitive = false;
                if (_toolbarShowSystemDialog != null && _toolbarShowSystemDialog.Sensitive) _toolbarShowSystemDialog.Sensitive = false;
                if (_toolbarLogoutUser != null && _toolbarLogoutUser.Sensitive) _toolbarLogoutUser.Sensitive = false;
                if (_toolbarShowChangeUserDialog != null && _toolbarShowChangeUserDialog.Sensitive) _toolbarShowChangeUserDialog.Sensitive = false;
                if (_toolbarCashDrawer != null && _toolbarCashDrawer.Sensitive) _toolbarCashDrawer.Sensitive = false;
                if (_toolbarFinanceDocuments != null && _toolbarFinanceDocuments.Sensitive) _toolbarFinanceDocuments.Sensitive = false;
                if (_toolbarNewFinanceDocument != null && _toolbarNewFinanceDocument.Sensitive) _toolbarNewFinanceDocument.Sensitive = false;
            }

            UpdateSaleOptionsPanelOrderButtons();
        }

        public void UpdateSaleOptionsPanelOrderButtons()
        {
            if (SaleOptionsPanel.BtnPayments != null)
            {
                if (POSSession.CurrentSession.CurrentOrderMainId != Guid.Empty && POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId))
                {
                    SaleOptionsPanel.BtnBarcode.Sensitive = true;

                    if (TotalItemsTicketListMode > 0 &&
                            CurrentOrderDetail.TotalFinal > 0.00m)
                    {
                        SaleOptionsPanel.BtnPayments.Sensitive = true;
                        SaleOptionsPanel.BtnSplitAccount.Sensitive = true;
                        SaleOptionsPanel.BtnChangeTable.Sensitive = false;
                        if (POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].OrderStatus == OrderStatus.Open)
                        {
                            SaleOptionsPanel.BtnListOrder.Sensitive = true;
                        }
                        else
                        {
                            SaleOptionsPanel.BtnListOrder.Sensitive = false;
                        }
                    }
                    else if (TotalItemsTicketListMode == 0)
                    {
                        if (POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].OrderStatus == OrderStatus.Open &&
                            ArticleBag.TotalFinal > 0.00m)
                        {
                            SaleOptionsPanel.BtnPayments.Sensitive = true;
                            SaleOptionsPanel.BtnSplitAccount.Sensitive = true;
                            SaleOptionsPanel.BtnChangeTable.Sensitive = true;
                            SaleOptionsPanel.BtnListOrder.Sensitive = true;
                        }
                        else
                        {
                            SaleOptionsPanel.BtnPayments.Sensitive = false;
                            SaleOptionsPanel.BtnSplitAccount.Sensitive = false;
                            SaleOptionsPanel.BtnChangeTable.Sensitive = false;
                            SaleOptionsPanel.BtnListOrder.Sensitive = false;
                        }
                    }
                }
                else
                {
                    SaleOptionsPanel.BtnPayments.Sensitive = false;
                    SaleOptionsPanel.BtnSplitAccount.Sensitive = false;
                    SaleOptionsPanel.BtnChangeTable.Sensitive = false;
                    SaleOptionsPanel.BtnListOrder.Sensitive = false;
                    SaleOptionsPanel.BtnBarcode.Sensitive = false;
                }

                if (ListMode == TicketListMode.Ticket) UpdateArticleBag();

                if (ArticleBag != null && ArticleBag.Count > 0)
                {
                    SaleOptionsPanel.BtnListMode.Sensitive = true;
                }
                else
                {
                    SaleOptionsPanel.BtnListMode.Sensitive = false;
                }
            }
        }

        private void UpdateTicketListTotal()
        {
            decimal TotalFinal;
            string labelTotalFinal;

            if (ListMode == TicketListMode.Ticket)
            {
                labelTotalFinal = GeneralUtils.GetResourceByName("global_total_ticket");
                TotalFinal = CurrentOrderDetail.TotalFinal;
            }
            else
            {
                labelTotalFinal = GeneralUtils.GetResourceByName("global_total_table_tickets");

                TotalFinal = ArticleBag.TotalFinal;
            }
            LabelTotalLabel.Text = labelTotalFinal;
            LabelTotal.Text = DataConversionUtils.DecimalToStringCurrency(TotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym);

            if (GlobalApp.UsbDisplay != null) GlobalApp.UsbDisplay.ShowOrder(CurrentOrderDetail, SelectedIndex);
        }

        public void UpdateOrderStatusBar()
        {
            if (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
            {
                if (
                    POSSession.CurrentSession.OrderMains != null &&
                    POSSession.CurrentSession.CurrentOrderMainId != null &&
                    POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId)
                )
                {
                    OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                    orderMain.UpdateTotals();

                    string labelCurrentTableFormat = "{0} {1} ({2}%) {3}";
                    string labelTotalTableFormat = "{0} : #{1}";
                    string lastUserName = (orderMain != null && orderMain.GlobalLastUser != null) ? string.Format(": {0}", orderMain.GlobalLastUser.Name) : string.Empty;

                    string global_table = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("global_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower());

                    SourceWindow.LabelCurrentTable.Text =
                      string.Format(labelCurrentTableFormat
                        , global_table
                        , orderMain.Table.Name
                        , POSSession.GetGlobalDiscount()
                        , lastUserName
                        );

                    SourceWindow.LabelTotalTable.Text =
                      string.Format(labelTotalTableFormat,

                      DataConversionUtils.DecimalToStringCurrency(orderMain.GlobalTotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym),
                        orderMain.GlobalTotalTickets
                      );

                    if (ListMode == TicketListMode.OrderMain)
                        LabelTotal.Text = DataConversionUtils.DecimalToStringCurrency(orderMain.GlobalTotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym);
                }
                else
                {
                    SourceWindow.LabelCurrentTable.Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("status_message_select_order_or_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower());
                }
            }
            else
            {
                SourceWindow.LabelCurrentTable.Text = GeneralUtils.GetResourceByName("status_message_open_cashdrawer");
            }
        }

        private decimal GetArticleDefaultQuantity(Guid pArticleOid)
        {
            fin_article article = XPOUtility.GetEntityById<fin_article>(pArticleOid);
            decimal defaultQuantity;
            if (article.DefaultQuantity > 0) { defaultQuantity = article.DefaultQuantity; } else { defaultQuantity = 1.00m; };

            return defaultQuantity;
        }

        private void RowInserted(object o, RowInsertedArgs args)
        {
            ListStore listStore = (ListStore)o;
            _treeIter = args.Iter;
            TreePath = listStore.GetPath(args.Iter);

            TotalItems = listStore.IterNChildren();
            if (ListMode == TicketListMode.Ticket) TotalItemsTicketListMode = listStore.IterNChildren();

            SelectedIndex = TotalItems - 1;
        }

        private void TreeView_CursorChanged(object sender, EventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            TreeSelection selection = treeView.Selection;
            TreeModel model;

            if (selection.GetSelected(out model, out _treeIter))
            {
                TreePath = model.GetPath(_treeIter);

                CurrentDetailArticleId = (Guid)ListStore.GetValue(_treeIter, 0);
                CurrentDetailArticle = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), CurrentDetailArticleId);

                if (ListMode == TicketListMode.Ticket)
                {
                    SelectedIndex = CurrentOrderDetail.Lines.FindIndex(item => item.ArticleOid == CurrentDetailArticleId);
                }
                else
                {
                    ArticleBagKey articleBagKey = (ArticleBagKey)ListStore.GetValue(_treeIter, 7);
                    if (articleBagKey != null)
                    {
                        SelectedIndex = ArticleBag[articleBagKey].ListIndex;
                    }
                }

            }
            else
            {
                SelectedIndex = -1;
            }

            UpdateSaleOptionsPanelButtons();
        }
    }
}
