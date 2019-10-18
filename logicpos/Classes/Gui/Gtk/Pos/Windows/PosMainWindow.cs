using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Logic.Others;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections;
using System.Drawing;
using System.Threading;

namespace logicpos
{
    public partial class PosMainWindow : PosBaseWindow
    {
     
        //Files
        private string _fileBaseButtonOverlay = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Buttons\Pos\button_overlay.png");
        
		/* IN006045 */
        //private string _clockFormat = GlobalFramework.Settings["dateTimeFormatStatusBar"];
        private string _clockFormat = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "frontoffice_datetime_format_status_bar");

        private Color _colorPosNumberPadLeftButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosNumberPadLeftButtonBackground"]);
        private Color _colorPosNumberRightButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosNumberRightButtonBackground"]);
        private Color _colorPosHelperBoxsBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosHelperBoxsBackground"]);
        //UI
        private Fixed _fixedWindow;
        private Label _labelClock;
        private TextView _textviewLog;
        private TicketList _ticketList;
        public TicketList TicketList
        {
            get { return _ticketList; }
        }
        private TouchButtonIconWithText _touchButtonPosToolbarApplicationClose;
        private TouchButtonIconWithText _touchButtonPosToolbarBackOffice;
        private TouchButtonIconWithText _touchButtonPosToolbarReports;
        private TouchButtonIconWithText _touchButtonPosToolbarShowSystemDialog;
        private TouchButtonIconWithText _touchButtonPosToolbarLogoutUser;
        private TouchButtonIconWithText _touchButtonPosToolbarShowChangeUserDialog;
        private TouchButtonIconWithText _touchButtonPosToolbarCashDrawer;
        private TouchButtonIconWithText _touchButtonPosToolbarFinanceDocuments;
        private TouchButtonIconWithText _touchButtonPosToolbarNewFinanceDocument;
        public TouchButtonIconWithText TouchButtonPosToolbarNewFinanceDocument
        {
            get { return _touchButtonPosToolbarNewFinanceDocument; }
            set { _touchButtonPosToolbarNewFinanceDocument = value; }
        }
        private TicketPad _ticketPad;
        //Others
        private uint _borderWidth = 5;

        //Public Properties
        private TablePad _tablePadFamily;
        internal TablePad TablePadFamily
        {
            get { return _tablePadFamily; }
            set { _tablePadFamily = value; }
        }
        //SubFamily TablePad
        private TablePad _tablePadSubFamily;
        internal TablePad TablePadSubFamily
        {
            get { return _tablePadSubFamily; }
            set { _tablePadSubFamily = value; }
        }
        //Article TablePad
        private TablePad _tablePadArticle;
        internal TablePad TablePadArticle
        {
            get { return _tablePadArticle; }
            set { _tablePadArticle = value; }
        }
        //BufferTextView
        private TextBuffer _bufferTextView;
        public TextBuffer BufferTextView
        {
            get { return _bufferTextView; }
            set { _bufferTextView = value; }
        }
        //LabelCurrentUserName
        private Label _labelTerminalInfo;
        public Label LabelTerminalInfo
        {
            get { return _labelTerminalInfo; }
            set { _labelTerminalInfo = value; }
        }
        //LabelCurrentTable
        private Label _labelCurrentTable;
        public Label LabelCurrentTable
        {
            get { return _labelCurrentTable; }
            set { _labelCurrentTable = value; }
        }
        //LabelTotalTable
        private Label _labelTotalTable;
        public Label LabelTotalTable
        {
            get { return _labelTotalTable; }
            set { _labelTotalTable = value; }
        }

        //Constructor
        public PosMainWindow(String pBackgroundImage)
            : base(pBackgroundImage)
        {
            try
            {
				/* IN009005 */
                //GlobalApp.DialogThreadNotify.WakeupMain();

                _fixedWindow = new Fixed();

                //New Thread InitUI 
				/* IN009005 */
				//Thread thread = new Thread(new ThreadStart(InitUI));
                //GlobalApp.DialogThreadNotify = new ThreadNotify (new ReadyEvent (Utils.ThreadDialogReadyEvent));
				//thread.Start();
				InitUI();

                //Use Startup Window, not this Window, because it is not visible, it is in construction mode
				/* IN009005 */
				//GlobalApp.DialogThreadWork = Utils.GetThreadDialog(GlobalApp.WindowStartup);
				//GlobalApp.DialogThreadWork.Run();

                //Call - To Update start _labelCurrentTable.Text
                _ticketList.UpdateOrderStatusBar();

                //Update WorkSessionUI Before Clock
                UpdateWorkSessionUI();

                //Clock
                StartClock();

                //Startup Filter
                _tablePadArticle.Filter = " AND (Favorite = 1)";

                //Always update buttons when construct window, may return from a program crash
                _ticketList.UpdateTicketListButtons();

                this.ScreenArea.Add(_fixedWindow);

                //Place Minimize EventBox : After InitUI, to be placed Above all Other
                bool _showMinimize = (!string.IsNullOrEmpty(GlobalFramework.Settings["appShowMinimize"])) 
                    ? Convert.ToBoolean(GlobalFramework.Settings["appShowMinimize"])
                    : false;
                if (_showMinimize)
                {
                    EventBox eventBoxMinimize = Utils.GetMinimizeEventBox();
                    eventBoxMinimize.ButtonReleaseEvent += delegate { Iconify(); };
                    _fixedWindow.Put(eventBoxMinimize, GlobalApp.ScreenSize.Width - 27 - 10, 10);
                }

                this.ShowAll();

                //Window Events
                this.WindowStateEvent += PosMainWindow_WindowStateEvent;
                //Update UI if has a Working Order (Initialized by SessionApp)
                this.ExposeEvent += delegate { UpdateUIIfHasWorkingOrder(); };
                this.KeyReleaseEvent += PosMainWindow_KeyReleaseEvent;

                //Hardware Events
                if (GlobalFramework.LoggedTerminal.BarcodeReader != null || GlobalFramework.LoggedTerminal.CardReader != null)
                {
                    GlobalApp.BarCodeReader.Captured += HWBarCodeReader_Captured;
                }                

                _log.Debug("PosMainWindow(String pBackgroundImage) :: Completed!"); /* IN009008 */
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void InitUI()
        {

            _log.Debug("void InitUI() :: Initializing UI for POS Main Window..."); /* IN009008 */

            //Init Theme Object
            Predicate <dynamic> predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosMainWindow");
            dynamic themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);

            //Shared error Message
            string errorMessage = "Node: <Window ID=\"PosMainWindow\">";

            //Assign Theme Vars + UI
            if (themeWindow != null)
            {
                try
                {
                    //Globals
                    Name = Convert.ToString(themeWindow.Globals.Name);

                    //Init Components
                    InitUIEventBoxImageLogo(themeWindow);
                    InitUIEventBoxStatusBar1(themeWindow);
                    InitUIEventBoxStatusBar2(themeWindow);
                    InitUIButtonFavorites(themeWindow);
                    InitUITablePads(themeWindow);
                    InitUIEventBoxPosTicketList(themeWindow);
                    InitUIEventBoxPosTicketPad(themeWindow);
                    //After InitUIEventBoxPosTicketList, require _ticketList initialized
                    InitUiEventboxToolbar(themeWindow);

                    _log.Debug("void InitUI() :: POS Main Window theme rendering completed!"); /* IN009008 */
                    //Notify Thread End
                    GlobalApp.DialogThreadNotify.WakeupMain();
                    
                    //Check if fiscal year was created
                    SortingCollection sortCollection = new SortingCollection();
                    sortCollection.Add(new SortProperty("FiscalYear", DevExpress.Xpo.DB.SortingDirection.Ascending));
                    CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                    ICollection collectionDocumentFinanceSeries = GlobalFramework.SessionXpo.GetObjects(GlobalFramework.SessionXpo.GetClassInfo(typeof(fin_documentfinanceyearserieterminal)), criteria, sortCollection, int.MaxValue, false, true);
                    if (collectionDocumentFinanceSeries.Count == 0)
                    {
                        Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_warning"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_warning_open_fiscal_year"));
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    Utils.ShowMessageTouchErrorRenderTheme(this, string.Format("{1}{0}{0}{2}", Environment.NewLine, errorMessage, ex.Message));
                }
            }
            else
            {
                Utils.ShowMessageTouchErrorRenderTheme(this, errorMessage);
            }
        }

        private void InitUIEventBoxImageLogo(dynamic pThemeWindow)
        {
            _log.Debug("void InitUIEventBoxImageLogo(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:EventBoxImageLogo
            Position eventBoxImageLogoPosition = Utils.StringToPosition(themeWindow.Objects.EventBoxImageLogo.Position);
            Size eventBoxImageLogoSize = Utils.StringToSize(themeWindow.Objects.EventBoxImageLogo.Size);
            bool eventBoxImageLogoVisible = Convert.ToBoolean(themeWindow.Objects.EventBoxImageLogo.Visible);
            bool eventBoxImageLogoVisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxImageLogo.VisibleWindow);
            Gdk.Color eventBoxImageLogoBackgroundColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxImageLogo.BackgroundColor);

            //UI
            EventBox eventBoxImageLogo = new EventBox();
            eventBoxImageLogo.WidthRequest = eventBoxImageLogoSize.Width;
            eventBoxImageLogo.HeightRequest = eventBoxImageLogoSize.Height;
            eventBoxImageLogo.VisibleWindow = eventBoxImageLogoVisibleWindow;
            if (eventBoxImageLogoVisibleWindow) eventBoxImageLogo.ModifyBg(Gtk.StateType.Normal, eventBoxImageLogoBackgroundColor);
            if (eventBoxImageLogoVisible) _fixedWindow.Put(eventBoxImageLogo, eventBoxImageLogoPosition.X, eventBoxImageLogoPosition.Y);
            eventBoxImageLogo.ButtonPressEvent += eventBoxImageLogo_ButtonPressEvent;
        }

        private void InitUIEventBoxStatusBar1(dynamic pThemeWindow)
        {
            _log.Debug("void InitUIEventBoxStatusBar1(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:EventBoxStatusBar1
            Position eventBoxStatusBar1Position = Utils.StringToPosition(themeWindow.Objects.EventBoxStatusBar1.Position); ;
            Size eventBoxStatusBar1Size = Utils.StringToSize(themeWindow.Objects.EventBoxStatusBar1.Size);
            bool eventBoxStatusBar1Visible = Convert.ToBoolean(themeWindow.Objects.EventBoxStatusBar1.Visible);
            bool eventBoxStatusBar1VisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxStatusBar1.VisibleWindow);
            Gdk.Color eventBoxStatusBar1BackgroundColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxStatusBar1.BackgroundColor);

            //Objects:EventBoxStatusBar1:LabelTerminalInfo
            Pango.FontDescription labelTerminalInfoFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar1.LabelTerminalInfo.Font);
            Gdk.Color labelTerminalInfoFontColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxStatusBar1.LabelTerminalInfo.FontColor);
            float labelTerminalInfoAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar1.LabelTerminalInfo.AlignmentX);

            //Objects:EventBoxStatusBar1:LabelClock
            Pango.FontDescription labelClockFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar1.LabelClock.Font);
            Gdk.Color labelClockFontColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxStatusBar1.LabelClock.FontColor);
            float labelClockAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar1.LabelClock.AlignmentX);

            //UI
            //eventBoxStatusBar1
            EventBox eventBoxStatusBar1 = new EventBox() { VisibleWindow = eventBoxStatusBar1VisibleWindow };
            eventBoxStatusBar1.WidthRequest = eventBoxStatusBar1Size.Width;
            eventBoxStatusBar1.HeightRequest = eventBoxStatusBar1Size.Height;
            eventBoxStatusBar1.ModifyBg(Gtk.StateType.Normal, eventBoxStatusBar1BackgroundColor);

            //EventBoxStatusBar1:LabelTerminalInfo
            _labelTerminalInfo = new Label(string.Format("{0} : {1}", GlobalFramework.LoggedTerminal.Designation, GlobalFramework.LoggedUser.Name));
            _labelTerminalInfo.ModifyFont(labelTerminalInfoFont);
            _labelTerminalInfo.ModifyFg(StateType.Normal, labelTerminalInfoFontColor);
            _labelTerminalInfo.SetAlignment(labelTerminalInfoAlignmentX, 0.5F);

            //EventBoxStatusBar1:LabelClock
            _labelClock = new Label(FrameworkUtils.CurrentDateTime(_clockFormat));
            _labelClock.ModifyFont(labelClockFont);
            _labelClock.ModifyFg(StateType.Normal, labelClockFontColor);
            _labelClock.SetAlignment(labelClockAlignmentX, 0.5F);

            //Pack HBox EventBoxStatusBar1
            HBox hboxStatusBar1 = new HBox(false, 0) { BorderWidth = _borderWidth };
            hboxStatusBar1.PackStart(_labelTerminalInfo, false, false, 0);
            hboxStatusBar1.PackStart(_labelClock, true, true, 0);
            eventBoxStatusBar1.Add(hboxStatusBar1);

            if (eventBoxStatusBar1Visible) _fixedWindow.Put(eventBoxStatusBar1, eventBoxStatusBar1Position.X, eventBoxStatusBar1Position.Y);
        }

        private void InitUIEventBoxStatusBar2(dynamic pThemeWindow)
        {
            _log.Debug("void InitUIEventBoxStatusBar2(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:EventBoxStatusBar2
            Position eventBoxStatusBar2Position = Utils.StringToPosition(themeWindow.Objects.EventBoxStatusBar2.Position); ;
            Size eventBoxStatusBar2Size = Utils.StringToSize(themeWindow.Objects.EventBoxStatusBar2.Size);
            bool eventBoxStatusBar2Visible = Convert.ToBoolean(themeWindow.Objects.EventBoxStatusBar2.Visible);
            bool eventBoxStatusBar2VisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxStatusBar2.VisibleWindow);
            Gdk.Color eventBoxStatusBar2BackgroundColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxStatusBar2.BackgroundColor);

            //Objects:EventBoxStatusBar2:LabelCurrentTableLabel
            Pango.FontDescription labelCurrentTableLabelFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTableLabel.Font);
            Gdk.Color labelCurrentTableLabelFontColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTableLabel.FontColor);
            float labelCurrentTableLabelAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTableLabel.AlignmentX);

            //Objects:EventBoxStatusBar2:LabelCurrentTable
            Pango.FontDescription labelCurrentTableFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTable.Font);
            Gdk.Color labelCurrentTableFontColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTable.FontColor);
            float labelCurrentTableAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTable.AlignmentX);

            //Objects:EventBoxStatusBar2:LabelTotalTableLabel
            Pango.FontDescription labelTotalTableLabelFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTableLabel.Font);
            Gdk.Color labelTotalTableLabelFontColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTableLabel.FontColor);
            float labelTotalTableLabelAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTableLabel.AlignmentX);

            //Objects:EventBoxStatusBar2:LabelTotalTable
            Pango.FontDescription labelTotalTableFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTable.Font);
            Gdk.Color labelTotalTableFontColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTable.FontColor);
            float labelTotalTableAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTable.AlignmentX);

            //UI

            //EventBoxStatusBar2
            EventBox eventBoxStatusBar2 = new EventBox() { VisibleWindow = eventBoxStatusBar2VisibleWindow };
            eventBoxStatusBar2.WidthRequest = eventBoxStatusBar2Size.Width;
            eventBoxStatusBar2.HeightRequest = eventBoxStatusBar2Size.Height;
            eventBoxStatusBar2.ModifyBg(Gtk.StateType.Normal, eventBoxStatusBar2BackgroundColor);

            //EventBoxStatusBar2:vboxCurrentTable:LabelCurrentTableLabel
            string global_table = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], string.Format("global_table_appmode_{0}", SettingsApp.CustomAppOperationMode.AppOperationTheme).ToLower()); /* IN008024 */
            Label labelCurrentTableLabel = new Label(global_table);
            labelCurrentTableLabel.ModifyFont(labelCurrentTableLabelFont);
            labelCurrentTableLabel.ModifyFg(StateType.Normal, labelCurrentTableLabelFontColor);
            labelCurrentTableLabel.SetAlignment(labelCurrentTableLabelAlignmentX, 0.5F);

            //EventBoxStatusBar2:vboxCurrentTable:LabelCurrentTable
            _labelCurrentTable = new Label();//Text assigned on TicketList.UpdateOrderStatusBar()
            _labelCurrentTable.ModifyFont(labelCurrentTableFont);
            _labelCurrentTable.ModifyFg(StateType.Normal, labelCurrentTableFontColor);
            _labelCurrentTable.SetAlignment(labelCurrentTableAlignmentX, 0.5F);

            //Pack
            VBox vboxCurrentTable = new VBox(false, 1);
            vboxCurrentTable.PackStart(labelCurrentTableLabel);
            vboxCurrentTable.PackStart(_labelCurrentTable);

            //EventBoxStatusBar2:vboxTotalTable:LabelTotalTableLabel
            Label labelTotalTableLabel = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_price_to_pay"));
            labelTotalTableLabel.ModifyFont(labelTotalTableLabelFont);
            labelTotalTableLabel.ModifyFg(StateType.Normal, labelTotalTableLabelFontColor);
            labelTotalTableLabel.SetAlignment(labelTotalTableLabelAlignmentX, 0.5F);

            //EventBoxStatusBar2:vboxTotalTable:LabelTotalTable
            _labelTotalTable = new Label(FrameworkUtils.DecimalToStringCurrency(0));
            _labelTotalTable.ModifyFont(labelTotalTableFont);
            _labelTotalTable.ModifyFg(StateType.Normal, labelTotalTableFontColor);
            _labelTotalTable.SetAlignment(labelTotalTableAlignmentX, 0.5F);

            //Pack
            VBox vboxTotalTable = new VBox(false, 1);
            vboxTotalTable.PackStart(labelTotalTableLabel);
            vboxTotalTable.PackStart(_labelTotalTable);

            //Pack HBox StatusBar
            HBox hboxStatusBar2 = new HBox(false, 0) { BorderWidth = _borderWidth };
            hboxStatusBar2.PackStart(vboxCurrentTable, true, true, 0);
            hboxStatusBar2.PackStart(vboxTotalTable, false, false, 0);
            eventBoxStatusBar2.Add(hboxStatusBar2);

            if (eventBoxStatusBar2Visible) _fixedWindow.Put(eventBoxStatusBar2, eventBoxStatusBar2Position.X, eventBoxStatusBar2Position.Y);
        }

        private void InitUIButtonFavorites(dynamic pThemeWindow)
        {
            _log.Debug("void InitUIButtonFavorites(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:ButtonFavorites
            Position buttonFavoritesPosition = Utils.StringToPosition(themeWindow.Objects.ButtonFavorites.Position);
            Size buttonFavoritesButtonSize = Utils.StringToSize(themeWindow.Objects.ButtonFavorites.ButtonSize);
            string buttonFavoritesImageFileName = themeWindow.Objects.ButtonFavorites.ImageFileName;
            string buttonFavoritesText = themeWindow.Objects.ButtonFavorites.Text;
            int buttonFavoritesFontSize = Convert.ToInt16(themeWindow.Objects.ButtonFavorites.FontSize);
            bool buttonFavoritesUseImageOverlay = Convert.ToBoolean(themeWindow.Objects.ButtonFavorites.UseImageOverlay);
            bool buttonFavoritesVisible = Convert.ToBoolean(themeWindow.Objects.ButtonFavorites.Visible);

            //UI

            string buttonFavoritesImageOverlay = (buttonFavoritesUseImageOverlay) ? _fileBaseButtonOverlay : string.Empty;
            TouchButtonImage buttonFavorites = new TouchButtonImage("buttonFavorites", Color.Transparent, buttonFavoritesText, buttonFavoritesFontSize, buttonFavoritesImageFileName, buttonFavoritesImageOverlay, buttonFavoritesButtonSize.Width, buttonFavoritesButtonSize.Height);
            buttonFavorites.Clicked += buttonFavorites_Clicked;

            if (buttonFavoritesVisible) _fixedWindow.Put(buttonFavorites, buttonFavoritesPosition.X, buttonFavoritesPosition.Y);
        }

        private void InitUITablePads(dynamic pThemeWindow)
        {
            _log.Debug("void InitUITablePads(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:TablePadFamilyButtonPrev
            Position TablePadFamilyButtonPrevPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.Position);
            Size TablePadFamilyButtonPrevSize = Utils.StringToSize(themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.Size);
            string TablePadFamilyButtonPrevImageFileName = themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.ImageFileName;
            //Objects:TablePadFamilyButtonNext
            Position TablePadFamilyButtonNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.Position);
            Size TablePadFamilyButtonNextSize = Utils.StringToSize(themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.Size);
            string TablePadFamilyButtonNextImageFileName = themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.ImageFileName;
            //Objects:TablePadFamily
            Position tablePadFamilyPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.Position);
            Size tablePadFamilyButtonSize = Utils.StringToSize(themeWindow.Objects.TablePadFamily.ButtonSize);
            TableConfig tablePadFamilyTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadFamily.TableConfig);
            bool tablePadFamilyVisible = Convert.ToBoolean(themeWindow.Objects.TablePadFamily.Visible);

            //Objects:TablePadSubFamilyButtonPrev
            Position TablePadSubFamilyButtonPrevPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.Position);
            Size TablePadSubFamilyButtonPrevSize = Utils.StringToSize(themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.Size);
            string TablePadSubFamilyButtonPrevImageFileName = themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.ImageFileName;
            //Objects:TablePadSubFamilyButtonNext
            Position TablePadSubFamilyButtonNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.Position);
            Size TablePadSubFamilyButtonNextSize = Utils.StringToSize(themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.Size);
            string TablePadSubFamilyButtonNextImageFileName = themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.ImageFileName;
            //Objects:TablePadSubFamily
            Position tablePadSubFamilyPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.Position);
            Size tablePadSubFamilyButtonSize = Utils.StringToSize(themeWindow.Objects.TablePadSubFamily.ButtonSize);
            TableConfig tablePadSubFamilyTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadSubFamily.TableConfig);
            bool tablePadSubFamilyVisible = Convert.ToBoolean(themeWindow.Objects.TablePadSubFamily.Visible);

            //Objects:TablePadArticleButtonPrev
            Position TablePadArticleButtonPrevPosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.Position);
            Size TablePadArticleButtonPrevSize = Utils.StringToSize(themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.Size);
            string TablePadArticleButtonPrevImageFileName = themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.ImageFileName;
            //Objects:TablePadArticleButtonNext
            Position TablePadArticleButtonNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.Position);
            Size TablePadArticleButtonNextSize = Utils.StringToSize(themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.Size);
            string TablePadArticleButtonNextImageFileName = themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.ImageFileName;
            //Objects:TablePadArticle
            Position tablePadArticlePosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.Position);
            Size tablePadArticleButtonSize = Utils.StringToSize(themeWindow.Objects.TablePadArticle.ButtonSize);
            TableConfig tablePadArticleTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadArticle.TableConfig);
            bool tablePadArticleVisible = Convert.ToBoolean(themeWindow.Objects.TablePadArticle.Visible);

            //UI

            //Objects:TablePadFamilyButtonPrev
            TouchButtonIcon TablePadFamilyButtonPrev = new TouchButtonIcon("TablePadFamilyButtonPrev", Color.Transparent, TablePadFamilyButtonPrevImageFileName, new Size(TablePadFamilyButtonPrevSize.Width - 2, TablePadFamilyButtonPrevSize.Height - 2), TablePadFamilyButtonPrevSize.Width, TablePadFamilyButtonPrevSize.Height);
            TablePadFamilyButtonPrev.Relief = ReliefStyle.None;
            TablePadFamilyButtonPrev.BorderWidth = 0;
            TablePadFamilyButtonPrev.CanFocus = false;
            //Objects:TablePadFamilyButtonNext
            TouchButtonIcon TablePadFamilyButtonNext = new TouchButtonIcon("TablePadFamilyButtonNext", Color.Transparent, TablePadFamilyButtonNextImageFileName, new Size(TablePadFamilyButtonNextSize.Width - 2, TablePadFamilyButtonNextSize.Height - 2), TablePadFamilyButtonNextSize.Width, TablePadFamilyButtonNextSize.Height);
            TablePadFamilyButtonNext.Relief = ReliefStyle.None;
            TablePadFamilyButtonNext.BorderWidth = 0;
            TablePadFamilyButtonNext.CanFocus = false;
            //Objects:TablePadFamily
            String sqlTablePadFamily = @"
                SELECT 
                    Oid as id, Designation as name, ButtonLabel as label, ButtonImage as image,
                    (SELECT COUNT(*) as childs FROM fin_articlesubfamily WHERE (Disabled IS NULL or Disabled  <> 1) AND Family = p.Oid) as childs
                FROM 
                    fin_articlefamily as p 
                WHERE 
                    (Disabled IS NULL or Disabled <> 1)
            ";
            _tablePadFamily = new TablePad(
                sqlTablePadFamily, 
                "ORDER BY Ord", 
                "", 
                Guid.Empty, 
                true,
                tablePadFamilyTableConfig.Rows, 
                tablePadFamilyTableConfig.Columns, 
                "buttonFamilyId", 
                Color.Transparent, 
                tablePadFamilyButtonSize.Width, 
                tablePadFamilyButtonSize.Height, 
                TablePadFamilyButtonPrev, 
                TablePadFamilyButtonNext
            );
            _tablePadFamily.SourceWindow = this;
            _tablePadFamily.Clicked += _tablePadFamily_Clicked;
            //Put
            if (tablePadFamilyVisible)
            {
                _fixedWindow.Put(TablePadFamilyButtonPrev, TablePadFamilyButtonPrevPosition.X, TablePadFamilyButtonPrevPosition.Y);
                _fixedWindow.Put(TablePadFamilyButtonNext, TablePadFamilyButtonNextPosition.X, TablePadFamilyButtonNextPosition.Y);
                _fixedWindow.Put(_tablePadFamily, tablePadFamilyPosition.X, tablePadFamilyPosition.Y);
            }

            //Objects:TablePadSubFamilyButtonPrev
            TouchButtonIcon TablePadSubFamilyButtonPrev = new TouchButtonIcon("TablePadSubFamilyButtonPrev", Color.Transparent, TablePadSubFamilyButtonPrevImageFileName, new Size(TablePadSubFamilyButtonPrevSize.Width - 6/*2*/, TablePadSubFamilyButtonPrevSize.Height - 6/*2*/), TablePadSubFamilyButtonPrevSize.Width, TablePadSubFamilyButtonPrevSize.Height);
            TablePadSubFamilyButtonPrev.Relief = ReliefStyle.None;
            TablePadSubFamilyButtonPrev.BorderWidth = 0;
            TablePadSubFamilyButtonPrev.CanFocus = false;
            //Objects:TablePadSubFamilyButtonNext
            TouchButtonIcon TablePadSubFamilyButtonNext = new TouchButtonIcon("TablePadSubFamilyButtonNext", Color.Transparent, TablePadSubFamilyButtonNextImageFileName, new Size(TablePadSubFamilyButtonNextSize.Width - 6/*2*/, TablePadSubFamilyButtonNextSize.Height - 6/*2*/), TablePadSubFamilyButtonNextSize.Width, TablePadSubFamilyButtonNextSize.Height);
            TablePadSubFamilyButtonNext.Relief = ReliefStyle.None;
            TablePadSubFamilyButtonNext.BorderWidth = 0;
            TablePadSubFamilyButtonNext.CanFocus = false;
            //Objects:TablePadSubFamily
            String sqlTablePadSubFamily = @"
                SELECT 
                    Oid as id, Designation as name, ButtonLabel as label, ButtonImage as image,
                    (SELECT COUNT(*) as childs FROM fin_article WHERE (Disabled IS NULL or Disabled  <> 1) AND SubFamily = p.Oid) as childs
                FROM 
                    fin_articlesubfamily as p 
                WHERE 
                    (Disabled IS NULL or Disabled <> 1)
            ";
            String filterTablePadSubFamily = " AND (Family = '" + TablePadFamily.SelectedButtonOid + "')";
            _tablePadSubFamily = new TablePad(
                sqlTablePadSubFamily, 
                "ORDER BY Ord", 
                filterTablePadSubFamily, 
                Guid.Empty, 
                true, 
                tablePadSubFamilyTableConfig.Rows, 
                tablePadSubFamilyTableConfig.Columns, 
                "buttonSubFamilyId", 
                Color.Transparent, 
                tablePadSubFamilyButtonSize.Width, 
                tablePadSubFamilyButtonSize.Height, 
                TablePadSubFamilyButtonPrev, 
                TablePadSubFamilyButtonNext
            );
            _tablePadSubFamily.SourceWindow = this;
            _tablePadSubFamily.Clicked += _tablePadSubFamily_Clicked;
            //Put
            if (tablePadSubFamilyVisible)
            {
                _fixedWindow.Put(TablePadSubFamilyButtonPrev, TablePadSubFamilyButtonPrevPosition.X, TablePadSubFamilyButtonPrevPosition.Y);
                _fixedWindow.Put(TablePadSubFamilyButtonNext, TablePadSubFamilyButtonNextPosition.X, TablePadSubFamilyButtonNextPosition.Y);
                _fixedWindow.Put(_tablePadSubFamily, tablePadSubFamilyPosition.X, tablePadSubFamilyPosition.Y);
            }

            //Objects:TablePadArticleButtonPrev
            TouchButtonIcon TablePadArticleButtonPrev = new TouchButtonIcon("TablePadArticleButtonPrev", Color.Transparent, TablePadArticleButtonPrevImageFileName, new Size(TablePadArticleButtonPrevSize.Width - 6/*2*/, TablePadArticleButtonPrevSize.Height - 6/*2*/), TablePadArticleButtonPrevSize.Width, TablePadArticleButtonPrevSize.Height);
            TablePadArticleButtonPrev.Relief = ReliefStyle.None;
            TablePadArticleButtonPrev.BorderWidth = 0;
            TablePadArticleButtonPrev.CanFocus = false;
            //Objects:TablePadArticleButtonNext
            TouchButtonIcon TablePadArticleButtonNext = new TouchButtonIcon("TablePadArticleButtonNext", Color.Transparent, TablePadArticleButtonNextImageFileName, new Size(TablePadArticleButtonNextSize.Width - 6/*2*/, TablePadArticleButtonNextSize.Height - 6/*2*/), TablePadArticleButtonNextSize.Width, TablePadArticleButtonNextSize.Height);
            TablePadArticleButtonNext.Relief = ReliefStyle.None;
            TablePadArticleButtonNext.BorderWidth = 0;
            TablePadArticleButtonNext.CanFocus = false;
            //Objects:TablePadArticle
            String sql = @"
                SELECT 
                    Oid as id, Designation as name, ButtonLabel as label, ButtonImage as image, Price1 as price, ButtonLabelHide 
                FROM 
                    fin_article 
                WHERE 
                    (Disabled IS NULL or Disabled <> 1)
            ";
            String filterTablePadArticle = " AND (SubFamily = '" + TablePadSubFamily.SelectedButtonOid + "')";
            _tablePadArticle = new TablePadArticle(
                sql, 
                "ORDER BY Ord", 
                filterTablePadArticle, 
                Guid.Empty, 
                false,
                tablePadArticleTableConfig.Rows, 
                tablePadArticleTableConfig.Columns, 
                "buttonArticleId", 
                Color.Transparent, 
                tablePadArticleButtonSize.Width, 
                tablePadArticleButtonSize.Height, 
                TablePadArticleButtonPrev, 
                TablePadArticleButtonNext
            ) { Sensitive = false };
            _tablePadArticle.SourceWindow = this;
            _tablePadArticle.Clicked += _tablePadArticle_Clicked;
            //Put
            if (tablePadArticleVisible)
            {
                _fixedWindow.Put(TablePadArticleButtonPrev, TablePadArticleButtonPrevPosition.X, TablePadArticleButtonPrevPosition.Y);
                _fixedWindow.Put(TablePadArticleButtonNext, TablePadArticleButtonNextPosition.X, TablePadArticleButtonNextPosition.Y);
                _fixedWindow.Put(_tablePadArticle, tablePadArticlePosition.X, tablePadArticlePosition.Y);
            }
        }

        private void InitUiEventboxToolbar(dynamic pThemeWindow)
        {
            _log.Debug("void InitUiEventboxToolbar(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //Objects:EventboxToolbar
            Position eventboxToolbarPosition = Utils.StringToPosition(themeWindow.Objects.EventboxToolbar.Position);
            Size eventboxToolbarSize = Utils.StringToSize(themeWindow.Objects.EventboxToolbar.Size);
            Size eventboxToolbarButtonSize = Utils.StringToSize(themeWindow.Objects.EventboxToolbar.ButtonSize);
            Size eventboxToolbarIconSize = Utils.StringToSize(themeWindow.Objects.EventboxToolbar.IconSize);
            string eventboxToolbarFont = themeWindow.Objects.EventboxToolbar.Font;
            System.Drawing.Color eventboxToolbarFontColor = FrameworkUtils.StringToColor(themeWindow.Objects.EventboxToolbar.FontColor);
            bool eventboxToolbarVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Visible);
            bool eventboxToolbarVisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.VisibleWindow);

            Gdk.Color eventboxToolbarBackgroundColor = Utils.StringToGdkColor(themeWindow.Objects.EventboxToolbar.BackgroundColor);

            //_log.Debug("after eventboxToolbarBackgroundColor");

            //Objects:EventboxToolbar:ButtonApplicationClose
            string buttonApplicationCloseName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonApplicationClose.Name;
            string buttonApplicationCloseText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonApplicationClose.Text;
            string buttonApplicationCloseImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonApplicationClose.ImageFileName;
            bool buttonApplicationCloseVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonApplicationClose.Visible);

            //Objects:EventboxToolbar:ButtonBackOffice
            string buttonBackOfficeName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonBackOffice.Name;
            string buttonBackOfficeText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonBackOffice.Text;
            string buttonBackOfficeImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonBackOffice.ImageFileName;
            bool buttonBackOfficeVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonBackOffice.Visible);

            //Objects:EventboxToolbar:ButtonReports
            string buttonReportsName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonReports.Name;
            string buttonReportsText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonReports.Text;
            string buttonReportsImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonReports.ImageFileName;
            bool buttonReportsVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonReports.Visible);

            //Objects:EventboxToolbar:ButtonShowSystemDialog
            string buttonShowSystemDialogName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonShowSystemDialog.Name;
            string buttonShowSystemDialogText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonShowSystemDialog.Text;
            string buttonShowSystemDialogImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonShowSystemDialog.ImageFileName;
            bool buttonShowSystemDialogVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonShowSystemDialog.Visible);

            //Objects:EventboxToolbar:ButtonLogoutUser
            string buttonLogoutUserName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonLogoutUser.Name;
            string buttonLogoutUserText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonLogoutUser.Text;
            string buttonLogoutUserImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonLogoutUser.ImageFileName;
            bool buttonLogoutUserVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonLogoutUser.Visible);

            //Objects:EventboxToolbar:ButtonShowChangeUserDialog
            string buttonShowChangeUserDialogName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonShowChangeUserDialog.Name;
            string buttonShowChangeUserDialogText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonShowChangeUserDialog.Text;
            string buttonShowChangeUserDialogImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonShowChangeUserDialog.ImageFileName;
            bool buttonShowChangeUserDialogVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonShowChangeUserDialog.Visible);

            //Objects:EventboxToolbar:ButtonCashDrawer
            string buttonCashDrawerName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonCashDrawer.Name;
            string buttonCashDrawerText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonCashDrawer.Text;
            string buttonCashDrawerImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonCashDrawer.ImageFileName;
            bool buttonCashDrawerVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonCashDrawer.Visible);

            //Objects:EventboxToolbar:ButtonFinanceDocuments
            string buttonFinanceDocumentsName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonFinanceDocuments.Name;
            string buttonFinanceDocumentsText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonFinanceDocuments.Text;
            string buttonFinanceDocumentsImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonFinanceDocuments.ImageFileName;
            bool buttonFinanceDocumentsVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonFinanceDocuments.Visible);

            //Objects:EventboxToolbar:ButtonNewFinanceDocument
            string buttonNewFinanceDocumentName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonNewFinanceDocument.Name;
            string buttonNewFinanceDocumentText = themeWindow.Objects.EventboxToolbar.Buttons.ButtonNewFinanceDocument.Text;
            string buttonNewFinanceDocumentImageFileName = themeWindow.Objects.EventboxToolbar.Buttons.ButtonNewFinanceDocument.ImageFileName;
            bool buttonNewFinanceDocumentVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Buttons.ButtonNewFinanceDocument.Visible);

            //UI
            //EventboxToolbar
            EventBox eventboxToolbar = new EventBox() { VisibleWindow = eventboxToolbarVisibleWindow };
            eventboxToolbar.WidthRequest = eventboxToolbarSize.Width;
            eventboxToolbar.HeightRequest = eventboxToolbarSize.Height;
            if (eventboxToolbarVisibleWindow) eventboxToolbar.ModifyBg(Gtk.StateType.Normal, eventboxToolbarBackgroundColor);
            if (eventboxToolbarVisible) _fixedWindow.Put(eventboxToolbar, eventboxToolbarPosition.X, eventboxToolbarPosition.Y);

            //_log.Debug("Local Func to Get Shared Buttons");
            //Local Func to Get Shared Buttons
            Func<string, string, string, TouchButtonIconWithText> getButton = (pName, pText, pImageFileName)
                => new TouchButtonIconWithText(
                pName,
                Color.Transparent,
                pText,
                eventboxToolbarFont,
                eventboxToolbarFontColor,
                pImageFileName,
                eventboxToolbarIconSize,
                eventboxToolbarButtonSize.Width,
                eventboxToolbarButtonSize.Height
             );
            //Create Button References with Local Func
            _touchButtonPosToolbarApplicationClose = getButton(buttonApplicationCloseName, buttonApplicationCloseText, buttonApplicationCloseImageFileName);
            _touchButtonPosToolbarBackOffice = getButton(buttonBackOfficeName, buttonBackOfficeText, buttonBackOfficeImageFileName);
            _touchButtonPosToolbarReports = getButton(buttonReportsName, buttonReportsText, buttonReportsImageFileName);
            _touchButtonPosToolbarShowSystemDialog = getButton(buttonShowSystemDialogName, buttonShowSystemDialogText, buttonShowSystemDialogImageFileName);
            _touchButtonPosToolbarLogoutUser = getButton(buttonLogoutUserName, buttonLogoutUserText, buttonLogoutUserImageFileName);
            _touchButtonPosToolbarShowChangeUserDialog = getButton(buttonShowChangeUserDialogName, buttonShowChangeUserDialogText, buttonShowChangeUserDialogImageFileName);
            _touchButtonPosToolbarCashDrawer = getButton(buttonCashDrawerName, buttonCashDrawerText, buttonCashDrawerImageFileName);
            _touchButtonPosToolbarFinanceDocuments = getButton(buttonFinanceDocumentsName, buttonFinanceDocumentsText, buttonFinanceDocumentsImageFileName);
            _touchButtonPosToolbarNewFinanceDocument = getButton(buttonNewFinanceDocumentName, buttonNewFinanceDocumentText, buttonNewFinanceDocumentImageFileName);

            //Toggle Sensitive Buttons
            _touchButtonPosToolbarNewFinanceDocument.Sensitive = (GlobalFramework.WorkSessionPeriodTerminal != null && GlobalFramework.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open);
            //Pack Buttons
            HBox hboxToolbar = new HBox(false, 0);
            hboxToolbar.BorderWidth = 10;
            
            if (buttonApplicationCloseVisible) hboxToolbar.PackStart(_touchButtonPosToolbarApplicationClose, false, false, 0);
            if (buttonBackOfficeVisible) hboxToolbar.PackStart(_touchButtonPosToolbarBackOffice, false, false, 0);
            if (buttonShowSystemDialogVisible) hboxToolbar.PackStart(_touchButtonPosToolbarShowSystemDialog, false, false, 0);
            if (buttonLogoutUserVisible) hboxToolbar.PackStart(_touchButtonPosToolbarLogoutUser, false, false, 0);
            if (buttonShowChangeUserDialogVisible) hboxToolbar.PackStart(_touchButtonPosToolbarShowChangeUserDialog, false, false, 0);
            if (buttonCashDrawerVisible) hboxToolbar.PackStart(_touchButtonPosToolbarCashDrawer, false, false, 0);
            if (buttonReportsVisible) hboxToolbar.PackStart(_touchButtonPosToolbarReports, false, false, 0);
            if (buttonFinanceDocumentsVisible) hboxToolbar.PackStart(_touchButtonPosToolbarFinanceDocuments, false, false, 0);
            if (buttonNewFinanceDocumentVisible) hboxToolbar.PackStart(_touchButtonPosToolbarNewFinanceDocument, false, false, 0);

            //PackIt
            eventboxToolbar.Add(hboxToolbar);

            //Assign Toolbar Button references to TicketList
            _ticketList.ToolbarApplicationClose = _touchButtonPosToolbarApplicationClose;
            _ticketList.ToolbarBackOffice = _touchButtonPosToolbarBackOffice;
            // Deprecated
            _ticketList.ToolbarReports = _touchButtonPosToolbarReports;
            _ticketList.ToolbarShowSystemDialog = _touchButtonPosToolbarShowSystemDialog;
            _ticketList.ToolbarLogoutUser = _touchButtonPosToolbarLogoutUser;
            _ticketList.ToolbarShowChangeUserDialog = _touchButtonPosToolbarShowChangeUserDialog;
            _ticketList.ToolbarCashDrawer = _touchButtonPosToolbarCashDrawer;
            _ticketList.ToolbarFinanceDocuments = _touchButtonPosToolbarFinanceDocuments;
            _ticketList.ToolbarNewFinanceDocument = _touchButtonPosToolbarNewFinanceDocument;

            //Events
            _touchButtonPosToolbarApplicationClose.Clicked += touchButtonPosToolbarApplicationClose_Clicked;
            _touchButtonPosToolbarBackOffice.Clicked += touchButtonPosToolbarBackOffice_Clicked;
            // Deprecated
            _touchButtonPosToolbarReports.Clicked += touchButtonPosToolbarReports_Clicked;
            _touchButtonPosToolbarShowSystemDialog.Clicked += touchButtonPosToolbarShowSystemDialog_Clicked;
            _touchButtonPosToolbarLogoutUser.Clicked += touchButtonPosToolbarLogoutUser_Clicked;
            _touchButtonPosToolbarShowChangeUserDialog.Clicked += touchButtonPosToolbarShowChangeUserDialog_Clicked;
            _touchButtonPosToolbarCashDrawer.Clicked += touchButtonPosToolbarCashDrawer_Clicked;
            _touchButtonPosToolbarNewFinanceDocument.Clicked += touchButtonPosToolbarNewFinanceDocument_Clicked;
            _touchButtonPosToolbarFinanceDocuments.Clicked += touchButtonPosToolbarFinanceDocuments_Clicked;
        }

        private void InitUIEventBoxPosTicketPad(dynamic pThemeWindow)
        {
            _log.Debug("void InitUIEventBoxPosTicketPad(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //Objects:EventBoxPosTicketPad
            Position eventBoxPosTicketPadPosition = Utils.StringToPosition(themeWindow.Objects.EventBoxPosTicketPad.Position);
            Size eventBoxPosTicketPadSize = Utils.StringToSize(themeWindow.Objects.EventBoxPosTicketPad.Size);
            Gdk.Color eventBoxPosTicketPadBackgroundColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxPosTicketPad.BackgroundColor);
            bool eventBoxPosTicketPadVisible = Convert.ToBoolean(themeWindow.Objects.EventBoxPosTicketPad.Visible);
            bool eventBoxPosTicketPadVisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxPosTicketPad.VisibleWindow);

            //UI
            _ticketPad = new TicketPad(
                "posTicketPad",
                _ticketList,
                themeWindow.Objects.EventBoxPosTicketPad.Buttons,
                eventBoxPosTicketPadPosition
             ) { Sensitive = false };

            _ticketPad.SourceWindow = this;
            EventBox eventBoxPosTicketPad = new EventBox() { VisibleWindow = eventBoxPosTicketPadVisibleWindow, WidthRequest = eventBoxPosTicketPadSize.Width, HeightRequest = eventBoxPosTicketPadSize.Height };
            if (eventBoxPosTicketPadVisibleWindow) eventBoxPosTicketPad.ModifyBg(StateType.Normal, eventBoxPosTicketPadBackgroundColor);
            eventBoxPosTicketPad.Add(_ticketPad);
            if (eventBoxPosTicketPadVisible) _fixedWindow.Put(eventBoxPosTicketPad, eventBoxPosTicketPadPosition.X, eventBoxPosTicketPadPosition.Y);
        }

        private void InitUIEventBoxPosTicketList(dynamic pThemeWindow)
        {
            _log.Debug("void InitUIEventBoxPosTicketList(dynamic pThemeWindow) :: Starting...");
            dynamic themeWindow = pThemeWindow;

            //Objects:EventBoxPosTicketList
            Position eventBoxPosTicketListPosition = Utils.StringToPosition(themeWindow.Objects.EventBoxPosTicketList.Position);
            Size eventBoxPosTicketListSize = Utils.StringToSize(themeWindow.Objects.EventBoxPosTicketList.Size);
            bool eventBoxPosTicketListVisible = Convert.ToBoolean(themeWindow.Objects.EventBoxPosTicketList.Visible);
            bool eventBoxPosTicketListVisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxPosTicketList.VisibleWindow);
            Gdk.Color eventBoxPosTicketListBackgroundColor = Utils.StringToGdkColor(themeWindow.Objects.EventBoxPosTicketList.BackgroundColor);

            //UI

            EventBox eventBoxPosTicketList = new EventBox() { VisibleWindow = eventBoxPosTicketListVisibleWindow, BorderWidth = 0 };
            eventBoxPosTicketList.WidthRequest = eventBoxPosTicketListSize.Width;
            eventBoxPosTicketList.HeightRequest = eventBoxPosTicketListSize.Height;
            if (eventBoxPosTicketListVisibleWindow) eventBoxPosTicketList.ModifyBg(StateType.Normal, eventBoxPosTicketListBackgroundColor);

            //Get ThemeObject to send to TicketList Constructor
            dynamic themeEventBoxPosTicketList = themeWindow.Objects.EventBoxPosTicketList;
            _ticketList = new TicketList(themeEventBoxPosTicketList) { SourceWindow = this};
            eventBoxPosTicketList.Add(_ticketList);
            if (eventBoxPosTicketListVisible) _fixedWindow.Put(eventBoxPosTicketList, eventBoxPosTicketListPosition.X, eventBoxPosTicketListPosition.Y);
        }

        //CURRENTLY Disabled, but very Usefull to Show Log in Screen
        private void BuildTextViewLog()
        {
            //TextviewLog
            EventBox eventBoxPosTextviewLog = new EventBox() { VisibleWindow = false, BorderWidth = 0 };
            eventBoxPosTextviewLog.WidthRequest = 326;
            eventBoxPosTextviewLog.HeightRequest = 84;
            //Add Text with > _bufferTextView.InsertAtCursor("Text" + Environment.NewLine);
            _textviewLog = new TextView();
            _textviewLog.SizeAllocated += new SizeAllocatedHandler(ScrollTextViewLog);
            _textviewLog.BorderWidth = 0;
            _bufferTextView = _textviewLog.Buffer;
            ScrolledWindow scrolledWindowTextviewLog = new ScrolledWindow();
            scrolledWindowTextviewLog.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrolledWindowTextviewLog.Add(_textviewLog);
            eventBoxPosTextviewLog.Add(scrolledWindowTextviewLog);
        }
    }
}