using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class EntityListing : UserControl
{
    private readonly List<(string Key, Control Input, string Kind)> _inputs = new();
    private string _title = string.Empty;
    private string? _mode;
    private Guid? _editingId;
    private string? _pendingAction;
    private bool _fillingFilters;
    private bool _filtersRequested;
    private Guid? _appliedCustomer;
    private string _importAction = string.Empty;
    private string _importFileName = string.Empty;
    private ExcelPreview.Sheet? _importSheet;
    private readonly ObservableCollection<ImportLine> _importRows = new();
    private bool _loadingModes;
    private bool _canEdit;
    private bool _updatingColumnChecks;
    private bool _vatBusy;
    private bool? _vatAutocomplete;
    private string? _notice;
    private string _columnSignature = string.Empty;
    private readonly List<ListingRow> _rows = new();
    private ListingLoad _load = null!;
    private readonly Dictionary<string, string> _columnKeys = new(StringComparer.Ordinal);
    private readonly Dictionary<string, bool> _defaultVisible = new(StringComparer.Ordinal);
    private readonly List<(DataGridColumn Column, bool Visible)> _columnSnapshot = new();
    private static readonly string[] DateKeys = ["Date", "StartDate", "ReadingDate", "CreatedAt", "UpdatedAt", "EndDate"];
    private readonly List<(Button Header, Control Page, string? ShowWhen)> _articleTabs = new();
    private ContentControl? _articlePages;

    public EntityListing()
    {
        InitializeComponent();
        ListingFilters.EnableCustomerSearch(CustomerBox);
        _load = new ListingLoad(Busy);
        ListingNotice.HideWhenEmpty(Notice);
        FiscalYearHost.Finished += OnFiscalYearFinished;
    }

    public event EventHandler? FiscalYearCompleted;

    private async void OnFiscalYearFinished(object? sender, string? message)
    {
        FiscalYearOverlay.IsVisible = false;
        await ReloadAsync();
        Notice.Text = message ?? string.Empty;
        FiscalYearCompleted?.Invoke(this, EventArgs.Empty);
    }

    public async Task ShowAsync(string title)
    {
        _title = title;
        TitleText.Text = title;
        SearchBox.Text = string.Empty;
        Notice.Text = string.Empty;
        ResetDates();
        DateFilters.IsVisible = UsesDateFilter(title);
        CustomerFilterField.IsVisible = title == "Recibos";
        if (title == "Recibos")
        {
            ResetCustomerFilter();
            EnsureCustomerFilter();
        }

        CatalogImportButton.IsVisible = title is "Artigos" or "Clientes";
        await ReloadAsync();
    }

    private static bool UsesDateFilter(string title)
        => title is "Recibos" or "Emissão Recibos" or "Sessões de Trab." or "Gestão de Stocks" or "Conta.Corr." or "Registro de alterações (Changelog)" or "Séries" or "SAF-T período";

    private IBackOfficeListingService? Service => AppComposition.Services?.GetService<IBackOfficeListingService>();

    private void OnSearchClick(object? sender, RoutedEventArgs e) => ApplyFilter();

    private void OnSearchChanged(object? sender, TextChangedEventArgs e) => ApplyFilter();

    private async void OnClearFilterClick(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        ResetDates();
        if (_title == "Recibos")
        {
            ResetCustomerFilter();
            await ReloadAsync();
            return;
        }

        ApplyFilter();
    }

    private async void OnResetClick(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        ResetDates();
        ResetCustomerFilter();
        RestoreColumns();
        await ReloadAsync();
    }

    private void OnFilterClick(object? sender, RoutedEventArgs e) => ApplyFilter();

    private void ResetDates()
    {
        StartDate.SelectedDate = new DateTime(DateTime.Today.Year, 1, 1);
        EndDate.SelectedDate = DateTime.Today;
    }

    private void OnCustomerFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_fillingFilters || _title != "Recibos" || CustomerFilterField.IsVisible == false)
        {
            return;
        }

        var next = ListingFilters.CustomerId(CustomerBox);
        if (next == _appliedCustomer)
        {
            return;
        }

        _appliedCustomer = next;
        _ = ReloadAsync();
    }

    private void EnsureCustomerFilter()
    {
        if (_filtersRequested)
        {
            return;
        }

        _filtersRequested = true;
        _ = LoadCustomerFilterAsync();
    }

    private async Task LoadCustomerFilterAsync()
    {
        try
        {
            var customers = await ListingFilters.CustomersAsync();
            _fillingFilters = true;
            CustomerBox.ItemsSource = customers;
            ListingFilters.SelectEveryone(CustomerBox);
            _appliedCustomer = null;
        }
        catch (Exception exception)
        {
            Notice.Text = exception.Message;
        }
        finally
        {
            _fillingFilters = false;
        }
    }

    private void ResetCustomerFilter()
    {
        _fillingFilters = true;
        ListingFilters.SelectEveryone(CustomerBox);
        _appliedCustomer = null;
        _fillingFilters = false;
    }

    private async void OnExportExcelClick(object? sender, RoutedEventArgs e)
    {
        if (_title == "Artigos")
        {
            await ExecuteAsync("export-articles", null, null);
            return;
        }

        if (_title == "Clientes")
        {
            await ExecuteAsync("export-customers", null, null);
            return;
        }

        await ExportAsync(true);
    }

    private async void OnCatalogImportClick(object? sender, RoutedEventArgs e) =>
        await ImportAsync(_title == "Clientes" ? "import-customers" : "import-articles");

    private async void OnExportPdfClick(object? sender, RoutedEventArgs e) => await ExportAsync(false);

    private async void OnModeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loadingModes)
        {
            return;
        }

        _mode = ModeBox.SelectedItem as string;
        await ReloadAsync();
    }

    private async void OnGridDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Visual visual && visual.FindAncestorOfType<Button>() is not null)
        {
            return;
        }

        if (Grid.SelectedItem is not ListingRow row)
        {
            return;
        }

        if (ShowsDocument)
        {
            await OpenListedDocumentAsync(row);
            return;
        }

        if (_canEdit)
        {
            await EditAsync(row.Id);
        }
    }

    private bool ShowsDocument => _title == "Recibos";

    private async Task OpenListedDocumentAsync(ListingRow row)
    {
        var service = Service;
        if (service is null || row.Id == Guid.Empty)
        {
            Notice.Text = "Selecione o documento.";
            return;
        }

        var result = await service.RunActionAsync("view-receipt", row.Id, null);
        if (result.Succeeded && string.IsNullOrWhiteSpace(result.Message) == false && File.Exists(result.Message))
        {
            Notice.Text = string.Empty;
            if (TopLevel.GetTopLevel(this) is IOfficeSurface office)
            {
                var title = string.IsNullOrWhiteSpace(row["Number"]) ? row["RefNo"] : row["Number"];
                await office.ShowPdfAsync(result.Message, title);
            }

            return;
        }

        Notice.Text = result.Error ?? "Não foi possível abrir o documento.";
    }

    private Task ReloadAsync() => _load.RunAsync(ReloadCoreAsync);

    private async Task ReloadCoreAsync()
    {
        var service = Service;
        if (service is null)
        {
            Notice.Text = "A aplicação ainda não está ligada à base de dados.";
            return;
        }

        try
        {
            var snapshot = await service.QueryAsync(_title, null, _mode, customerId: _title == "Recibos" ? ListingFilters.CustomerId(CustomerBox) : null);
            _canEdit = snapshot.CanEdit;
            _notice = snapshot.Notice;
            TitleText.Text = snapshot.Title;
            _rows.Clear();
            _rows.AddRange(snapshot.Rows);
            BindModes(snapshot.Modes);
            BindActions(snapshot);
            BindColumns(snapshot);
            ApplyFilter();
        }
        catch (Exception exception)
        {
            Notice.Text = exception.Message;
        }
    }

    private void BindModes(IReadOnlyList<string> modes)
    {
        var visible = modes.Count > 0;
        ModeBox.IsVisible = visible;
        if (visible == false)
        {
            return;
        }

        _loadingModes = true;
        ModeBox.ItemsSource = modes;
        ModeBox.SelectedItem = string.IsNullOrEmpty(_mode) ? modes[0] : _mode;
        _mode = ModeBox.SelectedItem as string;
        _loadingModes = false;
    }

    private void BindActions(ListingSnapshot snapshot)
    {
        ActionBar.Children.Clear();
        CreateBar.Children.Clear();
        var fiscalYear = _title == "Abertura de ano fiscal";
        if (snapshot.CanCreate || fiscalYear)
        {
            var label = fiscalYear ? "Criar ano fiscal" : "Novo";
            var content = new StackPanel { Classes = { "bo_doc_action_content" } };
            content.Children.Add(new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
            {
                Classes = { "bo_doc_action_icon" },
                Path = "avares://LogicPOS.App/Assets/Images/Listing/botao_novo_w.svg"
            });
            content.Children.Add(new TextBlock { Classes = { "bo_doc_action_text" }, Text = label });
            var create = new Button { Classes = { "bo_page_action" }, Content = content };
            ToolTip.SetTip(create, label);
            create.Click += async (_, _) =>
            {
                if (fiscalYear)
                {
                    FiscalYearOverlay.IsVisible = true;
                    await FiscalYearHost.ShowAsync();
                    return;
                }

                await EditAsync(null);
            };
            CreateBar.Children.Add(create);
        }

        foreach (var action in snapshot.Actions)
        {
            var key = action.Key;
            var button = new Button { Classes = { "bo_page_action" }, Content = action.Label };
            button.Click += async (_, _) => await RunActionAsync(key);
            ActionBar.Children.Add(button);
        }
    }

    private void BindColumns(ListingSnapshot snapshot)
    {
        var signature = string.Join("|", snapshot.Columns.Select(column => column.Key)) + snapshot.CanEdit + snapshot.CanDelete + ShowsDocument;
        if (signature == _columnSignature)
        {
            return;
        }

        _columnSignature = signature;
        _columnKeys.Clear();
        _defaultVisible.Clear();
        Grid.Columns.Clear();
        var starred = false;
        foreach (var column in snapshot.Columns)
        {
            var flexible = starred == false && column.Key is "Designation" or "Name" or "Message" or "Notes" or "Value";
            if (flexible)
            {
                starred = true;
            }

            var titleWidth = Math.Max(120, (column.Header?.Length ?? 0) * 8 + 64);
            Grid.Columns.Add(new DataGridTextColumn
            {
                Header = column.Header,
                Binding = new Binding($"[{column.Key}]"),
                IsVisible = column.Visible,
                MinWidth = titleWidth,
                MaxWidth = flexible ? double.PositiveInfinity : Math.Max(280, titleWidth),
                Width = flexible
                    ? new DataGridLength(1, DataGridLengthUnitType.Star)
                    : new DataGridLength(1, DataGridLengthUnitType.Auto)
            });
            _columnKeys[column.Header] = column.Key;
            _defaultVisible[column.Header] = column.Visible;
        }

        if (starred == false && Grid.Columns.Count > 0)
        {
            Grid.Columns[0].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            Grid.Columns[0].MaxWidth = double.PositiveInfinity;
        }

        if (ShowsDocument)
        {
            Grid.Columns.Add(ActionColumn("avares://LogicPOS.App/Assets/Images/Documents/botao_ver_b.svg", row => _ = OpenListedDocumentAsync(row), _ => true));
        }

        if (snapshot.CanEdit)
        {
            Grid.Columns.Add(ActionColumn("avares://LogicPOS.App/Assets/Images/Documents/botao_editar_b.svg", row => _ = EditAsync(row.Id), _ => true));
        }

        if (snapshot.CanDelete)
        {
            Grid.Columns.Add(ActionColumn("avares://LogicPOS.App/Assets/Images/Documents/botao_eliminar_b.svg", row => _ = DeleteAsync(row), row => row.CanDelete));
        }
    }

    private static DataGridTemplateColumn ActionColumn(string icon, Action<ListingRow> click, Func<ListingRow, bool> enabled)
    {
        return new DataGridTemplateColumn
        {
            Header = string.Empty,
            CanUserSort = false,
            CanUserReorder = false,
            MinWidth = 36,
            MaxWidth = 36,
            Width = new DataGridLength(36),
            CellTemplate = new FuncDataTemplate<ListingRow>((row, _) =>
            {
                var button = new Button { Classes = { "bo_doc_icon_button" }, IsEnabled = enabled(row) };
                button.Content = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
                {
                    Classes = { "bo_doc_row_icon" },
                    Path = icon
                };
                button.Click += (_, _) => click(row);
                return button;
            })
        };
    }

    private void ApplyFilter()
    {
        var term = SearchBox.Text?.Trim();
        var rows = _rows.Where(InDateRange).Where(row =>
            string.IsNullOrEmpty(term) || row.Values.Values.Any(value => value.Contains(term, StringComparison.CurrentCultureIgnoreCase))).ToList();
        Grid.ItemsSource = rows;
        Notice.Text = string.IsNullOrEmpty(term) && rows.Count == _rows.Count && _notice is not null ? _notice : $"{rows.Count} registo(s)";
    }

    private bool InDateRange(ListingRow row)
    {
        if (DateFilters.IsVisible == false)
        {
            return true;
        }

        var start = StartDate.SelectedDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1);
        var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
        if (end < start)
        {
            (start, end) = (end, start);
        }

        foreach (var key in DateKeys)
        {
            if (row.Values.TryGetValue(key, out var text) == false || string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            if (DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out var date)
                || DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                // Unset audit dates deserialize as year 1 and must not hide a row that was just saved.
                if (date.Year < 1900)
                {
                    continue;
                }

                var day = date.Date;
                return day >= start && day <= end;
            }
        }

        return true;
    }

    private void RestoreColumns()
    {
        foreach (var column in ChoosableColumns())
        {
            var header = column.Header?.ToString() ?? string.Empty;
            if (_defaultVisible.TryGetValue(header, out var visible))
            {
                column.IsVisible = visible;
            }
        }
    }

    private void OnChooseColumnsClick(object? sender, RoutedEventArgs e)
    {
        ColumnSearch.Text = string.Empty;
        _columnSnapshot.Clear();
        ColumnChecks.Children.Clear();
        var columns = ChoosableColumns().ToList();
        var selectAll = new CheckBox
        {
            Content = "Selecionar tudo",
            IsChecked = columns.All(column => column.IsVisible)
        };
        selectAll.IsCheckedChanged += OnSelectAllColumnsChanged;
        ColumnChecks.Children.Add(selectAll);
        foreach (var column in columns)
        {
            _columnSnapshot.Add((column, column.IsVisible));
            var box = new CheckBox
            {
                Content = column.Header?.ToString(),
                IsChecked = column.IsVisible,
                Tag = column,
                Classes = { "bo_doc_column_check" }
            };
            box.IsCheckedChanged += (_, _) => RefreshSelectAll();
            ColumnChecks.Children.Add(box);
        }

        ColumnChooser.IsVisible = true;
    }

    private void OnColumnSearchChanged(object? sender, TextChangedEventArgs e)
    {
        var term = ColumnSearch.Text?.Trim();
        foreach (var box in ColumnChecks.Children.OfType<CheckBox>())
        {
            if (box.Tag is not DataGridColumn)
            {
                continue;
            }

            var text = box.Content?.ToString() ?? string.Empty;
            box.IsVisible = string.IsNullOrEmpty(term) || text.Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        RefreshSelectAll();
    }

    private void OnSelectAllColumnsChanged(object? sender, RoutedEventArgs e)
    {
        if (_updatingColumnChecks || sender is not CheckBox selectAll)
        {
            return;
        }

        _updatingColumnChecks = true;
        var check = selectAll.IsChecked == true;
        foreach (var box in ColumnChecks.Children.OfType<CheckBox>())
        {
            if (box.IsVisible && box.Tag is DataGridColumn)
            {
                box.IsChecked = check;
            }
        }

        _updatingColumnChecks = false;
    }

    private void OnColumnsOkClick(object? sender, RoutedEventArgs e)
    {
        foreach (var box in ColumnChecks.Children.OfType<CheckBox>())
        {
            if (box.Tag is DataGridColumn column)
            {
                column.IsVisible = box.IsChecked == true;
            }
        }

        ColumnChooser.IsVisible = false;
    }

    private void OnColumnsCancelClick(object? sender, RoutedEventArgs e)
    {
        foreach (var (column, visible) in _columnSnapshot)
        {
            column.IsVisible = visible;
        }

        ColumnChooser.IsVisible = false;
    }

    private void RefreshSelectAll()
    {
        if (_updatingColumnChecks)
        {
            return;
        }

        var boxes = ColumnChecks.Children.OfType<CheckBox>().Where(box => box.IsVisible && box.Tag is DataGridColumn).ToList();
        if (ColumnChecks.Children.OfType<CheckBox>().FirstOrDefault(box => box.Tag is not DataGridColumn) is not CheckBox selectAll)
        {
            return;
        }

        _updatingColumnChecks = true;
        selectAll.IsChecked = boxes.Count > 0 && boxes.All(box => box.IsChecked == true);
        _updatingColumnChecks = false;
    }

    private IEnumerable<DataGridColumn> ChoosableColumns()
    {
        return Grid.Columns.Where(column => column.Header?.ToString() is not ("Editar" or "Eliminar"));
    }

    private async Task ExportAsync(bool excel)
    {
        var columns = ChoosableColumns().Where(column => column.IsVisible).ToList();
        var headers = columns.Select(column => column.Header?.ToString() ?? string.Empty).ToList();
        var rows = (Grid.ItemsSource as IEnumerable<ListingRow> ?? []).Select(row =>
            (IReadOnlyList<string>)headers.Select(header =>
                _columnKeys.TryGetValue(header, out var key) ? row[key] : string.Empty).ToList()).ToList();
        await ListingFileExport.SaveAsync(TopLevel.GetTopLevel(this), _title, excel, headers, rows);
    }

    private async Task EditAsync(Guid? id)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        _editingId = id;
        EditorTitle.Text = id is null ? "Novo" : "Editar";
        EditorNotice.Text = string.Empty;
        EditorFields.Children.Clear();
        _inputs.Clear();
        _articleTabs.Clear();
        _articlePages = null;
        IReadOnlyList<ListingField> fields;
        try
        {
            fields = await service.LoadFieldsAsync(_title, id);
        }
        catch (Exception exception)
        {
            Notice.Text = exception.Message;
            return;
        }
        var groups = OrderEditorGroups(fields);

        // Tabbed editors keep a fixed frame so switching tabs does not resize the popup
        var simpleForm = groups.Count <= 1;
        EditorFrame.Classes.Set("bo_entity_editor_fit", simpleForm);
        EditorFrame.Classes.Set("bo_entity_editor_narrow", simpleForm);
        EditorBody.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
        EditorBody.HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled;
        if (groups.Count > 1)
        {
            var imageField = fields.FirstOrDefault(field => field.Kind == "image");
            var headers = new StackPanel { Classes = { "bo_entity_tab_headers" } };
            var pages = new ContentControl();
            _articlePages = pages;
            foreach (var group in groups)
            {
                var groupFields = fields.Where(item => item.Group == group && item.Kind != "image").ToList();
                if (groupFields.Count == 0)
                {
                    continue;
                }

                var page = await LayoutGroupAsync(service, groupFields);
                var header = new Button { Classes = { "bo_entity_tab" }, Content = group };
                var showWhen = group == "Detalhes financeiros"
                    ? "TypeHasPrice"
                    : groupFields.Select(item => item.ShowWhen).FirstOrDefault(value => string.IsNullOrWhiteSpace(value) == false);
                var tab = (header, page, showWhen);
                header.Click += (_, _) => SelectArticleTab(tab);
                headers.Children.Add(header);
                _articleTabs.Add(tab);
            }

            var bar = new Grid { Classes = { "bo_entity_tab_bar" } };
            bar.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            bar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            bar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            Avalonia.Controls.Grid.SetColumn(headers, 0);
            bar.Children.Add(headers);
            if (imageField is not null)
            {
                var chrome = CreateImageChrome(imageField);
                _inputs.Add((imageField.Key, chrome.Preview, imageField.Kind));
                Avalonia.Controls.Grid.SetColumn(chrome.Browse, 1);
                Avalonia.Controls.Grid.SetColumn(chrome.Preview, 2);
                bar.Children.Add(chrome.Browse);
                bar.Children.Add(chrome.Preview);
            }

            var shell = new StackPanel();
            shell.Children.Add(bar);
            shell.Children.Add(pages);
            EditorFields.Children.Add(shell);
            WireArticleRules();
            ApplyArticleTabs();
        }
        else
        {
            foreach (var field in fields)
            {
                await AddFieldAsync(service, EditorFields, field);
            }
        }

        EditorOverlay.IsVisible = true;
    }

    private async Task AddFieldAsync(IBackOfficeListingService service, Panel panel, ListingField field)
    {
        panel.Children.Add(await CreateFieldBlockAsync(service, field, caption: field.Kind != "bool"));
    }

    private async Task<Control> LayoutGroupAsync(IBackOfficeListingService service, IReadOnlyList<ListingField> fields)
    {
        var root = new StackPanel();
        var prices = fields.Where(field => IsPriceCell(field.Key)).ToList();
        if (prices.Count > 0)
        {
            root.Children.Add(await BuildPriceTableAsync(service, prices));
        }

        root.Children.Add(await BuildColumnsAsync(service, fields.Where(field => IsPriceCell(field.Key) == false).ToList()));
        return root;
    }

    private async Task<Control> BuildPriceTableAsync(IBackOfficeListingService service, IReadOnlyList<ListingField> prices)
    {
        var grid = new Grid { Classes = { "bo_price_table" } };
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(120)));
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(70)));
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        AddPriceHeader(grid, 1, "Preço");
        AddPriceHeader(grid, 2, "Promoção");
        AddPriceHeader(grid, 3, "Usa");

        for (var index = 1; index <= 5; index++)
        {
            var value = prices.FirstOrDefault(field => field.Key == $"Price{index}");
            var promotion = prices.FirstOrDefault(field => field.Key == $"Price{index}Promotion");
            var use = prices.FirstOrDefault(field => field.Key == $"Price{index}UsePromotion");
            if (value is null && promotion is null && use is null)
            {
                continue;
            }

            var row = grid.RowDefinitions.Count;
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var label = new TextBlock { Classes = { "bo_new_doc_caption" }, Text = $"Preço {index}", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };
            Avalonia.Controls.Grid.SetRow(label, row);
            grid.Children.Add(label);
            if (value is not null)
            {
                Place(grid, await CreateInputAsync(service, value), row, 1, value);
            }

            if (promotion is not null)
            {
                Place(grid, await CreateInputAsync(service, promotion), row, 2, promotion);
            }

            if (use is not null)
            {
                Place(grid, await CreateInputAsync(service, use), row, 3, use);
            }
        }

        return grid;
    }

    private async Task<Control> BuildColumnsAsync(IBackOfficeListingService service, IReadOnlyList<ListingField> fields)
    {
        var grid = new Grid { Classes = { "bo_entity_columns" } };
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));

        var flags = new WrapPanel { Classes = { "bo_entity_flags" } };
        var row = 0;
        var column = 0;
        foreach (var field in fields)
        {
            if (field.Kind == "image")
            {
                continue;
            }

            if (field.Kind == "bool")
            {
                flags.Children.Add(await CreateFieldBlockAsync(service, field, caption: false));
                continue;
            }

            var wide = field.Kind == "multiline";
            if (wide && column == 1)
            {
                column = 0;
                row++;
            }

            EnsureRow(grid, row);
            var block = await CreateFieldBlockAsync(service, field, caption: true);
            Avalonia.Controls.Grid.SetRow(block, row);
            Avalonia.Controls.Grid.SetColumn(block, column);
            if (wide)
            {
                Avalonia.Controls.Grid.SetColumnSpan(block, 2);
            }

            grid.Children.Add(block);
            if (wide || column == 1)
            {
                column = 0;
                row++;
            }
            else
            {
                column++;
            }
        }

        if (flags.Children.Count > 0)
        {
            if (column == 1)
            {
                row++;
            }

            EnsureRow(grid, row);
            Avalonia.Controls.Grid.SetRow(flags, row);
            Avalonia.Controls.Grid.SetColumnSpan(flags, 2);
            grid.Children.Add(flags);
        }

        return grid;
    }

    private async Task<Control> CreateFieldBlockAsync(IBackOfficeListingService service, ListingField field, bool caption)
    {
        var input = await CreateInputAsync(service, field);
        if (field.Kind == "bool" && input is CheckBox check)
        {
            check.Content = field.Label;
        }

        _inputs.Add((field.Key, input, field.Kind));
        if (caption == false)
        {
            return input;
        }

        var stack = new StackPanel();
        stack.Children.Add(new TextBlock { Classes = { "bo_new_doc_caption" }, Text = field.Label });
        stack.Children.Add(input);
        return stack;
    }

    private void Place(Grid grid, Control input, int row, int column, ListingField field)
    {
        _inputs.Add((field.Key, input, field.Kind));
        Avalonia.Controls.Grid.SetRow(input, row);
        Avalonia.Controls.Grid.SetColumn(input, column);
        grid.Children.Add(input);
    }

    private static void EnsureRow(Grid grid, int row)
    {
        while (grid.RowDefinitions.Count <= row)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }
    }

    private static void AddPriceHeader(Grid grid, int column, string text)
    {
        var header = new TextBlock { Classes = { "bo_new_doc_caption" }, Text = text };
        Avalonia.Controls.Grid.SetColumn(header, column);
        grid.Children.Add(header);
    }

    private static bool IsPriceCell(string key)
        => key.Length > 5 && key.StartsWith("Price", StringComparison.Ordinal) && key[5] is >= '1' and <= '5';

    private async Task<Control> CreateInputAsync(IBackOfficeListingService service, ListingField field)
    {
        if (field.Kind == "image")
        {
            var chrome = CreateImageChrome(field);
            var stack = new StackPanel { Classes = { "bo_image_picker" }, Tag = chrome.Preview.Tag };
            stack.Children.Add(chrome.Preview);
            stack.Children.Add(chrome.Browse);
            return stack;
        }

        if (field.Kind == "bool")
        {
            return new CheckBox { Classes = { "bo_entity_input" }, IsChecked = field.Value == "true", IsEnabled = field.ReadOnly == false };
        }

        if (field.Kind == "lookup" && field.Lookup is not null)
        {
            var options = (await service.LookupAsync(field.Lookup)).ToList();
            if (field.Required == false)
            {
                options.Insert(0, new LookupOption { Id = Guid.Empty, Label = "(nenhum)" });
            }

            var search = new AutoCompleteBox
            {
                Classes = { "bo_entity_input" },
                ItemsSource = options,
                IsEnabled = field.ReadOnly == false
            };
            ListingFilters.EnableLookupSearch(search);
            var current = options.FirstOrDefault(option => option.Id.ToString() == field.Value);
            if (current is null && options.Count <= 40)
            {
                current = options.FirstOrDefault();
            }

            if (current is not null)
            {
                ListingFilters.SelectLookup(search, current);
            }

            if (field.Required == false && field.ReadOnly == false)
            {
                return WrapLookupClear(search);
            }

            return search;
        }

        if (field.Kind == "choice" && field.Options is { Count: > 0 } choices)
        {
            return new ComboBox
            {
                Classes = { "bo_entity_input" },
                ItemsSource = choices,
                SelectedItem = choices.FirstOrDefault(choice => choice == field.Value) ?? choices[0],
                IsEnabled = field.ReadOnly == false
            };
        }

        if (field.Kind == "enum")
        {
            return new TextBox { Classes = { "bo_entity_input" }, Text = field.Value, IsReadOnly = field.ReadOnly };
        }

        var box = new TextBox
        {
            Classes = { "bo_entity_input" },
            Text = field.Value,
            IsReadOnly = field.ReadOnly,
            AcceptsReturn = field.Kind == "multiline"
        };
        if (field.Kind == "multiline")
        {
            box.Classes.Add("bo_entity_multiline");
        }

        if (TouchFields.IsNumericKey(field.Key))
        {
            box.Classes.Add("bo_numeric");
        }

        var input = field.ReadOnly ? (Control)box : AttachKeyboard(box);
        if (_title == "Clientes" && field.Key == "FiscalNumber" && field.ReadOnly == false && input is DockPanel dock)
        {
            var vies = new Button { Classes = { "bo_doc_page_button" }, Content = "VIES" };
            vies.Click += async (_, _) => await CompleteVatAsync(true);
            DockPanel.SetDock(vies, Dock.Right);
            dock.Children.Insert(0, vies);
            box.LostFocus += async (_, _) => await CompleteVatAsync(false);
        }

        if (_title == "Impressoras" && field.Key == "Designation" && field.ReadOnly == false && input is DockPanel printerDock)
        {
            var pick = CreateInstalledPrinterButton(box);
            DockPanel.SetDock(pick, Dock.Right);
            printerDock.Children.Insert(0, pick);
        }

        if (_title == "Terminais" && field.Key == "HardwareId" && field.ReadOnly == false && input is DockPanel hardwareDock)
        {
            var bind = new Button { Classes = { "bo_doc_page_button" }, Content = "PC" };
            ToolTip.SetTip(bind, "Usar o Hardware ID desta máquina");
            bind.Click += (_, _) => box.Text = MachineIdentity.HardwareId;
            DockPanel.SetDock(bind, Dock.Right);
            hardwareDock.Children.Insert(0, bind);
        }

        return input;
    }

    private static Control WrapLookupClear(AutoCompleteBox search)
    {
        var clear = new Button
        {
            Classes = { "bo_lookup_clear" },
            Content = "×"
        };
        ToolTip.SetTip(clear, "Limpar");
        clear.Click += (_, _) =>
        {
            ListingFilters.SelectLookup(search, null);
            RefreshLookupClear(clear, search);
        };
        search.SelectionChanged += (_, _) => RefreshLookupClear(clear, search);
        search.TextChanged += (_, _) => RefreshLookupClear(clear, search);
        RefreshLookupClear(clear, search);

        var dock = new DockPanel { Classes = { "bo_touch_row" } };
        DockPanel.SetDock(clear, Dock.Right);
        dock.Children.Add(clear);
        dock.Children.Add(search);
        return dock;
    }

    private static void RefreshLookupClear(Button clear, AutoCompleteBox search)
        => clear.IsVisible = ListingFilters.SelectedLookup(search) is not null;

    private async void OnSaveClick(object? sender, RoutedEventArgs e) => await SaveEditorAsync();

    private async Task SaveEditorAsync()
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        var fields = new Dictionary<string, string>();
        foreach (var input in _inputs)
        {
            if (input.Input.Tag is ImagePick pick)
            {
                fields[input.Key] = pick.Data;
                fields[input.Key + "Extension"] = pick.Extension;
                continue;
            }

            fields[input.Key] = ReadInput(input.Input);
            var selected = ChosenLookup(input.Input);
            if (selected is not null && string.IsNullOrWhiteSpace(selected.Meta) == false)
            {
                fields[input.Key + "Meta"] = selected.Meta;
            }
        }

        if (_title == "Artigos")
        {
            var invalid = ArticleFormRules.Validate(fields, VatIsDutyFree(), _editingId);
            if (invalid is not null)
            {
                EditorNotice.Text = invalid;
                return;
            }
        }
        else
        {
            var invalid = GtkFormRules.Validate(_title, fields);
            if (invalid is not null)
            {
                EditorNotice.Text = invalid;
                return;
            }
        }

        var result = await service.SaveAsync(_title, _editingId, fields);
        EditorNotice.Text = result.Error ?? string.Empty;
        Notice.Text = result.Error ?? string.Empty;
        ShowResultToast(result.Succeeded, result.Error, result.Message ?? "Registo gravado com sucesso.");
        if (result.Succeeded)
        {
            EditorOverlay.IsVisible = false;
            await ReloadAsync();
        }
    }

    private async Task CompleteVatAsync(bool force)
    {
        if (_vatBusy || _title != "Clientes")
        {
            return;
        }

        _vatBusy = true;
        try
        {
            if (force == false && await VatAutocompleteEnabledAsync() == false)
            {
                return;
            }

            var fiscal = _inputs.FirstOrDefault(item => item.Key == "FiscalNumber");
            if (fiscal.Input is null)
            {
                return;
            }

            var fiscalNumber = ReadInput(fiscal.Input);
            if (fiscalNumber.Length < 3)
            {
                return;
            }

            var info = await EuropeanVatLookup.FindAsync(CountryCode(), fiscalNumber);
            if (info is null)
            {
                EditorNotice.Text = "NIF não encontrado no VIES.";
                return;
            }

            SetText("Name", info.Name);
            SetText("Address", info.Address);
            SetText("City", info.City);
            SetText("ZipCode", info.ZipCode);
            SetText("Locality", info.Locality);
            EditorNotice.Text = "Dados preenchidos a partir do VIES.";
        }
        finally
        {
            _vatBusy = false;
        }
    }

    private async Task<bool> VatAutocompleteEnabledAsync()
    {
        if (_vatAutocomplete is bool cached)
        {
            return cached;
        }

        var service = Service;
        if (service is null)
        {
            _vatAutocomplete = true;
            return true;
        }

        try
        {
            var snapshot = await service.QueryAsync("Parâmetros de Sistema");
            var row = snapshot.Rows.FirstOrDefault(item =>
                item.Values.TryGetValue("Token", out var token) && token == "USE_EUROPEAN_VAT_AUTOCOMPLETE");
            if (row is null || row.Values.TryGetValue("Value", out var value) == false)
            {
                _vatAutocomplete = true;
                return true;
            }

            _vatAutocomplete = value is "1" or "true" or "True" or "Sim";
        }
        catch (Exception)
        {
            _vatAutocomplete = true;
        }

        return _vatAutocomplete.Value;
    }

    private string CountryCode()
    {
        var country = _inputs.FirstOrDefault(item => item.Key == "CountryId").Input;
        if (ChosenLookup(country) is { } option && string.IsNullOrWhiteSpace(option.Meta) == false)
        {
            return option.Meta;
        }

        var name = CultureInfo.CurrentUICulture.Name;
        var split = name.IndexOf('-');
        return split > 0 ? name[(split + 1)..] : "PT";
    }

    private void SetText(string key, string value)
    {
        var input = _inputs.FirstOrDefault(item => item.Key == key).Input;
        if (input is DockPanel dock)
        {
            input = dock.Children.OfType<TextBox>().FirstOrDefault();
        }

        if (input is TextBox box && box.IsReadOnly == false)
        {
            box.Text = value;
        }
    }

    private static LookupOption? ChosenLookup(Control? input)
    {
        if (input is DockPanel dock)
        {
            input = dock.Children.OfType<AutoCompleteBox>().FirstOrDefault() ?? input;
        }

        if (input is AutoCompleteBox box)
        {
            return ListingFilters.SelectedLookup(box);
        }

        return input is ComboBox combo && combo.SelectedItem is LookupOption option && option.Id != Guid.Empty
            ? option
            : null;
    }

    private static string ReadInput(Control input)
    {
        if (input is DockPanel dock)
        {
            var lookup = dock.Children.OfType<AutoCompleteBox>().FirstOrDefault();
            input = lookup ?? dock.Children.OfType<TextBox>().FirstOrDefault() ?? input;
        }

        if (input is CheckBox check)
        {
            return check.IsChecked == true ? "true" : "false";
        }

        if (input is AutoCompleteBox)
        {
            var picked = ListingFilters.SelectedLookup(input as AutoCompleteBox);
            return picked is null ? string.Empty : picked.Id.ToString();
        }

        if (input is ComboBox combo && combo.SelectedItem is LookupOption option)
        {
            return option.Id == Guid.Empty ? string.Empty : option.Id.ToString();
        }

        if (input is ComboBox choice && choice.SelectedItem is string selectedChoice)
        {
            return selectedChoice;
        }

        if (input is TextBox box)
        {
            return box.Text ?? string.Empty;
        }

        return string.Empty;
    }

    private static Button CreateInstalledPrinterButton(TextBox target)
    {
        var button = new Button
        {
            Classes = { "bo_touch_key" },
            Content = new Image
            {
                Classes = { "bo_printer_pick_icon" },
                Source = new Bitmap(Avalonia.Platform.AssetLoader.Open(new Uri("avares://LogicPOS.App/Assets/Images/Documents/icon_doc_print_b.png")))
            }
        };
        ToolTip.SetTip(button, "Impressoras instaladas no Windows");
        button.Click += (_, _) =>
        {
            var menu = new MenuFlyout { Placement = PlacementMode.BottomEdgeAlignedRight };
            FillInstalledPrinters(menu, target);
            menu.ShowAt(button);
        };
        return button;
    }

    private static void FillInstalledPrinters(MenuFlyout menu, TextBox target)
    {
        IReadOnlyList<string> printers;
        try
        {
            printers = WindowsPrinters.Installed();
        }
        catch (Exception exception)
        {
            menu.Items.Add(new MenuItem { Classes = { "bo_printer_pick_item" }, Header = exception.Message, IsEnabled = false });
            return;
        }

        if (printers.Count == 0)
        {
            menu.Items.Add(new MenuItem { Classes = { "bo_printer_pick_item" }, Header = "Não existem impressoras instaladas.", IsEnabled = false });
            return;
        }

        foreach (var printer in printers)
        {
            var item = new MenuItem { Classes = { "bo_printer_pick_item" }, Header = printer };
            if (string.Equals(printer, target.Text, StringComparison.OrdinalIgnoreCase))
            {
                item.Classes.Add("selected");
            }

            item.Click += (_, _) => target.Text = printer;
            menu.Items.Add(item);
        }
    }

    private static Control AttachKeyboard(TextBox box)
    {
        var button = new Button
        {
            Classes = { "bo_touch_key" },
            Content = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
            {
                Classes = { "bo_touch_key_icon" },
                Path = "avares://LogicPOS.App/Assets/Images/Listing/botao_teclado.svg"
            }
        };
        button.Click += (_, _) =>
        {
            box.Focus();
            TouchKeyboard.Show(TouchFields.IsNumeric(box));
        };
        box.AddHandler(InputElement.PointerPressedEvent, (_, args) =>
        {
            if (args.Pointer.Type == PointerType.Touch)
            {
                box.Focus();
                TouchKeyboard.Show(TouchFields.IsNumeric(box));
            }
        }, RoutingStrategies.Tunnel);
        var dock = new DockPanel { Classes = { "bo_touch_row" } };
        DockPanel.SetDock(button, Dock.Right);
        dock.Children.Add(button);
        dock.Children.Add(box);
        return dock;
    }

    private void OnEditorCancelClick(object? sender, RoutedEventArgs e) => EditorOverlay.IsVisible = false;

    private static List<string> OrderEditorGroups(IReadOnlyList<ListingField> fields)
    {
        var present = fields
            .Select(field => field.Group)
            .Where(group => string.IsNullOrWhiteSpace(group) == false)
            .Select(group => group!)
            .Distinct(StringComparer.Ordinal)
            .ToList();
        if (present.Count == 0)
        {
            return present;
        }

        var ordered = new List<string>();
        foreach (var name in BackOfficeListingService.ArticleTabOrder)
        {
            if (present.Contains(name))
            {
                ordered.Add(name);
            }
        }

        foreach (var name in present)
        {
            if (ordered.Contains(name) == false)
            {
                ordered.Add(name);
            }
        }

        return ordered;
    }

    private void WireArticleRules()
    {
        foreach (var input in _inputs)
        {
            if (input.Input is CheckBox check && input.Key is "IsComposed" or "UniqueArticles")
            {
                check.IsCheckedChanged += (_, _) => ApplyArticleTabs();
            }

            if (input.Key == "TypeId" && input.Input is AutoCompleteBox typeBox)
            {
                typeBox.PropertyChanged += (_, args) =>
                {
                    if (args.Property == AutoCompleteBox.TextProperty || args.Property == AutoCompleteBox.SelectedItemProperty)
                    {
                        ApplyArticleTabs();
                    }
                };
            }
        }
    }

    private void SelectArticleTab((Button Header, Control Page, string? ShowWhen) tab)
    {
        foreach (var item in _articleTabs)
        {
            item.Header.Classes.Remove("bo_entity_tab_on");
        }

        tab.Header.Classes.Add("bo_entity_tab_on");
        if (_articlePages is not null)
        {
            _articlePages.Content = tab.Page;
        }
    }

    private void ApplyArticleTabs()
    {
        foreach (var tab in _articleTabs)
        {
            tab.Header.IsVisible = ShowArticleTab(tab.ShowWhen);
        }

        var current = _articleTabs.FirstOrDefault(tab => _articlePages?.Content == tab.Page);
        if (current.Header is null || current.Header.IsVisible == false)
        {
            var next = _articleTabs.FirstOrDefault(tab => tab.Header.IsVisible);
            if (next.Header is not null)
            {
                SelectArticleTab(next);
            }
        }
    }

    private bool ShowArticleTab(string? showWhen)
    {
        if (string.IsNullOrWhiteSpace(showWhen))
        {
            return true;
        }

        if (showWhen == "TypeHasPrice")
        {
            return TypeHasPrice();
        }

        return _inputs.FirstOrDefault(input => input.Key == showWhen).Input is CheckBox check && check.IsChecked == true;
    }

    private bool TypeHasPrice()
    {
        var option = ChosenLookup(_inputs.FirstOrDefault(input => input.Key == "TypeId").Input);
        if (option is null || option.Id == Guid.Empty)
        {
            return false;
        }

        return option.Meta is not "0";
    }

    private bool VatIsDutyFree()
    {
        foreach (var key in new[] { "VatOnTableId", "VatDirectSellingId" })
        {
            var option = ChosenLookup(_inputs.FirstOrDefault(input => input.Key == key).Input);
            if (option is not null
                && decimal.TryParse(option.Meta, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate)
                && rate == 0)
            {
                return true;
            }
        }

        return false;
    }

    private ImageChrome CreateImageChrome(ListingField field)
    {
        var pick = new ImagePick();
        var photo = new Image { Classes = { "bo_image_photo" } };
        var frame = new Border { Classes = { "bo_image_frame" }, ClipToBounds = true, Child = photo };
        var remove = new Button { Classes = { "bo_image_remove" }, Content = "×", IsVisible = false };
        var wrap = new Grid { Classes = { "bo_image_photo_wrap" }, Tag = pick };
        wrap.Children.Add(frame);
        wrap.Children.Add(remove);
        var browse = new Button { Classes = { "bo_image_browse" }, Content = "Procurar..." };

        void Show(Bitmap? bitmap)
        {
            photo.Source = bitmap;
            remove.IsVisible = bitmap is not null;
        }

        remove.Click += (_, _) =>
        {
            pick.Data = string.Empty;
            pick.Extension = string.Empty;
            Show(null);
        };
        browse.Click += async (_, _) =>
        {
            var file = await PickImageAsync();
            if (file is not null)
            {
                await LoadImageAsync(pick, file, Show);
            }
        };
        DragDrop.SetAllowDrop(wrap, true);
        wrap.AddHandler(DragDrop.DropEvent, async (_, args) =>
        {
            var file = args.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault();
            if (file is not null)
            {
                await LoadImageAsync(pick, file, Show);
            }
        });
        if (string.IsNullOrWhiteSpace(field.Value) == false)
        {
            Show(DecodeImage(field.Value));
            if (photo.Source is not null)
            {
                pick.Data = field.Value;
            }
        }

        return new ImageChrome(wrap, browse);
    }

    private async Task<IStorageFile?> PickImageAsync()
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is null)
        {
            return null;
        }

        var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Imagem",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Imagens") { Patterns = ["*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp", "*.webp"] }]
        });
        return files.FirstOrDefault();
    }

    private async Task LoadImageAsync(ImagePick pick, IStorageFile file, Action<Bitmap?> show)
    {
        var extension = Path.GetExtension(file.Name).TrimStart('.').ToLowerInvariant();
        if (extension is not ("jpg" or "jpeg" or "png" or "gif" or "bmp" or "webp"))
        {
            EditorNotice.Text = "A imagem tem de ser jpg, png, gif, bmp ou webp.";
            return;
        }

        await using var stream = await file.OpenReadAsync();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        if (memory.Length > 2 * 1024 * 1024)
        {
            EditorNotice.Text = "A imagem não pode exceder 2 MB.";
            return;
        }

        pick.Data = Convert.ToBase64String(memory.ToArray());
        pick.Extension = extension == "jpeg" ? "jpg" : extension;
        memory.Position = 0;
        show(new Bitmap(memory));
        EditorNotice.Text = string.Empty;
    }

    private static Bitmap? DecodeImage(string value)
    {
        try
        {
            var bytes = Convert.FromBase64String(value);
            return new Bitmap(new MemoryStream(bytes));
        }
        catch (FormatException)
        {
            if (File.Exists(value))
            {
                return new Bitmap(value);
            }

            return null;
        }
    }

    private sealed class ImageChrome
    {
        public ImageChrome(Control preview, Button browse)
        {
            Preview = preview;
            Browse = browse;
        }

        public Control Preview { get; }

        public Button Browse { get; }
    }

    private sealed class ImagePick
    {
        public string Data { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;
    }

    private async Task DeleteAsync(ListingRow row)
    {
        if (row.CanDelete == false)
        {
            return;
        }

        var service = Service;
        if (service is null)
        {
            return;
        }

        var result = await service.DeleteAsync(_title, row.Id);
        Notice.Text = result.Error ?? string.Empty;
        ShowResultToast(result.Succeeded, result.Error, result.Message ?? "Registo eliminado com sucesso.");
        await ReloadAsync();
    }

    private void ShowResultToast(bool succeeded, string? error, string success)
    {
        if (succeeded)
        {
            Toast.Success(this, success);
            return;
        }

        Toast.Error(this, error ?? "Não foi possível concluir a operação.");
    }

    private async Task RunActionAsync(string action)
    {
        if (action is "stock-in" or "stock-out")
        {
            OpenPrompt(action, "Quantidade", "Quantidade a movimentar", "1");
            return;
        }

        if (action is "cash-in" or "cash-out")
        {
            OpenPrompt(action, "Caixa", "Valor", "0,00");
            return;
        }

        if (action == "restore")
        {
            OpenPrompt(action, "Restaurar", "Escreva RESTAURAR para substituir a base atual.", string.Empty);
            return;
        }

        if (action is "import-articles" or "import-customers")
        {
            await ImportAsync(action);
            return;
        }

        if (action == "restore-file")
        {
            await RestoreFileAsync();
            return;
        }

        if (action == "apply-privileges")
        {
            if (EditorOverlay.IsVisible == false || _inputs.Any(input => input.Key.StartsWith("perm:", StringComparison.Ordinal)) == false)
            {
                Notice.Text = "Abra o perfil, marque os privilégios e volte a aplicar.";
                return;
            }

            await SaveEditorAsync();
            if (TopLevel.GetTopLevel(this) is BackOfficeWindow office)
            {
                await office.ApplyMenuAccessAsync();
            }

            return;
        }

        if (action == "undo-receipt")
        {
            if (Grid.SelectedItem is not ListingRow receipt || receipt.Id == Guid.Empty)
            {
                Notice.Text = "Selecione o recibo.";
                return;
            }

            OpenPrompt(action, "Anular recibo", "Motivo da anulação", string.Empty);
            return;
        }

        if (action == "open-report")
        {
            await OpenReportAsync();
            return;
        }

        if (action is "export-saft" or "saft-period" or "open-audit")
        {
            var start = StartDate.SelectedDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1);
            var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
            await ExecuteAsync(action, null, $"{start:yyyy-MM-dd}|{end:yyyy-MM-dd}");
            return;
        }

        if (action == "register-at")
        {
            if (Grid.SelectedItem is not ListingRow series)
            {
                Notice.Text = "Selecione a série.";
                return;
            }

            await ExecuteAsync(action, series.Id, null);
            return;
        }

        if (action == "request-agt")
        {
            OpenPrompt(action, "AGT", "Tipo de documento", "FT");
            return;
        }

        if (action == "agt-consult")
        {
            if (Grid.SelectedItem is not ListingRow document || string.IsNullOrWhiteSpace(document["Number"]))
            {
                Notice.Text = "Selecione o documento.";
                return;
            }

            await ExecuteAsync(action, null, document["Number"]);
            return;
        }

        var selected = (Grid.SelectedItem as ListingRow)?.Id;
        await ExecuteAsync(action, selected, null);
    }

    private void OpenPrompt(string action, string title, string label, string value)
    {
        _pendingAction = action;
        PromptTitle.Text = title;
        PromptLabel.Text = label;
        PromptValue.Text = value;
        PromptOverlay.IsVisible = true;
    }

    private async void OnPromptConfirmClick(object? sender, RoutedEventArgs e)
    {
        var action = _pendingAction;
        PromptOverlay.IsVisible = false;
        if (action is null)
        {
            return;
        }

        if (action == "restore")
        {
            if (PromptValue.Text != "RESTAURAR")
            {
                Notice.Text = "Restauração cancelada.";
                return;
            }

            var path = (Grid.SelectedItem as ListingRow)?["Path"];
            await ExecuteAsync(action, null, path);
            return;
        }

        await ExecuteAsync(action, (Grid.SelectedItem as ListingRow)?.Id, PromptValue.Text);
    }

    private void OnPromptCancelClick(object? sender, RoutedEventArgs e) => PromptOverlay.IsVisible = false;

    private async Task ImportAsync(string action)
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is null)
        {
            return;
        }

        var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Importar",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Excel") { Patterns = ["*.xlsx"] }]
        });
        var file = files.FirstOrDefault();
        if (file is null)
        {
            return;
        }

        await using var stream = await file.OpenReadAsync();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        ExcelPreview.Sheet sheet;
        try
        {
            sheet = ExcelPreview.Read(memory.ToArray());
        }
        catch (Exception)
        {
            Notice.Text = "Não foi possível ler o ficheiro Excel.";
            return;
        }

        if (sheet.Rows.Count == 0)
        {
            Notice.Text = "O ficheiro não tem linhas para importar.";
            return;
        }

        ShowImportPreview(action, file.Name, sheet);
    }

    private void ShowImportPreview(string action, string fileName, ExcelPreview.Sheet sheet)
    {
        _importAction = action;
        _importFileName = fileName;
        _importSheet = sheet;
        _importRows.Clear();
        foreach (var cells in sheet.Rows)
        {
            _importRows.Add(new ImportLine(cells));
        }

        ImportTitle.Text = action == "import-customers" ? "Importar Clientes" : "Importar Artigos";
        ImportGrid.Columns.Clear();
        var starIndex = sheet.Headers.Count > 1 ? 1 : 0;
        for (var index = 0; index < sheet.Headers.Count; index++)
        {
            if (sheet.Headers[index] is "Designation" or "Name" or "Designação" or "Nome")
            {
                starIndex = index;
                break;
            }
        }

        for (var index = 0; index < sheet.Headers.Count; index++)
        {
            var titleWidth = Math.Max(120, sheet.Headers[index].Length * 8 + 64);
            var flexible = index == starIndex;
            ImportGrid.Columns.Add(new DataGridTextColumn
            {
                Header = sheet.Headers[index],
                Binding = new Binding($"[c{index}]"),
                MinWidth = titleWidth,
                MaxWidth = flexible ? double.PositiveInfinity : Math.Max(280, titleWidth),
                Width = flexible
                    ? new DataGridLength(1, DataGridLengthUnitType.Star)
                    : new DataGridLength(1, DataGridLengthUnitType.Auto)
            });
        }

        ImportGrid.Columns.Add(ImportRemoveColumn());
        ImportGrid.ItemsSource = _importRows;
        UpdateImportNotice();
        ImportOverlay.IsVisible = true;
    }

    private DataGridTemplateColumn ImportRemoveColumn()
    {
        return new DataGridTemplateColumn
        {
            Header = string.Empty,
            CanUserSort = false,
            CanUserReorder = false,
            MinWidth = 36,
            MaxWidth = 36,
            Width = new DataGridLength(36),
            CellTemplate = new FuncDataTemplate<ImportLine>((line, _) =>
            {
                var button = new Button { Classes = { "bo_doc_icon_button" } };
                ToolTip.SetTip(button, "Remover");
                button.Content = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
                {
                    Classes = { "bo_doc_row_icon" },
                    Path = "avares://LogicPOS.App/Assets/Images/Documents/botao_eliminar_b.svg"
                };
                button.Click += (_, _) => RemoveImportLine(line);
                return button;
            })
        };
    }

    private void OnImportRemoveClick(object? sender, RoutedEventArgs e)
    {
        var selected = ImportGrid.SelectedItems.OfType<ImportLine>().ToList();
        if (selected.Count == 0)
        {
            ImportNotice.Text = "Selecione as linhas a remover.";
            return;
        }

        foreach (var line in selected)
        {
            _importRows.Remove(line);
        }

        UpdateImportNotice();
    }

    private async void OnImportConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (_importSheet is null)
        {
            return;
        }

        if (_importRows.Count == 0)
        {
            ImportNotice.Text = "Não há linhas para importar.";
            return;
        }

        var action = _importAction;
        var fileName = _importFileName;
        var bytes = ExcelPreview.Write(_importSheet.Headers, _importRows.Select(line => line.Cells), _importSheet.HasHeader);
        ImportOverlay.IsVisible = false;
        await ExecuteAsync(action, null, fileName + "\n" + Convert.ToBase64String(bytes));
    }

    private void OnImportCancelClick(object? sender, RoutedEventArgs e) => ImportOverlay.IsVisible = false;

    private void RemoveImportLine(ImportLine line)
    {
        _importRows.Remove(line);
        UpdateImportNotice();
    }

    private void UpdateImportNotice()
    {
        ImportNotice.Text = _importRows.Count == 1
            ? "1 linha. Selecione as que não quer importar."
            : $"{_importRows.Count} linhas. Selecione as que não quer importar.";
    }

    private sealed class ImportLine
    {
        public ImportLine(string[] cells) => Cells = cells;

        public string[] Cells { get; }

        public string this[string key]
        {
            get
            {
                if (key.Length > 1 && key[0] == 'c' && int.TryParse(key.AsSpan(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out var index)
                    && index >= 0 && index < Cells.Length)
                {
                    return Cells[index];
                }

                return string.Empty;
            }
        }
    }

    private async Task RestoreFileAsync()
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is null)
        {
            return;
        }

        var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Restaurar de ficheiro",
            AllowMultiple = false
        });
        if (files.Count == 0)
        {
            return;
        }

        await ExecuteAsync("restore-file", null, files[0].Name);
    }

    private async Task OpenReportAsync()
    {
        if (Grid.SelectedItem is not ListingRow row)
        {
            Notice.Text = "Selecione um relatório.";
            return;
        }

        var service = Service;
        if (service is null)
        {
            return;
        }

        var start = StartDate.SelectedDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1);
        var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
        var result = await service.RunActionAsync("open-report", row.Id, $"{start:yyyy-MM-dd}|{end:yyyy-MM-dd}");
        if (result.Succeeded && string.IsNullOrWhiteSpace(result.Message) == false && File.Exists(result.Message))
        {
            Process.Start(new ProcessStartInfo(result.Message) { UseShellExecute = true });
            Notice.Text = "Relatório aberto.";
            return;
        }

        Notice.Text = result.Error ?? "Não foi possível abrir o relatório.";
    }

    private async Task ExecuteAsync(string action, Guid? id, string? extra)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        var result = await service.RunActionAsync(action, id, extra);
        if (result.Succeeded && string.IsNullOrWhiteSpace(result.Message) == false && File.Exists(result.Message))
        {
            Process.Start(new ProcessStartInfo(result.Message) { UseShellExecute = true });
            Notice.Text = "Ficheiro aberto.";
            await ReloadAsync();
            return;
        }

        Notice.Text = result.Error ?? result.Message ?? string.Empty;
        await ReloadAsync();
    }
}
