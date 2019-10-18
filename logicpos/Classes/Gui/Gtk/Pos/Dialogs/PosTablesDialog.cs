using Gtk;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Logic.Others;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.datalayer.Enums;
using logicpos.shared.Enums;
using System;
using System.Drawing;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    public partial class PosTablesDialog : PosBaseDialog
    {
        //Sizes
        private Size _sizePosSmallButtonScroller = Utils.StringToSize(GlobalFramework.Settings["sizePosSmallButtonScroller"]);
        private Size _sizePosTableButton = Utils.StringToSize(GlobalFramework.Settings["sizePosTableButton"]);
        private Size _sizeIconScrollLeftRight = new Size(62, 31);
        //Files
        private String _fileScrollLeftImage = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Buttons\Pos\button_subfamily_article_scroll_left.png");
        private String _fileScrollRightImage = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Buttons\Pos\button_subfamily_article_scroll_right.png");
        //UI
        private Fixed _fixedContent;
        private TablePad _tablePadPlace;
        private TablePad _tablePadOrder;
        private TablePad _tablePadTable;
        private HBox _hboxTableScrollers;
        private HBox _hboxOrderScrollers;
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        private TouchButtonIconWithText _buttonTableFilterAll;
        private TouchButtonIconWithText _buttonTableFilterFree;
        private TouchButtonIconWithText _buttonTableFilterOpen;
        private TouchButtonIconWithText _buttonTableFilterReserved;
        //private TouchButtonIconWithText _buttonOrderChangeTable;
        private TouchButtonIconWithText _buttonTableReservation;
        private TouchButtonIconWithText _buttonTableViewOrders;
        private TouchButtonIconWithText _buttonTableViewTables;
        //Used when we need to access CurrentButton Reference
        private TouchButtonTable _currentButton;
        //ResponseType (Above 10)
        //private ResponseType _responseTypeOrderChangeTable = (ResponseType)11;
        private ResponseType _responseTypeViewOrders = (ResponseType)12;
        private ResponseType _responseTypeViewTables = (ResponseType)13;
        private ResponseType _responseTypeTableReservation = (ResponseType)14;
        //Other    
        private int _tablesStatusShowAllIndex = 10;
        private int _currentTableStatusId = 10;
        private TableViewMode _currentViewMode = TableViewMode.Orders;
        private TableFilterMode _FilterMode;
        //Base Queries
        private String _sqlPlaceBase;
        private String _sqlPlaceBaseOrder;
        private String _sqlPlaceBaseTable;

        //Public Properties
        private Guid _currentTableButtonOid;
        public Guid CurrentTableOid
        {
            get { return _currentTableButtonOid; }
            set { _currentTableButtonOid = value; }
        }
        public PosTablesDialog(Window pSourceWindow, DialogFlags pDialogFlags, TableFilterMode pFilterMode = TableFilterMode.Default)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_orders");
            //TODO:THEME
            //Size windowSize = new Size(837, 650);
            Size windowSize = new Size(720, 580);
            String fileDefaultWindowIcon = string.Empty;
			/* IN009035 */
            String fileActionViewOrders = string.Empty;
            /* IN008024 */
            if (!SettingsApp.IsDefaultTheme)
            {
                fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_tables_retail.png");
                fileActionViewOrders = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_retail_view_order.png");
            }
            else
            {
                fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_tables.png");
                fileActionViewOrders = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_table_view_order.png");
            }

            //ActionArea Icons
            String fileActionTableReservation = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_table_reservation.png");
            String fileActionTableFilterAll = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_table_filter_all.png");
            String fileActionTableFilterFree = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_table_filter_free.png");
            String fileActionTableFilterOpen = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_table_filter_open.png");
            String fileActionTableFilterReserved = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_table_filter_reserved.png");
            String fileActionTableViewTables = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_table_view_tables.png");

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
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //Table Actions  
            _buttonTableReservation = new TouchButtonIconWithText("touchButtonTableReservation_DialogActionArea", _colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_button_label_table_reservation"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionTableReservation, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height) { Sensitive = false };
            //Tables
            _buttonTableFilterAll = new TouchButtonIconWithText("touchButtonTableFilterAll_Green", Color.Transparent, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_orders_button_label_tables_all"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionTableFilterAll, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height) { Visible = false, Sensitive = false };
            _buttonTableFilterFree = new TouchButtonIconWithText("touchButtonTableFilterFree_Green", Color.Transparent, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_orders_button_label_tables_free"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionTableFilterFree, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height) { Visible = false };
            _buttonTableFilterOpen = new TouchButtonIconWithText("touchButtonTableFilterOpen_Green", Color.Transparent, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_orders_button_label_tables_open"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionTableFilterOpen, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height) { Visible = false };
            _buttonTableFilterReserved = new TouchButtonIconWithText("touchButtonTableFilterReserved_Green", Color.Transparent, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_orders_button_label_tables_reserved"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionTableFilterReserved, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height) { Visible = false };
            //Views
            _buttonTableViewOrders = new TouchButtonIconWithText("touchButtonViewOrders_Red", Color.Transparent, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_orders_button_label_view_orders"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionViewOrders, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);
            _buttonTableViewTables = new TouchButtonIconWithText("touchButtonViewTables_Green", Color.Transparent, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_orders_button_label_view_tables"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionTableViewTables, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);
            //Orders
            //_buttonOrderChangeTable = new TouchButtonIconWithText("touchButtonOrderChangeTable_Red", Color.Transparent, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_button_label_change_table, _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, _fileActionDefault, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height) { Sensitive = false };

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Orders
            //actionAreaButtons.Add(new ActionAreaButton(_buttonOrderChangeTable, _responseTypeOrderChangeTable));
            //Tables
            if (SettingsApp.IsDefaultTheme)/* IN008024 */
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
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _fixedContent, actionAreaButtons);

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
            //Colors
            //Color colorPosButtonFamilyBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosButtonFamilyBackground"]);

            //Scrollers
            TouchButtonIcon buttonPosScrollersPlacePrev = new TouchButtonIcon("buttonPosScrollersTablePrev", Color.White, _fileScrollLeftImage, _sizeIconScrollLeftRight, _sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height);
            TouchButtonIcon buttonPosScrollersPlaceNext = new TouchButtonIcon("buttonPosScrollersTableNext", Color.White, _fileScrollRightImage, _sizeIconScrollLeftRight, _sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height);
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
            //Color colorPosButtonArticleBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosButtonArticleBackground"]);

            //Scrollers
            TouchButtonIcon buttonPosScrollersOrderPrev = new TouchButtonIcon("buttonPosScrollersOrderPrev", Color.White, _fileScrollLeftImage, _sizeIconScrollLeftRight, _sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height);
            TouchButtonIcon buttonPosScrollersOrderNext = new TouchButtonIcon("buttonPosScrollersOrderNext", Color.White, _fileScrollRightImage, _sizeIconScrollLeftRight, _sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height);
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
            String sql = string.Format(@"
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
            String filter = string.Format("AND (Place = '{0}')", _tablePadPlace.SelectedButtonOid);

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
            //Color colorPosButtonArticleBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosButtonArticleBackground"]);

            //Scrollers
            TouchButtonIcon buttonPosScrollersTablePrev = new TouchButtonIcon("buttonPosScrollersTablePrev", Color.White, _fileScrollLeftImage, _sizeIconScrollLeftRight, _sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height);
            TouchButtonIcon buttonPosScrollersTableNext = new TouchButtonIcon("buttonPosScrollersTableNext", Color.White, _fileScrollRightImage, _sizeIconScrollLeftRight, _sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height);
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
            String sql = @"SELECT Oid as id, Designation as name, NULL as label, NULL as image, TableStatus as status, TotalOpen as total, DateTableOpen as dateopen, DateTableClosed as dateclosed FROM pos_configurationplacetable WHERE (Disabled IS NULL or Disabled  <> 1)";
            String filter = string.Format("AND (Place = '{0}')", _tablePadPlace.SelectedButtonOid);

            //if in FilterMode (Change Table) OnlyFreeTables add TableStatus Filter
            if (_FilterMode == TableFilterMode.OnlyFreeTables) filter = string.Format("{0} AND (TableStatus = {1}  OR TableStatus IS NULL)", filter, (int)TableStatus.Free);

            //Prepare current table
            Guid currentTableOid = Guid.Empty;
            if (GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp.CurrentOrderMainOid)
              && GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid].Table != null)
            {
                currentTableOid = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid].Table.Oid;
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