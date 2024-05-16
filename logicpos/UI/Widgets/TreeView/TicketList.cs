using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Logic.Others;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using logicpos.shared.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using LogicPOS.Settings.Extensions;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.Orders;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public partial class TicketList : Box
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Settings: App
        /* IN008024 */
        //private string _appOperationModeToken = LogicPOS.Settings.GeneralSettings.Settings["appOperationModeToken"];
        //Settings: Colors
        private readonly Color _colorPosTicketListModeTicketBackground = GeneralSettings.Settings["colorPosTicketListModeTicketBackground"].StringToColor();
        private readonly Color _colorPosTicketListModeOrderMainBackground = GeneralSettings.Settings["colorPosTicketListModeOrderMainBackground"].StringToColor();
        private readonly Color _colorPosTicketListModeEditBackground = GeneralSettings.Settings["colorPosTicketListModeEditBackground"].StringToColor();
        //SessionApp
        private Guid _currentOrderMainOid;
        private int _currentTicketId;
        private Guid _currentDetailArticleOid;
        private int _listStoreModelSelectedIndex = -1;
        //Global For Both Models, Tickets and OrderMain
        private int _listStoreModelTotalItems = 0;
        private int _listStoreModelTotalItemsTicketListMode = 0;

        public OrderDetail CurrentOrderDetails { get; set; }
        public OrderDetail CurrentOrderDetailsAll { get; set; }
        private ArticleBag _articleBag;
        private fin_article _currentDetailArticle;
        //TreeView
        private TreeIter _treeIter;
        private TreePath _treePath;
        //UI
        private TreeView _treeView;
        private Label _labelTotal;
        private Label _labelLabelTotal;

        public PosMainWindow SourceWindow { get; set; }
        public ListStore ListStoreModel { get; set; }
        internal TicketListMode ListMode { get; set; } = TicketListMode.Ticket;

        //TicketPad Button References
        private TouchButtonIconWithText _buttonKeyPrev;
        public TouchButtonIconWithText ButtonKeyPrev
        {
            set { _buttonKeyPrev = value; _buttonKeyPrev.Clicked += _buttonKeyPrev_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyNext;
        public TouchButtonIconWithText ButtonKeyNext
        {
            set { _buttonKeyNext = value; _buttonKeyNext.Clicked += _buttonKeyNext_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyDelete;
        public TouchButtonIconWithText ButtonKeyDelete
        {
            set { _buttonKeyDelete = value; _buttonKeyDelete.Clicked += _buttonKeyDelete_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyChangeQuantity;
        public TouchButtonIconWithText ButtonKeyChangeQuantity
        {
            set { _buttonKeyChangeQuantity = value; _buttonKeyChangeQuantity.Clicked += _buttonKeyChangeQuantity_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyChangePrice;
        public TouchButtonIconWithText ButtonKeyChangePrice
        {
            set { _buttonKeyChangePrice = value; _buttonKeyChangePrice.Clicked += _buttonKeyChangePrice_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyWeight;
        public TouchButtonIconWithText ButtonKeyWeight
        {
            set { _buttonKeyWeight = value; _buttonKeyWeight.Clicked += _buttonKeyWeight_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyGifts;
        public TouchButtonIconWithText ButtonKeyGifts
        {
            set { _buttonKeyGifts = value; _buttonKeyGifts.Clicked += _buttonKeyGifts_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyFinishOrder;
        public TouchButtonIconWithText ButtonKeyFinishOrder
        {
            set { _buttonKeyFinishOrder = value; _buttonKeyFinishOrder.Clicked += _buttonKeyFinishOrder_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyPayments;
        public TouchButtonIconWithText ButtonKeyPayments
        {
            // Shared Event for Payments and SplitAccount
            set { _buttonKeyPayments = value; _buttonKeyPayments.Clicked += _buttonKeyPayments_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeySplitAccount;
        public TouchButtonIconWithText ButtonKeySplitAccount
        {
            // Shared Event for Payments and SplitAccount
            set { _buttonKeySplitAccount = value; _buttonKeySplitAccount.Clicked += _buttonKeyPayments_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyBarCode;
        public TouchButtonIconWithText ButtonKeyBarCode
        {
            set { _buttonKeyBarCode = value; _buttonKeyBarCode.Clicked += _buttonKeyBarCode_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyCardCode;
        public TouchButtonIconWithText ButtonKeyCardCode
        {
            set { _buttonKeyCardCode = value; _buttonKeyCardCode.Clicked += _buttonKeyCardCode_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyListOrder;
        public TouchButtonIconWithText ButtonKeyListOrder
        {
            set { _buttonKeyListOrder = value; _buttonKeyListOrder.Clicked += _buttonKeyListOrder_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyChangeTable;
        public TouchButtonIconWithText ButtonKeyChangeTable
        {
            set { _buttonKeyChangeTable = value; _buttonKeyChangeTable.Clicked += _buttonKeyChangeTable_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyListMode;
        public TouchButtonIconWithText ButtonKeyListMode
        {
            set { _buttonKeyListMode = value; _buttonKeyListMode.Clicked += _buttonKeyListMode_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyDecrease;
        public TouchButtonIconWithText ButtonKeyDecrease
        {
            set { _buttonKeyDecrease = value; _buttonKeyDecrease.Clicked += _buttonKeyDecrease_Clicked; }
        }

        private TouchButtonIconWithText _buttonKeyIncrease;
        public TouchButtonIconWithText ButtonKeyIncrease
        {
            set { _buttonKeyIncrease = value; _buttonKeyIncrease.Clicked += _buttonKeyIncrease_Clicked; }
        }

        //Toolbar Button References
        private TouchButtonIconWithText _toolbarApplicationClose;
        public TouchButtonIconWithText ToolbarApplicationClose
        {
            set { _toolbarApplicationClose = value; }
        }

        private TouchButtonIconWithText _toolbarLogoutUser;
        public TouchButtonIconWithText ToolbarLogoutUser
        {
            set { _toolbarLogoutUser = value; }
        }

        private TouchButtonIconWithText _toolbarShowSystemDialog;
        public TouchButtonIconWithText ToolbarShowSystemDialog
        {
            set { _toolbarShowSystemDialog = value; }
        }

        private TouchButtonIconWithText _toolbarShowChangeUserDialog;
        public TouchButtonIconWithText ToolbarShowChangeUserDialog
        {
            set { _toolbarShowChangeUserDialog = value; }
        }

        private TouchButtonIconWithText _toolbarBackOffice;
        public TouchButtonIconWithText ToolbarBackOffice
        {
            set { _toolbarBackOffice = value; }
        }

        private TouchButtonIconWithText _toolbarReports;
        public TouchButtonIconWithText ToolbarReports
        {
            set { _toolbarReports = value; }
        }

        private TouchButtonIconWithText _toolbarCashDrawer;
        public TouchButtonIconWithText ToolbarCashDrawer
        {
            set { _toolbarCashDrawer = value; }
        }

        private TouchButtonIconWithText _toolbarFinanceDocuments;
        public TouchButtonIconWithText ToolbarFinanceDocuments
        {
            set { _toolbarFinanceDocuments = value; }
        }

        private TouchButtonIconWithText _toolbarNewFinanceDocument;
        public TouchButtonIconWithText ToolbarNewFinanceDocument
        {
            set { _toolbarNewFinanceDocument = value; }
        }

        //Constructor
        public TicketList(dynamic pThemeObject)
        {
            InitUI(pThemeObject);

            // Override DataReceived
            if (GlobalApp.WeighingBalance != null && GlobalApp.WeighingBalance.IsPortOpen())
            {
                // Catch DataReceived here in the right Context of TicketList
                GlobalApp.WeighingBalance.ComPort().DataReceived += WeighingBalanceDataReceived;
            }
        }

        private void InitUI(dynamic pThemeObject)
        {
            try
            {
                this.BorderWidth = 10;

                //Objects:EventBoxPosTicketList:EventBoxTotal
                Gdk.Color eventBoxTotalBackgroundColor = (pThemeObject.EventBoxTotal.BackgroundColor as string).StringToGdkColor();
                //Objects:Columns
                Pango.FontDescription columnsFontTitle = Pango.FontDescription.FromString(pThemeObject.Columns.FontTitle);
                Pango.FontDescription columnsFontData = Pango.FontDescription.FromString(pThemeObject.Columns.FontData);

                //Objects:EventBoxPosTicketList:EventBoxTotal:LabelLabelTotal
                Pango.FontDescription labelLabelTotalFont = Pango.FontDescription.FromString(pThemeObject.EventBoxTotal.LabelLabelTotal.Font);
                Gdk.Color labelLabelTotalFontColor = (pThemeObject.EventBoxTotal.LabelLabelTotal.FontColor as string).StringToGdkColor();
                float labelLabelTotalAlignmentX = Convert.ToSingle(pThemeObject.EventBoxTotal.LabelLabelTotal.AlignmentX);
                //Objects:EventBoxPosTicketList:EventBoxTotal:LabelTotal
                Pango.FontDescription labelTotalFont = Pango.FontDescription.FromString(pThemeObject.EventBoxTotal.LabelTotal.Font);
                Gdk.Color labelTotalFontColor = (pThemeObject.EventBoxTotal.LabelTotal.FontColor as string).StringToGdkColor();
                float labelTotalAlignmentX = Convert.ToSingle(pThemeObject.EventBoxTotal.LabelTotal.AlignmentX);

                //Objects:EventBoxPosTicketList:Columns:DesignationWidth
                int columnDesignationWidth = Convert.ToInt16(pThemeObject.Columns.DesignationWidth);

                //scrolledWindow
                ScrolledWindow scrolledWindow = new ScrolledWindow();
                scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Always);

                //Label LabelTotal
                _labelLabelTotal = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_ticket"));
                _labelLabelTotal.ModifyFont(labelLabelTotalFont);
                _labelLabelTotal.ModifyFg(StateType.Normal, labelLabelTotalFontColor);
                _labelLabelTotal.SetAlignment(labelLabelTotalAlignmentX, 0.0F);

                //Label Total
                _labelTotal = new Label();
                //_labelTotal.SetAlignment(0.0F, 0.0F);
                _labelTotal.ModifyFont(labelTotalFont);
                _labelTotal.ModifyFg(StateType.Normal, labelTotalFontColor);
                _labelTotal.SetAlignment(labelTotalAlignmentX, 0.0F);
                _labelTotal.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(0, XPOSettings.ConfigurationSystemCurrency.Acronym);

                HBox hboxTotal = new HBox(false, 4);
                hboxTotal.PackStart(_labelLabelTotal, true, true, 5);
                hboxTotal.PackStart(_labelTotal, false, false, 5);

                //eventBoxTotal
                EventBox eventBoxTotal = new EventBox() { BorderWidth = 0/*, VisibleWindow = false*/ };
                //REPLACED BY THEME BACKGROUNDS
                eventBoxTotal.ModifyBg(StateType.Normal, eventBoxTotalBackgroundColor);
                eventBoxTotal.Add(hboxTotal);

                //_treeView
                //Init TreeView Model
                ListStoreModel = new ListStore(typeof(Guid), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(ArticleBagKey));
                //Assign TreeView Model Reference to TreeView Model
                _treeView = new TreeView(ListStoreModel) { RulesHint = false, CanFocus = false };
                InitColumns(_treeView, columnsFontTitle, columnsFontData, columnDesignationWidth);
                //Events, before working on Model
                _treeView.CursorChanged += _treeView_CursorChanged;
                ListStoreModel.RowInserted += _listStoreModel_RowInserted;

                //Only UpdateModel if has a CurrentOrderMainId
                if (POSSession.CurrentSession.OrderMains.Count > 0) UpdateModel();
                scrolledWindow.Add(_treeView);

                //Update ArticleBag
                UpdateArticleBag();

                //Pack in VBox
                VBox vbox = new VBox(false, 0);
                vbox.PackStart(scrolledWindow, true, true, 0);
                vbox.PackStart(eventBoxTotal, false, false, 2);

                //Pack in Box
                PackStart(vbox, true, true, 0);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void InitColumns(TreeView pTreeView, Pango.FontDescription pColumnTitleFontDesc, Pango.FontDescription pColumnDataFontDesc, int pWidthDesignation)
        {
            CellRendererText rendererText;
            TreeViewColumn column;
            int sharedWidth = 65;

            int widthDesignation = pWidthDesignation - 10;

            Pango.FontDescription fontDescTitle = pColumnTitleFontDesc;
            Pango.FontDescription fontDesc = pColumnDataFontDesc;

            Label labelDesignation = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "pos_ticketlist_label_designation")) { Visible = true };
            Label labelPrice = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "pos_ticketlist_label_price")) { Visible = true };
            Label labelQuantity = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "pos_ticketlist_label_quantity")) { Visible = true };
            Label labelDiscount = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "pos_ticketlist_label_discount")) { Visible = true };
            Label labelVat = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "pos_ticketlist_label_vat")) { Visible = true };
            Label labelTotal = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "pos_ticketlist_label_total")) { Visible = true };
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
            try
            {
                //Always Init a New Model, and Init Values
                ListStoreModel.Clear();
                _listStoreModelSelectedIndex = -1;
                _listStoreModelTotalItems = 0;

                //Init Values from SessionApp
                _currentOrderMainOid = POSSession.CurrentSession.CurrentOrderMainId;
                _currentTicketId = POSSession.CurrentSession.OrderMains[_currentOrderMainOid].CurrentTicketId;
                CurrentOrderDetails = POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails;

                //Change BackGround Color ListMode
                Gdk.Color colorListMode = (ListMode == TicketListMode.Ticket) ? colorListMode = _colorPosTicketListModeTicketBackground.ToGdkColor() : colorListMode = _colorPosTicketListModeOrderMainBackground.ToGdkColor();
                _treeView.ModifyBase(StateType.Normal, colorListMode);

                //Ticket Mode
                if (ListMode == TicketListMode.Ticket)
                {
                    //Reset TicketList Mode, only if is in TicketList Mode, else keep unchanged
                    _listStoreModelTotalItemsTicketListMode = 0;

                    //Start Append Items to Model
                    foreach (OrderDetailLine line in CurrentOrderDetails.Lines)
                    {
                        if (line.Properties.Quantity > 0)
                        {
                            ListStoreModel.AppendValues(
                              line.ArticleOid,
                              line.Designation,
                              LogicPOS.Utility.DataConversionUtils.DecimalToString(line.Properties.PriceFinal),
                              LogicPOS.Utility.DataConversionUtils.DecimalToString(line.Properties.Quantity),
                              line.Properties.DiscountArticle.ToString(),
                              line.Properties.Vat.ToString(),
                              LogicPOS.Utility.DataConversionUtils.DecimalToString(line.Properties.TotalFinal)
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
                    foreach (var item in _articleBag)
                    {
                        ArticleBagKey articleBagKey = new ArticleBagKey(item.Key.ArticleId, item.Key.Designation, item.Key.Price, item.Key.Discount, item.Key.Vat);

                        //Detect and Assign VatExemptionReason to ArticleBak Key
                        if (item.Key.VatExemptionReasonOid != null && item.Key.VatExemptionReasonOid != Guid.Empty)
                        {
                            articleBagKey.VatExemptionReasonOid = item.Key.VatExemptionReasonOid;
                        }
                        if (item.Value.Quantity > 0)
                        {
                            ListStoreModel.AppendValues(
                            item.Key.ArticleId,
                            item.Key.Designation,
                            LogicPOS.Utility.DataConversionUtils.DecimalToString(item.Value.PriceFinal),
                            LogicPOS.Utility.DataConversionUtils.DecimalToString(item.Value.Quantity),
                            item.Key.Discount.ToString(),
                            item.Key.Vat.ToString(),
                            LogicPOS.Utility.DataConversionUtils.DecimalToString(item.Value.TotalFinal),
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
                _treeView.SetCursor(_treePath, null, false);
                _treeView.ScrollToCell(_treePath, _treeView.Columns[0], false, 0, 0);

                //Init _currentDetailArticleOid, If Model is Empty the Value 0 is Assigned
                if (ListStoreModel.Data.Count > 0) _currentDetailArticleOid = (Guid)ListStoreModel.GetValue(_treeIter, 0);

                UpdateTicketListButtons();
                UpdateTicketListTotal();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public void TicketDecrease()
        {
            try
            {
                if (ListMode == TicketListMode.OrderMain)
                {

                    OrderDetailLine newLine;
                    //Init Values from SessionApp
                    _currentOrderMainOid = POSSession.CurrentSession.CurrentOrderMainId;
                    _currentTicketId = POSSession.CurrentSession.OrderMains[_currentOrderMainOid].CurrentTicketId;
                    CurrentOrderDetails = POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails;
                    int currentTicketAux = _currentTicketId;
                    bool newArticleLine = true;
                    //Change BackGround Color ListMode
                    //Gdk.Color colorListMode = (_listMode == TicketListMode.EditList) ? colorListMode = Utils.ColorToGdkColor(_colorPosTicketListModeEditBackground) : colorListMode = Utils.ColorToGdkColor(_colorPosTicketListModeOrderMainBackground);
                    //_treeView.ModifyBase(StateType.Normal, colorListMode);

                    //If ticket was empty Create new line
                    if (POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines.Count == 0)
                    {
                        POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails = new OrderDetail();

                        if (_currentDetailArticle != null)
                        {
                            newLine = new OrderDetailLine(_currentDetailArticle.Oid, _currentDetailArticle.Designation,
                                ArticleUtils.GetArticlePrice((fin_article)XPOHelper.GetXPGuidObject(typeof(fin_article), _currentDetailArticle.Oid),
                                (AppOperationModeSettings.AppMode == AppOperationMode.Retail) ? TaxSellType.TakeAway : TaxSellType.Normal));

                            newLine.Properties.PriceNet = Convert.ToDecimal((string)ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price));
                            newLine.Properties.TotalFinal = Convert.ToDecimal((string)ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Total)) - Convert.ToDecimal((string)ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price));
                            //newLine.Properties.Quantity -= GetArticleDefaultQuantity(_currentDetailArticle.Oid);
                            newLine.Properties.Quantity = -1;

                            POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines.Add(newLine);
                        }
                    }
                    //Else update line with article update
                    else
                    {
                        if (_currentDetailArticle != null)
                        {
                            for (int i = 0; i < POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines.Count; i++)
                            {
                                if (POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].ArticleOid == _currentDetailArticle.Oid &&
                                    POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].Properties.PriceNet == POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].Properties.PriceNet)
                                {
                                    POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].Properties.Quantity = -1;
                                    POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].Properties.PriceNet = POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].Properties.PriceFinal;
                                    POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].Properties.TotalFinal = (POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].Properties.TotalFinal
                                                                                                                                                                           - (POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines[i].Properties.PriceNet));
                                    newArticleLine = false;
                                }
                            }
                            //If new article in current ticket
                            if (newArticleLine)
                            {

                                if (_currentDetailArticle != null)
                                {

                                    newLine = new OrderDetailLine(_currentDetailArticle.Oid, _currentDetailArticle.Designation,
                                        ArticleUtils.GetArticlePrice((fin_article)XPOHelper.GetXPGuidObject(typeof(fin_article), _currentDetailArticle.Oid),
                                        (AppOperationModeSettings.AppMode == AppOperationMode.Retail) ? TaxSellType.TakeAway : TaxSellType.Normal));

                                    newLine.Properties.PriceNet = Convert.ToDecimal((string)ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price));
                                    newLine.Properties.TotalFinal = Convert.ToDecimal((string)ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Total)) - Convert.ToDecimal((string)ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price));

                                    //newLine.Properties.Quantity -= GetArticleDefaultQuantity(_currentDetailArticle.Oid);
                                    newLine.Properties.Quantity = -1;
                                    POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails.Lines.Add(newLine);
                                }
                            }
                        }
                    }


                    POSSession.CurrentSession.OrderMains[_currentOrderMainOid].FinishOrder(XPOSettings.Session, false, true);
                    //UpdateModel();
                    //UpdateOrderStatusBar();
                    _buttonKeyFinishOrder.Sensitive = true;
                    //UpdateTicketListTotal();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public void InsertOrUpdate(string pArticleEAN)
        {
            //public static XPGuidObject GetXPGuidObjectFromCriteria(Session pSession, Type pXPGuidObjectType, string pCriteriaFilter)
            string sql = string.Format("SELECT Oid FROM fin_article WHERE BarCode = '{0}';", pArticleEAN);
            Guid articleGuid = XPOHelper.GetGuidFromQuery(sql);

            if (articleGuid != Guid.Empty)
            {
                InsertOrUpdate(articleGuid);
            }
            else
            {
                sql = string.Format("SELECT Oid FROM fin_article WHERE Code = '{0}';", pArticleEAN);
                articleGuid = XPOHelper.GetGuidFromQuery(sql);
                if (articleGuid != Guid.Empty)
                {
                    InsertOrUpdate(articleGuid);
                }
                else
                {
                    string message = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_invalid_code");
                    logicpos.Utils.ShowMessageTouch(SourceWindow, DialogFlags.DestroyWithParent, new Size(400, 300), MessageType.Error, ButtonsType.Ok, "Código Inválido", message);
                    return;
                }
            }
        }

        public void ArticleNotFound()
        {
            string message = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_invalid_code");
            logicpos.Utils.ShowMessageTouch(SourceWindow, DialogFlags.DestroyWithParent, new Size(400, 300), MessageType.Error, ButtonsType.Ok, "Código Inválido", message);
            return;
        }

        public void WsNotFound()
        {
            string message = string.Format("O Web service não se encontra em funcionamento");
            logicpos.Utils.ShowMessageTouch(SourceWindow, DialogFlags.DestroyWithParent, new Size(400, 300), MessageType.Error, ButtonsType.Ok, "Web Service não encontrado", message);
            return;
        }


        public void InsertOrUpdate(Guid pArticleOid)
        {
            InsertOrUpdate(pArticleOid, new ParkingTicketResult());
        }

        public void InsertOrUpdate(Guid pArticleOid, ParkingTicketResult parkingTicketResult)
        {
            bool requireToChooseVatExemptionReason = true;

            try
            {
                // Override Default with Config Value
                requireToChooseVatExemptionReason = Convert.ToBoolean(GeneralSettings.Settings["requireToChooseVatExemptionReason"]);
            }
            catch (Exception)
            {
                _logger.Error("Error Missing Config Parameter Key: [requireToChooseVatExemptionReason]");
            }

            try
            {
                //Get Article
                fin_article article = (fin_article)XPOHelper.GetXPGuidObject(typeof(fin_article), pArticleOid);

                //Force Refresh Cache 
                article.Reload();
                //article1 used to get original article designation
                string originalDesignation = article.Designation;
                /* TK013134 */
                string ean = parkingTicketResult.Ean;
                bool isAppUseParkingTicketModule = GeneralSettings.AppUseParkingTicketModule && !string.IsNullOrEmpty(ean);
                // Override default Designation with pToken1 : Priority
                if (isAppUseParkingTicketModule)
                {
                    //article.Designation = $"{article.Designation} [{ean}]";
                    //article.Notes = $"[{ean}] {parkingTicketResult.Description}";
                    /* IN009239 */
                    //article.Designation = $"{article.Designation} [{ean}]"
                    if (parkingTicketResult.Ean.Length == 13)
                    {
                        string designation = article.Designation;

                        int index = designation.LastIndexOf("[");
                        if (index > 0)
                            designation = designation.Substring(0, index);
                        article.Designation = $"{article.Designation} [{parkingTicketResult.Minutes} min.] [{parkingTicketResult.Ean}]";
                        article.Notes = $"[{ean}] {parkingTicketResult.Description}";/* IN009239 */
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
                    /* 
     * This was a unnecessary double-check already being done by ParkingTicket class.
     * When a parking ticket is already in the order list, it is unique and its quantity is '1' ever. We can't have 2+ units of the same.
     * But variable _listStoreModelSelectedIndex is settled to '0' without the line code below... Therefore, setting it to '-1'
     */
                    //Get current Index with LINQ : To Get OrderDetail articleId Index, If Exists
                    // _listStoreModelSelectedIndex = _currentOrderDetails.Lines.FindIndex(item => item.Designation.Contains(pToken1));
                    _listStoreModelSelectedIndex = -1;
                }
                else
                {
                    //Get current Index with LINQ : To Get OrderDetail articleId Index, If Exists
                    _listStoreModelSelectedIndex = CurrentOrderDetails.Lines.FindIndex(item => item.ArticleOid == pArticleOid);
                };

                //Get Article defaultQuantity
                decimal defaultQuantity = GetArticleDefaultQuantity(pArticleOid);
                if (GeneralSettings.AppUseParkingTicketModule) { defaultQuantity = article.DefaultQuantity; if (defaultQuantity == 0) defaultQuantity = 1; }
                else { defaultQuantity = GetArticleDefaultQuantity(pArticleOid); }
                decimal price = 0.0m;

                //Check if is a TaxDutyFree(Isento) Article, and Show Info Message.
                //TODO
                //Create Protection for Invalid Product, ex Product Without Vat, Create a Shared Method to check if is a Valid Article
                if (requireToChooseVatExemptionReason && article.VatDirectSelling.Oid == InvoiceSettings.XpoOidConfigurationVatRateDutyFree && article.VatExemptionReason == null)
                {
                    //TODO: Implement VatExemptionReason in TicketList (Both Modes) 
                    //Guid vatExemptionReasonGuid = GetVatExemptionReason();
                    logicpos.Utils.ShowMessageTouch(SourceWindow, DialogFlags.DestroyWithParent, new Size(400, 300), MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_vatrate_free_article_detected"), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_article_without_vat_exception_reason_detected"));
                    return;
                }

                //Check if ticket is exited and show message
                if (parkingTicketResult.AlreadyExit)
                {
                    string message = string.Format("Numero do ticket: {0}\n\n{1}\n\nData de Saida: {2}", parkingTicketResult.Ean, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_article_already_exited"), parkingTicketResult.DateExits);
                    logicpos.Utils.ShowMessageTouch(SourceWindow, DialogFlags.DestroyWithParent, new Size(450, 350), MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_already_exited"), message);
                    return;
                }
                //Check if ticket is already payed and show message
                else if (parkingTicketResult.AlreadyPaid)
                {
                    string message = string.Format("Numero do ticket: {0}\n\n{1}\nData de pagamento: {2}\n\nPode sair até: {3} ", parkingTicketResult.Ean, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_article_already_paid"), parkingTicketResult.DatePaid, parkingTicketResult.DateTolerance);
                    logicpos.Utils.ShowMessageTouch(SourceWindow, DialogFlags.DestroyWithParent, new Size(450, 350), MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_already_paid"), message);
                    return;
                }

                //Update
                //Ticket não incrementar com codigo barras desconhecido (acrescentado -> && GlobalFramework.AppUseParkingTicketModule && isAppUseParkingTicketModule)
                if (_listStoreModelSelectedIndex != -1 && GeneralSettings.AppUseParkingTicketModule && isAppUseParkingTicketModule)

                {
                    //Update orderDetails SessionApp
                    CurrentOrderDetails.Update(_listStoreModelSelectedIndex, CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity += defaultQuantity);

                    //Update TreeView Model
                    _treeIter = (TreeIter)CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].TreeIter;
                    //Update Quantity
                    ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Quantity, LogicPOS.Utility.DataConversionUtils.DecimalToString(CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity));
                    //Update Total
                    ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Total, LogicPOS.Utility.DataConversionUtils.DecimalToString(CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.TotalFinal));
                }
                //Insert
                else
                {
                    bool showMessage;
                    if (logicpos.Utils.CheckStocks())
                    {
                        if (!logicpos.Utils.ShowMessageMinimumStock(SourceWindow, pArticleOid, Convert.ToDecimal(ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Quantity)) + defaultQuantity, out showMessage))
                        {
                            if (showMessage) return;
                        }
                    }
                    //Get Place Object to extract TaxSellType Normal|TakeWay
                    OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                    pos_configurationplace configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), currentOrderMain.Table.PlaceId);
                    fin_articletype articletype = (fin_articletype)XPOSettings.Session.GetObjectByKey(typeof(fin_articletype), article.Type.Oid);

                    if (configurationPlace == null) { configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), POSSettings.XpoOidConfigurationPlaceTableDefaultOpenTable); }
                    //Use VatDirectSelling if in Retail or in TakeWay mode
                    TaxSellType taxSellType = (AppOperationModeSettings.AppMode == AppOperationMode.Retail || configurationPlace.MovementType.VatDirectSelling) ? TaxSellType.TakeAway : TaxSellType.Normal;
                    decimal priceTax = (taxSellType == TaxSellType.Normal) ? article.VatOnTable.Value : article.VatDirectSelling.Value;

                    //Get PriceFinal to Request Price Dialog
                    PriceProperties priceProperties = ArticleUtils.GetArticlePrice(article, taxSellType);
                    price = priceProperties.PriceFinal;
                    PricePropertiesSourceMode sourceMode;

                    //If article dont have price or article has a variable price defined, request final price
                    /* TK013134 */
                    if (isAppUseParkingTicketModule)
                    {
                        /* Parking Ticket refers to an input from other app, therefore the same scenario as manual price inputs */
                        sourceMode = PricePropertiesSourceMode.FromTotalFinal;
                        price = parkingTicketResult.Price;
                        // article.Notes = parkingTicketResult.Description; /* IN009239 */
                        //Message with ticket details before pay
                        string message = string.Empty;
                        if (parkingTicketResult.Ean.Length == 13)
                        {
                            message = string.Format(" Numero do ticket: {0}\n\n Data de Emissão: {1}\n Duração: {2} minuto(s)\n Descrição: {3}\n", parkingTicketResult.Ean, parkingTicketResult.Date, parkingTicketResult.Minutes, parkingTicketResult.Description);
                            logicpos.Utils.ShowMessageTouch(SourceWindow, DialogFlags.DestroyWithParent, new Size(480, 350), MessageType.Info, ButtonsType.Ok, "Ticket Details", message);
                        }
                        else
                        {
                            message = string.Format(" Numero do cartão: {0} \n", parkingTicketResult.Ean);
                            logicpos.Utils.ShowMessageTouch(SourceWindow, DialogFlags.DestroyWithParent, new Size(480, 350), MessageType.Info, ButtonsType.Ok, "Ticket Details", message);
                        }

                    }
                    //Proteção para artigos do tipo "Sem Preço" [IN:013329]
                    else if (!articletype.HavePrice)
                    {
                        sourceMode = PricePropertiesSourceMode.FromPriceNet;
                        price = 0.00m;
                        article.Designation = string.Format(articletype.Designation + " : " + originalDesignation);
                    }
                    else if (price <= 0.0m || article.PVPVariable == true)
                    {
                        MoneyPadResult result = PosMoneyPadDialog.RequestDecimalValue(SourceWindow, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_moneypad_product_price"), price);
                        if (result.Response == ResponseType.Cancel) return;
                        sourceMode = PricePropertiesSourceMode.FromTotalFinal;
                        price = result.Value;
                    }
                    //If Article has Price, Use its PriceNet
                    else
                    {
                        sourceMode = PricePropertiesSourceMode.FromPriceNet;
                        price = priceProperties.PriceNet;
                    }

                    //TODO: Be sure that Money Dialog dont Permit <= 0 Values
                    if (price <= 0.0m && articletype.HavePrice)
                    {
                        throw new Exception("defaultPrice <= 0.0m");
                    }

                    //Get PriceFinal Here
                    priceProperties = PriceProperties.GetPriceProperties(
                      sourceMode,
                      article.PriceWithVat,
                      price,
                      1.0m,
                      article.Discount,
                      POSSession.GetGlobalDiscount(),
                      priceTax
                    );

                    //Add VatExemptionReason to VatExemptionReason
                    if (article.VatExemptionReason != null) priceProperties.VatExemptionReason = article.VatExemptionReason.Oid;
                    int i = 0;
                    foreach (var item in CurrentOrderDetails.Lines)
                    {
                        _logger.Debug(item.ArticleOid);
                        _logger.Debug(pArticleOid);
                        _logger.Debug(item.Properties.PriceFinal);
                        _logger.Debug(priceProperties.PriceFinal);
                        if (item.ArticleOid == pArticleOid && item.Properties.PriceFinal == priceProperties.PriceFinal)
                        {
                            _listStoreModelSelectedIndex = i;
                            break;
                        }
                        i++;
                    }
                    _logger.Debug(_listStoreModelSelectedIndex);

                    //Check if item is already on ticket list and his position
                    bool itemInTicket = false;
                    decimal newQuantity = 0.0m;
                    decimal newTotalPrice = 0.0m;

                    ListStoreModel.GetIterFirst(out _treeIter);
                    for (int itemPosition = 0; itemPosition < ListStoreModel.IterNChildren(); itemPosition++)
                    {
                        if ((Guid)(ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.ArticleId)) == pArticleOid &&
                                Convert.ToDecimal(ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Price)) == Math.Round(priceProperties.PriceFinal, CultureSettings.DecimalRoundTo))
                        {

                            _listStoreModelSelectedIndex = itemPosition;

                            //Update TreeView Model Price and quantity
                            newQuantity = Convert.ToDecimal(ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Quantity)) + defaultQuantity;
                            newTotalPrice = newQuantity * Convert.ToDecimal(CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.PriceFinal);

                            //Update orderDetails 
                            CurrentOrderDetails.Update(_listStoreModelSelectedIndex, newQuantity, Convert.ToDecimal(CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.PriceUser));


                            //_listStoreModel.SetValue(_treeIter, (int)TicketListColumns.Price, LogicPOS.Utility.DataConversionUtils.DecimalToString(newPrice));
                            ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Quantity, LogicPOS.Utility.DataConversionUtils.DecimalToString(newQuantity));

                            //Update Total
                            ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Total, LogicPOS.Utility.DataConversionUtils.DecimalToString(newTotalPrice));

                            CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].TreeIter = _treeIter;

                            itemInTicket = true;

                            break;
                        }
                        ListStoreModel.IterNext(ref _treeIter);
                    }
                    //Update Total Items Member
                    _listStoreModelTotalItems = ListStoreModel.IterNChildren();

                    if (!itemInTicket)
                    {
                        //Insert into orderDetails SessionApp
                        CurrentOrderDetails.Insert(article.Oid, article.Designation, priceProperties);

                        //Insert into TreeView Model : [0]ArticleId,[1]Designation,[2]Price,[3]Quantity,[4]Discount,[5]Vat,[6]Total
                        object[] columnValues = new object[Enum.GetNames(typeof(TicketListColumns)).Length];
                        columnValues[0] = article.Oid;
                        columnValues[1] = article.Designation;
                        columnValues[2] = LogicPOS.Utility.DataConversionUtils.DecimalToString(CurrentOrderDetails.Lines[CurrentOrderDetails.Lines.Count - 1].Properties.PriceFinal);
                        columnValues[3] = LogicPOS.Utility.DataConversionUtils.DecimalToString(defaultQuantity);
                        columnValues[4] = CurrentOrderDetails.Lines[CurrentOrderDetails.Lines.Count - 1].Properties.DiscountArticle.ToString();
                        columnValues[5] = CurrentOrderDetails.Lines[CurrentOrderDetails.Lines.Count - 1].Properties.Vat.ToString();
                        columnValues[6] = LogicPOS.Utility.DataConversionUtils.DecimalToString(CurrentOrderDetails.Lines[CurrentOrderDetails.Lines.Count - 1].Properties.TotalFinal);

                        //Assign Global TreeIter
                        _treeIter = ListStoreModel.AppendValues(columnValues);

                        //Assign SessionApp TreeIter, get ArticleId with LINQ
                        _listStoreModelSelectedIndex = CurrentOrderDetails.Lines.FindIndex(item => item.ArticleOid == pArticleOid);
                        CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].TreeIter = _treeIter;
                        if (GeneralSettings.AppUseParkingTicketModule) { CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity = defaultQuantity; }
                    }
                };

                //Shared SetCursor
                _treeView.SetCursor(ListStoreModel.GetPath(_treeIter), null, false);
                _treeView.ScrollToCell(ListStoreModel.GetPath(_treeIter), null, false, 0, 0);

                //Update Total
                UpdateTicketListTotal();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public void ChangeQuantity(decimal pQuantity)
        {
            _listStoreModelSelectedIndex = CurrentOrderDetails.Lines.FindIndex(item => item.ArticleOid == (Guid)ListStoreModel.GetValue(_treeIter,
                    (int)TicketListColumns.ArticleId));

            decimal oldValueQnt = CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity;

            //If oldValue is diferent from New One
            if (oldValueQnt != pQuantity)
            {
                //Update orderDetails SessionApp
                CurrentOrderDetails.Update(_listStoreModelSelectedIndex, pQuantity);
                //Update TreeView Model: Quantity
                ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Quantity, LogicPOS.Utility.DataConversionUtils.DecimalToString(pQuantity));
                CurrentOrderDetails.Lines[CurrentOrderDetails.Lines.Count - 1].Properties.Quantity = pQuantity;
                //Update TreeView Model: Total
                ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Total, LogicPOS.Utility.DataConversionUtils.DecimalToString(CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.TotalFinal));
                //Update Total
                UpdateTicketListTotal();
                if (GeneralSettings.AppUseParkingTicketModule) CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity = pQuantity; ;
            }
        }

        //Used from events in Button _buttonKeyDelete_Clicked & _buttonKeyDecrease_Clicked and 
        //DeleteLine Quantity, Ore Decrease Line Quantity
        public void DeleteItem_Event(TicketListDeleteMode pMode)
        {
            if (ListMode == TicketListMode.Ticket)
            {
                try
                {
                    if (pMode == TicketListDeleteMode.Decrease)
                    {
                        DeleteItem_ListModeTicket();
                    }
                    else if (pMode == TicketListDeleteMode.Delete)
                    {
                        try
                        {
                            //Decrease Quantity
                            CurrentOrderDetails.Update(_listStoreModelSelectedIndex, CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity -= CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity);

                            //If Quantity <= 0 Remove Article From Model and SessionApp
                            if (CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity <= 0)
                            {
                                //Remove from SessionApp
                                CurrentOrderDetails.Delete(_listStoreModelSelectedIndex);

                                //Remove from TreviewModel
                                ListStoreModel.Remove(ref _treeIter);

                                //Update Total Items Member
                                _listStoreModelTotalItems = ListStoreModel.IterNChildren();

                                if (ListMode == TicketListMode.Ticket) _listStoreModelTotalItemsTicketListMode = ListStoreModel.IterNChildren();

                                Previous();

                                UpdateModel();

                                //Update Buttons
                                UpdateTicketListButtons();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message, ex);
                        }
                    }
                    else
                    {

                        //WIP: Put Quantity to Work, to Delete all Items in Line
                        //if (pMode == TicketListDeleteMode.Delete)
                        //{
                        //Get articleBagKey from Model
                        //  ArticleBagKey articleBagKey = (ArticleBagKey)_listStoreModel.GetValue(_treeIter, 7);
                        //Delete Line Sending all Quantity in Line
                        //  DeleteItem_ListModeOrderMain(_articleBag[articleBagKey].Quantity);
                        //}
                        //else
                        //{
                        //Decrease, Without sending Quantity
                        DeleteItem_ListModeOrderMain();
                        //}
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
            else if (ListMode == TicketListMode.OrderMain)
            {
                DeleteItem_ListModeOrderMain();
            }
            // Cancel order Lindote 2020/06/08
            // Passou para o botão Eliminar  

        }

        public void DeleteItem_ListModeTicket()
        {
            //Get Article defaultQuantity
            _currentOrderMainOid = POSSession.CurrentSession.CurrentOrderMainId;
            _currentTicketId = POSSession.CurrentSession.OrderMains[_currentOrderMainOid].CurrentTicketId;
            CurrentOrderDetails = POSSession.CurrentSession.OrderMains[_currentOrderMainOid].OrderTickets[_currentTicketId].OrderDetails;
            decimal defaultQuantity = GetArticleDefaultQuantity(_currentDetailArticleOid);
            DeleteItem_ListModeTicket(defaultQuantity);
        }

        public void DeleteItem_ListModeTicket(decimal pRemoveQuantity)
        {
            //Get Current Quantity
            var quantity = ((string)ListStoreModel.GetValue(_treeIter, (int)TicketListColumns.Quantity)).Replace('.',',');

            decimal totalQtd = Convert.ToDecimal(quantity) - pRemoveQuantity;

            //Decrease Quantity by defaultQuantity
            CurrentOrderDetails.Update(_listStoreModelSelectedIndex, totalQtd);

            //If Quantity <= 0 Remove Article From Model and SessionApp
            if (CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity <= 0)
            {
                //Remove from SessionApp
                CurrentOrderDetails.Delete(_listStoreModelSelectedIndex);

                //Remove from TreviewModel
                ListStoreModel.Remove(ref _treeIter);

                //Update Total Items Member
                _listStoreModelTotalItems = ListStoreModel.IterNChildren();

                if (ListMode == TicketListMode.Ticket) _listStoreModelTotalItemsTicketListMode = ListStoreModel.IterNChildren();

                Previous();

                //Update Buttons
                UpdateTicketListButtons();
            }
            //Update Model Quantity
            else
            {
                //Update Quantity
                ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Quantity, LogicPOS.Utility.DataConversionUtils.DecimalToString(CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.Quantity));
                //Update Total
                ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Total, LogicPOS.Utility.DataConversionUtils.DecimalToString(CurrentOrderDetails.Lines[_listStoreModelSelectedIndex].Properties.TotalFinal));
            }

            //Update Total
            UpdateTicketListTotal();
        }

        public void DeleteItem_ListModeOrderMain()
        {
            //Get Article defaultQuantity
            decimal defaultQuantity = GetArticleDefaultQuantity(_currentDetailArticleOid);
            DeleteItem_ListModeOrderMain(defaultQuantity);
            //UpdateArticleBag after Delete Article
            UpdateArticleBag();
        }

        public void DeleteItem_ListModeOrderMain(decimal pRemoveQuantity)
        {
            bool canDeleteItem = true;

            //Get articleBagKey from Model
            ArticleBagKey articleBagKey = (ArticleBagKey)ListStoreModel.GetValue(_treeIter, 7);

            if (canDeleteItem)
            {

                //Call ArticleBag Delete, without PartialPayment Items
                decimal currentTotalQuantity = _articleBag.DeleteFromDocumentOrder(articleBagKey, pRemoveQuantity);
                //REQUIRE to ReCalc Line TotalFinal Without PartialPayment Items
                decimal currentTotalFinal = currentTotalQuantity * _articleBag[articleBagKey].PriceFinal;

                //Add items deleted into current ticket
                //Update items decreased in current ticket
                TicketDecrease();

                if (currentTotalQuantity <= 0)
                {
                    //Remove from TreviewModel
                    ListStoreModel.Remove(ref _treeIter);

                    //Update Total Items Member
                    _listStoreModelTotalItems = ListStoreModel.IterNChildren();

                    Previous();

                    //Update Buttons
                    UpdateTicketListButtons();
                }
                else
                {
                    //Update Quantity
                    ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Quantity, LogicPOS.Utility.DataConversionUtils.DecimalToString(currentTotalQuantity));
                    //Update Total
                    ListStoreModel.SetValue(_treeIter, (int)TicketListColumns.Total, LogicPOS.Utility.DataConversionUtils.DecimalToString(currentTotalFinal));
                }

                UpdateOrderStatusBar();
            }
        }

        public void Previous()
        {
            if (_treePath is null) return;
            _treePath.Prev();
            _treeView.SetCursor(_treePath, null, false);

        }

        public void Next()
        {
            if (_treePath is null) return;
            _treePath.Next();
            _treeView.SetCursor(_treePath, null, false);

        }

        public void UpdateArticleBag()
        {
            try
            {
                if (POSSession.CurrentSession.CurrentOrderMainId != Guid.Empty && POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId))
                    _articleBag = ArticleBag.TicketOrderToArticleBag(POSSession.CurrentSession.OrderMains[_currentOrderMainOid]);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("UpdateArticleBag Error: [{0}]", ex.Message));
            }
            //_logger.Debug(string.Format("UpdateArticleBag TotalQuantity: [{0}]", _articleBag.TotalQuantity));
        }

        public void UpdateTicketListButtons()
        {
            //Debug
            //_logger.Debug(String.Format("TicketListMode: [{0}], listStoreModelTotalItems: [{1}], currentOrderDetails.TotalItems: [{2}]", Enum.GetName(typeof(TicketListMode), _listMode), _listStoreModelTotalItems, _currentOrderDetails.TotalItems));

            //No Items, always disable All actions
            if (_listStoreModelTotalItems == 0)
            {
                //if (_buttonKeyPrev != null && _buttonKeyPrev.Sensitive) _buttonKeyPrev.Sensitive = false;
                //if (_buttonKeyNext != null && _buttonKeyNext.Sensitive) _buttonKeyNext.Sensitive = false;
                if (_buttonKeyIncrease != null && _buttonKeyIncrease.Sensitive) _buttonKeyIncrease.Sensitive = false;
                if (_buttonKeyDecrease != null && _buttonKeyDecrease.Sensitive) _buttonKeyDecrease.Sensitive = false;
                if (_buttonKeyDelete != null && _buttonKeyDelete.Sensitive) _buttonKeyDelete.Sensitive = false;
                if (_buttonKeyChangeQuantity != null && _buttonKeyChangeQuantity.Sensitive) _buttonKeyChangeQuantity.Sensitive = false;
                if (_buttonKeyChangePrice != null && _buttonKeyChangePrice.Sensitive) _buttonKeyChangePrice.Sensitive = false;
                if (_buttonKeyWeight != null && _buttonKeyWeight.Sensitive) _buttonKeyWeight.Sensitive = false;
                //if (_buttonKeyGifts != null && _buttonKeyGifts.Sensitive) _buttonKeyGifts.Sensitive = false;
                if (_buttonKeySplitAccount != null && _buttonKeySplitAccount.Sensitive) _buttonKeySplitAccount.Sensitive = false;
            }
            else
            {
                //If has Items always enable default actions
                if (ListMode == TicketListMode.Ticket && _listStoreModelSelectedIndex > -1)
                {
                    if (_buttonKeyIncrease != null && !_buttonKeyIncrease.Sensitive) _buttonKeyIncrease.Sensitive = true;
                    if (_buttonKeyDecrease != null && !_buttonKeyDecrease.Sensitive) _buttonKeyDecrease.Sensitive = true;
                    //Only Enabled in TicketListMode.Ticket
                    if (_buttonKeyDelete != null && _listStoreModelSelectedIndex > -1) _buttonKeyDelete.Sensitive = GeneralSettings.HasPermissionTo("TICKETLIST_DELETE");
                    if (_buttonKeyChangePrice != null && !_buttonKeyChangePrice.Sensitive) _buttonKeyChangePrice.Sensitive = GeneralSettings.HasPermissionTo("TICKETLIST_CHANGE_PRICE");
                    if (_buttonKeyChangeQuantity != null && !_buttonKeyChangeQuantity.Sensitive) _buttonKeyChangeQuantity.Sensitive = true;
                    if (_buttonKeyWeight != null) _buttonKeyWeight.Sensitive = (GlobalApp.WeighingBalance != null && GlobalApp.WeighingBalance.IsPortOpen() && _currentDetailArticle.UseWeighingBalance);
                    //if (_buttonKeyGifts != null && !_buttonKeyGifts.Sensitive) _buttonKeyGifts.Sensitive = (_articleBag.Count > 1);
                    if (_buttonKeySplitAccount != null && !_buttonKeySplitAccount.Sensitive) _buttonKeySplitAccount.Sensitive = (_articleBag.Count > 1 && _articleBag.TotalFinal > 0.00m);
                }
                else
                {
                    if (_buttonKeyIncrease != null && _buttonKeyIncrease.Sensitive) _buttonKeyIncrease.Sensitive = false;
                    if (_buttonKeyDecrease != null && _buttonKeyDecrease.Sensitive) _buttonKeyDecrease.Sensitive = false;
                    //Always Disabled in Orders
                    //_buttonKeyDelete.Sensitive = false;
                    //Enabled Again
                    if (_buttonKeyDelete != null && _listStoreModelSelectedIndex > -1) _buttonKeyDelete.Sensitive = GeneralSettings.HasPermissionTo("TICKETLIST_DELETE");
                    if (_buttonKeyChangePrice != null && _buttonKeyChangePrice.Sensitive) _buttonKeyChangePrice.Sensitive = false;
                    if (_buttonKeyChangeQuantity != null && _buttonKeyChangeQuantity.Sensitive) _buttonKeyChangeQuantity.Sensitive = false;
                    if (_buttonKeyWeight != null && _buttonKeyWeight.Sensitive) _buttonKeyWeight.Sensitive = false;
                    //if (_buttonKeyGifts != null && _buttonKeyGifts.Sensitive) _buttonKeyGifts.Sensitive = false;
                    if (_buttonKeySplitAccount != null && _buttonKeySplitAccount.Sensitive) _buttonKeySplitAccount.Sensitive = (_articleBag.Count > 1 && _articleBag.TotalFinal > 0.00m); ;
                }


                //In First Item with One Item
                //if (_listStoreModelSelectedIndex == 0)
                //{
                //    if (_buttonKeyPrev != null && _buttonKeyPrev.Sensitive) _buttonKeyPrev.Sensitive = false;
                //    if (_buttonKeyNext != null)
                //    {
                //        //If has more than One Item Enable Next
                //        if (_listStoreModelTotalItems > 1)
                //        {
                //            if (!_buttonKeyNext.Sensitive) _buttonKeyNext.Sensitive = true;
                //        }
                //        //If has One Item Disable Next
                //        else
                //        {
                //            if (_buttonKeyNext.Sensitive) _buttonKeyNext.Sensitive = false;
                //        }
                //    }
                //}
                ////Last Item
                //else if (_listStoreModelSelectedIndex == _listStoreModelTotalItems - 1)
                //{
                //    if (_buttonKeyPrev != null && !_buttonKeyPrev.Sensitive) _buttonKeyPrev.Sensitive = true;
                //    if (_buttonKeyNext != null && _buttonKeyNext.Sensitive) _buttonKeyNext.Sensitive = false;
                //}
                ////Middle Items
                //else
                //{
                //    if (_buttonKeyPrev != null && !_buttonKeyPrev.Sensitive) _buttonKeyPrev.Sensitive = true;
                //    if (_buttonKeyNext != null && !_buttonKeyNext.Sensitive) _buttonKeyNext.Sensitive = true;
                //};
            };

            //Toolbar Buttons
            if (_listStoreModelTotalItemsTicketListMode == 0)
            {
                if (_buttonKeyFinishOrder != null && _buttonKeyFinishOrder.Sensitive) _buttonKeyFinishOrder.Sensitive = false;
                //Toolbar Buttons
                //Commented to Always Leave app event when have articles in session
                //if (_toolbarApplicationClose != null) _toolbarApplicationClose.Sensitive = true;

                //TODO: PRIVILEGIOS
                if (_toolbarBackOffice != null /*&& !_toolbarBackOffice.Sensitive*/) _toolbarBackOffice.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_ACCESS");
                if (_toolbarReports != null /*&& !_toolbarReports.Sensitive*/) _toolbarReports.Sensitive = GeneralSettings.HasPermissionTo("REPORT_ACCESS");
                if (_toolbarShowSystemDialog != null && !_toolbarShowSystemDialog.Sensitive) _toolbarShowSystemDialog.Sensitive = GeneralSettings.HasPermissionTo("SYSTEM_ACCESS");
                if (_toolbarLogoutUser != null && !_toolbarLogoutUser.Sensitive) _toolbarLogoutUser.Sensitive = true;
                if (_toolbarShowChangeUserDialog != null && !_toolbarShowChangeUserDialog.Sensitive) _toolbarShowChangeUserDialog.Sensitive = true;
                if (_toolbarCashDrawer != null /*&& !_toolbarCashDrawer.Sensitive*/) _toolbarCashDrawer.Sensitive = (GeneralSettings.HasPermissionTo("WORKSESSION_ALL"));
                if (_toolbarFinanceDocuments != null && !_toolbarFinanceDocuments.Sensitive) _toolbarFinanceDocuments.Sensitive = true;
                //With Valid Open WorkSessionPeriodTerminal
                if (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
                {
                    if (_toolbarNewFinanceDocument != null && !_toolbarNewFinanceDocument.Sensitive) _toolbarNewFinanceDocument.Sensitive = true;
                }
            }
            else
            {
                if (_buttonKeyFinishOrder != null && !_buttonKeyFinishOrder.Sensitive) _buttonKeyFinishOrder.Sensitive = true;
                //Toolbar Buttons
                //Commented to Always Leave app event when have articles in session
                //if (_toolbarApplicationClose != null && _toolbarApplicationClose.Sensitive) _toolbarApplicationClose.Sensitive = false;
                if (_toolbarBackOffice != null && _toolbarBackOffice.Sensitive) _toolbarBackOffice.Sensitive = false;
                if (_toolbarReports != null && _toolbarReports.Sensitive) _toolbarReports.Sensitive = false;
                if (_toolbarShowSystemDialog != null && _toolbarShowSystemDialog.Sensitive) _toolbarShowSystemDialog.Sensitive = false;
                if (_toolbarLogoutUser != null && _toolbarLogoutUser.Sensitive) _toolbarLogoutUser.Sensitive = false;
                if (_toolbarShowChangeUserDialog != null && _toolbarShowChangeUserDialog.Sensitive) _toolbarShowChangeUserDialog.Sensitive = false;
                if (_toolbarCashDrawer != null && _toolbarCashDrawer.Sensitive) _toolbarCashDrawer.Sensitive = false;
                if (_toolbarFinanceDocuments != null && _toolbarFinanceDocuments.Sensitive) _toolbarFinanceDocuments.Sensitive = false;
                if (_toolbarNewFinanceDocument != null && _toolbarNewFinanceDocument.Sensitive) _toolbarNewFinanceDocument.Sensitive = false;
            }

            UpdateTicketListOrderButtons();
        }

        public void UpdateTicketListOrderButtons()
        {
            //Check if Buttons are already created
            if (_buttonKeyPayments != null)
            {
                //If has a Working Order
                if (POSSession.CurrentSession.CurrentOrderMainId != Guid.Empty && POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId))
                {
                    // Always Enable Buttons if have Order/Table Open
                    _buttonKeyBarCode.Sensitive = true;

                    if (_listStoreModelTotalItemsTicketListMode > 0 &&
                            CurrentOrderDetails.TotalFinal > 0.00m)
                    {
                        _buttonKeyPayments.Sensitive = true;
                        _buttonKeySplitAccount.Sensitive = true;
                        _buttonKeyChangeTable.Sensitive = false;
                        if (POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].OrderStatus == OrderStatus.Open)
                        {
                            _buttonKeyListOrder.Sensitive = true;
                        }
                        else
                        {
                            _buttonKeyListOrder.Sensitive = false;
                        }
                    }
                    else if (_listStoreModelTotalItemsTicketListMode == 0)
                    {
                        if (POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].OrderStatus == OrderStatus.Open &&
                            _articleBag.TotalFinal > 0.00m)
                        {
                            _buttonKeyPayments.Sensitive = true;
                            _buttonKeySplitAccount.Sensitive = true;
                            _buttonKeyChangeTable.Sensitive = true;
                            _buttonKeyListOrder.Sensitive = true;
                        }
                        else
                        {
                            _buttonKeyPayments.Sensitive = false;
                            _buttonKeySplitAccount.Sensitive = false;
                            _buttonKeyChangeTable.Sensitive = false;
                            _buttonKeyListOrder.Sensitive = false;
                        }
                    }
                }
                else
                {
                    _buttonKeyPayments.Sensitive = false;
                    _buttonKeySplitAccount.Sensitive = false;
                    _buttonKeyChangeTable.Sensitive = false;
                    _buttonKeyListOrder.Sensitive = false;
                    _buttonKeyBarCode.Sensitive = false;
                }

                //Force Update Article Bag Count in TicketMode, Required to Update ListMode Button, for Sync MultiUser
                if (ListMode == TicketListMode.Ticket) UpdateArticleBag();

                if (_articleBag != null && _articleBag.Count > 0)
                {
                    _buttonKeyListMode.Sensitive = true;
                }
                else
                {
                    _buttonKeyListMode.Sensitive = false;
                }
            }
        }

        //Update TicketList Widget Total
        private void UpdateTicketListTotal()
        {
            decimal TotalFinal;
            string labelTotalFinal;

            if (ListMode == TicketListMode.Ticket)
            {
                labelTotalFinal = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_ticket");
                TotalFinal = CurrentOrderDetails.TotalFinal;
            }
            else
            {
                labelTotalFinal = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_table_tickets");
                //Toatal From ArticleBag and Not From OrderMain, This way we can check if ArticleBag is equal to OrderMain Totals, in Both Status Bars
                TotalFinal = _articleBag.TotalFinal;
            }
            _labelLabelTotal.Text = labelTotalFinal;
            _labelTotal.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym);

            //Update Display
            if (GlobalApp.UsbDisplay != null) GlobalApp.UsbDisplay.ShowOrder(CurrentOrderDetails, _listStoreModelSelectedIndex);
        }

        public void UpdateOrderStatusBar()
        {
            //_logger.Debug("void UpdateOrderStatusBar() :: Starting..."); /* IN009008 */
            //If CashDrawer Open
            if (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
            {
                //If has Working Order
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
                    /* IN008024 */
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
                        //Totals From OrderMain and Not From ArticleBag
                        LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(orderMain.GlobalTotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym),
                        orderMain.GlobalTotalTickets
                      );

                    //If in OrderMain Mode Update Total
                    if (ListMode == TicketListMode.OrderMain)
                        _labelTotal.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(orderMain.GlobalTotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym);
                }
                else
                {
                    /* IN008024 */
                    SourceWindow.LabelCurrentTable.Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("status_message_select_order_or_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower());
                }
            }
            //If CashDrawer Close
            else
            {
                SourceWindow.LabelCurrentTable.Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "status_message_open_cashdrawer");
            }
        }

        private decimal GetArticleDefaultQuantity(Guid pArticleOid)
        {
            //Get Article
            fin_article article = (fin_article)XPOHelper.GetXPGuidObject(typeof(fin_article), pArticleOid);
            //Get Default Article Quantity
            decimal defaultQuantity;
            if (article.DefaultQuantity > 0) { defaultQuantity = article.DefaultQuantity; } else { defaultQuantity = 1.00m; };

            return defaultQuantity;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        //Used to assign _treePath to last Inserted Item in listStore, ex get _treePath to last, to start in last Item in Model
        private void _listStoreModel_RowInserted(object o, RowInsertedArgs args)
        {
            ListStore listStore = (ListStore)o;
            _treeIter = args.Iter;
            _treePath = listStore.GetPath(args.Iter);

            //Update Total Items Member
            _listStoreModelTotalItems = listStore.IterNChildren();
            if (ListMode == TicketListMode.Ticket) _listStoreModelTotalItemsTicketListMode = listStore.IterNChildren();

            //Assign current index to _listStoreModelTotalItems - 1
            _listStoreModelSelectedIndex = _listStoreModelTotalItems - 1;
        }

        private void _treeView_CursorChanged(object sender, EventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            TreeSelection selection = treeView.Selection;
            TreeModel model;

            // The _treeIter will point to the selected row
            if (selection.GetSelected(out model, out _treeIter))
            {
                _treePath = model.GetPath(_treeIter);
                try
                {
                    // Get Article Oid from treeIter
                    _currentDetailArticleOid = (Guid)ListStoreModel.GetValue(_treeIter, 0);
                    // Get Article
                    _currentDetailArticle = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), _currentDetailArticleOid);

                    //Ticket List Mode
                    if (ListMode == TicketListMode.Ticket)
                    {
                        //Get ArticleId from OrderDetails with LINQ
                        _listStoreModelSelectedIndex = CurrentOrderDetails.Lines.FindIndex(item => item.ArticleOid == _currentDetailArticleOid);
                    }
                    //OrderMain List Mode
                    else
                    {
                        ArticleBagKey articleBagKey = (ArticleBagKey)ListStoreModel.GetValue(_treeIter, 7);
                        if (articleBagKey != null)
                        {
                            _listStoreModelSelectedIndex = _articleBag[articleBagKey].ListIndex;
                        }
                    }
                    //Debug
                    //_logger.Debug(string.Format("_treeView_CursorChanged(): _currentDetailArticleId [{0}], _listStoreModelSelectedIndex [{1}], _currentDetailArticle.Designation [{2}]", _currentDetailArticleOid, _listStoreModelSelectedIndex, _currentDetailArticle.Designation));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
            else
            {
                _listStoreModelSelectedIndex = -1;
            }
            //Update Button State
            UpdateTicketListButtons();
        }
    }
}
