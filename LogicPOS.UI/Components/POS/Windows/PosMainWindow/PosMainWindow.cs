using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Logic.Others;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Drawing;
using Image = Gtk.Image;

namespace logicpos
{
    public partial class PosMainWindow : PosBaseWindow
    {

        //Files
        private readonly string _fileBaseButtonOverlay = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";

        /* IN006045 */
        //private string _clockFormat = LogicPOS.Settings.AppSettings.Instance.dateTimeFormatStatusBar"];
        private readonly string _clockFormat = GeneralUtils.GetResourceByName("frontoffice_datetime_format_status_bar");

        private readonly Color _colorPosNumberPadLeftButtonBackground = AppSettings.Instance.colorPosNumberPadLeftButtonBackground;
        private readonly Color _colorPosNumberRightButtonBackground = AppSettings.Instance.colorPosNumberRightButtonBackground;
        private readonly Color _colorPosHelperBoxsBackground = AppSettings.Instance.colorPosHelperBoxsBackground;
        //UI
        private readonly Fixed _fixedWindow;
        private Label _labelClock;
        private TextView _textviewLog;

        public TicketList TicketList { get; private set; }

        private IconButtonWithText _touchButtonPosToolbarApplicationClose;
        private IconButtonWithText _touchButtonPosToolbarBackOffice;
        private IconButtonWithText _touchButtonPosToolbarReports;
        private IconButtonWithText _touchButtonPosToolbarShowSystemDialog;
        private IconButtonWithText _touchButtonPosToolbarLogoutUser;
        private IconButtonWithText _touchButtonPosToolbarShowChangeUserDialog;
        private IconButtonWithText _touchButtonPosToolbarCashDrawer;
        private IconButtonWithText _touchButtonPosToolbarFinanceDocuments;

        public IconButtonWithText TouchButtonPosToolbarNewFinanceDocument { get; set; }

        private TicketPad _ticketPad;
        //Others
        private readonly uint _borderWidth = 5;

        internal TablePad TablePadFamily { get; set; }
        internal TablePad TablePadSubFamily { get; set; }

        internal TablePad TablePadArticle { get; set; }
        public TextBuffer BufferTextView { get; set; }

        public Label LabelTerminalInfo { get; set; }
        public Label LabelCurrentTable { get; set; }

        public Label LabelTotalTable { get; set; }

        //Constructor
        public PosMainWindow(string pBackgroundImage)
            : base(pBackgroundImage)
        {
            _fixedWindow = new Fixed();

            InitUI();

            TicketList.UpdateOrderStatusBar();

            UpdateWorkSessionUI();

            StartClock();

            TablePadArticle.Filter = " AND (Favorite = 1)";

            TicketList.UpdateTicketListButtons();

            this.ScreenArea.Add(_fixedWindow);

            bool _showMinimize = AppSettings.Instance.appShowMinimize;
            if (_showMinimize)
            {
                EventBox eventBoxMinimize = GtkUtils.CreateMinimizeButton();
                eventBoxMinimize.ButtonReleaseEvent += delegate { Iconify(); };
                _fixedWindow.Put(eventBoxMinimize, GlobalApp.ScreenSize.Width - 27 - 10, 10);
            }

            this.ShowAll();

            this.WindowStateEvent += PosMainWindow_WindowStateEvent;
            this.ExposeEvent += delegate { UpdateUIIfHasWorkingOrder(); };
            this.KeyReleaseEvent += PosMainWindow_KeyReleaseEvent;

            //Hardware Events
            if (TerminalSettings.LoggedTerminal.BarcodeReader != null || TerminalSettings.LoggedTerminal.CardReader != null)
            {
                GlobalApp.BarCodeReader.Captured += HWBarCodeReader_Captured;
            }

            _logger.Debug("PosMainWindow(String pBackgroundImage) :: Completed!");
        }

        private void InitUI()
        {

            _logger.Debug("void InitUI() :: Initializing UI for POS Main Window..."); /* IN009008 */

            //Init Theme Object
            Predicate<dynamic> predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosMainWindow");
            dynamic themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);

            //Shared error Message
            string errorMessage = "Node: <Window ID=\"PosMainWindow\">";

            //Assign Theme Vars + UI
            if (themeWindow != null)
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
               
                InitUiEventboxToolbar(themeWindow);

                _logger.Debug("void InitUI() :: POS Main Window theme rendering completed!"); /* IN009008 */
                //Notify Thread End
                GlobalApp.DialogThreadNotify.WakeupMain();

                SortingCollection sortCollection = new SortingCollection
                        {
                            new SortProperty("FiscalYear", DevExpress.Xpo.DB.SortingDirection.Ascending)
                        };
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                ICollection collectionDocumentFinanceSeries = XPOSettings.Session.GetObjects(XPOSettings.Session.GetClassInfo(typeof(fin_documentfinanceyearserieterminal)), criteria, sortCollection, int.MaxValue, false, true);
                if (collectionDocumentFinanceSeries.Count == 0)
                {
                    Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_warning"), GeneralUtils.GetResourceByName("global_warning_open_fiscal_year"));
                }
            }
            else
            {
                Utils.ShowMessageTouchErrorRenderTheme(this, errorMessage);
            }
        }

        private void InitUIEventBoxImageLogo(dynamic pThemeWindow)
        {
            _logger.Debug("void InitUIEventBoxImageLogo(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;
            //Objects:EventBoxImageLogo
            Point eventBoxImageLogoPosition = Utils.StringToPosition(themeWindow.Objects.EventBoxImageLogo.Position);
            Size eventBoxImageLogoSize = (themeWindow.Objects.EventBoxImageLogo.Size as string).ToSize();
            bool eventBoxImageLogoVisible = Convert.ToBoolean(themeWindow.Objects.EventBoxImageLogo.Visible);
            bool eventBoxImageLogoVisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxImageLogo.VisibleWindow);
            Gdk.Color eventBoxImageLogoBackgroundColor = (themeWindow.Objects.EventBoxImageLogo.BackgroundColor as string).StringToGdkColor();

            //LOGO
            Image imageLogo = new Image(Utils.GetThemeFileLocation(AppSettings.Instance.fileImageBackOfficeLogo));
            if (PluginSettings.LicenceManager != null)
            {
                string fileImageBackOfficeLogo = string.Format(PathsSettings.Paths["themes"] + @"Default\Images\logicPOS_logicpulse_login.png");

                if (!string.IsNullOrEmpty(LicenseSettings.LicenseReseller) && LicenseSettings.LicenseReseller == "NewTech")
                {
                    fileImageBackOfficeLogo = string.Format(PathsSettings.Paths["themes"] + @"Default\Images\Branding\{0}\logicPOS_logicpulse_login.png", "NT");
                }

                var bitmapImage = PluginSettings.LicenceManager.DecodeImage(fileImageBackOfficeLogo, eventBoxImageLogoSize.Width, eventBoxImageLogoSize.Height);
                Gdk.Pixbuf pixbufImageLogo = Utils.ImageToPixbuf(bitmapImage);
                imageLogo = new Image(pixbufImageLogo);
            }

            //fix.Put(imageLogo, GlobalApp.ScreenSize.Width - 300, 50);
            //UI
            EventBox eventBoxImageLogo = new EventBox();
            eventBoxImageLogo.WidthRequest = eventBoxImageLogoSize.Width;
            eventBoxImageLogo.HeightRequest = eventBoxImageLogoSize.Height;
            eventBoxImageLogo.VisibleWindow = eventBoxImageLogoVisibleWindow;
            if (eventBoxImageLogoVisibleWindow) eventBoxImageLogo.ModifyBg(StateType.Normal, eventBoxImageLogoBackgroundColor);
            if (eventBoxImageLogoVisible) _fixedWindow.Put(eventBoxImageLogo, eventBoxImageLogoPosition.X, eventBoxImageLogoPosition.Y);

            eventBoxImageLogo.Add(imageLogo);
            eventBoxImageLogo.ButtonPressEvent += eventBoxImageLogo_ButtonPressEvent;

        }

        private void InitUIEventBoxStatusBar1(dynamic pThemeWindow)
        {
            _logger.Debug("void InitUIEventBoxStatusBar1(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:EventBoxStatusBar1
            Point eventBoxStatusBar1Position = Utils.StringToPosition(themeWindow.Objects.EventBoxStatusBar1.Position); ;
            Size eventBoxStatusBar1Size = (themeWindow.Objects.EventBoxStatusBar1.Size as string).ToSize();
            bool eventBoxStatusBar1Visible = Convert.ToBoolean(themeWindow.Objects.EventBoxStatusBar1.Visible);
            bool eventBoxStatusBar1VisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxStatusBar1.VisibleWindow);
            Gdk.Color eventBoxStatusBar1BackgroundColor = (themeWindow.Objects.EventBoxStatusBar1.BackgroundColor as string).StringToGdkColor();

            //Objects:EventBoxStatusBar1:LabelTerminalInfo
            Pango.FontDescription labelTerminalInfoFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar1.LabelTerminalInfo.Font);
            Gdk.Color labelTerminalInfoFontColor = (themeWindow.Objects.EventBoxStatusBar1.LabelTerminalInfo.FontColor as string).StringToGdkColor();
            float labelTerminalInfoAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar1.LabelTerminalInfo.AlignmentX);

            //Objects:EventBoxStatusBar1:LabelClock
            Pango.FontDescription labelClockFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar1.LabelClock.Font);
            Gdk.Color labelClockFontColor = (themeWindow.Objects.EventBoxStatusBar1.LabelClock.FontColor as string).StringToGdkColor();
            float labelClockAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar1.LabelClock.AlignmentX);

            //UI
            //eventBoxStatusBar1
            EventBox eventBoxStatusBar1 = new EventBox() { VisibleWindow = eventBoxStatusBar1VisibleWindow };
            eventBoxStatusBar1.WidthRequest = eventBoxStatusBar1Size.Width;
            eventBoxStatusBar1.HeightRequest = eventBoxStatusBar1Size.Height;
            eventBoxStatusBar1.ModifyBg(StateType.Normal, eventBoxStatusBar1BackgroundColor);

            //EventBoxStatusBar1:LabelTerminalInfo
            LabelTerminalInfo = new Label(string.Format("{0} : {1}", TerminalSettings.LoggedTerminal.Designation, XPOSettings.LoggedUser.Name));
            LabelTerminalInfo.ModifyFont(labelTerminalInfoFont);
            LabelTerminalInfo.ModifyFg(StateType.Normal, labelTerminalInfoFontColor);
            LabelTerminalInfo.SetAlignment(labelTerminalInfoAlignmentX, 0.5F);

            //EventBoxStatusBar1:LabelClock
            _labelClock = new Label(XPOUtility.CurrentDateTime(_clockFormat));
            _labelClock.ModifyFont(labelClockFont);
            _labelClock.ModifyFg(StateType.Normal, labelClockFontColor);
            _labelClock.SetAlignment(labelClockAlignmentX, 0.5F);

            //Pack HBox EventBoxStatusBar1
            HBox hboxStatusBar1 = new HBox(false, 0) { BorderWidth = _borderWidth };
            hboxStatusBar1.PackStart(LabelTerminalInfo, false, false, 0);
            hboxStatusBar1.PackStart(_labelClock, true, true, 0);
            eventBoxStatusBar1.Add(hboxStatusBar1);

            if (eventBoxStatusBar1Visible) _fixedWindow.Put(eventBoxStatusBar1, eventBoxStatusBar1Position.X, eventBoxStatusBar1Position.Y);
        }

        private void InitUIEventBoxStatusBar2(dynamic pThemeWindow)
        {
            _logger.Debug("void InitUIEventBoxStatusBar2(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:EventBoxStatusBar2
            Point eventBoxStatusBar2Position = Utils.StringToPosition(themeWindow.Objects.EventBoxStatusBar2.Position); ;
            Size eventBoxStatusBar2Size = (themeWindow.Objects.EventBoxStatusBar2.Size as string).ToSize();
            bool eventBoxStatusBar2Visible = Convert.ToBoolean(themeWindow.Objects.EventBoxStatusBar2.Visible);
            bool eventBoxStatusBar2VisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxStatusBar2.VisibleWindow);
            Gdk.Color eventBoxStatusBar2BackgroundColor = (themeWindow.Objects.EventBoxStatusBar2.BackgroundColor as string).StringToGdkColor();

            //Objects:EventBoxStatusBar2:LabelCurrentTableLabel
            Pango.FontDescription labelCurrentTableLabelFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTableLabel.Font);
            Gdk.Color labelCurrentTableLabelFontColor = (themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTableLabel.FontColor as string).StringToGdkColor();
            float labelCurrentTableLabelAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTableLabel.AlignmentX);

            //Objects:EventBoxStatusBar2:LabelCurrentTable
            Pango.FontDescription labelCurrentTableFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTable.Font);
            Gdk.Color labelCurrentTableFontColor = (themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTable.FontColor as string).StringToGdkColor();
            float labelCurrentTableAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar2.LabelCurrentTable.AlignmentX);

            //Objects:EventBoxStatusBar2:LabelTotalTableLabel
            Pango.FontDescription labelTotalTableLabelFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTableLabel.Font);
            Gdk.Color labelTotalTableLabelFontColor = (themeWindow.Objects.EventBoxStatusBar2.LabelTotalTableLabel.FontColor as string).StringToGdkColor();
            float labelTotalTableLabelAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTableLabel.AlignmentX);

            //Objects:EventBoxStatusBar2:LabelTotalTable
            Pango.FontDescription labelTotalTableFont = Pango.FontDescription.FromString(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTable.Font);
            Gdk.Color labelTotalTableFontColor = (themeWindow.Objects.EventBoxStatusBar2.LabelTotalTable.FontColor as string).StringToGdkColor();
            float labelTotalTableAlignmentX = Convert.ToSingle(themeWindow.Objects.EventBoxStatusBar2.LabelTotalTable.AlignmentX);

            //UI

            //EventBoxStatusBar2
            EventBox eventBoxStatusBar2 = new EventBox() { VisibleWindow = eventBoxStatusBar2VisibleWindow };
            eventBoxStatusBar2.WidthRequest = eventBoxStatusBar2Size.Width;
            eventBoxStatusBar2.HeightRequest = eventBoxStatusBar2Size.Height;
            eventBoxStatusBar2.ModifyBg(StateType.Normal, eventBoxStatusBar2BackgroundColor);

            //EventBoxStatusBar2:vboxCurrentTable:LabelCurrentTableLabel
            string global_table = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("global_table_appmode_{0}", AppOperationModeSettings.CustomAppOperationMode.AppOperationTheme).ToLower()); /* IN008024 */
            Label labelCurrentTableLabel = new Label(global_table);
            labelCurrentTableLabel.ModifyFont(labelCurrentTableLabelFont);
            labelCurrentTableLabel.ModifyFg(StateType.Normal, labelCurrentTableLabelFontColor);
            labelCurrentTableLabel.SetAlignment(labelCurrentTableLabelAlignmentX, 0.5F);

            //EventBoxStatusBar2:vboxCurrentTable:LabelCurrentTable
            LabelCurrentTable = new Label();//Text assigned on TicketList.UpdateOrderStatusBar()
            LabelCurrentTable.ModifyFont(labelCurrentTableFont);
            LabelCurrentTable.ModifyFg(StateType.Normal, labelCurrentTableFontColor);
            LabelCurrentTable.SetAlignment(labelCurrentTableAlignmentX, 0.5F);

            //Pack
            VBox vboxCurrentTable = new VBox(false, 1);
            vboxCurrentTable.PackStart(labelCurrentTableLabel);
            vboxCurrentTable.PackStart(LabelCurrentTable);

            //EventBoxStatusBar2:vboxTotalTable:LabelTotalTableLabel
            Label labelTotalTableLabel = new Label(GeneralUtils.GetResourceByName("global_total_price_to_pay"));
            labelTotalTableLabel.ModifyFont(labelTotalTableLabelFont);
            labelTotalTableLabel.ModifyFg(StateType.Normal, labelTotalTableLabelFontColor);
            labelTotalTableLabel.SetAlignment(labelTotalTableLabelAlignmentX, 0.5F);

            //EventBoxStatusBar2:vboxTotalTable:LabelTotalTable
            LabelTotalTable = new Label(DataConversionUtils.DecimalToStringCurrency(0, XPOSettings.ConfigurationSystemCurrency.Acronym));
            LabelTotalTable.ModifyFont(labelTotalTableFont);
            LabelTotalTable.ModifyFg(StateType.Normal, labelTotalTableFontColor);
            LabelTotalTable.SetAlignment(labelTotalTableAlignmentX, 0.5F);

            //Pack
            VBox vboxTotalTable = new VBox(false, 1);
            vboxTotalTable.PackStart(labelTotalTableLabel);
            vboxTotalTable.PackStart(LabelTotalTable);

            //Pack HBox StatusBar
            HBox hboxStatusBar2 = new HBox(false, 0) { BorderWidth = _borderWidth };
            hboxStatusBar2.PackStart(vboxCurrentTable, true, true, 0);
            hboxStatusBar2.PackStart(vboxTotalTable, false, false, 0);
            eventBoxStatusBar2.Add(hboxStatusBar2);

            if (eventBoxStatusBar2Visible) _fixedWindow.Put(eventBoxStatusBar2, eventBoxStatusBar2Position.X, eventBoxStatusBar2Position.Y);
        }

        private void InitUIButtonFavorites(dynamic pThemeWindow)
        {
            _logger.Debug("void InitUIButtonFavorites(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:ButtonFavorites
            Point buttonFavoritesPosition = Utils.StringToPosition(themeWindow.Objects.ButtonFavorites.Position);
            Size buttonFavoritesButtonSize = (themeWindow.Objects.ButtonFavorites.ButtonSize as string).ToSize();
            string buttonFavoritesImageFileName = themeWindow.Objects.ButtonFavorites.ImageFileName;
            string buttonFavoritesText = themeWindow.Objects.ButtonFavorites.Text;
            int buttonFavoritesFontSize = Convert.ToInt16(themeWindow.Objects.ButtonFavorites.FontSize);
            bool buttonFavoritesUseImageOverlay = Convert.ToBoolean(themeWindow.Objects.ButtonFavorites.UseImageOverlay);
            bool buttonFavoritesVisible = Convert.ToBoolean(themeWindow.Objects.ButtonFavorites.Visible);

            //UI

            string buttonFavoritesImageOverlay = (buttonFavoritesUseImageOverlay) ? _fileBaseButtonOverlay : string.Empty;

            ImageButton buttonFavorites = new ImageButton(
                new ButtonSettings
                {
                    Name = "buttonFavorites",
                    Text = buttonFavoritesText,
                    FontSize = buttonFavoritesFontSize,
                    Image = buttonFavoritesImageFileName,
                    Overlay = buttonFavoritesImageOverlay,
                    ButtonSize = new Size(buttonFavoritesButtonSize.Width, buttonFavoritesButtonSize.Height)
                });


            buttonFavorites.Clicked += buttonFavorites_Clicked;

            if (buttonFavoritesVisible) _fixedWindow.Put(buttonFavorites, buttonFavoritesPosition.X, buttonFavoritesPosition.Y);
        }

        private void InitUITablePads(dynamic pThemeWindow)
        {
            _logger.Debug("void InitUITablePads(dynamic pThemeWindow) :: Starting..."); /* IN009008 */
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:TablePadFamilyButtonPrev
            Point TablePadFamilyButtonPrevPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.Position);
            Size TablePadFamilyButtonPrevSize = (themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.Size as string).ToSize();
            string TablePadFamilyButtonPrevImageFileName = themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.ImageFileName;
            //Objects:TablePadFamilyButtonNext
            Point TablePadFamilyButtonNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.Position);
            Size TablePadFamilyButtonNextSize = (themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.Size as string).ToSize();
            string TablePadFamilyButtonNextImageFileName = themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.ImageFileName;
            //Objects:TablePadFamily
            Point tablePadFamilyPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.Position);
            Size tablePadFamilyButtonSize = (themeWindow.Objects.TablePadFamily.ButtonSize as string).ToSize();
            TableConfig tablePadFamilyTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadFamily.TableConfig);
            bool tablePadFamilyVisible = Convert.ToBoolean(themeWindow.Objects.TablePadFamily.Visible);

            //Objects:TablePadSubFamilyButtonPrev
            Point TablePadSubFamilyButtonPrevPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.Position);
            Size TablePadSubFamilyButtonPrevSize = (themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.Size as string).ToSize();
            string TablePadSubFamilyButtonPrevImageFileName = themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.ImageFileName;
            //Objects:TablePadSubFamilyButtonNext
            Point TablePadSubFamilyButtonNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.Position);
            Size TablePadSubFamilyButtonNextSize = (themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.Size as string).ToSize();
            string TablePadSubFamilyButtonNextImageFileName = themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.ImageFileName;
            //Objects:TablePadSubFamily
            Point tablePadSubFamilyPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.Position);
            Size tablePadSubFamilyButtonSize = (themeWindow.Objects.TablePadSubFamily.ButtonSize as string).ToSize();
            TableConfig tablePadSubFamilyTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadSubFamily.TableConfig);
            bool tablePadSubFamilyVisible = Convert.ToBoolean(themeWindow.Objects.TablePadSubFamily.Visible);

            //Objects:TablePadArticleButtonPrev
            Point TablePadArticleButtonPrevPosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.Position);
            Size TablePadArticleButtonPrevSize = (themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.Size as string).ToSize();
            string TablePadArticleButtonPrevImageFileName = themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.ImageFileName;
            //Objects:TablePadArticleButtonNext
            Point TablePadArticleButtonNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.Position);
            Size TablePadArticleButtonNextSize = (themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.Size as string).ToSize();
            string TablePadArticleButtonNextImageFileName = themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.ImageFileName;
            //Objects:TablePadArticle
            Point tablePadArticlePosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.Position);
            Size tablePadArticleButtonSize = (themeWindow.Objects.TablePadArticle.ButtonSize as string).ToSize();
            TableConfig tablePadArticleTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadArticle.TableConfig);
            bool tablePadArticleVisible = Convert.ToBoolean(themeWindow.Objects.TablePadArticle.Visible);

            //UI

            //Objects:TablePadFamilyButtonPrev
            IconButton TablePadFamilyButtonPrev = new IconButton(
                new ButtonSettings
                {
                    Name = "TablePadFamilyButtonPrev",
                    Icon = TablePadFamilyButtonPrevImageFileName,
                    IconSize = new Size(TablePadFamilyButtonPrevSize.Width - 2, TablePadFamilyButtonPrevSize.Height - 2),
                    ButtonSize = new Size(TablePadFamilyButtonPrevSize.Width, TablePadFamilyButtonPrevSize.Height)
                });

            TablePadFamilyButtonPrev.Relief = ReliefStyle.None;
            TablePadFamilyButtonPrev.BorderWidth = 0;
            TablePadFamilyButtonPrev.CanFocus = false;

            //Objects:TablePadFamilyButtonNext
            IconButton TablePadFamilyButtonNext = new IconButton(
                new ButtonSettings
                {
                    Name = "TablePadFamilyButtonNext",
                    Icon = TablePadFamilyButtonNextImageFileName,
                    IconSize = new Size(TablePadFamilyButtonNextSize.Width - 2, TablePadFamilyButtonNextSize.Height - 2),
                    ButtonSize = new Size(TablePadFamilyButtonNextSize.Width, TablePadFamilyButtonNextSize.Height)
                });

            TablePadFamilyButtonNext.Relief = ReliefStyle.None;
            TablePadFamilyButtonNext.BorderWidth = 0;
            TablePadFamilyButtonNext.CanFocus = false;
            //Objects:TablePadFamily
            string sqlTablePadFamily = @"
                SELECT 
                    Oid as id, Designation as name, ButtonLabel as label, ButtonImage as image,
                    (SELECT COUNT(*) as childs FROM fin_articlesubfamily WHERE (Disabled IS NULL or Disabled  <> 1) AND Family = p.Oid) as childs
                FROM 
                    fin_articlefamily as p 
                WHERE 
                    (Disabled IS NULL or Disabled <> 1)
            ";
            TablePadFamily = new TablePad(
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
            TablePadFamily.SourceWindow = this;
            TablePadFamily.Clicked += _tablePadFamily_Clicked;
            //Put
            if (tablePadFamilyVisible)
            {
                _fixedWindow.Put(TablePadFamilyButtonPrev, TablePadFamilyButtonPrevPosition.X, TablePadFamilyButtonPrevPosition.Y);
                _fixedWindow.Put(TablePadFamilyButtonNext, TablePadFamilyButtonNextPosition.X, TablePadFamilyButtonNextPosition.Y);
                _fixedWindow.Put(TablePadFamily, tablePadFamilyPosition.X, tablePadFamilyPosition.Y);
            }

            //Objects:TablePadSubFamilyButtonPrev
            IconButton TablePadSubFamilyButtonPrev = new IconButton(
                new ButtonSettings
                {
                    Name = "TablePadSubFamilyButtonPrev",
                    Icon = TablePadSubFamilyButtonPrevImageFileName,
                    IconSize = new Size(TablePadSubFamilyButtonPrevSize.Width - 6, TablePadSubFamilyButtonPrevSize.Height - 6),
                    ButtonSize = new Size(TablePadSubFamilyButtonPrevSize.Width, TablePadSubFamilyButtonPrevSize.Height)
                });

            TablePadSubFamilyButtonPrev.Relief = ReliefStyle.None;
            TablePadSubFamilyButtonPrev.BorderWidth = 0;
            TablePadSubFamilyButtonPrev.CanFocus = false;
            //Objects:TablePadSubFamilyButtonNext
            IconButton TablePadSubFamilyButtonNext = new IconButton(new ButtonSettings { Name = "TablePadSubFamilyButtonNext", Icon = TablePadSubFamilyButtonNextImageFileName, IconSize = new Size(TablePadSubFamilyButtonNextSize.Width - 6, TablePadSubFamilyButtonNextSize.Height - 6), ButtonSize = new Size(TablePadSubFamilyButtonNextSize.Width, TablePadSubFamilyButtonNextSize.Height) });
            TablePadSubFamilyButtonNext.Relief = ReliefStyle.None;
            TablePadSubFamilyButtonNext.BorderWidth = 0;
            TablePadSubFamilyButtonNext.CanFocus = false;
            //Objects:TablePadSubFamily
            string sqlTablePadSubFamily = @"
                SELECT 
                    Oid as id, Designation as name, ButtonLabel as label, ButtonImage as image,
                    (SELECT COUNT(*) as childs FROM fin_article WHERE (Disabled IS NULL or Disabled  <> 1) AND SubFamily = p.Oid) as childs
                FROM 
                    fin_articlesubfamily as p 
                WHERE 
                    (Disabled IS NULL or Disabled <> 1)
            ";
            string filterTablePadSubFamily = " AND (Family = '" + TablePadFamily.SelectedButtonOid + "')";
            TablePadSubFamily = new TablePad(
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
            TablePadSubFamily.SourceWindow = this;
            TablePadSubFamily.Clicked += _tablePadSubFamily_Clicked;
            //Put
            if (tablePadSubFamilyVisible)
            {
                _fixedWindow.Put(TablePadSubFamilyButtonPrev, TablePadSubFamilyButtonPrevPosition.X, TablePadSubFamilyButtonPrevPosition.Y);
                _fixedWindow.Put(TablePadSubFamilyButtonNext, TablePadSubFamilyButtonNextPosition.X, TablePadSubFamilyButtonNextPosition.Y);
                _fixedWindow.Put(TablePadSubFamily, tablePadSubFamilyPosition.X, tablePadSubFamilyPosition.Y);
            }

            //Objects:TablePadArticleButtonPrev
            IconButton TablePadArticleButtonPrev = new IconButton(new ButtonSettings { Name = "TablePadArticleButtonPrev", Icon = TablePadArticleButtonPrevImageFileName, IconSize = new Size(TablePadArticleButtonPrevSize.Width - 6, TablePadArticleButtonPrevSize.Height - 6), ButtonSize = new Size(TablePadArticleButtonPrevSize.Width, TablePadArticleButtonPrevSize.Height) });
            TablePadArticleButtonPrev.Relief = ReliefStyle.None;
            TablePadArticleButtonPrev.BorderWidth = 0;
            TablePadArticleButtonPrev.CanFocus = false;
            //Objects:TablePadArticleButtonNext
            IconButton TablePadArticleButtonNext = new IconButton(new ButtonSettings { Name = "TablePadArticleButtonNext", Icon = TablePadArticleButtonNextImageFileName, IconSize = new Size(TablePadArticleButtonNextSize.Width - 6, TablePadArticleButtonNextSize.Height - 6), ButtonSize = new Size(TablePadArticleButtonNextSize.Width, TablePadArticleButtonNextSize.Height) });
            TablePadArticleButtonNext.Relief = ReliefStyle.None;
            TablePadArticleButtonNext.BorderWidth = 0;
            TablePadArticleButtonNext.CanFocus = false;
            //Objects:TablePadArticle
            string sql = @"
                SELECT 
                    Oid as id, Designation as name, ButtonLabel as label, ButtonImage as image, Price1 as price, ButtonLabelHide 
                FROM 
                    fin_article 
                WHERE 
                    (Disabled IS NULL or Disabled <> 1)
            ";
            string filterTablePadArticle = " AND (SubFamily = '" + TablePadSubFamily.SelectedButtonOid + "')";
            TablePadArticle = new TablePadArticle(
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
            )
            { Sensitive = false };
            TablePadArticle.SourceWindow = this;
            TablePadArticle.Clicked += _tablePadArticle_Clicked;
            //Put
            if (tablePadArticleVisible)
            {
                _fixedWindow.Put(TablePadArticleButtonPrev, TablePadArticleButtonPrevPosition.X, TablePadArticleButtonPrevPosition.Y);
                _fixedWindow.Put(TablePadArticleButtonNext, TablePadArticleButtonNextPosition.X, TablePadArticleButtonNextPosition.Y);
                _fixedWindow.Put(TablePadArticle, tablePadArticlePosition.X, tablePadArticlePosition.Y);
            }
        }

        private void InitUiEventboxToolbar(dynamic pThemeWindow)
        {
            dynamic themeWindow = pThemeWindow;

            //Objects:EventboxToolbar
            Point eventboxToolbarPosition = Utils.StringToPosition(themeWindow.Objects.EventboxToolbar.Position);
            Size eventboxToolbarSize = (themeWindow.Objects.EventboxToolbar.Size as string).ToSize();
            Size eventboxToolbarButtonSize = (themeWindow.Objects.EventboxToolbar.ButtonSize as string).ToSize();
            Size eventboxToolbarIconSize = (themeWindow.Objects.EventboxToolbar.IconSize as string).ToSize();
            string eventboxToolbarFont = themeWindow.Objects.EventboxToolbar.Font;
            Color eventboxToolbarFontColor = (themeWindow.Objects.EventboxToolbar.FontColor as string).StringToColor();
            bool eventboxToolbarVisible = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.Visible);
            bool eventboxToolbarVisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventboxToolbar.VisibleWindow);

            Gdk.Color eventboxToolbarBackgroundColor = (themeWindow.Objects.EventboxToolbar.BackgroundColor as string).StringToGdkColor();

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
            if (eventboxToolbarVisibleWindow) eventboxToolbar.ModifyBg(StateType.Normal, eventboxToolbarBackgroundColor);
            if (eventboxToolbarVisible) _fixedWindow.Put(eventboxToolbar, eventboxToolbarPosition.X, eventboxToolbarPosition.Y);

            //_logger.Debug("Local Func to Get Shared Buttons");
            //Local Func to Get Shared Buttons
            Func<string, string, string, IconButtonWithText> getButton = (pName, pText, pImageFileName)
                => new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = pName,
                        Text = pText,
                        Font = eventboxToolbarFont,
                        FontColor = eventboxToolbarFontColor,
                        Icon = pImageFileName,
                        IconSize = eventboxToolbarIconSize,
                        ButtonSize = new Size(eventboxToolbarButtonSize.Width, eventboxToolbarButtonSize.Height)

                    });

            //Create Button References with Local Func
            _touchButtonPosToolbarApplicationClose = getButton(buttonApplicationCloseName, buttonApplicationCloseText, buttonApplicationCloseImageFileName);
            _touchButtonPosToolbarBackOffice = getButton(buttonBackOfficeName, buttonBackOfficeText, buttonBackOfficeImageFileName);
            _touchButtonPosToolbarReports = getButton(buttonReportsName, buttonReportsText, buttonReportsImageFileName);
            _touchButtonPosToolbarShowSystemDialog = getButton(buttonShowSystemDialogName, buttonShowSystemDialogText, buttonShowSystemDialogImageFileName);
            _touchButtonPosToolbarLogoutUser = getButton(buttonLogoutUserName, buttonLogoutUserText, buttonLogoutUserImageFileName);
            _touchButtonPosToolbarShowChangeUserDialog = getButton(buttonShowChangeUserDialogName, buttonShowChangeUserDialogText, buttonShowChangeUserDialogImageFileName);
            _touchButtonPosToolbarCashDrawer = getButton(buttonCashDrawerName, buttonCashDrawerText, buttonCashDrawerImageFileName);
            _touchButtonPosToolbarFinanceDocuments = getButton(buttonFinanceDocumentsName, buttonFinanceDocumentsText, buttonFinanceDocumentsImageFileName);
            TouchButtonPosToolbarNewFinanceDocument = getButton(buttonNewFinanceDocumentName, buttonNewFinanceDocumentText, buttonNewFinanceDocumentImageFileName);

            //Toggle Sensitive Buttons
            TouchButtonPosToolbarNewFinanceDocument.Sensitive = (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open);
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
            if (buttonNewFinanceDocumentVisible) hboxToolbar.PackStart(TouchButtonPosToolbarNewFinanceDocument, false, false, 0);

            //PackIt
            eventboxToolbar.Add(hboxToolbar);

            //Assign Toolbar Button references to TicketList
            TicketList.ToolbarApplicationClose = _touchButtonPosToolbarApplicationClose;
            TicketList.ToolbarBackOffice = _touchButtonPosToolbarBackOffice;
            // Deprecated
            TicketList.ToolbarReports = _touchButtonPosToolbarReports;
            TicketList.ToolbarShowSystemDialog = _touchButtonPosToolbarShowSystemDialog;
            TicketList.ToolbarLogoutUser = _touchButtonPosToolbarLogoutUser;
            TicketList.ToolbarShowChangeUserDialog = _touchButtonPosToolbarShowChangeUserDialog;
            TicketList.ToolbarCashDrawer = _touchButtonPosToolbarCashDrawer;
            TicketList.ToolbarFinanceDocuments = _touchButtonPosToolbarFinanceDocuments;
            TicketList.ToolbarNewFinanceDocument = TouchButtonPosToolbarNewFinanceDocument;

            //Events
            _touchButtonPosToolbarApplicationClose.Clicked += touchButtonPosToolbarApplicationClose_Clicked;
            _touchButtonPosToolbarBackOffice.Clicked += touchButtonPosToolbarBackOffice_Clicked;
            // Deprecated
            _touchButtonPosToolbarReports.Clicked += touchButtonPosToolbarReports_Clicked;
            _touchButtonPosToolbarShowSystemDialog.Clicked += delegate { throw new NotImplementedException(); };
            _touchButtonPosToolbarLogoutUser.Clicked += touchButtonPosToolbarLogoutUser_Clicked;
            _touchButtonPosToolbarShowChangeUserDialog.Clicked += touchButtonPosToolbarShowChangeUserDialog_Clicked;
            _touchButtonPosToolbarCashDrawer.Clicked += touchButtonPosToolbarCashDrawer_Clicked;
            TouchButtonPosToolbarNewFinanceDocument.Clicked += touchButtonPosToolbarNewFinanceDocument_Clicked;
            _touchButtonPosToolbarFinanceDocuments.Clicked += touchButtonPosToolbarFinanceDocuments_Clicked;
        }

        private void InitUIEventBoxPosTicketPad(dynamic pThemeWindow)
        {
            dynamic themeWindow = pThemeWindow;

            //Objects:EventBoxPosTicketPad
            Point eventBoxPosTicketPadPosition = Utils.StringToPosition(themeWindow.Objects.EventBoxPosTicketPad.Position);
            Size eventBoxPosTicketPadSize = (themeWindow.Objects.EventBoxPosTicketPad.Size as string).ToSize();
            Gdk.Color eventBoxPosTicketPadBackgroundColor = (themeWindow.Objects.EventBoxPosTicketPad.BackgroundColor as string).StringToGdkColor();
            bool eventBoxPosTicketPadVisible = Convert.ToBoolean(themeWindow.Objects.EventBoxPosTicketPad.Visible);
            bool eventBoxPosTicketPadVisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxPosTicketPad.VisibleWindow);

            //UI
            _ticketPad = new TicketPad(
                "posTicketPad",
                TicketList,
                themeWindow.Objects.EventBoxPosTicketPad.Buttons,
                eventBoxPosTicketPadPosition
             )
            { Sensitive = false };

            _ticketPad.SourceWindow = this;
            EventBox eventBoxPosTicketPad = new EventBox() { VisibleWindow = eventBoxPosTicketPadVisibleWindow, WidthRequest = eventBoxPosTicketPadSize.Width, HeightRequest = eventBoxPosTicketPadSize.Height };
            if (eventBoxPosTicketPadVisibleWindow) eventBoxPosTicketPad.ModifyBg(StateType.Normal, eventBoxPosTicketPadBackgroundColor);
            eventBoxPosTicketPad.Add(_ticketPad);
            if (eventBoxPosTicketPadVisible) _fixedWindow.Put(eventBoxPosTicketPad, eventBoxPosTicketPadPosition.X, eventBoxPosTicketPadPosition.Y);
        }

        private void InitUIEventBoxPosTicketList(dynamic pThemeWindow)
        {
            _logger.Debug("void InitUIEventBoxPosTicketList(dynamic pThemeWindow) :: Starting...");
            dynamic themeWindow = pThemeWindow;

            //Objects:EventBoxPosTicketList
            Point eventBoxPosTicketListPosition = Utils.StringToPosition(themeWindow.Objects.EventBoxPosTicketList.Position);
            Size eventBoxPosTicketListSize = (themeWindow.Objects.EventBoxPosTicketList.Size as string).ToSize();
            bool eventBoxPosTicketListVisible = Convert.ToBoolean(themeWindow.Objects.EventBoxPosTicketList.Visible);
            bool eventBoxPosTicketListVisibleWindow = Convert.ToBoolean(themeWindow.Objects.EventBoxPosTicketList.VisibleWindow);
            Gdk.Color eventBoxPosTicketListBackgroundColor = (themeWindow.Objects.EventBoxPosTicketList.BackgroundColor as string).StringToGdkColor();

            //UI

            EventBox eventBoxPosTicketList = new EventBox() { VisibleWindow = eventBoxPosTicketListVisibleWindow, BorderWidth = 0 };
            eventBoxPosTicketList.WidthRequest = eventBoxPosTicketListSize.Width;
            eventBoxPosTicketList.HeightRequest = eventBoxPosTicketListSize.Height;
            if (eventBoxPosTicketListVisibleWindow) eventBoxPosTicketList.ModifyBg(StateType.Normal, eventBoxPosTicketListBackgroundColor);

            //Get ThemeObject to send to TicketList Constructor
            dynamic themeEventBoxPosTicketList = themeWindow.Objects.EventBoxPosTicketList;
            TicketList = new TicketList(themeEventBoxPosTicketList) { SourceWindow = this };
            eventBoxPosTicketList.Add(TicketList);
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
            BufferTextView = _textviewLog.Buffer;
            ScrolledWindow scrolledWindowTextviewLog = new ScrolledWindow();
            scrolledWindowTextviewLog.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrolledWindowTextviewLog.Add(_textviewLog);
            eventBoxPosTextviewLog.Add(scrolledWindowTextviewLog);
        }
    }
}