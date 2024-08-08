using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Logic.Others;
using logicpos.shared.Enums;
using LogicPOS.Domain.Enums;
using LogicPOS.Settings;
using LogicPOS.Shared;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    public partial class PosTablesDialog : BaseDialog
    {
        //Sizes
        private Size _sizePosSmallButtonScroller = AppSettings.Instance.sizePosSmallButtonScroller;
        private Size _sizePosTableButton = AppSettings.Instance.sizePosTableButton;
        private Size _sizeIconScrollLeftRight = new Size(62, 31);
        //Files
        private readonly string _fileScrollLeftImage = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_left.png";
        private readonly string _fileScrollRightImage = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_right.png";
        //UI
        private readonly Fixed _fixedContent;
        private TablePad _tablePadPlace;
        private TablePad _tablePadOrder;
        private TablePad _tablePadTable;
        private HBox _hboxTableScrollers;
        private HBox _hboxOrderScrollers;
        private readonly IconButtonWithText _buttonOk;
        private readonly IconButtonWithText _buttonCancel;
        private readonly IconButtonWithText _buttonTableFilterAll;
        private readonly IconButtonWithText _buttonTableFilterFree;
        private readonly IconButtonWithText _buttonTableFilterOpen;
        private readonly IconButtonWithText _buttonTableFilterReserved;
        //private TouchButtonIconWithText _buttonOrderChangeTable;
        private readonly IconButtonWithText _buttonTableReservation;
        private readonly IconButtonWithText _buttonTableViewOrders;
        private readonly IconButtonWithText _buttonTableViewTables;
        //Used when we need to access CurrentButton Reference
        private TableButton _currentButton;
        //ResponseType (Above 10)
        //private ResponseType _responseTypeOrderChangeTable = (ResponseType)11;
        private readonly ResponseType _responseTypeViewOrders = (ResponseType)12;
        private readonly ResponseType _responseTypeViewTables = (ResponseType)13;
        private readonly ResponseType _responseTypeTableReservation = (ResponseType)14;
        //Other    
        private readonly int _tablesStatusShowAllIndex = 10;
        private int _currentTableStatusId = 10;
        private TableViewMode _currentViewMode = TableViewMode.Orders;
        private readonly TableFilterMode _FilterMode;
        //Base Queries
        private readonly string _sqlPlaceBase;
        private readonly string _sqlPlaceBaseOrder;
        private readonly string _sqlPlaceBaseTable;

        //Public Properties
        private Guid _currentTableButtonOid;
        public Guid CurrentTableOid
        {
            get { return _currentTableButtonOid; }
            set { _currentTableButtonOid = value; }
        }
        public PosTablesDialog(Window parentWindow, DialogFlags pDialogFlags, TableFilterMode pFilterMode = TableFilterMode.Default)
            : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_orders");
            //TODO:THEME
            //Size windowSize = new Size(837, 650);
            Size windowSize = new Size(720, 580);
            string fileDefaultWindowIcon = string.Empty;
            /* IN009035 */
            string fileActionViewOrders = string.Empty;
            /* IN008024 */
            if (!AppOperationModeSettings.IsDefaultTheme)
            {
                fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_tables_retail.png";
                fileActionViewOrders = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_retail_view_order.png";
            }
            else
            {
                fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_tables.png";
                fileActionViewOrders = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_view_order.png";
            }

            //ActionArea Icons
            string fileActionTableReservation = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_reservation.png";
            string fileActionTableFilterAll = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_all.png";
            string fileActionTableFilterFree = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_free.png";
            string fileActionTableFilterOpen = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_open.png";
            string fileActionTableFilterReserved = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_filter_reserved.png";
            string fileActionTableViewTables = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_table_view_tables.png";

            //Parameters
            _FilterMode = pFilterMode;

            //Init Content
            _fixedContent = new Fixed();

            //Init Base Queries
            _sqlPlaceBase = @"
                SELECT 
                    Oid as id, Designation as name, NULL as label, ButtonImage as image, {0}
                FROM 
                    pos_configurationplace as p 
                WHERE 
                    (Disabled IS NULL or Disabled  <> 1)
            ";

            //Used to filter Places Tab, based o View Mode (Order or Table)
            _sqlPlaceBaseOrder = string.Format(_sqlPlaceBase,
                string.Format(@"
                    (SELECT COUNT(*) FROM fin_documentordermain as om LEFT JOIN pos_configurationplacetable as pt ON om.PlaceTable = pt.Oid WHERE (om.OrderStatus = {0} AND pt.Place = p.Oid)) as childs", (int)OrderStatus.Open)
                );
            _sqlPlaceBaseTable = string.Format(_sqlPlaceBase, @"(SELECT COUNT(*) FROM pos_configurationplacetable WHERE (Disabled IS NULL or Disabled <> 1) AND Place = p.Oid) as childs");

            //Build TablePads
            BuildPlace();
            BuildOrders();
            BuildTable();

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            //Table Actions  
            _buttonTableReservation = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonTableReservation_DialogActionArea",
                    BackgroundColor = ColorSettings.ActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("pos_button_label_table_reservation"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionTableReservation,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                })
            { Sensitive = false };

            _buttonTableFilterAll = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonTableFilterAll_Green",
                    BackgroundColor = Color.Transparent,
                    Text = GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_all"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionTableFilterAll,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                })
            { Visible = false, Sensitive = false };




            _buttonTableFilterFree = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonTableFilterFree_Green",
                    BackgroundColor = ColorSettings.ActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_free"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionTableFilterFree,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                })
            { Visible = false };

            _buttonTableFilterOpen = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonTableFilterOpen_Green",
                    Text = GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_open"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionTableFilterOpen,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                })
            { Visible = false };

            _buttonTableFilterReserved = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonTableFilterReserved_Green",
                    Text = GeneralUtils.GetResourceByName("dialog_orders_button_label_tables_reserved"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionTableFilterReserved,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                })
            { Visible = false };

            //Views
            _buttonTableViewOrders = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonViewOrders_Red",
                    Text = GeneralUtils.GetResourceByName("dialog_orders_button_label_view_orders"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionViewOrders,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                });

            _buttonTableViewTables = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonViewTables_Green",
                    BackgroundColor = Color.Transparent,
                    Text = GeneralUtils.GetResourceByName("dialog_orders_button_label_view_tables"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionTableViewTables,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                });

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Orders
            //actionAreaButtons.Add(new ActionAreaButton(_buttonOrderChangeTable, _responseTypeOrderChangeTable));
            //Tables
            if (AppOperationModeSettings.IsDefaultTheme)/* IN008024 */
            {
                actionAreaButtons.Add(new ActionAreaButton(_buttonTableFilterAll, (ResponseType)_tablesStatusShowAllIndex));
                actionAreaButtons.Add(new ActionAreaButton(_buttonTableFilterFree, (ResponseType)TableStatus.Free));
                actionAreaButtons.Add(new ActionAreaButton(_buttonTableFilterOpen, (ResponseType)TableStatus.Open));
                actionAreaButtons.Add(new ActionAreaButton(_buttonTableFilterReserved, (ResponseType)TableStatus.Reserved));
                //ViewMode
                actionAreaButtons.Add(new ActionAreaButton(_buttonTableViewOrders, _responseTypeViewOrders));
                actionAreaButtons.Add(new ActionAreaButton(_buttonTableViewTables, _responseTypeViewTables));
                //Modal Result
                actionAreaButtons.Add(new ActionAreaButton(_buttonTableReservation, _responseTypeTableReservation));
            }
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _fixedContent, actionAreaButtons);

            this.ExposeEvent += delegate
            {
                //Disable Buttons if in OnlyFreeTables FilterMode
                if (_FilterMode == TableFilterMode.OnlyFreeTables)
                {
                    //Filter Buttons
                    _buttonTableFilterAll.Visible = false;
                    _buttonTableFilterFree.Visible = false;
                    _buttonTableFilterOpen.Visible = false;
                    _buttonTableFilterReserved.Visible = false;
                    //Other ActionButtons
                    _buttonTableViewOrders.Visible = false;
                    _buttonTableReservation.Visible = false;
                }
            };

            //After Init, Disabled Widgets
            ToggleViewMode();
        }

        private void BuildPlace()
        {
            //Scrollers
            IconButton buttonPosScrollersPlacePrev = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonPosScrollersTablePrev",
                    BackgroundColor = Color.White,
                    Icon = _fileScrollLeftImage,
                    IconSize = _sizeIconScrollLeftRight,
                    ButtonSize = new Size(_sizePosSmallButtonScroller.Width,
                                          _sizePosSmallButtonScroller.Height)
                });

            IconButton buttonPosScrollersPlaceNext = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonPosScrollersTableNext",
                    BackgroundColor = Color.White,
                    Icon = _fileScrollRightImage,
                    IconSize = _sizeIconScrollLeftRight,
                    ButtonSize = new Size(_sizePosSmallButtonScroller.Width,
                                          _sizePosSmallButtonScroller.Height)
                });


            buttonPosScrollersPlacePrev.Relief = ReliefStyle.None;
            buttonPosScrollersPlaceNext.Relief = ReliefStyle.None;
            buttonPosScrollersPlacePrev.BorderWidth = 0;
            buttonPosScrollersPlaceNext.BorderWidth = 0;
            buttonPosScrollersPlacePrev.CanFocus = false;
            buttonPosScrollersPlaceNext.CanFocus = false;
            HBox hboxPlaceScrollers = new HBox(true, 0);
            hboxPlaceScrollers.PackStart(buttonPosScrollersPlacePrev);
            hboxPlaceScrollers.PackStart(buttonPosScrollersPlaceNext);

            //TablePad Places
            //TODO:THEME
            //TableConfig tableConfig = new TableConfig(6, 1);
            TableConfig tableConfig = new TableConfig(5, 1);
            _tablePadPlace = new TablePad(
                _sqlPlaceBaseOrder,
                "ORDER BY Ord",
                "",
                Guid.Empty,
                true,
                tableConfig.Rows,
                tableConfig.Columns,
                "buttonPlaceId",
                Color.Transparent,
                _sizePosTableButton.Width,
                _sizePosTableButton.Height,
                buttonPosScrollersPlacePrev,
                buttonPosScrollersPlaceNext
            );
            //Click Event
            _tablePadPlace.Clicked += tablePadPlace_Clicked;

            _fixedContent.Put(_tablePadPlace, 0, 0);
            //TODO:THEME
            //_fixedContent.Put(hboxPlaceScrollers, 0, 493);
            _fixedContent.Put(hboxPlaceScrollers, 0, 493 - _sizePosTableButton.Height);
        }

        private void BuildOrders()
        {
            //Colors
            //Color colorPosButtonArticleBackground = FrameworkUtils.StringToColor(LogicPOS.Settings.AppSettings.Instance.colorPosButtonArticleBackground"]);

            //Scrollers
            IconButton buttonPosScrollersOrderPrev = new IconButton(new ButtonSettings { Name = "buttonPosScrollersOrderPrev", BackgroundColor = Color.White, Icon = _fileScrollLeftImage, IconSize = _sizeIconScrollLeftRight, ButtonSize = new Size(_sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height) });
            IconButton buttonPosScrollersOrderNext = new IconButton(new ButtonSettings { Name = "buttonPosScrollersOrderNext", BackgroundColor = Color.White, Icon = _fileScrollRightImage, IconSize = _sizeIconScrollLeftRight, ButtonSize = new Size(_sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height) });
            buttonPosScrollersOrderPrev.Relief = ReliefStyle.None;
            buttonPosScrollersOrderNext.Relief = ReliefStyle.None;
            buttonPosScrollersOrderPrev.BorderWidth = 0;
            buttonPosScrollersOrderNext.BorderWidth = 0;
            buttonPosScrollersOrderPrev.CanFocus = false;
            buttonPosScrollersOrderNext.CanFocus = false;

            _hboxOrderScrollers = new HBox(true, 0);
            _hboxOrderScrollers.PackStart(buttonPosScrollersOrderPrev, false, false, 0);
            _hboxOrderScrollers.PackStart(buttonPosScrollersOrderNext, false, false, 0);

            //TablePad Tables
            //String sql = string.Format(@"SELECT om.Oid as id, concat(om.Oid, ':', om.OrderStatus, ':',Place) as name, NULL as label, NULL as image 
            string sql = string.Format(@"
                SELECT 
                    om.Oid as id, Designation as name, NULL as label, NULL as image, TableStatus as status, TotalOpen as total, DateTableOpen as dateopen, DateTableClosed as dateclosed 
                FROM 
                    fin_documentordermain as om
                LEFT JOIN 
                    pos_configurationplacetable as pt ON om.PlaceTable = pt.Oid
                WHERE 
                    (om.OrderStatus = {0})"
                , (int)OrderStatus.Open)
            ;
            string filter = string.Format("AND (Place = '{0}')", _tablePadPlace.SelectedButtonOid);

            //TODO:THEME
            //TableConfig tableConfig = new TableConfig(6, 5);
            TableConfig tableConfig = new TableConfig(5, 4);
            _tablePadOrder = new TablePadTable(
                sql,
                "ORDER BY Ord",
                filter,
                Guid.Empty,
                true,
                tableConfig.Rows,
                tableConfig.Columns,
                "buttonOrderId",
                Color.Transparent,
                _sizePosTableButton.Width,
                _sizePosTableButton.Height,
                buttonPosScrollersOrderPrev,
                buttonPosScrollersOrderNext
              );

            //Events
            _tablePadOrder.Clicked += _tablePadOrder_Clicked;

            _fixedContent.Put(_tablePadOrder, 143, 0);
            //TODO:THEME
            //_fixedContent.Put(_hboxOrderScrollers, 690, 493);
            _fixedContent.Put(_hboxOrderScrollers, 690, 493 - _sizePosTableButton.Height);
        }

        private void BuildTable()
        {
            //Colors
            //Color colorPosButtonArticleBackground = FrameworkUtils.StringToColor(LogicPOS.Settings.AppSettings.Instance.colorPosButtonArticleBackground"]);

            //Scrollers
            IconButton buttonPosScrollersTablePrev = new IconButton(new ButtonSettings { Name = "buttonPosScrollersTablePrev", BackgroundColor = Color.White, Icon = _fileScrollLeftImage, IconSize = _sizeIconScrollLeftRight, ButtonSize = new Size(_sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height) });
            IconButton buttonPosScrollersTableNext = new IconButton(new ButtonSettings { Name = "buttonPosScrollersTableNext", BackgroundColor = Color.White, Icon = _fileScrollRightImage, IconSize = _sizeIconScrollLeftRight, ButtonSize = new Size(_sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height) });
            buttonPosScrollersTablePrev.Relief = ReliefStyle.None;
            buttonPosScrollersTableNext.Relief = ReliefStyle.None;
            buttonPosScrollersTablePrev.BorderWidth = 0;
            buttonPosScrollersTableNext.BorderWidth = 0;
            buttonPosScrollersTablePrev.CanFocus = false;
            buttonPosScrollersTableNext.CanFocus = false;

            _hboxTableScrollers = new HBox(true, 0);
            _hboxTableScrollers.PackStart(buttonPosScrollersTablePrev, false, false, 0);
            _hboxTableScrollers.PackStart(buttonPosScrollersTableNext, false, false, 0);

            //TablePad Tables
            string sql = @"SELECT Oid as id, Designation as name, NULL as label, NULL as image, TableStatus as status, TotalOpen as total, DateTableOpen as dateopen, DateTableClosed as dateclosed FROM pos_configurationplacetable WHERE (Disabled IS NULL or Disabled  <> 1)";
            string filter = string.Format("AND (Place = '{0}')", _tablePadPlace.SelectedButtonOid);

            //if in FilterMode (Change Table) OnlyFreeTables add TableStatus Filter
            if (_FilterMode == TableFilterMode.OnlyFreeTables) filter = string.Format("{0} AND (TableStatus = {1}  OR TableStatus IS NULL)", filter, (int)TableStatus.Free);

            //Prepare current table
            Guid currentTableOid = Guid.Empty;
            if (POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId)
              && POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].Table != null)
            {
                currentTableOid = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId].Table.Oid;
            }

            //Initialize TablePad
            TableConfig tableConfig = new TableConfig(5, 4);
            _tablePadTable = new TablePadTable(
                sql,
                "ORDER BY Ord",
                filter,
                currentTableOid,
                true,
                tableConfig.Rows,
                tableConfig.Columns,
                "buttonTableId",
                Color.Transparent,
                _sizePosTableButton.Width,
                _sizePosTableButton.Height,
                buttonPosScrollersTablePrev,
                buttonPosScrollersTableNext
            );

            //Always Assign SelectedButton Reference to Dialog Reference, Prevent OK Withou Select Table (Toggle Mode)
            _currentTableButtonOid = _tablePadTable.SelectedButtonOid;

            //Events
            _tablePadTable.Clicked += tablePadTable_Clicked;

            _fixedContent.Put(_tablePadTable, 143, 0);
            //TODO:THEME
            //_fixedContent.Put(_hboxTableScrollers, 690, 493);
            _fixedContent.Put(_hboxTableScrollers, 690 - _sizePosTableButton.Width, 493 - _sizePosTableButton.Height);
        }
    }
}