using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
using LogicPOS.Core.Authentication;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class PosWindow : Window, IOfficeSurface
{
    private const string ClockFormat = "dddd, dd' de 'MMMM' de 'yyyy' || 'HH:mm:ss tt";

    private string _userName;
    private string _terminalName = string.Empty;
    private BackOfficeWindow? _backOffice;
    private readonly DispatcherTimer _clock = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly Dictionary<string, Bitmap> _bitmaps = new();

    private readonly List<Control> _menuControls = new();
    private PosLayout _layout = PosLayout.For(1920, 1080);
    private PosCatalog _catalog = PosCatalog.Empty;
    private TextBlock? _terminalLabel;
    private TextBlock? _clockLabel;
    private Button? _familyPrevious;
    private Button? _familyNext;
    private Button? _subfamilyPrevious;
    private Button? _subfamilyNext;
    private Button? _articlePrevious;
    private Button? _articleNext;
    private Guid? _familyId;
    private Guid? _subfamilyId;
    private bool _showFavorites;
    private int _familyPage;
    private int _subfamilyPage;
    private int _articlePage;
    private readonly PosTicket _ticket = new();
    private readonly ScannerInputReader _scanner;
    private TextBlock? _amountDue;
    private TextBlock? _ticketTotal;
    private ScrollViewer? _ticketLines;
    private StackPanel? _ticketRows;
    private TextBlock? _keypadValue;
    private bool _orderListMode;
    private int _orderSelectedIndex = -1;
    private bool _orderEditBusy;
    private Guid? _openOrderId;
    private PosTableSlot? _currentTable;
    private readonly List<PosTicketLine> _orderLines = new();
    private int _orderTicketCount;
    private TextBlock? _currentTableLabel;
    private List<PosTableSlot> _tableSlots = new();
    private string _tableFilter = "all";
    private Guid? _tablePlaceId;
    private string? _pickedTableKey;
    private int _tablesPage;
    private int _tablesPlacesPage;
    private const int TablesPerPage = 15;
    private const int TablesPlacesPerPage = 3;
    private const double TablesPlaceColumnWidth = 128;
    private Guid? _pendingUserId;
    private string _pendingUserName = string.Empty;
    private string? _sessionAction;
    private bool _dayOpen;
    private bool _terminalOpen;
    private decimal _cashTotal;
    private TextBlock? _sessionStateLabel;
    private readonly Dictionary<string, Button> _toolbarButtons = new();
    private readonly Dictionary<string, Button> _padButtons = new();
    private PosCustomer? _saleCustomer;
    private bool _replaceKeypad;
    private bool _priceTyping;
    private decimal _priceMoney;
    private TicketPrompt _prompt = TicketPrompt.None;
    private StockManagementView? _stockView;

    private enum TicketPrompt
    {
        None,
        Delete,
        DeleteOrderLine,
        Quantity,
        Price,
        Weight,
        Barcode,
        Card,
        ChangeUser,
        Notice
    }

    public PosWindow()
        : this(string.Empty)
    {
    }

    public PosWindow(string userName)
    {
        _userName = userName;
        var configuration = AppComposition.Services?.GetService<IConfiguration>();
        _scanner = ScannerInputReader.FromConfiguration(configuration);
        _scanner.Captured += OnScannerCaptured;
        InitializeComponent();
        NewDocumentHost.PreviewRequested += async (path, title) => await ShowPdfAsync(path, title);
        ReportsHost.PreviewRequested += async (path, title) => await ShowPdfAsync(path, title);
        ReportsHost.Closed += (_, _) => ReportsOverlay.IsVisible = false;
        BuildKeypad();
        TouchFields.Attach(this);
        AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);
        _clock.Tick += (_, _) => UpdateClock();
        Opened += async (_, _) =>
        {
            try
            {
                await OpenAsync();
            }
            catch (Exception exception)
            {
                App.WriteCrash("PosWindow.OpenAsync", exception);
            }
        };
        Closed += (_, _) =>
        {
            _clock.Stop();
            _scanner.Reset();
            _scanner.Captured -= OnScannerCaptured;
        };
    }

    public event EventHandler? LoggedOut;

    private async Task OpenAsync()
    {
        var screen = Screens?.ScreenFromVisual(this) ?? Screens?.Primary;
        var width = (int)Math.Round(screen?.Bounds.Width ?? 1920d);
        var height = (int)Math.Round(screen?.Bounds.Height ?? 1080d);
        _layout = PosLayout.For(width, height);
        BuildChrome();
        UpdateClock();
        _clock.Start();

        var services = AppComposition.Services;
        if (services is null)
        {
            RefreshMenus();
            return;
        }

        _terminalName = await services.GetRequiredService<ILoginService>().GetFirstTerminalNameAsync() ?? string.Empty;
        if (_terminalLabel is not null)
        {
            _terminalLabel.Text = string.IsNullOrWhiteSpace(_terminalName) ? _userName : $"{_terminalName} : {_userName}";
        }

        await ReloadCatalogAsync();
        _familyId = _catalog.Families.FirstOrDefault()?.Id;
        _subfamilyId = SubfamiliesOfSelectedFamily().FirstOrDefault()?.Id;
        await RefreshWorkSessionAsync();
        RefreshMenus();
        await LoadDefaultTableAsync();
    }

    private async Task ReloadCatalogAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        try
        {
            _catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync();
        }
        catch
        {
            return;
        }

        if (_catalog.Families.Any(item => item.Id == _familyId) == false)
        {
            _familyId = _catalog.Families.FirstOrDefault()?.Id;
            _subfamilyId = null;
        }

        if (SubfamiliesOfSelectedFamily().Any(item => item.Id == _subfamilyId) == false)
        {
            _subfamilyId = SubfamiliesOfSelectedFamily().FirstOrDefault()?.Id;
        }

        RefreshMenus();
    }

    private void BuildChrome()
    {
        Surface.Children.Clear();
        _menuControls.Clear();
        _toolbarButtons.Clear();
        _padButtons.Clear();
        var layout = _layout;

        var logoImage = new Image
        {
            Classes = { "pos_logo" },
            Source = LoadBitmap("avares://logicpos/Assets/Images/logicpos_logo.png")
        };
        RenderOptions.SetBitmapInterpolationMode(logoImage, BitmapInterpolationMode.HighQuality);
        var logo = new Border { Classes = { "pos_logo_box" } };
        logo.Child = logoImage;
        Place(logo, layout.LogoX, layout.LogoY, layout.LogoWidth, layout.LogoHeight);

        var status = new Border { Classes = { "pos_status_bar_1" } };
        var statusGrid = new Grid { Classes = { "pos_status_bar_content" } };
        statusGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        statusGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        statusGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        _terminalLabel = new TextBlock { Classes = { "pos_status_text" }, Text = _userName };
        _sessionStateLabel = new TextBlock { Classes = { "pos_status_text" }, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White };
        Grid.SetColumn(_sessionStateLabel, 1);
        _clockLabel = new TextBlock { Classes = { "pos_status_text" }, HorizontalAlignment = HorizontalAlignment.Right };
        Grid.SetColumn(_clockLabel, 2);
        statusGrid.Children.Add(_terminalLabel);
        statusGrid.Children.Add(_sessionStateLabel);
        statusGrid.Children.Add(_clockLabel);
        status.Child = statusGrid;
        Place(status, layout.StatusBar1X, layout.StatusBar1Y, layout.StatusBar1Width, layout.StatusBar1Height);

        if (layout.StatusBar2Width > 0)
        {
            var order = new Border { Classes = { "pos_status_bar_2" } };
            var orderGrid = new Grid { Classes = { "pos_status_bar_content" } };
            orderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            orderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            orderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            orderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            AddStatusText(orderGrid, "Ordem", 0, 0, "pos_status_text_small", HorizontalAlignment.Left);
            _currentTableLabel = new TextBlock { Classes = { "pos_status_text" }, Text = CurrentTableCaption(), HorizontalAlignment = HorizontalAlignment.Left };
            Grid.SetRow(_currentTableLabel, 1);
            orderGrid.Children.Add(_currentTableLabel);
            AddStatusText(orderGrid, "Total a pagar", 0, 1, "pos_status_text_small", HorizontalAlignment.Right);
            _amountDue = new TextBlock { Classes = { "pos_status_text" }, Text = "0", HorizontalAlignment = HorizontalAlignment.Right };
            Grid.SetRow(_amountDue, 1);
            Grid.SetColumn(_amountDue, 1);
            orderGrid.Children.Add(_amountDue);
            order.Child = orderGrid;
            Place(order, layout.StatusBar2X, layout.StatusBar2Y, layout.StatusBar2Width, layout.StatusBar2Height);
        }

        PlaceTicketList(layout);
        PlaceTicketPad(layout);
        PlaceToolbar(layout);

        _familyPrevious = CreateScroll("avares://logicpos/Assets/Images/button_family_scroll_up.png", () => ChangePage(ref _familyPage, -1, FamilyPageCount()));
        _familyNext = CreateScroll("avares://logicpos/Assets/Images/button_family_scroll_down.png", () => ChangePage(ref _familyPage, 1, FamilyPageCount()));
        _subfamilyPrevious = CreateScroll("avares://logicpos/Assets/Images/Pos/button_subfamily_article_scroll_left.png", () => ChangePage(ref _subfamilyPage, -1, SubfamilyPageCount()));
        _subfamilyNext = CreateScroll("avares://logicpos/Assets/Images/Pos/button_subfamily_article_scroll_right.png", () => ChangePage(ref _subfamilyPage, 1, SubfamilyPageCount()));
        _articlePrevious = CreateScroll("avares://logicpos/Assets/Images/Pos/button_subfamily_article_scroll_left.png", () => ChangePage(ref _articlePage, -1, ArticlePageCount()));
        _articleNext = CreateScroll("avares://logicpos/Assets/Images/Pos/button_subfamily_article_scroll_right.png", () => ChangePage(ref _articlePage, 1, ArticlePageCount()));
        Place(_familyPrevious, layout.FamilyPreviousX, layout.FamilyPreviousY, layout.ButtonWidth, layout.StatusBar1Height);
        Place(_familyNext, layout.FamilyNextX, layout.FamilyNextY, layout.ButtonWidth, layout.StatusBar1Height);
        Place(_subfamilyPrevious, layout.SubfamilyPreviousX, layout.SubfamilyPreviousY, layout.ScrollWidth, layout.StatusBar1Height);
        Place(_subfamilyNext, layout.SubfamilyNextX, layout.SubfamilyNextY, layout.ScrollWidth, layout.StatusBar1Height);
        Place(_articlePrevious, layout.ArticlePreviousX, layout.ArticlePreviousY, layout.ScrollWidth, layout.StatusBar1Height);
        Place(_articleNext, layout.ArticleNextX, layout.ArticleNextY, layout.ScrollWidth, layout.StatusBar1Height);
    }

    private void PlaceTicketList(PosLayout layout)
    {
        var list = new Border { Classes = { "pos_ticket_list" } };
        var dock = new DockPanel { LastChildFill = true };
        var total = new Border { Classes = { "pos_ticket_total" } };
        DockPanel.SetDock(total, Dock.Bottom);
        var totalGrid = new Grid { Classes = { "pos_status_bar_content" } };
        totalGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        totalGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        var totalLabel = new TextBlock { Classes = { "pos_ticket_total_label" }, Text = "Total" };
        _ticketTotal = new TextBlock { Classes = { "pos_ticket_total_value" }, Text = 0m.ToString("C") };
        Grid.SetColumn(_ticketTotal, 1);
        totalGrid.Children.Add(totalLabel);
        totalGrid.Children.Add(_ticketTotal);
        total.Child = totalGrid;

        var header = new Border { Classes = { "pos_ticket_header_row" } };
        DockPanel.SetDock(header, Dock.Top);
        var columns = new Grid();
        columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(layout.DesignationColumnWidth)));
        columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(layout.PriceColumnWidth)));
        columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(layout.QuantityColumnWidth)));
        columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(layout.TotalColumnWidth)));
        AddTicketHeader(columns, "Designação", 0, false, layout.TicketFontSize);
        AddTicketHeader(columns, "Preço", 1, true, layout.TicketFontSize);
        AddTicketHeader(columns, "Qtd.", 2, true, layout.TicketFontSize);
        AddTicketHeader(columns, "Total", 3, true, layout.TicketFontSize);
        header.Child = columns;

        _ticketRows = new StackPanel();
        _ticketLines = new ScrollViewer { Classes = { "pos_ticket_lines" }, Content = _ticketRows };
        dock.Children.Add(total);
        dock.Children.Add(header);
        dock.Children.Add(_ticketLines);
        list.Child = dock;
        var buttonLeft = layout.TicketPadX + PosLayout.Margin + 2;
        var buttonTop = layout.TicketPadY + PosLayout.Margin + 2;
        var buttonWidth = (layout.TicketButtonWidth * PosLayout.TicketColumns) - 4;
        const int totalGap = 12;
        Place(list, buttonLeft, layout.TicketListY, buttonWidth, Math.Max(0, buttonTop - layout.TicketListY - totalGap));
        RefreshTicket();
    }

    private void PlaceTicketPad(PosLayout layout)
    {
        AddPadButton("Anterior", "icon_pos_ticketpad_prev.png", 0, 0, 1, "pos_pad_button_grey", SelectPreviousLine);
        AddPadButton("Próximo", "icon_pos_ticketpad_next.png", 0, 1, 1, "pos_pad_button_grey", SelectNextLine);
        AddPadButton("Eliminar", "icon_pos_ticketpad_delete.png", 0, 2, 1, "pos_pad_button_grey", RequestDeleteTicket);
        AddPadButton("Quantidade", "icon_pos_ticketpad_change_quantity.png", 0, 3, 1, "pos_pad_button_grey", RequestQuantity);
        AddPadButton("Aumentar", "icon_pos_ticketpad_increase.png", 1, 0, 1, "pos_pad_button_grey", IncreaseSelected);
        AddPadButton("Diminuir", "icon_pos_ticketpad_decrease.png", 1, 1, 1, "pos_pad_button_grey", DecreaseSelected);
        AddPadButton("Alt. lista", "icon_pos_ticketpad_list_mode.png", 1, 2, 1, "pos_pad_button_grey", ToggleListMode);
        AddPadButton("Preço", "icon_pos_ticketpad_change_price.png", 1, 3, 1, "pos_pad_button_grey", RequestPrice);
        AddPadButton("Fin.pedido", "icon_pos_retail_ticketpad_finish_order.png", 2, 0, 2, "pos_pad_button_green", FinishTicket);
        AddPadButton("Pagamentos", "icon_pos_ticketpad_payments.png", 2, 1, 1, "pos_pad_button_green", RequestPayments);
        AddPadButton("Reemb. Volta", "icon_pos_ticketpad_payments.png", 3, 1, 1, "pos_pad_button_grey", ShowVoltaRefund);
        AddPadButton("Ordens", "icon_pos_retail_view_orders.png", 2, 2, 1, "pos_pad_button_green", ShowOrders);
        AddPadButton("Cartão", "icon_pos_ticketpad_card_entry.png", 3, 2, 1, "pos_pad_button_grey", RequestCard);
        AddPadButton("Cód.Barras/Artigo", "icon_pos_ticketpad_barcode.png", 2, 3, 1, "pos_pad_button_grey", RequestBarcode);
        AddPadButton("Peso", "icon_pos_ticketpad_weight.png", 3, 3, 1, "pos_pad_button_grey", RequestWeight);
        ApplyFrontOfficeRules();
    }

    private void PlaceToolbar(PosLayout layout)
    {
        var buttons = new (string Text, string Icon, string Style, Action? Click)[]
        {
            ("Fechar", "icon_pos_toolbar_application_close.png", "pos_toolbar_button_close", ShowQuit),
            ("BackOffice", "icon_pos_toolbar_back_office.png", "pos_toolbar_button", ShowBackOffice),
            ("Sair da Sessão", "icon_pos_toolbar_logout_user.png", "pos_toolbar_button", Logout),
            ("Utilizador", "icon_pos_toolbar_show_change_user_dialog.png", "pos_toolbar_button", ShowChangeUser),
            ("Sessão", "icon_pos_toolbar_cashdrawer.png", "pos_toolbar_button", ShowSession),
            ("Relatórios", "icon_pos_toolbar_reports.png", "pos_toolbar_button", ShowReports),
            ("Documentos", "icon_pos_toolbar_finance_document.png", "pos_toolbar_button", ShowDocuments),
            ("Novo Doc.", "icon_pos_toolbar_finance_new_document.png", "pos_toolbar_button", () => _ = ShowNewDocumentAsync())
        };

        var x = layout.ToolbarX + PosLayout.Margin;
        var y = layout.ToolbarY + PosLayout.Margin;
        foreach (var item in buttons)
        {
            var button = new Button
            {
                Classes = { item.Style },
                Content = CreateIconContent($"avares://logicpos/Assets/Images/Pos/{item.Icon}", item.Text, layout.ToolbarIconSize)
            };
            if (item.Click is not null)
            {
                var click = item.Click;
                button.Click += (_, _) => click();
            }

            Place(button, x, y, layout.ToolbarButtonWidth, layout.ToolbarButtonHeight, inset: 2);
            _toolbarButtons[item.Text] = button;
            x += layout.ToolbarButtonWidth;
        }

        ApplyFrontOfficeRules();
    }

    private void RefreshMenus()
    {
        foreach (var control in _menuControls)
        {
            Surface.Children.Remove(control);
        }

        _menuControls.Clear();
        var layout = _layout;

        var favoritesButton = CreateMenuButton("Favoritos", false, _showFavorites, () =>
        {
            _showFavorites = true;
            _articlePage = 0;
            RefreshMenus();
        });
        var favoritesImage = new Image
        {
            Classes = { "pos_favorites_image" },
            Source = LoadBitmap("avares://logicpos/Assets/Images/Pos/button_favorites.png")
        };
        var favoritesCaption = new TextBlock { Classes = { "pos_menu_caption" }, Text = "Favoritos" };
        var favorites = new Grid();
        favorites.Children.Add(favoritesImage);
        favorites.Children.Add(favoritesCaption);
        favoritesButton.Content = favorites;
        PlaceMenu(favoritesButton, layout.FavoritesX, layout.FavoritesY, layout.ButtonWidth, layout.ButtonHeight);

        var families = Page(_catalog.Families, _familyPage, layout.FamilyRows);
        for (var index = 0; index < families.Count; index++)
        {
            var family = families[index];
            var button = CreateMenuButton(family.Text, false, family.Id == _familyId && _showFavorites == false, () => SelectFamily(family.Id));
            PlaceMenu(button, layout.FamilyX, layout.FamilyY + (index * layout.ButtonHeight), layout.ButtonWidth, layout.ButtonHeight);
        }

        PlaceEmptySlots(families.Count, 1, layout.FamilyRows, layout.FamilyX, layout.FamilyY);

        var subfamilyColumns = VisibleSubfamilyColumns();
        var subfamilies = Page(SubfamiliesOfSelectedFamily(), _subfamilyPage, subfamilyColumns);
        for (var index = 0; index < subfamilies.Count; index++)
        {
            var subfamily = subfamilies[index];
            var button = CreateMenuButton(subfamily.Text, false, subfamily.Id == _subfamilyId && _showFavorites == false, () => SelectSubfamily(subfamily.Id));
            PlaceMenu(button, layout.SubfamilyX + (index * layout.ButtonWidth), layout.SubfamilyY, layout.ButtonWidth, layout.ButtonHeight);
        }

        PlaceEmptySlots(subfamilies.Count, subfamilyColumns, 1, layout.SubfamilyX, layout.SubfamilyY);

        var articleCount = layout.ArticleColumns * layout.ArticleRows;
        var articles = Page(ArticlesForCurrentView(), _articlePage, articleCount);
        for (var index = 0; index < articles.Count; index++)
        {
            var column = index % layout.ArticleColumns;
            var row = index / layout.ArticleColumns;
            var article = articles[index];
            var button = CreateMenuButton(article.Text, true, false, () => AddArticle(article));
            PlaceMenu(
                button,
                layout.ArticleX + (column * layout.ButtonWidth),
                layout.ArticleY + (row * layout.ButtonHeight),
                layout.ButtonWidth,
                layout.ButtonHeight);
        }

        PlaceEmptySlots(articles.Count, layout.ArticleColumns, layout.ArticleRows, layout.ArticleX, layout.ArticleY);

        SetScrollEnabled(_familyPrevious, _familyPage > 0);
        SetScrollEnabled(_familyNext, _familyPage < FamilyPageCount() - 1);
        SetScrollEnabled(_subfamilyPrevious, _subfamilyPage > 0);
        SetScrollEnabled(_subfamilyNext, _subfamilyPage < SubfamilyPageCount() - 1);
        SetScrollEnabled(_articlePrevious, _articlePage > 0);
        SetScrollEnabled(_articleNext, _articlePage < ArticlePageCount() - 1);
        ApplyFrontOfficeRules();
    }

    private void SelectFamily(Guid familyId)
    {
        _showFavorites = false;
        _familyId = familyId;
        _subfamilyPage = 0;
        _articlePage = 0;
        _subfamilyId = SubfamiliesOfSelectedFamily().FirstOrDefault()?.Id;
        RefreshMenus();
    }

    private void SelectSubfamily(Guid subfamilyId)
    {
        _showFavorites = false;
        _subfamilyId = subfamilyId;
        _articlePage = 0;
        RefreshMenus();
    }

    private IReadOnlyList<PosMenuItem> SubfamiliesOfSelectedFamily()
    {
        if (_familyId is null)
        {
            return Array.Empty<PosMenuItem>();
        }

        return _catalog.Subfamilies.Where(item => item.ParentId == _familyId.Value).ToList();
    }

    private IReadOnlyList<PosArticle> ArticlesForCurrentView()
    {
        if (_showFavorites)
        {
            return _catalog.Articles.Where(item => item.Favorite).ToList();
        }

        if (_subfamilyId is null)
        {
            return Array.Empty<PosArticle>();
        }

        return _catalog.Articles.Where(item => item.SubfamilyId == _subfamilyId.Value).ToList();
    }

    private int FamilyPageCount() => PageCount(_catalog.Families.Count, _layout.FamilyRows);

    private int SubfamilyPageCount() => PageCount(SubfamiliesOfSelectedFamily().Count, VisibleSubfamilyColumns());

    private int ArticlePageCount() => PageCount(ArticlesForCurrentView().Count, _layout.ArticleColumns * _layout.ArticleRows);

    private static int PageCount(int count, int pageSize)
    {
        if (pageSize < 1)
        {
            return 1;
        }

        var pages = (int)Math.Ceiling(count / (double)pageSize);
        return pages < 1 ? 1 : pages;
    }

    private static IReadOnlyList<T> Page<T>(IReadOnlyList<T> items, int page, int pageSize)
    {
        return items.Skip(page * pageSize).Take(pageSize).ToList();
    }

    private void ChangePage(ref int page, int delta, int pageCount)
    {
        var next = page + delta;
        if (next < 0 || next >= pageCount)
        {
            return;
        }

        page = next;
        RefreshMenus();
    }

    private int VisibleSubfamilyColumns()
    {
        if (_layout.ButtonWidth < 1)
        {
            return _layout.SubfamilyColumns;
        }

        var fit = (_layout.SubfamilyPreviousX - _layout.SubfamilyX) / _layout.ButtonWidth;
        return fit < 1 ? _layout.SubfamilyColumns : Math.Min(_layout.SubfamilyColumns, fit);
    }

    private void PlaceEmptySlots(int filled, int columns, int rows, int originX, int originY)
    {
        if (columns < 1 || rows < 1)
        {
            return;
        }

        var total = columns * rows;
        for (var index = Math.Max(0, filled); index < total; index++)
        {
            var column = index % columns;
            var row = index / columns;
            PlaceMenu(
                new Border { Classes = { "pos_menu_slot" } },
                originX + (column * _layout.ButtonWidth),
                originY + (row * _layout.ButtonHeight),
                _layout.ButtonWidth,
                _layout.ButtonHeight);
        }
    }

    private Button CreateMenuButton(string text, bool article, bool selected, Action? click)
    {
        var button = new Button
        {
            Classes = { "pos_menu_button", article ? "pos_menu_button_grey" : "pos_menu_button_green" },
            Content = new TextBlock { Classes = { "pos_menu_caption" }, Text = text }
        };
        if (selected)
        {
            button.Classes.Add("pos_menu_button_selected");
        }

        if (article)
        {
            button.IsEnabled = _terminalOpen;
        }

        if (click is not null)
        {
            button.Click += (_, _) => click();
        }

        return button;
    }

    private Button CreateScroll(string iconUri, Action click)
    {
        var button = new Button
        {
            Classes = { "pos_scroll" },
            Content = new Image
            {
                Classes = { "pos_scroll_icon" },
                Source = LoadBitmap(iconUri)
            }
        };
        button.Click += (_, _) => click();
        return button;
    }

    private void AddPadButton(string text, string iconFile, int column, int row, int columnSpan, string styleClass, Action click)
    {
        var layout = _layout;
        var button = new Button
        {
            Classes = { "pos_pad_button", styleClass },
            Content = CreateIconContent($"avares://logicpos/Assets/Images/Pos/{iconFile}", text, layout.TicketIconSize)
        };
        button.Click += (_, _) => click();
        _padButtons[text] = button;
        Place(
            button,
            layout.TicketPadX + PosLayout.Margin + (column * layout.TicketButtonWidth),
            layout.TicketPadY + PosLayout.Margin + (row * layout.TicketButtonHeight),
            layout.TicketButtonWidth * columnSpan,
            layout.TicketButtonHeight,
            inset: 2);
    }

    private Control CreateIconContent(string iconUri, string text, int iconSize)
    {
        var panel = new StackPanel
        {
            Spacing = 2,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        panel.Children.Add(new Image
        {
            Classes = { "pos_button_icon" },
            Width = iconSize,
            Height = iconSize,
            Source = LoadBitmap(iconUri)
        });
        panel.Children.Add(new TextBlock
        {
            Classes = { "pos_button_caption" },
            Text = text
        });
        return panel;
    }

    private static void AddStatusText(Grid grid, string text, int row, int column, string styleClass, HorizontalAlignment alignment)
    {
        var block = new TextBlock
        {
            Classes = { styleClass },
            Text = text,
            HorizontalAlignment = alignment
        };
        Grid.SetRow(block, row);
        Grid.SetColumn(block, column);
        grid.Children.Add(block);
    }

    private static void AddTicketHeader(Grid grid, string text, int column, bool right, int fontSize)
    {
        var block = new TextBlock
        {
            Classes = { right ? "pos_ticket_header_right" : "pos_ticket_header" },
            Text = text,
            FontSize = fontSize
        };
        Grid.SetColumn(block, column);
        grid.Children.Add(block);
    }

    private void PlaceMenu(Control control, int x, int y, int width, int height)
    {
        Place(control, x, y, width, height, inset: 2);
        _menuControls.Add(control);
    }

    private void Place(Control control, int x, int y, int width, int height, int inset = 0)
    {
        control.Width = width - (inset * 2);
        control.Height = height - (inset * 2);
        Canvas.SetLeft(control, x + inset);
        Canvas.SetTop(control, y + inset);
        Surface.Children.Add(control);
    }

    private async Task RefreshWorkSessionAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        try
        {
            var state = await services.GetRequiredService<IPosOrderService>().GetWorkSessionAsync();
            _dayOpen = state.DayOpen;
            _terminalOpen = state.TerminalOpen;
        }
        catch
        {
            _dayOpen = false;
            _terminalOpen = false;
        }

        ApplyFrontOfficeRules();
    }

    private void ApplyFrontOfficeRules()
    {
        var selling = _terminalOpen;
        var editingTicket = _orderListMode == false;
        var hasLines = _ticket.HasLines;
        var hasOrder = _orderLines.Count > 0;

        foreach (var control in _menuControls)
        {
            if (control is Button button && button.Classes.Contains("pos_menu_button_grey"))
            {
                button.IsEnabled = selling;
            }
        }

        var lineEditable = editingTicket ? hasLines : SelectedOrderLine is not null && _orderEditBusy == false;
        SetNamedButton(_padButtons, "Anterior", selling && (editingTicket ? hasLines : hasOrder));
        SetNamedButton(_padButtons, "Próximo", selling && (editingTicket ? hasLines : hasOrder));
        SetNamedButton(_padButtons, "Aumentar", selling && lineEditable);
        SetNamedButton(_padButtons, "Diminuir", selling && lineEditable);
        SetNamedButton(_padButtons, "Quantidade", selling && lineEditable);
        SetNamedButton(_padButtons, "Preço", selling && lineEditable);
        SetNamedButton(_padButtons, "Peso", selling && hasLines && editingTicket);
        SetNamedButton(_padButtons, "Fin.pedido", selling && hasLines && editingTicket);
        SetNamedButton(_padButtons, "Eliminar", selling && lineEditable);
        SetNamedButton(_padButtons, "Pagamentos", selling && (hasLines || hasOrder));
        SetNamedButton(_padButtons, "Ordens", selling);
        SetNamedButton(_padButtons, "Alt. lista", selling);
        SetNamedButton(_padButtons, "Cartão", selling);
        SetNamedButton(_padButtons, "Cód.Barras/Artigo", selling);
        SetNamedButton(_padButtons, "Reemb. Volta", selling && hasLines == false);

        var toolbarEnabled = hasLines == false;
        SetNamedButton(_toolbarButtons, "BackOffice", toolbarEnabled);
        SetNamedButton(_toolbarButtons, "Sair da Sessão", toolbarEnabled);
        SetNamedButton(_toolbarButtons, "Utilizador", toolbarEnabled);
        SetNamedButton(_toolbarButtons, "Sessão", toolbarEnabled);
        SetNamedButton(_toolbarButtons, "Relatórios", toolbarEnabled);
        SetNamedButton(_toolbarButtons, "Documentos", toolbarEnabled);
        SetNamedButton(_toolbarButtons, "Novo Doc.", toolbarEnabled);

        DaySessionCaption.Text = _dayOpen ? "Fecho do dia" : "Abertura do dia";
        // Close-day requires terminal closed first; open-day is always available
        DaySessionButton.IsEnabled = _dayOpen == false || _terminalOpen == false;
        // Cash drawer is only available when the day is open
        SessionMenuButton.IsEnabled = _dayOpen;

        if (_sessionStateLabel is not null)
        {
            var dayText = _dayOpen ? "Aberto" : "Fechado";
            var sessionText = _terminalOpen ? "Aberta" : "Fechada";
            _sessionStateLabel.Text = $"Dia: {dayText}  |  Sessão: {sessionText}";
        }
    }

    private static void SetNamedButton(Dictionary<string, Button> buttons, string name, bool enabled)
    {
        if (buttons.TryGetValue(name, out var button))
        {
            button.IsEnabled = enabled;
        }
    }

    private static void SetScrollEnabled(Button? button, bool enabled)
    {
        if (button is not null)
        {
            button.IsEnabled = enabled;
        }
    }
    private void UpdateClock()
    {
        if (_clockLabel is not null)
        {
            _clockLabel.Text = DateTime.Now.ToString(ClockFormat);
        }
    }

    public async Task ShowNewDocumentAsync(Guid? draftId = null)
    {
        NewDocumentOverlay.IsVisible = true;
        var created = await NewDocumentHost.ShowAsync(draftId);
        NewDocumentOverlay.IsVisible = false;
        if (created)
        {
            if (DocumentsOverlay.IsVisible)
            {
                await DocumentsHost.ReloadAsync();
            }

            if (string.IsNullOrEmpty(NewDocumentHost.CreatedPdfPath) == false)
            {
                await ShowPdfAsync(NewDocumentHost.CreatedPdfPath, NewDocumentHost.CreatedPdfTitle);
            }
        }
    }

    public async Task ShowPdfAsync(string path, string? title)
    {
        var reportsOpen = ReportsOverlay.IsVisible;
        ReportsOverlay.IsVisible = false;
        PdfOverlay.IsVisible = true;
        await PdfHost.ShowAsync(path, title);
        PdfOverlay.IsVisible = false;
        ReportsOverlay.IsVisible = reportsOpen;
    }

    private void ShowReports()
    {
        HideListingOverlays();
        ReportsOverlay.IsVisible = true;
        _ = ReportsHost.ShowAsync();
    }

    private void HideListingOverlays()
    {
        DocumentsOverlay.IsVisible = false;
        ReceiptOverlay.IsVisible = false;
        EntityOverlay.IsVisible = false;
        StockOverlay.IsVisible = false;
        NewDocumentOverlay.IsVisible = false;
        ReportsOverlay.IsVisible = false;
        PdfOverlay.IsVisible = false;
    }

    private async Task ShowDocumentsListingAsync()
    {
        HideListingOverlays();
        DocumentsOverlay.IsVisible = true;
        await DocumentsHost.ReloadAsync();
    }

    private async Task ShowReceiptEmissionAsync()
    {
        HideListingOverlays();
        ReceiptOverlay.IsVisible = true;
        await ReceiptHost.ReloadAsync();
    }

    private async Task ShowEntityListingAsync(string title)
    {
        HideListingOverlays();
        EntityTitle.Text = title;
        EntityOverlay.IsVisible = true;
        await EntityHost.ShowAsync(title);
    }

    private async Task ShowStockOverlayAsync()
    {
        HideListingOverlays();
        StockOverlay.IsVisible = true;
        EnsureStockView();
        if (_stockView is not null)
        {
            await _stockView.ReloadAsync();
        }
    }

    private void EnsureStockView()
    {
        if (_stockView is not null)
        {
            return;
        }

        try
        {
            _stockView = new StockManagementView();
            StockHost.Children.Clear();
            StockHost.Children.Add(_stockView);
        }
        catch (Exception exception)
        {
            App.WriteCrash("PosWindow.EnsureStockView", exception);
            StockHost.Children.Clear();
            StockHost.Children.Add(new TextBlock
            {
                Text = exception.Message,
                Classes = { "login_alert_message" }
            });
        }
    }

    private void OnCloseNamedOverlayClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string name })
        {
            return;
        }

        if (this.FindControl<Panel>(name) is Panel overlay)
        {
            overlay.IsVisible = false;
        }
    }

    private void OnTogglePopupSizeClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string frameName, Content: Panel icons })
        {
            return;
        }

        if (this.FindControl<Border>(frameName) is not Border frame || icons.Children.Count < 2)
        {
            return;
        }

        var expanded = double.IsNaN(frame.Width);
        if (expanded)
        {
            frame.Width = 1100;
            frame.Height = 700;
            frame.HorizontalAlignment = HorizontalAlignment.Center;
            frame.VerticalAlignment = VerticalAlignment.Center;
        }
        else
        {
            frame.Width = double.NaN;
            frame.Height = double.NaN;
            frame.HorizontalAlignment = HorizontalAlignment.Stretch;
            frame.VerticalAlignment = VerticalAlignment.Stretch;
        }

        icons.Children[0].IsVisible = expanded;
        icons.Children[1].IsVisible = expanded == false;
    }

    private bool TryCloseListingOverlay()
    {
        if (PdfOverlay.IsVisible || NewDocumentOverlay.IsVisible || ReportsOverlay.IsVisible)
        {
            return false;
        }

        Panel? open = StockOverlay.IsVisible ? StockOverlay
            : EntityOverlay.IsVisible ? EntityOverlay
            : ReceiptOverlay.IsVisible ? ReceiptOverlay
            : DocumentsOverlay.IsVisible ? DocumentsOverlay
            : null;
        if (open is null)
        {
            return false;
        }

        open.IsVisible = false;
        return true;
    }

    private void ShowBackOffice() => ShowBackOffice(null);

    private void ShowBackOffice(string? page)
    {
        var session = string.IsNullOrWhiteSpace(_terminalName) ? _userName : $"{_terminalName} : {_userName}";
        if (_backOffice is null)
        {
            _backOffice = new BackOfficeWindow(session);
            _backOffice.ReturnToPos += async (_, _) =>
            {
                _backOffice.Hide();
                Show();
                Activate();
                await ReloadCatalogAsync();
            };
            _backOffice.LoggedOut += (_, _) => Logout();
        }

        Hide();
        _backOffice.Show();
        _backOffice.Activate();
        _backOffice.RefreshSession();
        if (string.IsNullOrWhiteSpace(page) == false)
        {
            _backOffice.OpenPage(page);
        }
    }

    private void ShowQuit()
    {
        QuitOverlay.IsVisible = true;
    }

    private void Logout()
    {
        LoggedOut?.Invoke(this, EventArgs.Empty);
        Close();
    }

    private void OnQuitConfirmClick(object? sender, RoutedEventArgs e)
    {
        QuitOverlay.IsVisible = false;
        if (global::Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow?.Close();
        }

        Close();
    }

    private void OnQuitDismissClick(object? sender, RoutedEventArgs e)
    {
        QuitOverlay.IsVisible = false;
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (QuitOverlay.IsVisible)
        {
            if (e.Key == Key.Enter)
            {
                OnQuitConfirmClick(this, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                QuitOverlay.IsVisible = false;
                e.Handled = true;
            }

            return;
        }

        if (e.Key == Key.Escape
            && FocusManager?.GetFocusedElement() is not TextBox
            && TryCloseListingOverlay())
        {
            e.Handled = true;
            return;
        }

        if (CanCaptureScanner() == false)
        {
            return;
        }

        if (_scanner.TryHandleKey(e.Key, e.KeyModifiers, out var handled) && handled)
        {
            e.Handled = true;
        }
    }

    private bool CanCaptureScanner()
    {
        if (IsVisible == false || _backOffice is { IsVisible: true })
        {
            return false;
        }

        if (QuitOverlay.IsVisible
            || SessionOverlay.IsVisible
            || ListOverlay.IsVisible
            || DocumentsMenuOverlay.IsVisible
            || DocumentsOverlay.IsVisible
            || ReceiptOverlay.IsVisible
            || EntityOverlay.IsVisible
            || StockOverlay.IsVisible
            || NewDocumentOverlay.IsVisible
            || ReportsOverlay.IsVisible
            || PdfOverlay.IsVisible)
        {
            return false;
        }

        // Manual barcode/card dialog already owns the text box.
        if (_prompt is TicketPrompt.Barcode or TicketPrompt.Card)
        {
            return false;
        }

        if (TicketOverlay.IsVisible && _prompt is not TicketPrompt.None)
        {
            return false;
        }

        // Do not steal keys while the operator types in a focused text box.
        if (FocusManager?.GetFocusedElement() is TextBox)
        {
            return false;
        }

        return true;
    }

    private async void OnScannerCaptured(object? sender, ScannerCapturedEventArgs e)
    {
        switch (e.Device)
        {
            case ScannerDevice.BarCodeReader:
                if (_terminalOpen == false)
                {
                    ShowQuestion(TicketPrompt.Notice, "Sessão", "Abra o dia e a sessão antes de registar artigos.", question: false, confirmText: "Ok", showCancel: false);
                    break;
                }

                TryAddArticleByCode(e.Code);
                break;
            case ScannerDevice.CardReader:
            {
                var result = await LookupCardAsync(e.Code);
                ShowQuestion(
                    TicketPrompt.Notice,
                    "Cartão",
                    result.Found ? $"Cliente: {result.Text}" : result.Text,
                    question: false,
                    confirmText: "Ok",
                    showCancel: false);
                break;
            }
        }
    }

    private Bitmap LoadBitmap(string uri)
    {
        if (_bitmaps.TryGetValue(uri, out var cached))
        {
            return cached;
        }

        var bitmap = new Bitmap(AssetLoader.Open(new Uri(uri)));
        _bitmaps[uri] = bitmap;
        return bitmap;
    }

    private void AddArticle(PosArticle article)
    {
        if (_terminalOpen == false)
        {
            return;
        }

        _orderListMode = false;
        _ticket.Add(article);
        RefreshTicket();
    }

    private void SelectPreviousLine()
    {
        if (_orderListMode)
        {
            _orderSelectedIndex = Math.Max(0, _orderSelectedIndex - 1);
        }
        else
        {
            _ticket.SelectPrevious();
        }

        RefreshTicket();
    }

    private void SelectNextLine()
    {
        if (_orderListMode)
        {
            _orderSelectedIndex = Math.Min(_orderLines.Count - 1, _orderSelectedIndex + 1);
        }
        else
        {
            _ticket.SelectNext();
        }

        RefreshTicket();
    }

    private void IncreaseSelected()
    {
        if (SelectedOrderLine is { } line)
        {
            _ = ChangeOrderLineAsync(line, line.Quantity + 1m, line.NetUnitPrice);
            return;
        }

        _ticket.IncreaseSelected();
        RefreshTicket();
    }

    private void DecreaseSelected()
    {
        if (SelectedOrderLine is { } line)
        {
            _ = ChangeOrderLineAsync(line, Math.Max(0m, line.Quantity - 1m), line.NetUnitPrice);
            return;
        }

        _ticket.DecreaseSelected();
        RefreshTicket();
    }

    private PosTicketLine? SelectedOrderLine =>
        _orderListMode && _orderSelectedIndex >= 0 && _orderSelectedIndex < _orderLines.Count
            ? _orderLines[_orderSelectedIndex]
            : null;

    private async Task ChangeOrderLineAsync(PosTicketLine line, decimal quantity, decimal netUnitPrice)
    {
        var services = AppComposition.Services;
        if (_orderEditBusy || services is null || _openOrderId is not Guid orderId)
        {
            return;
        }

        // Stored lines are matched by article and unit price, so a price change replaces the whole line
        var priceChanged = netUnitPrice != line.NetUnitPrice;
        var reduce = priceChanged ? line.Quantity : Math.Max(0m, line.Quantity - quantity);
        var add = priceChanged ? quantity : Math.Max(0m, quantity - line.Quantity);
        if (reduce == 0m && add == 0m)
        {
            return;
        }

        _orderEditBusy = true;
        string? error = null;
        try
        {
            var orders = services.GetRequiredService<IPosOrderService>();
            if (reduce > 0m)
            {
                var reduced = await orders.ReduceLineAsync(orderId, line.ArticleId, line.NetUnitPrice, reduce);
                error = reduced.Succeeded ? null : reduced.Error ?? "Não foi possível alterar a ordem.";
            }

            if (error is null && add > 0m)
            {
                var saved = await orders.SaveAsync(orderId, [new PosStoredLine(line.ArticleId, add, netUnitPrice)], _currentTable?.Id);
                error = saved.Succeeded ? null : saved.Error ?? "Não foi possível alterar a ordem.";
            }
        }
        catch (Exception exception)
        {
            error = exception.Message;
        }
        finally
        {
            _orderEditBusy = false;
        }

        await ReloadCurrentTableAsync();
        if (error is not null)
        {
            ShowQuestion(TicketPrompt.Notice, "Ordens", error, question: false, confirmText: "Ok", showCancel: false);
        }
    }

    private void ToggleListMode()
    {
        _orderListMode = !_orderListMode;
        RefreshTicket();
    }

    private async void FinishTicket()
    {
        await FinishTicketAsync();
    }

    private async Task<bool> FinishTicketAsync()
    {
        if (_ticket.HasLines == false)
        {
            return true;
        }

        var services = AppComposition.Services;
        if (services is null)
        {
            return false;
        }

        var lines = _ticket.Lines
            .Select(line => new PosStoredLine(line.ArticleId, line.Quantity, line.NetUnitPrice))
            .ToList();
        var saved = await services.GetRequiredService<IPosOrderService>().SaveAsync(_openOrderId, lines, _currentTable?.Id);
        if (saved.Succeeded == false)
        {
            ShowQuestion(TicketPrompt.Notice, "Aviso", saved.Error ?? "Não foi possível gravar o pedido.", question: false, confirmText: "Ok", showCancel: false);
            return false;
        }

        _ticket.Clear();
        await ReloadCurrentTableAsync();
        return true;
    }

    private string CurrentTableCaption()
    {
        return _currentTable is null ? string.Empty : $"Ordem {_currentTable.Name}";
    }

    private async Task LoadDefaultTableAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        try
        {
            var orderService = services.GetRequiredService<IPosOrderService>();
            var tables = await orderService.ListTablesAsync();
            var defaultId = await orderService.GetDefaultTableIdAsync();
            var table = tables.FirstOrDefault(item => item.Id == defaultId) ?? tables.FirstOrDefault();
            if (table is not null)
            {
                await SetCurrentTableAsync(table);
            }
        }
        catch
        {
            // Tables are optional: the POS keeps selling without a current table
        }
    }

    private async Task ReloadCurrentTableAsync()
    {
        var services = AppComposition.Services;
        if (services is null || _currentTable is null)
        {
            RefreshTicket();
            return;
        }

        var tables = await services.GetRequiredService<IPosOrderService>().ListTablesAsync();
        var table = tables.FirstOrDefault(item => item.Id == _currentTable.Id) ?? _currentTable;
        await SetCurrentTableAsync(table);
    }

    private async Task SetCurrentTableAsync(PosTableSlot table)
    {
        var sameOrder = _openOrderId is not null && _openOrderId == table.OrderId;
        _currentTable = table;
        _openOrderId = table.OrderId;
        _orderLines.Clear();
        _orderTicketCount = 0;

        var services = AppComposition.Services;
        if (services is not null && table.OrderId is Guid orderId)
        {
            var stored = await services.GetRequiredService<IPosOrderService>().ReadLinesAsync(orderId);
            _orderTicketCount = stored.Select(line => line.TicketKey).Distinct().Count();
            foreach (var storedLine in stored)
            {
                var article = _catalog.Articles.FirstOrDefault(item => item.Id == storedLine.ArticleId);
                if (article is null)
                {
                    continue;
                }

                _orderLines.Add(new PosTicketLine(article)
                {
                    Quantity = storedLine.Quantity,
                    NetUnitPrice = storedLine.NetUnitPrice
                });
            }
        }

        _orderSelectedIndex = sameOrder ? Math.Min(_orderSelectedIndex, _orderLines.Count - 1) : -1;

        // GTK: selecting a table always presents the order items
        _orderListMode = true;
        if (_currentTableLabel is not null)
        {
            _currentTableLabel.Text = CurrentTableCaption();
        }

        RefreshTicket();
    }

    private async void ShowOrders()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        IReadOnlyList<PosTableSlot> tables;
        try
        {
            tables = await services.GetRequiredService<IPosOrderService>().ListTablesAsync();
        }
        catch (Exception exception)
        {
            ShowQuestion(TicketPrompt.Notice, "Aviso", exception.Message, question: false, confirmText: "Ok", showCancel: false);
            return;
        }

        if (tables.Count == 0)
        {
            IReadOnlyList<PosOpenOrder> orders;
            try
            {
                orders = await services.GetRequiredService<IPosOrderService>().ListOpenAsync();
            }
            catch (Exception exception)
            {
                ShowQuestion(TicketPrompt.Notice, "Aviso", exception.Message, question: false, confirmText: "Ok", showCancel: false);
                return;
            }

            ShowList("Ordens", orders.Select(order => (order.Id.ToString(), order.Label)).ToList(), "Não existem ordens abertas.");
            return;
        }

        _tableSlots = tables.ToList();
        _tableFilter = "all";
        var current = _tableSlots.FirstOrDefault(item => item.Id == _currentTable?.Id);
        _tablePlaceId = current?.PlaceId ?? _tableSlots.FirstOrDefault()?.PlaceId;
        _pickedTableKey = current?.Id.ToString();
        var placeIndex = _tableSlots.Select(item => item.PlaceId).Distinct().ToList().IndexOf(_tablePlaceId ?? Guid.Empty);
        _tablesPlacesPage = Math.Max(0, placeIndex) / TablesPlacesPerPage;
        var tableIndex = current is null ? -1 : _tableSlots.Where(MatchesTableFilter).ToList().IndexOf(current);
        _tablesPage = Math.Max(0, tableIndex) / TablesPerPage;
        ShowTableDialog();
    }

    private void ShowTableDialog()
    {
        ListTitle.Text = "Ordens";
        ListTitleIcon.IsVisible = true;
        ListTitleIcon.Source = LoadBitmap("avares://logicpos/Assets/Images/Dialogs/icon_window_tables_retail.png");
        ListFrame.Width = 760;
        ListFrame.Height = 520;
        ListFrame.MaxHeight = double.PositiveInfinity;
        TablesBar.IsVisible = true;
        ListActions.IsVisible = false;
        RenderTables();
        ListOverlay.IsVisible = true;
    }

    private void RestoreListChrome()
    {
        TablesBar.IsVisible = false;
        ListActions.IsVisible = true;
        ListFrame.Width = 560;
        ListFrame.Height = double.NaN;
        ListFrame.MaxHeight = 360;
    }

    private void RenderTables()
    {
        ListHost.Children.Clear();
        var places = _tableSlots
            .GroupBy(table => table.PlaceId)
            .Select(group => (Id: group.Key, Name: group.First().PlaceName))
            .ToList();
        if (_tablePlaceId is null || places.Any(place => place.Id == _tablePlaceId) == false)
        {
            _tablePlaceId = places.FirstOrDefault().Id;
        }

        var body = new Grid { Classes = { "pos_tables_body" } };
        body.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(TablesPlaceColumnWidth)));
        body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        body.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        var placesPanel = new StackPanel();
        var tablesPanel = new WrapPanel { Orientation = Orientation.Horizontal };
        Grid.SetColumn(tablesPanel, 1);
        body.Children.Add(placesPanel);
        body.Children.Add(tablesPanel);

        var placePages = Math.Max(1, (places.Count + TablesPlacesPerPage - 1) / TablesPlacesPerPage);
        _tablesPlacesPage = Math.Clamp(_tablesPlacesPage, 0, placePages - 1);
        foreach (var place in places.Skip(_tablesPlacesPage * TablesPlacesPerPage).Take(TablesPlacesPerPage))
        {
            var placeId = place.Id;
            var button = new Button
            {
                Classes = { "pos_doc_menu", "pos_tables_place" },
                Content = new StackPanel
                {
                    Children =
                    {
                        new Image
                        {
                            Classes = { "pos_doc_menu_icon" },
                            Source = LoadBitmap("avares://logicpos/Assets/Images/BackOffice/icon_other_tables.png")
                        },
                        new TextBlock { Classes = { "pos_doc_menu_caption" }, Text = place.Name }
                    }
                }
            };
            if (placeId == _tablePlaceId)
            {
                button.Classes.Add("selected");
            }

            button.Click += (_, _) =>
            {
                _tablePlaceId = placeId;
                _tablesPage = 0;
                RenderTables();
            };
            placesPanel.Children.Add(button);
        }

        var visibleTables = _tableSlots.Where(MatchesTableFilter).ToList();
        var tablePages = Math.Max(1, (visibleTables.Count + TablesPerPage - 1) / TablesPerPage);
        _tablesPage = Math.Clamp(_tablesPage, 0, tablePages - 1);
        foreach (var table in visibleTables.Skip(_tablesPage * TablesPerPage).Take(TablesPerPage))
        {
            var key = table.Id.ToString();
            var content = new StackPanel { Classes = { "pos_table_content" } };
            content.Children.Add(new TextBlock { Classes = { "pos_table_name" }, Text = table.Name });
            content.Children.Add(new TextBlock
            {
                Classes = { "pos_table_time" },
                Text = table.State == PosTableState.Open && table.OpenedAt is DateTime openedAt
                    ? $"Aberta às {openedAt:HH:mm}"
                    : string.Empty
            });
            var status = new Border { Classes = { "pos_table_status" } };
            if (table.State == PosTableState.Open)
            {
                status.Classes.Add("pos_table_status_open");
                status.Child = new TextBlock { Classes = { "pos_table_status_text" }, Text = table.Total.ToString("C") };
            }
            else if (table.State == PosTableState.Reserved)
            {
                status.Classes.Add("pos_table_status_reserved");
                status.Child = new TextBlock { Classes = { "pos_table_status_text" }, Text = "Reservada" };
            }

            content.Children.Add(status);
            var button = new Button
            {
                Classes = { "pos_table_button" },
                Content = content
            };
            if (_pickedTableKey == key)
            {
                button.Classes.Add("selected");
            }

            button.Click += (_, _) =>
            {
                _pickedTableKey = key;
                RenderTables();
            };
            tablesPanel.Children.Add(button);
        }

        var placeScroll = CreateTablesScroller(
            () => { _tablesPlacesPage--; RenderTables(); },
            () => { _tablesPlacesPage++; RenderTables(); },
            _tablesPlacesPage > 0,
            _tablesPlacesPage < placePages - 1);
        Grid.SetRow(placeScroll, 1);
        body.Children.Add(placeScroll);

        var tableScroll = CreateTablesScroller(
            () => { _tablesPage--; RenderTables(); },
            () => { _tablesPage++; RenderTables(); },
            _tablesPage > 0,
            _tablesPage < tablePages - 1);
        tableScroll.HorizontalAlignment = HorizontalAlignment.Right;
        Grid.SetRow(tableScroll, 1);
        Grid.SetColumn(tableScroll, 1);
        body.Children.Add(tableScroll);

        ListHost.Children.Add(body);
        UpdateTablesActions();
    }

    private StackPanel CreateTablesScroller(Action previous, Action next, bool canPrevious, bool canNext)
    {
        var previousButton = CreateScroll("avares://logicpos/Assets/Images/Pos/button_subfamily_article_scroll_left.png", previous);
        var nextButton = CreateScroll("avares://logicpos/Assets/Images/Pos/button_subfamily_article_scroll_right.png", next);
        previousButton.Classes.Add("pos_tables_scroll");
        nextButton.Classes.Add("pos_tables_scroll");
        previousButton.IsEnabled = canPrevious;
        nextButton.IsEnabled = canNext;
        return new StackPanel
        {
            Classes = { "pos_tables_scroller" },
            Children = { previousButton, nextButton }
        };
    }

    private bool MatchesTableFilter(PosTableSlot table)
    {
        if (_tablePlaceId is Guid placeId && table.PlaceId != placeId)
        {
            return false;
        }

        return _tableFilter switch
        {
            "free" => table.State == PosTableState.Free,
            "open" => table.State == PosTableState.Open,
            "reserved" => table.State == PosTableState.Reserved,
            _ => true
        };
    }

    private void OnTablesFilterClick(object? sender, RoutedEventArgs e)
    {
        _tableFilter = (sender as Button)?.Tag as string ?? "all";
        _tablesPage = 0;
        if (sender is Button button && button.Parent is Panel panel)
        {
            foreach (var child in panel.Children.OfType<Button>().Where(item => item.Tag is string))
            {
                child.Classes.Remove("selected");
            }

            button.Classes.Add("selected");
        }

        RenderTables();
    }

    private async void OnTablesOrdersClick(object? sender, RoutedEventArgs e)
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        var orders = await services.GetRequiredService<IPosOrderService>().ListOpenAsync();
        var picked = _tableSlots.FirstOrDefault(item => item.Id.ToString() == _pickedTableKey);
        if (picked is not null)
        {
            orders = orders.Where(order => order.Id == picked.OrderId).ToList();
        }

        OrdersTitle.Text = picked is null ? "Ordens" : $"Ordens :: Ordem {picked.Name}";
        OrdersHost.Children.Clear();
        if (orders.Count == 0)
        {
            OrdersHost.Children.Add(new TextBlock { Classes = { "pos_list_empty" }, Text = "Não existem ordens abertas." });
        }

        foreach (var order in orders)
        {
            var key = order.Id.ToString();
            var button = new Button { Classes = { "pos_list_item" }, Content = order.Label };
            button.Click += async (_, _) =>
            {
                OrdersOverlay.IsVisible = false;
                await OpenOrderAsync(key);
            };
            OrdersHost.Children.Add(button);
        }

        OrdersOverlay.IsVisible = true;
    }

    private void OnOrdersDismissClick(object? sender, RoutedEventArgs e)
    {
        OrdersOverlay.IsVisible = false;
    }

    private const string VoltaTitleBase = "Talão Reembolso Volta";
    private const string VoltaArticleCode = "SDRVDEP";
    private const string VoltaPaymentToken = "OU";
    private PosArticle? _voltaArticle;
    private PosCustomer? _voltaCustomer;

    private async void ShowVoltaRefund()
    {
        _voltaArticle = _catalog.Articles.FirstOrDefault(item =>
            string.Equals(item.Code, VoltaArticleCode, StringComparison.OrdinalIgnoreCase));
        if (_voltaArticle is null)
        {
            ShowQuestion(TicketPrompt.Notice, VoltaTitleBase, "Crie o artigo de depósito SDRVDEP antes de emitir o Talão Reembolso Volta.", question: false, confirmText: "Ok", showCancel: false);
            return;
        }

        var services = AppComposition.Services;
        try
        {
            _voltaCustomer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync();
            var documents = await services.GetRequiredService<IPosDocumentService>().ListSourceDocumentsAsync();
            VoltaOrigin.ItemsSource = documents
                .Where(item => IsVoltaOriginLabel(item.Label))
                .Select(item => new ComboBoxItem { Content = item.Label, Tag = item.Id })
                .Prepend(new ComboBoxItem { Content = string.Empty, Tag = null })
                .ToList();
            VoltaOrigin.SelectedIndex = 0;
        }
        catch (Exception exception)
        {
            ShowQuestion(TicketPrompt.Notice, VoltaTitleBase, exception.Message, question: false, confirmText: "Ok", showCancel: false);
            return;
        }

        VoltaCustomer.Text = _voltaCustomer?.Name ?? string.Empty;
        VoltaCustomerMatches.Children.Clear();
        VoltaQuantity.Text = "1";
        VoltaPaymentMethod.Text = "Outros (OU)";
        VoltaNotice.Text = string.Empty;
        UpdateVoltaTitle();
        VoltaOverlay.IsVisible = true;
        VoltaQuantity.Focus();
    }

    private static bool IsVoltaOriginLabel(string label)
    {
        var text = label.TrimStart();
        return text.StartsWith("FS ", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("FR ", StringComparison.OrdinalIgnoreCase);
    }

    private decimal VoltaNetUnitPrice()
    {
        if (_voltaArticle is null)
        {
            return 0m;
        }

        return _voltaArticle.PriceIncludesVat
            ? _voltaArticle.CatalogPrice / (1m + _voltaArticle.VatPercentage / 100m)
            : _voltaArticle.CatalogPrice;
    }

    private bool TryGetVoltaQuantity(out decimal quantity)
    {
        var text = (VoltaQuantity.Text ?? string.Empty).Trim().Replace(',', '.');
        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out quantity) && quantity > 0;
    }

    private void UpdateVoltaTitle()
    {
        var total = 0m;
        if (_voltaArticle is not null && TryGetVoltaQuantity(out var quantity))
        {
            total = Math.Round(quantity * VoltaNetUnitPrice() * (1m + _voltaArticle.VatPercentage / 100m), 2);
        }

        VoltaTitle.Text = $"{VoltaTitleBase} - Total a reembolsar: {total:N2}€";
    }

    private void OnVoltaQuantityChanged(object? sender, TextChangedEventArgs e)
    {
        if (VoltaTitle is not null)
        {
            UpdateVoltaTitle();
        }
    }

    private void OnVoltaCustomerKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            _ = SearchVoltaCustomerAsync();
        }
    }

    private void OnVoltaCustomerLostFocus(object? sender, RoutedEventArgs e)
    {
        if (string.Equals(VoltaCustomer.Text, _voltaCustomer?.Name, StringComparison.Ordinal) == false)
        {
            _ = SearchVoltaCustomerAsync();
        }
    }

    private async Task SearchVoltaCustomerAsync()
    {
        var text = (VoltaCustomer.Text ?? string.Empty).Trim();
        VoltaCustomerMatches.Children.Clear();
        if (text.Length == 0)
        {
            _voltaCustomer = null;
            return;
        }

        IReadOnlyList<PosCustomer> customers;
        try
        {
            customers = await AppComposition.Services.GetRequiredService<IPosCustomerService>().SearchAsync(text);
        }
        catch (Exception exception)
        {
            VoltaNotice.Text = exception.Message;
            return;
        }

        if (customers.Count == 1)
        {
            SelectVoltaCustomer(customers[0]);
            return;
        }

        _voltaCustomer = null;
        if (customers.Count == 0)
        {
            VoltaNotice.Text = "Cliente não encontrado.";
            return;
        }

        foreach (var customer in customers.Take(4))
        {
            var button = new Button { Classes = { "pos_list_item" }, Content = $"{customer.Name} ({customer.FiscalNumber})" };
            button.Click += (_, _) => SelectVoltaCustomer(customer);
            VoltaCustomerMatches.Children.Add(button);
        }
    }

    private void SelectVoltaCustomer(PosCustomer customer)
    {
        _voltaCustomer = customer;
        VoltaCustomer.Text = customer.Name;
        VoltaCustomerMatches.Children.Clear();
        VoltaNotice.Text = string.Empty;
    }

    private void OnVoltaDismissClick(object? sender, RoutedEventArgs e)
    {
        VoltaOverlay.IsVisible = false;
    }

    private async void OnVoltaConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (_voltaArticle is null)
        {
            VoltaNotice.Text = "Crie o artigo de depósito SDRVDEP antes de emitir o Talão Reembolso Volta.";
            return;
        }

        if (_voltaCustomer is null)
        {
            VoltaNotice.Text = "Selecione o cliente.";
            return;
        }

        if (TryGetVoltaQuantity(out var quantity) == false)
        {
            VoltaNotice.Text = "Indique uma quantidade válida.";
            return;
        }

        var originId = (VoltaOrigin.SelectedItem as ComboBoxItem)?.Tag as Guid?;
        var line = new PosSaleLine(_voltaArticle.Id, quantity, VoltaNetUnitPrice(), 0m, _voltaArticle.VatRateId);
        var header = new PosDocumentHeader { PaymentToken = VoltaPaymentToken, ParentDocumentId = originId };

        VoltaConfirm.IsEnabled = false;
        try
        {
            var result = await AppComposition.Services.GetRequiredService<IPosDocumentService>()
                .IssueDocumentAsync("TRV", _voltaCustomer.Id, [line], header);
            if (result.Succeeded == false)
            {
                VoltaNotice.Text = result.Error ?? "Não foi possível emitir o Talão Reembolso Volta.";
                return;
            }

            VoltaOverlay.IsVisible = false;
            var printError = await FrontOfficePrinting.PrintInvoiceAsync(result.DocumentId);
            var message = printError is null
                ? $"Documento {result.Number} emitido."
                : $"Documento {result.Number} emitido.{Environment.NewLine}Erro ao imprimir: {printError}";
            ShowQuestion(TicketPrompt.Notice, VoltaTitleBase, message, question: false, confirmText: "Ok", showCancel: false);
        }
        catch (Exception exception)
        {
            VoltaNotice.Text = exception.Message;
        }
        finally
        {
            VoltaConfirm.IsEnabled = true;
        }
    }

    private async void OnTablesReserveClick(object? sender, RoutedEventArgs e)
    {
        var services = AppComposition.Services;
        var table = _tableSlots.FirstOrDefault(item => item.Id.ToString() == _pickedTableKey);
        if (services is null || table is null || table.Id == _currentTable?.Id)
        {
            return;
        }

        var orderService = services.GetRequiredService<IPosOrderService>();
        PosDocumentResult result;
        try
        {
            // GTK: free table -> reserve; reserved or orphan open table -> release
            result = table.State == PosTableState.Free
                ? await orderService.ReserveTableAsync(table.Id)
                : await orderService.FreeTableAsync(table.Id);
            if (result.Succeeded)
            {
                _tableSlots = (await orderService.ListTablesAsync()).ToList();
            }
        }
        catch (Exception exception)
        {
            result = PosDocumentResult.Fail(exception.Message);
        }

        RenderTables();
        if (result.Succeeded == false)
        {
            ShowQuestion(TicketPrompt.Notice, "Ordens", result.Error ?? "Não foi possível alterar a reserva.", question: false, confirmText: "Ok", showCancel: false);
        }
    }

    private void UpdateTablesActions()
    {
        var table = _tableSlots.FirstOrDefault(item => item.Id.ToString() == _pickedTableKey);
        TablesReserveButton.IsEnabled = table is not null && table.Id != _currentTable?.Id;
        TablesOkButton.IsEnabled = table is not null && table.State != PosTableState.Reserved;
    }

    private async void OnTablesOkClick(object? sender, RoutedEventArgs e)
    {
        if (_pickedTableKey is null)
        {
            ShowQuestion(TicketPrompt.Notice, "Ordens", "Selecione uma mesa.", question: false, confirmText: "Ok", showCancel: false);
            return;
        }

        var table = _tableSlots.First(item => item.Id.ToString() == _pickedTableKey);
        if (table.State == PosTableState.Reserved)
        {
            ShowQuestion(TicketPrompt.Notice, "Ordens", "Mesa reservada.", question: false, confirmText: "Ok", showCancel: false);
            return;
        }

        if (_ticket.HasLines && table.Id != _currentTable?.Id)
        {
            ShowQuestion(TicketPrompt.Notice, "Aviso", "Finalize ou cancele o ticket atual antes de mudar de mesa.", question: false, confirmText: "Ok", showCancel: false);
            return;
        }

        ListOverlay.IsVisible = false;
        RestoreListChrome();
        if (table.Id != _currentTable?.Id || _ticket.HasLines == false)
        {
            await SetCurrentTableAsync(table);
        }
    }

    private void ShowDocuments()
    {
        DocumentsMenuOverlay.IsVisible = true;
    }

    private void OnDocumentsMenuCloseClick(object? sender, RoutedEventArgs e)
    {
        DocumentsMenuOverlay.IsVisible = false;
    }

    private async void OnDocumentsMenuClick(object? sender, RoutedEventArgs e)
    {
        DocumentsMenuOverlay.IsVisible = false;
        var choice = (sender as Button)?.Tag as string;
        switch (choice)
        {
            case "documents":
                await ShowDocumentsListingAsync();
                return;
            case "receipts-emission":
                await ShowReceiptEmissionAsync();
                return;
            case "receipts":
                await ShowEntityListingAsync("Recibos");
                return;
            case "current-account":
                await ShowEntityListingAsync("Conta.Corr.");
                return;
            case "sessions":
                await ShowEntityListingAsync("Sessões de Trab.");
                return;
            case "stock":
                await ShowStockOverlayAsync();
                return;
            default:
                ShowInDevelopment();
                return;
        }
    }

    private void ShowList(string title, IReadOnlyList<(string Key, string Text)> items, string emptyText)
    {
        RestoreListChrome();
        ListTitle.Text = title;
        if (title == "Ordens")
        {
            ListTitleIcon.IsVisible = true;
            ListTitleIcon.Source = LoadBitmap("avares://logicpos/Assets/Images/Dialogs/icon_window_orders.png");
        }

        ListHost.Children.Clear();
        if (items.Count == 0)
        {
            ListHost.Children.Add(new TextBlock { Classes = { "pos_list_empty" }, Text = emptyText });
        }

        foreach (var item in items)
        {
            var key = item.Key;
            var button = new Button
            {
                Classes = { "pos_list_item" },
                Content = item.Text
            };
            if (title == "Ordens")
            {
                button.Click += async (_, _) => await OpenOrderAsync(key);
            }
            else if (title == "Utilizadores")
            {
                button.Click += (_, _) => BeginUserPin(key, item.Text);
            }

            ListHost.Children.Add(button);
        }

        ListOverlay.IsVisible = true;
    }

    private void ShowTiles(string title, IReadOnlyList<(string Key, string Text, bool Marked)> items, Func<string, Task> onClick)
    {
        RestoreListChrome();
        ListTitle.Text = title;
        ListTitleIcon.IsVisible = true;
        ListTitleIcon.Source = LoadBitmap("avares://logicpos/Assets/Images/Dialogs/icon_window_tables.png");
        ListDismissText.Text = "Fechar";
        ListHost.Children.Clear();
        if (items.Count == 0)
        {
            ListHost.Children.Add(new TextBlock { Classes = { "pos_list_empty" }, Text = "Não existem registos." });
            ListOverlay.IsVisible = true;
            return;
        }

        var wrap = new WrapPanel { Orientation = Orientation.Horizontal };
        foreach (var item in items)
        {
            var key = item.Key;
            var button = new Button { Classes = { "login_user" } };
            if (item.Marked)
            {
                button.Classes.Add("selected");
            }

            var panel = new Panel();
            panel.Children.Add(new Image
            {
                Classes = { "login_user_icon" },
                Source = LoadBitmap(title == "Mesas"
                    ? "avares://logicpos/Assets/Images/BackOffice/icon_other_tables.png"
                    : "avares://logicpos/Assets/Images/icon_user_default.png")
            });
            panel.Children.Add(new Border
            {
                Classes = { "login_user_caption" },
                Child = new TextBlock { Classes = { "login_user_name" }, Text = item.Text }
            });
            button.Content = panel;
            button.Click += async (_, _) => await onClick(key);
            wrap.Children.Add(button);
        }

        ListHost.Children.Add(wrap);
        ListOverlay.IsVisible = true;
    }

    private async Task OpenOrderAsync(string key)
    {
        if (Guid.TryParse(key, out var orderId) == false)
        {
            return;
        }

        if (_ticket.HasLines)
        {
            ListOverlay.IsVisible = false;
            ShowQuestion(TicketPrompt.Notice, "Aviso", "Finalize ou cancele o ticket atual antes de abrir uma ordem.", question: false, confirmText: "Ok", showCancel: false);
            return;
        }

        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        ListOverlay.IsVisible = false;
        RestoreListChrome();
        var tables = await services.GetRequiredService<IPosOrderService>().ListTablesAsync();
        var table = tables.FirstOrDefault(item => item.OrderId == orderId)
            ?? new PosTableSlot(_currentTable?.Id ?? Guid.Empty, _currentTable?.Name ?? string.Empty, PosTableState.Open, orderId, _currentTable?.PlaceId ?? Guid.Empty, _currentTable?.PlaceName ?? string.Empty);
        await SetCurrentTableAsync(table);
    }

    private void OnListDismissClick(object? sender, RoutedEventArgs e)
    {
        ListOverlay.IsVisible = false;
    }

    private void RequestDeleteTicket()
    {
        if (SelectedOrderLine is { } orderLine)
        {
            ShowQuestion(
                TicketPrompt.DeleteOrderLine,
                "Aviso",
                $"Tem a certeza que deseja remover \"{orderLine.Designation}\" da ordem?",
                question: true,
                confirmText: "Sim",
                showCancel: true);
            return;
        }

        if (_ticket.HasLines == false)
        {
            return;
        }

        ShowQuestion(
            TicketPrompt.Delete,
            "Aviso",
            "Tem a certeza que deseja cancelar o pedido atual?",
            question: true,
            confirmText: "Sim",
            showCancel: true);
    }

    private void RequestQuantity()
    {
        var line = _orderListMode ? SelectedOrderLine : _ticket.Selected;
        if (line is null)
        {
            return;
        }

        ShowKeypad(TicketPrompt.Quantity, "Quantidade", line.Quantity);
    }

    private void RequestWeight()
    {
        var line = _ticket.Selected;
        if (line is null)
        {
            return;
        }

        ShowKeypad(TicketPrompt.Weight, "Peso", line.Quantity);
    }

    private void RequestPrice()
    {
        var line = _orderListMode ? SelectedOrderLine : _ticket.Selected;
        if (line is null)
        {
            return;
        }

        ShowKeypad(TicketPrompt.Price, "Preço do Produto", line.DisplayUnitPrice);
    }

    private void RequestCard()
    {
        _prompt = TicketPrompt.Card;
        TicketBarcode.Text = string.Empty;
        ShowDialogChrome("Cartão de Cliente", question: true, confirmText: "Ok", showCancel: true);
        TicketQuestionBody.IsVisible = false;
        TicketKeypad.IsVisible = false;
        TicketBarcode.IsVisible = true;
        TicketBarcode.Focus();
    }

    private async Task<(bool Found, string Text)> LookupCardAsync(string? card)
    {
        if (string.IsNullOrWhiteSpace(card))
        {
            return (false, "Indique o número do cartão.");
        }

        var services = AppComposition.Services;
        if (services is null)
        {
            return (false, "A aplicação não está pronta.");
        }

        var customer = await services.GetRequiredService<IPosCustomerService>().FindByCardAsync(card.Trim());
        if (customer is null)
        {
            return (false, "Cartão não encontrado.");
        }

        _saleCustomer = customer;
        return (true, customer.Name);
    }

    private async void ShowChangeUser()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        var users = await services.GetRequiredService<ILoginService>().GetUsersAsync();
        ShowUserButtons(users.Select(user => (user.Id.ToString(), user.Name)).ToList(), key =>
        {
            var user = users.First(item => item.Id.ToString() == key);
            BeginUserPin(key, user.Name);
        });
    }

    private void ShowUserButtons(IReadOnlyList<(string Key, string Name)> users, Action<string> onClick)
    {
        RestoreListChrome();
        ListTitle.Text = "Mudar de utilizador";
        ListTitleIcon.IsVisible = true;
        ListTitleIcon.Source = LoadBitmap("avares://logicpos/Assets/Images/Pos/icon_pos_toolbar_show_change_user_dialog.png");
        ListDismissText.Text = "Cancelar";
        ListHost.Children.Clear();
        var wrap = new WrapPanel { Orientation = Orientation.Horizontal };
        foreach (var user in users)
        {
            var key = user.Key;
            var button = new Button
            {
                Classes = { "pos_list_item" },
                Content = user.Name,
                Width = 124,
                MinHeight = 72,
                Margin = new Avalonia.Thickness(6),
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            button.Click += (_, _) => onClick(key);
            wrap.Children.Add(button);
        }

        ListHost.Children.Add(wrap);
        ListOverlay.IsVisible = true;
    }

    private void BeginUserPin(string key, string name)
    {
        if (Guid.TryParse(key, out var userId) == false)
        {
            return;
        }

        _pendingUserId = userId;
        _pendingUserName = name;
        ListOverlay.IsVisible = false;
        ShowKeypad(TicketPrompt.ChangeUser, name, 0);
        if (_keypadValue is not null)
        {
            _keypadValue.Text = string.Empty;
        }

        _replaceKeypad = true;
    }

    private async Task<string?> ApplyUserPinAsync()
    {
        if (_pendingUserId is not Guid userId)
        {
            return "Selecione um utilizador.";
        }

        var pin = (_keypadValue?.Text ?? string.Empty).Replace(",", string.Empty).Trim();
        var services = AppComposition.Services;
        if (services is null)
        {
            return "A aplicação não está pronta.";
        }

        var result = await services.GetRequiredService<ILoginService>().SignInAsync(userId, pin);
        if (result.Succeeded == false)
        {
            return result.Message;
        }

        _userName = _pendingUserName;
        if (_terminalLabel is not null)
        {
            _terminalLabel.Text = string.IsNullOrWhiteSpace(_terminalName) ? _userName : $"{_terminalName} : {_userName}";
        }

        return null;
    }

    private void ShowSession()
    {
        SessionOverlay.IsVisible = true;
        SessionNotice.Text = string.Empty;
        SessionHome.IsVisible = true;
        SessionCash.IsVisible = false;
        SessionTitle.Text = "Sessão de Trabalho";
        SessionFrame.Width = 428;
        SessionFrame.Height = 165;
        _sessionAction = null;
    }

    private async void OnDaySessionClick(object? sender, RoutedEventArgs e)
    {
        if (_dayOpen)
        {
            // Close day
            var result = await RunSessionAsync("close-day", null);
            if (result.Succeeded)
            {
                SessionOverlay.IsVisible = false;
            }

            var message = result.Error ?? result.Message ?? string.Empty;
            if (string.IsNullOrWhiteSpace(message) == false)
            {
                ShowQuestion(TicketPrompt.Notice, "Sessão de Trabalho", message, question: false, confirmText: "Ok", showCancel: false);
            }
        }
        else
        {
            // Only open the day; then show cash drawer so user can choose opening amount
            var result = await RunSessionAsync("open-day", null);
            if (result.Succeeded)
            {
                OnShowCashDrawerClick(sender, e);
            }
            else
            {
                var message = result.Error ?? result.Message ?? string.Empty;
                if (string.IsNullOrWhiteSpace(message) == false)
                {
                    ShowQuestion(TicketPrompt.Notice, "Sessão de Trabalho", message, question: false, confirmText: "Ok", showCancel: false);
                }
            }
        }
    }

    private async void OnShowCashDrawerClick(object? sender, RoutedEventArgs e)
    {
        _cashTotal = await LoadCashTotalAsync();
        var totalText = _cashTotal.ToString("0.00", CultureInfo.CurrentCulture);

        SessionHome.IsVisible = false;
        SessionCash.IsVisible = true;
        SessionTitle.Text = $"Caixa :: Total em Caixa: {totalText} €";
        SessionFrame.Width = 680;
        SessionFrame.Height = 360;
        SessionAmount.IsVisible = true;
        SessionConfirm.IsVisible = true;
        SessionNotice.Text = string.Empty;

        // GTK: button enable state and initial selection depend on terminal session state
        if (_terminalOpen)
        {
            // Terminal open: can close, cash-in, cash-out. Cannot open again.
            SessionOpen.IsEnabled = false;
            SessionClose.IsEnabled = true;
            SessionIn.IsEnabled = true;
            SessionOut.IsEnabled = true;
            ActivateCashButton(SessionClose, "close-session");
        }
        else
        {
            // Terminal closed: only open is available
            SessionOpen.IsEnabled = true;
            SessionClose.IsEnabled = false;
            SessionIn.IsEnabled = false;
            SessionOut.IsEnabled = false;
            ActivateCashButton(SessionOpen, "open-session");
        }
    }

    /// <summary>
    /// Select a cash drawer button and configure fields to match GTK behaviour.
    /// Clears values when switching between buttons (GTK ActivateButton).
    /// </summary>
    private void ActivateCashButton(Button target, string action)
    {
        // Unmark all and mark the new selection
        foreach (var button in new[] { SessionOpen, SessionClose, SessionIn, SessionOut })
        {
            button.Classes.Remove("selected");
        }

        target.Classes.Add("selected");
        _sessionAction = action;
        SessionNotice.Text = string.Empty;

        var totalText = _cashTotal.ToString("0.00", CultureInfo.CurrentCulture);
        if (action == "open-session")
        {
            // GTK: amount field shows current total, read-only, description not required
            SessionAmount.Text = totalText;
            SessionAmount.IsReadOnly = true;
            SessionNote.Text = string.Empty;
            SessionDescLabel.Text = "Descrição";
        }
        else if (action == "close-session")
        {
            // GTK: closing starts from the drawer total and lets the user adjust it
            SessionAmount.Text = totalText;
            SessionAmount.IsReadOnly = false;
            SessionNote.Text = string.Empty;
            SessionDescLabel.Text = "Descrição";
            SessionAmount.Focus();
        }
        else
        {
            // cash-in / cash-out: amount editable, description is required (GTK)
            SessionAmount.Text = string.Empty;
            SessionAmount.IsReadOnly = false;
            SessionNote.Text = string.Empty;
            SessionDescLabel.Text = "Descrição *";
            SessionAmount.Focus();
        }

        ValidateCashDialog();
    }

    /// <summary>
    /// Enable/disable Ok based on current field state, matching GTK ValidateDialog.
    /// </summary>
    private void ValidateCashDialog()
    {
        if (_sessionAction is null)
        {
            SessionConfirm.IsEnabled = false;
            return;
        }

        // Open allows zero, so we only need a selection
        if (_sessionAction == "open-session")
        {
            SessionConfirm.IsEnabled = true;
            return;
        }

        // Parse the amount
        var hasAmount = decimal.TryParse(SessionAmount.Text,
            System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.CurrentCulture, out var amount)
            && amount >= 0;

        if (_sessionAction == "close-session")
        {
            // Close needs a valid amount >= 0
            SessionConfirm.IsEnabled = hasAmount;
            return;
        }

        // cash-in / cash-out: amount > 0 and description required (GTK)
        var hasDesc = string.IsNullOrWhiteSpace(SessionNote.Text) == false;
        SessionConfirm.IsEnabled = hasAmount && amount > 0 && hasDesc;
    }

    private void OnOpenSessionClick(object? sender, RoutedEventArgs e)
    {
        ActivateCashButton(SessionOpen, "open-session");
    }

    private void OnCloseSessionClick(object? sender, RoutedEventArgs e)
    {
        ActivateCashButton(SessionClose, "close-session");
    }

    private void OnCashInClick(object? sender, RoutedEventArgs e)
    {
        ActivateCashButton(SessionIn, "cash-in");
    }

    private void OnCashOutClick(object? sender, RoutedEventArgs e)
    {
        ActivateCashButton(SessionOut, "cash-out");
    }

    private async void OnSessionAmountClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_sessionAction))
        {
            SessionNotice.Text = "Selecione o movimento.";
            return;
        }

        var action = _sessionAction;
        var payload = action is "cash-in" or "cash-out"
            ? $"{SessionAmount.Text}|{SessionNote.Text}"
            : SessionAmount.Text;

        var result = await RunSessionAsync(action, payload);
        if (result.Succeeded)
        {
            SessionOverlay.IsVisible = false;
        }
    }

    private void OnSessionPrintClick(object? sender, RoutedEventArgs e)
    {
        ShowQuestion(TicketPrompt.Notice, "Caixa", "Impressão do movimento de caixa ainda não está disponível.", question: false, confirmText: "Ok", showCancel: false);
    }

    private void OnSessionCloseClick(object? sender, RoutedEventArgs e) => SessionOverlay.IsVisible = false;

    private void OnSessionFieldChanged(object? sender, TextChangedEventArgs e)
    {
        ValidateCashDialog();
    }

    private async Task<decimal> LoadCashTotalAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return 0;
        }

        var result = await services.GetRequiredService<IBackOfficeListingService>().RunActionAsync("cash-total", null, null);
        if (result.Succeeded
            && decimal.TryParse(result.Message, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
        {
            return amount;
        }

        return 0;
    }

    private async Task<ListingSaveResult> RunSessionAsync(string action, string? amount)
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return ListingSaveResult.Fail("A aplicação não está pronta.");
        }

        var result = await services.GetRequiredService<IBackOfficeListingService>().RunActionAsync(action, null, amount);
        SessionNotice.Text = result.Error ?? result.Message ?? string.Empty;
        await RefreshWorkSessionAsync();
        return result;
    }

    private void RequestBarcode()
    {
        _prompt = TicketPrompt.Barcode;
        TicketBarcode.Text = string.Empty;
        ShowDialogChrome("Código Barras / Código Artigo", question: true, confirmText: "Ok", showCancel: true);
        TicketQuestionBody.IsVisible = false;
        TicketKeypad.IsVisible = false;
        TicketBarcode.IsVisible = true;
        TicketBarcode.Focus();
    }

    private async void RequestPayments()
    {
        if (_ticket.HasLines == false && _orderLines.Count == 0)
        {
            return;
        }

        // GTK: an open ticket is finished into the table order before paying the order
        if (_ticket.HasLines && await FinishTicketAsync() == false)
        {
            return;
        }

        if (_orderLines.Count == 0)
        {
            return;
        }

        var payment = new PaymentWindow(_orderLines.ToList(), _saleCustomer);
        var paid = await payment.ShowDialog<bool>(this);
        if (paid == false)
        {
            return;
        }

        if (payment.PartialPayment)
        {
            foreach (var index in payment.PaidLineIndexes.OrderByDescending(item => item))
            {
                if (index >= 0 && index < _orderLines.Count)
                {
                    _orderLines.RemoveAt(index);
                }
            }

            RefreshTicket();
            return;
        }

        if (_openOrderId is Guid orderId)
        {
            var orderService = AppComposition.Services?.GetService<IPosOrderService>();
            if (orderService is not null)
            {
                try
                {
                    await orderService.CloseAsync(orderId);
                }
                catch (Exception exception)
                {
                    _saleCustomer = null;
                    await ReloadCurrentTableAsync();
                    ShowQuestion(TicketPrompt.Notice, "Ordens", $"O pagamento foi registado, mas não foi possível fechar a ordem: {exception.Message}", question: false, confirmText: "Ok", showCancel: false);
                    return;
                }
            }
        }

        _saleCustomer = null;
        await ReloadCurrentTableAsync();
    }

    private void ShowInDevelopment()
    {
        ShowQuestion(
            TicketPrompt.Notice,
            "Desenvolvimento",
            "Esta funcionalidade está em Desenvolvimento",
            question: false,
            confirmText: "Ok",
            showCancel: false);
    }

    private void ShowQuestion(TicketPrompt prompt, string title, string message, bool question, string confirmText, bool showCancel)
    {
        _prompt = prompt;
        ShowDialogChrome(title, question, confirmText, showCancel);
        TicketDialogMessage.Text = message;
        TicketQuestionBody.IsVisible = true;
        TicketKeypad.IsVisible = false;
        PricePad.IsVisible = false;
        TicketBarcode.IsVisible = false;
    }

    private void ShowKeypad(TicketPrompt prompt, string title, decimal value)
    {
        _prompt = prompt;
        _replaceKeypad = true;
        _keypadValue!.Text = value.ToString("0.00", CultureInfo.CurrentCulture);
        ShowDialogChrome(title, question: true, confirmText: "Ok", showCancel: true);
        TicketQuestionBody.IsVisible = false;
        TicketBarcode.IsVisible = false;
        TicketDialogCancelText.Text = "Não";

        if (prompt == TicketPrompt.Price)
        {
            _priceTyping = false;
            _priceMoney = 0;
            PriceEntry.Text = value > 0 ? value.ToString("0.00", CultureInfo.CurrentCulture) : string.Empty;
            TicketKeypad.IsVisible = false;
            PricePad.IsVisible = true;
            TicketDialogCancelText.Text = "Cancelar";
            TicketDialogIcon.Source = LoadBitmap("avares://logicpos/Assets/Images/Pos/Payments/icon_window_payments.png");
            TicketFrame.Width = 540;
            TicketFrame.Height = 530;
            return;
        }

        PricePad.IsVisible = false;
        TicketKeypad.IsVisible = true;
        TicketFrame.Width = 420;
        TicketFrame.Height = 390;
    }

    private void ShowDialogChrome(string title, bool question, string confirmText, bool showCancel)
    {
        TicketFrame.Width = 400;
        TicketFrame.Height = 300;
        TicketDialogTitle.Text = title;
        TicketDialogConfirmText.Text = confirmText;
        TicketDialogCancel.IsVisible = showCancel;
        var icon = question ? "question" : "info";
        TicketDialogIcon.Source = LoadBitmap($"avares://logicpos/Assets/Images/Dialogs/icon_pos_dialog_{icon}_window.png");
        TicketDialogSymbol.Source = LoadBitmap($"avares://logicpos/Assets/Images/Dialogs/icon_pos_dialog_{icon}.png");
        TicketDialogConfirmIcon.Source = LoadBitmap(showCancel
            ? "avares://logicpos/Assets/Images/Dialogs/icon_pos_dialog_action_yes.png"
            : "avares://logicpos/Assets/Images/Dialogs/icon_pos_dialog_action_ok.png");
        TicketDialogActions.IsVisible = true;
        TicketOverlay.IsVisible = true;
    }

    private void BuildKeypad()
    {
        _keypadValue = new TextBlock
        {
            Classes = { "pos_ticket_cell" },
            FontSize = 16,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 4, 0, 2)
        };
        TicketKeypad.Children.Add(_keypadValue);

        var pad = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("72,*,72"),
            RowDefinitions = new RowDefinitions("*,*,*,*,*"),
            Height = 250
        };

        AddQuantityShortcut(pad, "1", 0, 0);
        AddQuantityShortcut(pad, "2", 1, 0);
        AddQuantityShortcut(pad, "3", 2, 0);
        AddQuantityShortcut(pad, "4", 3, 0);
        AddQuantityShortcut(pad, "5", 4, 0);
        AddQuantityShortcut(pad, "6", 0, 2);
        AddQuantityShortcut(pad, "7", 1, 2);
        AddQuantityShortcut(pad, "8", 2, 2);
        AddQuantityShortcut(pad, "9", 3, 2);
        AddQuantityShortcut(pad, "10", 4, 2);

        var keys = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*"),
            RowDefinitions = new RowDefinitions("*,*,*,*"),
            Margin = new Avalonia.Thickness(4, 0)
        };
        Grid.SetRowSpan(keys, 5);
        Grid.SetColumn(keys, 1);
        AddQuantityDigit(keys, "7", 0, 0);
        AddQuantityDigit(keys, "8", 0, 1);
        AddQuantityDigit(keys, "9", 0, 2);
        AddQuantityDigit(keys, "4", 1, 0);
        AddQuantityDigit(keys, "5", 1, 1);
        AddQuantityDigit(keys, "6", 1, 2);
        AddQuantityDigit(keys, "1", 2, 0);
        AddQuantityDigit(keys, "2", 2, 1);
        AddQuantityDigit(keys, "3", 2, 2);
        AddQuantityDigit(keys, "0", 3, 0);
        AddQuantityDigit(keys, ",", 3, 1);

        var clear = new Button
        {
            Classes = { "login_pin_key", "login_pin_ce" },
            Content = "CE",
            Width = double.NaN,
            Height = double.NaN,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Avalonia.Thickness(2)
        };
        clear.Click += (_, _) =>
        {
            if (_keypadValue is not null)
            {
                _keypadValue.Text = "0";
            }

            _replaceKeypad = true;
        };
        Grid.SetRow(clear, 3);
        Grid.SetColumn(clear, 2);
        keys.Children.Add(clear);
        pad.Children.Add(keys);
        TicketKeypad.Children.Add(pad);
    }

    private void AddQuantityShortcut(Grid pad, string caption, int row, int column)
    {
        var button = new Button
        {
            Classes = { "pos_price_money" },
            Content = caption
        };
        button.Click += (_, _) =>
        {
            if (decimal.TryParse(caption, NumberStyles.Number, CultureInfo.InvariantCulture, out var quantity) == false)
            {
                return;
            }

            _ticket.SetSelectedQuantity(quantity);
            RefreshTicket();
            CloseTicketDialog();
        };
        Grid.SetRow(button, row);
        Grid.SetColumn(button, column);
        pad.Children.Add(button);
    }

    private void AddQuantityDigit(Grid keys, string caption, int row, int column)
    {
        var button = new Button
        {
            Classes = { "pos_price_money" },
            Content = caption
        };
        button.Click += (_, _) => AppendKey(caption);
        Grid.SetRow(button, row);
        Grid.SetColumn(button, column);
        keys.Children.Add(button);
    }

    private void AppendKey(string caption)
    {
        if (_keypadValue is null)
        {
            return;
        }

        if (_replaceKeypad)
        {
            _keypadValue.Text = caption == "," ? "0," : caption;
            _replaceKeypad = false;
            return;
        }

        var current = _keypadValue.Text ?? string.Empty;
        if (caption == "," && current.Contains(','))
        {
            return;
        }

        _keypadValue.Text = current == "0" && caption != "," ? caption : current + caption;
    }

    private async void OnTicketDialogConfirmClick(object? sender, RoutedEventArgs e)
    {
        switch (_prompt)
        {
            case TicketPrompt.Delete:
                _ticket.Clear();
                _orderListMode = true;
                RefreshTicket();
                break;
            case TicketPrompt.DeleteOrderLine:
                if (SelectedOrderLine is { } removedLine)
                {
                    CloseTicketDialog();
                    await ChangeOrderLineAsync(removedLine, 0m, removedLine.NetUnitPrice);
                    return;
                }

                break;
            case TicketPrompt.Quantity:
            case TicketPrompt.Weight:
                if (TryReadKeypad(out var quantity))
                {
                    if (SelectedOrderLine is { } quantityLine)
                    {
                        CloseTicketDialog();
                        await ChangeOrderLineAsync(quantityLine, quantity, quantityLine.NetUnitPrice);
                        return;
                    }

                    _ticket.SetSelectedQuantity(quantity);
                    RefreshTicket();
                }

                break;
            case TicketPrompt.Price:
                if (TryReadKeypad(out var price))
                {
                    if (SelectedOrderLine is { } priceLine)
                    {
                        CloseTicketDialog();
                        await ChangeOrderLineAsync(
                            priceLine,
                            priceLine.Quantity,
                            PosTicketLine.ToNetUnitPrice(price, priceLine.VatPercentage, priceLine.PriceIncludesVat));
                        return;
                    }

                    _ticket.SetSelectedDisplayPrice(price);
                    RefreshTicket();
                }

                break;
            case TicketPrompt.Barcode:
                if (TryAddArticleByCode(TicketBarcode.Text) == false)
                {
                    return;
                }

                break;
            case TicketPrompt.Card:
                var cardMessage = await LookupCardAsync(TicketBarcode.Text);
                CloseTicketDialog();
                ShowQuestion(TicketPrompt.Notice, cardMessage.Found ? "Cliente" : "Aviso", cardMessage.Text, question: false, confirmText: "Ok", showCancel: false);
                return;
            case TicketPrompt.ChangeUser:
                var userMessage = await ApplyUserPinAsync();
                CloseTicketDialog();
                if (userMessage is not null)
                {
                    ShowQuestion(TicketPrompt.Notice, "Aviso", userMessage, question: false, confirmText: "Ok", showCancel: false);
                }

                return;
        }

        CloseTicketDialog();
    }

    private void OnTicketDialogDismissClick(object? sender, RoutedEventArgs e)
    {
        CloseTicketDialog();
    }

    private void CloseTicketDialog()
    {
        _prompt = TicketPrompt.None;
        TicketOverlay.IsVisible = false;
        TicketKeypad.IsVisible = false;
        PricePad.IsVisible = false;
        TicketBarcode.IsVisible = false;
        TicketQuestionBody.IsVisible = true;
        TicketDialogCancelText.Text = "Não";
        TicketFrame.Height = 300;
    }

    private void OnPriceMoneyClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string tag)
        {
            return;
        }

        if (decimal.TryParse(tag, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount) == false)
        {
            return;
        }

        if (_priceTyping)
        {
            _priceMoney = 0;
            _priceTyping = false;
        }

        _priceMoney += amount;
        PriceEntry.Text = _priceMoney.ToString("0.00", CultureInfo.CurrentCulture);
    }

    private void OnPriceDigitClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var caption = button.Tag as string ?? button.Content as string ?? string.Empty;
        if (_priceTyping == false)
        {
            PriceEntry.Text = caption == "," ? "0," : caption;
            _priceTyping = true;
            return;
        }

        var current = PriceEntry.Text ?? string.Empty;
        if (caption == "," && current.Contains(','))
        {
            return;
        }

        PriceEntry.Text = current.Length == 0 && caption != "," ? caption : current + caption;
    }

    private void OnPriceClearClick(object? sender, RoutedEventArgs e)
    {
        var current = PriceEntry.Text ?? string.Empty;
        PriceEntry.Text = current.Length <= 1 ? string.Empty : current[..^1];
        _priceTyping = true;
    }

    private bool TryReadKeypad(out decimal value)
    {
        var text = _prompt == TicketPrompt.Price ? PriceEntry.Text : _keypadValue?.Text;
        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
    }

    private bool TryAddArticleByCode(string? code)
    {
        var text = code?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return true;
        }

        var article = _catalog.Articles.FirstOrDefault(item =>
            string.Equals(item.Code, text, StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.Barcode, text, StringComparison.OrdinalIgnoreCase));

        if (article is null)
        {
            ShowQuestion(TicketPrompt.Notice, "Aviso", "Artigo não encontrado", question: false, confirmText: "Ok", showCancel: false);
            return false;
        }

        AddArticle(article);
        return true;
    }

    private void RefreshTicket()
    {
        if (_ticketRows is null || _ticketLines is null)
        {
            ApplyFrontOfficeRules();
            return;
        }

        _ticketRows.Children.Clear();
        if (_orderListMode)
        {
            _ticketLines.Classes.Add("order");
        }
        else
        {
            _ticketLines.Classes.Remove("order");
        }

        Border? selectedRow = null;
        var visibleLines = _orderListMode ? (IReadOnlyList<PosTicketLine>)_orderLines : _ticket.Lines;
        for (var index = 0; index < visibleLines.Count; index++)
        {
            var line = visibleLines[index];
            var rowIndex = index;
            var row = new Border { Classes = { "pos_ticket_row" } };
            if (index == (_orderListMode ? _orderSelectedIndex : _ticket.SelectedIndex))
            {
                row.Classes.Add("selected");
                selectedRow = row;
            }

            var columns = new Grid();
            columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(_layout.DesignationColumnWidth)));
            columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(_layout.PriceColumnWidth)));
            columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(_layout.QuantityColumnWidth)));
            columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(_layout.TotalColumnWidth)));
            AddTicketCell(columns, line.Designation, 0, false);
            AddTicketCell(columns, line.DisplayUnitPrice.ToString("0.00"), 1, true);
            AddTicketCell(columns, line.Quantity.ToString("0.00"), 2, true);
            AddTicketCell(columns, line.Total.ToString("0.00"), 3, true);
            row.Child = columns;
            row.PointerPressed += (_, args) =>
            {
                if (_orderListMode)
                {
                    _orderSelectedIndex = rowIndex;
                }
                else
                {
                    _ticket.Select(rowIndex);
                }

                RefreshTicket();
                args.Handled = true;
            };

            _ticketRows.Children.Add(row);
        }

        var orderTotal = _orderLines.Sum(line => line.Total);
        if (_ticketTotal is not null)
        {
            _ticketTotal.Text = (_orderListMode ? orderTotal : _ticket.Total).ToString("C");
        }

        if (_amountDue is not null)
        {
            var ticketCount = _orderTicketCount + (_ticket.HasLines ? 1 : 0);
            _amountDue.Text = $"{orderTotal + _ticket.Total:0.00} : #{ticketCount}";
        }

        selectedRow?.BringIntoView();
        ApplyFrontOfficeRules();
    }

    private void AddTicketCell(Grid grid, string text, int column, bool right)
    {
        var block = new TextBlock
        {
            Classes = { right ? "pos_ticket_cell_right" : "pos_ticket_cell" },
            Text = text,
            FontSize = _layout.TicketFontSize
        };
        Grid.SetColumn(block, column);
        grid.Children.Add(block);
    }
}
