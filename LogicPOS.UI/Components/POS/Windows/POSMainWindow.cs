using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos;
using logicpos.Classes.Logic.Others;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Drawing;
using Image = Gtk.Image;

namespace LogicPOS.UI.Components.Windows
{
    public partial class POSMainWindow : POSWindow
    {
        public string ClockTimeFormat => GeneralUtils.GetResourceByName("frontoffice_datetime_format_status_bar");

        #region Components
        public Fixed FixedWindow { get; set; } = new Fixed();
        public Label LabelClock { get; set; }
        public TextView TextViewLog { get; set; }
        public TicketList TicketList { get; set; }
        public IconButtonWithText BtnQuit { get; set; }
        public IconButtonWithText BtnBackOffice { get; set; }
        public IconButtonWithText BtnReports { get; set; }
        public IconButtonWithText BtnShowSystemDialog { get; set; }
        public IconButtonWithText BtnLogOut { get; set; }
        public IconButtonWithText BtnChangeUser { get; set; }
        public IconButtonWithText BtnCashDrawer { get; set; }
        public IconButtonWithText BtnDocuments { get; set; }
        public IconButtonWithText BtnNewDocument { get; set; }
        public SaleOptionsPanel SaleOptionsPanel { get; set; }
        internal ArticleFamiliesMenu MenuFamilies { get; set; }
        internal ArticleSubfamiliesMenu MenuSubfamilies { get; set; }
        internal ArticlesMenu MenuArticles { get; set; }
        public TextBuffer BufferTextView { get; set; }
        public Label LabelTerminalInfo { get; set; }
        public Label LabelCurrentTable { get; set; } = new Label();
        public Label LabelTotalTable { get; set; }
        #endregion

        public POSMainWindow(string backgroundImage)
            : base(backgroundImage)
        {
            InitUI();

            SaleContext.Initialize(this);

            UpdateWorkSessionUI();

            StartClock();


            this.ScreenArea.Add(FixedWindow);

            bool _showMinimize = AppSettings.Instance.appShowMinimize;
            if (_showMinimize)
            {
                EventBox eventBoxMinimize = GtkUtils.CreateMinimizeButton();
                eventBoxMinimize.ButtonReleaseEvent += delegate { Iconify(); };
                FixedWindow.Put(eventBoxMinimize, GlobalApp.ScreenSize.Width - 27 - 10, 10);
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
        }

        private void InitUI()
        {
            Predicate<dynamic> predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosMainWindow");
            dynamic theme = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);

            string errorMessage = "Node: <Window ID=\"PosMainWindow\">";

            if (theme != null)
            {
                //Globals
                Name = Convert.ToString(theme.Globals.Name);

                //Init Components
                InitUIEventBoxImageLogo(theme);
                InitUIEventBoxStatusBar1(theme);
                InitUIEventBoxStatusBar2(theme);
                InitUIButtonFavorites(theme);
                InitializeSaleItemsPage(theme);
                InitializeSaleOptionsPanel(theme);
                InitializeMenus(theme);


                InitUiEventboxToolbar(theme);

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
            if (eventBoxImageLogoVisible) FixedWindow.Put(eventBoxImageLogo, eventBoxImageLogoPosition.X, eventBoxImageLogoPosition.Y);

            eventBoxImageLogo.Add(imageLogo);
            eventBoxImageLogo.ButtonPressEvent += ImageLogo_Clicked;

        }

        private void InitUIEventBoxStatusBar1(dynamic pThemeWindow)
        {
            dynamic themeWindow = pThemeWindow;

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
            LabelClock = new Label(XPOUtility.CurrentDateTime(ClockTimeFormat));
            LabelClock.ModifyFont(labelClockFont);
            LabelClock.ModifyFg(StateType.Normal, labelClockFontColor);
            LabelClock.SetAlignment(labelClockAlignmentX, 0.5F);

            //Pack HBox EventBoxStatusBar1
            HBox hboxStatusBar1 = new HBox(false, 0) { BorderWidth = 5 };
            hboxStatusBar1.PackStart(LabelTerminalInfo, false, false, 0);
            hboxStatusBar1.PackStart(LabelClock, true, true, 0);
            eventBoxStatusBar1.Add(hboxStatusBar1);

            if (eventBoxStatusBar1Visible) FixedWindow.Put(eventBoxStatusBar1, eventBoxStatusBar1Position.X, eventBoxStatusBar1Position.Y);
        }

        private void InitUIEventBoxStatusBar2(dynamic pThemeWindow)
        {
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
            HBox hboxStatusBar2 = new HBox(false, 0) { BorderWidth = 5 };
            hboxStatusBar2.PackStart(vboxCurrentTable, true, true, 0);
            hboxStatusBar2.PackStart(vboxTotalTable, false, false, 0);
            eventBoxStatusBar2.Add(hboxStatusBar2);

            if (eventBoxStatusBar2Visible) FixedWindow.Put(eventBoxStatusBar2, eventBoxStatusBar2Position.X, eventBoxStatusBar2Position.Y);
        }

        private void InitUIButtonFavorites(dynamic pThemeWindow)
        {
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

            string buttonFavoritesImageOverlay = (buttonFavoritesUseImageOverlay) ? PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png" : string.Empty;

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


            buttonFavorites.Clicked += ButtonFavorites_Clicked;

            if (buttonFavoritesVisible) FixedWindow.Put(buttonFavorites, buttonFavoritesPosition.X, buttonFavoritesPosition.Y);
        }

        private void InitializeMenus(dynamic pThemeWindow)
        {
            dynamic themeWindow = pThemeWindow;

            //VARS

            //Objects:TablePadFamilyButtonPrev
            Point btnMenuFamiliesPreviousPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.Position);
            Size TablePadFamilyButtonPrevSize = (themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.Size as string).ToSize();
            string TablePadFamilyButtonPrevImageFileName = themeWindow.Objects.TablePadFamily.TablePadFamilyButtonPrev.ImageFileName;
            //Objects:TablePadFamilyButtonNext
            Point btnMenuFamiliesNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.Position);
            Size TablePadFamilyButtonNextSize = (themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.Size as string).ToSize();
            string TablePadFamilyButtonNextImageFileName = themeWindow.Objects.TablePadFamily.TablePadFamilyButtonNext.ImageFileName;
            //Objects:TablePadFamily
            Point menuFamiliesPosition = Utils.StringToPosition(themeWindow.Objects.TablePadFamily.Position);
            Size tablePadFamilyButtonSize = (themeWindow.Objects.TablePadFamily.ButtonSize as string).ToSize();
            TableConfig tablePadFamilyTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadFamily.TableConfig);
            bool showFamiliesMenu = Convert.ToBoolean(themeWindow.Objects.TablePadFamily.Visible);

            //Objects:TablePadSubFamilyButtonPrev
            Point btnMenuSubfamiliesPreviousPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.Position);
            Size TablePadSubFamilyButtonPrevSize = (themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.Size as string).ToSize();
            string TablePadSubFamilyButtonPrevImageFileName = themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonPrev.ImageFileName;
            //Objects:TablePadSubFamilyButtonNext
            Point btnMenuSubfamiliesNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.Position);
            Size TablePadSubFamilyButtonNextSize = (themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.Size as string).ToSize();
            string TablePadSubFamilyButtonNextImageFileName = themeWindow.Objects.TablePadSubFamily.TablePadSubFamilyButtonNext.ImageFileName;
            //Objects:TablePadSubFamily
            Point menuSubfamiliesPosition = Utils.StringToPosition(themeWindow.Objects.TablePadSubFamily.Position);
            Size tablePadSubFamilyButtonSize = (themeWindow.Objects.TablePadSubFamily.ButtonSize as string).ToSize();
            TableConfig tablePadSubFamilyTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadSubFamily.TableConfig);
            bool showSubfamiliesMenu = Convert.ToBoolean(themeWindow.Objects.TablePadSubFamily.Visible);

            //Objects:TablePadArticleButtonPrev
            Point btnMenuArticlesPreviousPosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.Position);
            Size TablePadArticleButtonPrevSize = (themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.Size as string).ToSize();
            string TablePadArticleButtonPrevImageFileName = themeWindow.Objects.TablePadArticle.TablePadArticleButtonPrev.ImageFileName;
            //Objects:TablePadArticleButtonNext
            Point btnMenuArticlesNextPosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.Position);
            Size TablePadArticleButtonNextSize = (themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.Size as string).ToSize();
            string TablePadArticleButtonNextImageFileName = themeWindow.Objects.TablePadArticle.TablePadArticleButtonNext.ImageFileName;
            //Objects:TablePadArticle
            Point tablePadArticlePosition = Utils.StringToPosition(themeWindow.Objects.TablePadArticle.Position);
            Size tablePadArticleButtonSize = (themeWindow.Objects.TablePadArticle.ButtonSize as string).ToSize();
            TableConfig tablePadArticleTableConfig = Utils.StringToTableConfig(themeWindow.Objects.TablePadArticle.TableConfig);
            bool showArticlesMenu = Convert.ToBoolean(themeWindow.Objects.TablePadArticle.Visible);

            //UI

            IconButton btnFamiliesPrevious = new IconButton(
                new ButtonSettings
                {
                    Name = "TablePadFamilyButtonPrev",
                    Icon = TablePadFamilyButtonPrevImageFileName,
                    IconSize = new Size(TablePadFamilyButtonPrevSize.Width - 2, TablePadFamilyButtonPrevSize.Height - 2),
                    ButtonSize = new Size(TablePadFamilyButtonPrevSize.Width, TablePadFamilyButtonPrevSize.Height)
                });

            btnFamiliesPrevious.Relief = ReliefStyle.None;
            btnFamiliesPrevious.BorderWidth = 0;
            btnFamiliesPrevious.CanFocus = false;

            IconButton btnFamiliesNext = new IconButton(
                new ButtonSettings
                {
                    Name = "TablePadFamilyButtonNext",
                    Icon = TablePadFamilyButtonNextImageFileName,
                    IconSize = new Size(TablePadFamilyButtonNextSize.Width - 2, TablePadFamilyButtonNextSize.Height - 2),
                    ButtonSize = new Size(TablePadFamilyButtonNextSize.Width, TablePadFamilyButtonNextSize.Height)
                });

            btnFamiliesNext.Relief = ReliefStyle.None;
            btnFamiliesNext.BorderWidth = 0;
            btnFamiliesNext.CanFocus = false;


            MenuFamilies = new ArticleFamiliesMenu(btnFamiliesPrevious, btnFamiliesNext);
            MenuFamilies.SourceWindow = this;

            if (showFamiliesMenu)
            {
                FixedWindow.Put(btnFamiliesPrevious, btnMenuFamiliesPreviousPosition.X, btnMenuFamiliesPreviousPosition.Y);
                FixedWindow.Put(btnFamiliesNext, btnMenuFamiliesNextPosition.X, btnMenuFamiliesNextPosition.Y);
                FixedWindow.Put(MenuFamilies, menuFamiliesPosition.X, menuFamiliesPosition.Y);
            }

            IconButton btnSubfamiliesPrevious = new IconButton(
                new ButtonSettings
                {
                    Name = "TablePadSubFamilyButtonPrev",
                    Icon = TablePadSubFamilyButtonPrevImageFileName,
                    IconSize = new Size(TablePadSubFamilyButtonPrevSize.Width - 6, TablePadSubFamilyButtonPrevSize.Height - 6),
                    ButtonSize = new Size(TablePadSubFamilyButtonPrevSize.Width, TablePadSubFamilyButtonPrevSize.Height)
                });

            btnSubfamiliesPrevious.Relief = ReliefStyle.None;
            btnSubfamiliesPrevious.BorderWidth = 0;
            btnSubfamiliesPrevious.CanFocus = false;


            IconButton btnSubfamiliesNext = new IconButton(new ButtonSettings { Name = "TablePadSubFamilyButtonNext", Icon = TablePadSubFamilyButtonNextImageFileName, IconSize = new Size(TablePadSubFamilyButtonNextSize.Width - 6, TablePadSubFamilyButtonNextSize.Height - 6), ButtonSize = new Size(TablePadSubFamilyButtonNextSize.Width, TablePadSubFamilyButtonNextSize.Height) });
            btnSubfamiliesNext.Relief = ReliefStyle.None;
            btnSubfamiliesNext.BorderWidth = 0;
            btnSubfamiliesNext.CanFocus = false;

            MenuSubfamilies = new ArticleSubfamiliesMenu(MenuFamilies, btnSubfamiliesPrevious, btnSubfamiliesNext);
            MenuSubfamilies.SourceWindow = this;

            if (showSubfamiliesMenu)
            {
                FixedWindow.Put(btnSubfamiliesPrevious, btnMenuSubfamiliesPreviousPosition.X, btnMenuSubfamiliesPreviousPosition.Y);
                FixedWindow.Put(btnSubfamiliesNext, btnMenuSubfamiliesNextPosition.X, btnMenuSubfamiliesNextPosition.Y);
                FixedWindow.Put(MenuSubfamilies, menuSubfamiliesPosition.X, menuSubfamiliesPosition.Y);
            }

            IconButton btnMenuArticlesPrevious = new IconButton(new ButtonSettings { Name = "TablePadArticleButtonPrev", Icon = TablePadArticleButtonPrevImageFileName, IconSize = new Size(TablePadArticleButtonPrevSize.Width - 6, TablePadArticleButtonPrevSize.Height - 6), ButtonSize = new Size(TablePadArticleButtonPrevSize.Width, TablePadArticleButtonPrevSize.Height) });
            btnMenuArticlesPrevious.Relief = ReliefStyle.None;
            btnMenuArticlesPrevious.BorderWidth = 0;
            btnMenuArticlesPrevious.CanFocus = false;

            IconButton btnMenuArticlesNext = new IconButton(new ButtonSettings { Name = "TablePadArticleButtonNext", Icon = TablePadArticleButtonNextImageFileName, IconSize = new Size(TablePadArticleButtonNextSize.Width - 6, TablePadArticleButtonNextSize.Height - 6), ButtonSize = new Size(TablePadArticleButtonNextSize.Width, TablePadArticleButtonNextSize.Height) });
            btnMenuArticlesNext.Relief = ReliefStyle.None;
            btnMenuArticlesNext.BorderWidth = 0;
            btnMenuArticlesNext.CanFocus = false;


            MenuArticles = new ArticlesMenu(MenuSubfamilies,
                                            btnMenuArticlesPrevious,
                                            btnMenuArticlesNext,
                                            SaleContext.ItemsPage)
            { Sensitive = false };

            MenuArticles.SourceWindow = this;

            if (showArticlesMenu)
            {
                FixedWindow.Put(btnMenuArticlesPrevious, btnMenuArticlesPreviousPosition.X, btnMenuArticlesPreviousPosition.Y);
                FixedWindow.Put(btnMenuArticlesNext, btnMenuArticlesNextPosition.X, btnMenuArticlesNextPosition.Y);
                FixedWindow.Put(MenuArticles, tablePadArticlePosition.X, tablePadArticlePosition.Y);
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
            if (eventboxToolbarVisible) FixedWindow.Put(eventboxToolbar, eventboxToolbarPosition.X, eventboxToolbarPosition.Y);

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
            BtnQuit = getButton(buttonApplicationCloseName, buttonApplicationCloseText, buttonApplicationCloseImageFileName);
            BtnBackOffice = getButton(buttonBackOfficeName, buttonBackOfficeText, buttonBackOfficeImageFileName);
            BtnReports = getButton(buttonReportsName, buttonReportsText, buttonReportsImageFileName);
            BtnShowSystemDialog = getButton(buttonShowSystemDialogName, buttonShowSystemDialogText, buttonShowSystemDialogImageFileName);
            BtnLogOut = getButton(buttonLogoutUserName, buttonLogoutUserText, buttonLogoutUserImageFileName);
            BtnChangeUser = getButton(buttonShowChangeUserDialogName, buttonShowChangeUserDialogText, buttonShowChangeUserDialogImageFileName);
            BtnCashDrawer = getButton(buttonCashDrawerName, buttonCashDrawerText, buttonCashDrawerImageFileName);
            BtnDocuments = getButton(buttonFinanceDocumentsName, buttonFinanceDocumentsText, buttonFinanceDocumentsImageFileName);
            BtnNewDocument = getButton(buttonNewFinanceDocumentName, buttonNewFinanceDocumentText, buttonNewFinanceDocumentImageFileName);

            //Toggle Sensitive Buttons
            BtnNewDocument.Sensitive = (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open);
            //Pack Buttons
            HBox hboxToolbar = new HBox(false, 0);
            hboxToolbar.BorderWidth = 10;

            if (buttonApplicationCloseVisible) hboxToolbar.PackStart(BtnQuit, false, false, 0);
            if (buttonBackOfficeVisible) hboxToolbar.PackStart(BtnBackOffice, false, false, 0);
            if (buttonShowSystemDialogVisible) hboxToolbar.PackStart(BtnShowSystemDialog, false, false, 0);
            if (buttonLogoutUserVisible) hboxToolbar.PackStart(BtnLogOut, false, false, 0);
            if (buttonShowChangeUserDialogVisible) hboxToolbar.PackStart(BtnChangeUser, false, false, 0);
            if (buttonCashDrawerVisible) hboxToolbar.PackStart(BtnCashDrawer, false, false, 0);
            if (buttonReportsVisible) hboxToolbar.PackStart(BtnReports, false, false, 0);
            if (buttonFinanceDocumentsVisible) hboxToolbar.PackStart(BtnDocuments, false, false, 0);
            if (buttonNewFinanceDocumentVisible) hboxToolbar.PackStart(BtnNewDocument, false, false, 0);

            //PackIt
            eventboxToolbar.Add(hboxToolbar);

            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            BtnQuit.Clicked += BtnQuit_Clicked;
            BtnBackOffice.Clicked += BtnBackOffice_Clicked;
            BtnReports.Clicked += BtnReports_Clicked;
            BtnShowSystemDialog.Clicked += delegate { throw new NotImplementedException(); };
            BtnLogOut.Clicked += BtnLogOut_Clicked;
            BtnChangeUser.Clicked += BtnChangeUser_Clicked;
            BtnCashDrawer.Clicked += BtnCashDrawer_Clicked;
            BtnNewDocument.Clicked += BtnNewDocument_Clicked;
            BtnDocuments.Clicked += BtnDocuments_Clicked;
        }

        private void InitializeSaleOptionsPanel(dynamic theme)
        {
            Point position = Utils.StringToPosition(theme.Objects.EventBoxPosTicketPad.Position);
            Size size = (theme.Objects.EventBoxPosTicketPad.Size as string).ToSize();

            SaleOptionsPanel = new SaleOptionsPanel(SaleContext.ItemsPage,theme.Objects.EventBoxPosTicketPad.Buttons) { Sensitive = false };

            SaleOptionsPanel.SourceWindow = this;
            EventBox saleOptionsPanelEventBox = new EventBox() { VisibleWindow = false, WidthRequest = size.Width, HeightRequest = size.Height };

            saleOptionsPanelEventBox.Add(SaleOptionsPanel);
            FixedWindow.Put(saleOptionsPanelEventBox, position.X, position.Y);
        }

        private void InitializeSaleItemsPage(dynamic theme)
        {
            Point position = Utils.StringToPosition(theme.Objects.EventBoxPosTicketList.Position);
            Size size = (theme.Objects.EventBoxPosTicketList.Size as string).ToSize();

            EventBox saleItemsPageEventBox = new EventBox() { VisibleWindow = false, BorderWidth = 0 };
            saleItemsPageEventBox.WidthRequest = size.Width;
            saleItemsPageEventBox.HeightRequest = size.Height;

            dynamic saleItemsPageTheme = theme.Objects.EventBoxPosTicketList;
            SaleContext.ItemsPage = new SaleItemsPage(this, saleItemsPageTheme, SaleContext.CurrentOrder);
            saleItemsPageEventBox.Add(SaleContext.ItemsPage);
            
            FixedWindow.Put(saleItemsPageEventBox, position.X, position.Y);
        }
    }
}