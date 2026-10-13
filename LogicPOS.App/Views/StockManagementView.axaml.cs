using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class StockManagementView : UserControl
{
    private enum StockTab
    {
        Articles,
        Movements,
        History,
        Warehouse
    }

    private enum EditorMode
    {
        None,
        CreateMovement,
        EditMovement,
        AdjustArticle,
        ChangeLocation,
        Exchange,
        EditUnique,
        WarehousePosition
    }

    private readonly List<object> _rows = [];
    private ListingLoad _load = null!;
    private StockTab _tab = StockTab.Articles;
    private EditorMode _editor = EditorMode.None;
    private int _page = 1;
    private const int PageSize = 50;
    private Guid? _editId;
    private byte[]? _attachedPdf;
    private AutoCompleteBox? _supplierBox;
    private CalendarDatePicker? _dateBox;
    private TextBox? _documentBox;
    private TextBox? _notesBox;
    private TextBlock? _attachLabel;
    private StackPanel? _linesHost;
    private TextBox? _quantityBox;
    private TextBox? _priceBox;
    private TextBox? _minimumBox;
    private TextBox? _totalBox;
    private ComboBox? _locationBox;
    private AutoCompleteBox? _exchangeBox;
    private AutoCompleteBox? _articleBox;
    private TextBox? _serialBox;
    private Func<Task>? _pendingConfirm;
    private Func<Task>? _pendingCancel;
    private readonly List<MovementLineControls> _lineControls = [];
    private readonly List<AutoCompleteBox> _childBoxes = [];
    private readonly HashSet<Guid> _labelIds = [];
    private ComboBox? _saleDocumentBox;
    private CalendarDatePicker? _saleDateBox;
    private decimal _editPrice;
    private Guid _editLocationId;
    private bool _moduleChecked;
    private bool _simpleOnly;
    private bool _filtersLoaded;
    private bool _printLabels = true;

    private sealed class MovementLineControls
    {
        public required AutoCompleteBox Article { get; init; }
        public required TextBox Quantity { get; init; }
        public required TextBox Price { get; init; }
        public required TextBox Serial { get; init; }
        public required ComboBox Location { get; init; }
    }

    public StockManagementView()
    {
        InitializeComponent();
        _load = new ListingLoad(Busy);
        ListingNotice.HideWhenEmpty(Notice);
        StartDate.SelectedDate = DateTime.Today.AddMonths(-1);
        EndDate.SelectedDate = DateTime.Today;
        ApplyToolbar();
        TouchFields.Attach(this);
        EditorOverlay.PropertyChanged += (_, args) =>
        {
            if (args.Property == IsVisibleProperty && EditorOverlay.IsVisible)
            {
                TouchFields.Attach(EditorHost);
            }
        };
    }

    public async Task ReloadAsync()
    {
        if (await EnsureModuleAsync() == false)
        {
            return;
        }

        await LoadAsync(resetPage: true);
    }

    private IStockManagementService? Service => AppComposition.Services?.GetService<IStockManagementService>();

    private async Task LoadAsync(bool resetPage)
    {
        var service = Service;
        if (service is null)
        {
            Notice.Text = "Serviço de stocks indisponível.";
            return;
        }

        if (resetPage)
        {
            _page = 1;
            _labelIds.Clear();
        }

        await _load.RunAsync(async () =>
        {
            var request = BuildRequest();
            _rows.Clear();
            switch (_tab)
            {
                case StockTab.Articles:
                {
                    var page = await service.GetArticlesAsync(request);
                    _rows.AddRange(page.Items);
                    CountLabel.Text = $"{page.TotalCount} artigo(s) · página {page.Page}/{page.TotalPages}";
                    break;
                }
                case StockTab.Movements:
                {
                    var page = await service.GetMovementsAsync(request);
                    _rows.AddRange(page.Items);
                    CountLabel.Text = $"{page.TotalCount} movimento(s) · página {page.Page}/{page.TotalPages}";
                    break;
                }
                case StockTab.History:
                {
                    var page = await service.GetHistoryAsync(request);
                    _rows.AddRange(page.Items);
                    CountLabel.Text = $"{page.TotalCount} série(s) · página {page.Page}/{page.TotalPages}";
                    break;
                }
                case StockTab.Warehouse:
                {
                    var page = await service.GetWarehouseArticlesAsync(request);
                    _rows.AddRange(page.Items);
                    CountLabel.Text = $"{page.TotalCount} posição(ões) · página {page.Page}/{page.TotalPages}";
                    break;
                }
            }

            BindGrid();
            ApplyToolbar();
            Notice.Text = string.Empty;
        });
    }

    private StockPageRequest BuildRequest() => new()
    {
        Page = _page,
        PageSize = PageSize,
        Search = string.IsNullOrWhiteSpace(SearchBox.Text) ? null : SearchBox.Text.Trim(),
        StartDate = _tab is StockTab.Movements or StockTab.History ? StartDate.SelectedDate?.Date : null,
        EndDate = _tab is StockTab.Movements or StockTab.History ? EndDate.SelectedDate?.Date : null,
        ArticleId = ListingFilters.SelectedLookup(FilterArticle)?.Id,
        CustomerId = _tab == StockTab.Movements ? SelectedFilterId(FilterCustomer) : null
    };

    private static Guid? SelectedFilterId(ComboBox box)
        => box.SelectedItem is LookupOption option && option.Id != Guid.Empty ? option.Id : null;

    private void BindGrid()
    {
        Grid.Columns.Clear();
        switch (_tab)
        {
            case StockTab.Articles:
                AddText("Code", "Código", 120);
                AddText("Designation", "Designação", 220, star: true);
                AddText("TotalStock", "Total Stock", 120);
                AddText("MinimumStock", "Stock mínimo", 120);
                AddText("Unit", "Unidade de medida", 140);
                AddText("UpdatedAt", "Atualizado em", 150);
                AddEditAction();
                break;
            case StockTab.Movements:
                AddText("Movement", "Movimento", 110);
                AddText("Date", "Data", 110);
                AddText("Customer", "Entidade", 160);
                AddText("DocumentNumber", "Número do Doc.", 150);
                AddText("Article", "Artigo", 200, star: true);
                AddText("Quantity", "Quantidade", 110);
                AddText("UpdatedAt", "Atualizado em", 150);
                AddEditAction();
                AddDeleteAction();
                break;
            case StockTab.History:
                AddLabelSelectColumn();
                AddText("Article", "Designação", 200, star: true);
                AddText("SerialNumber", "Número de série", 150);
                AddText("SoldText", "Vendido", 90);
                AddText("Status", "Estado", 110);
                AddText("ComposedText", "Artigo Composto", 130);
                AddText("PurchaseDate", "Data de Compra", 130);
                AddText("Supplier", "Fornecedor", 150);
                AddText("PurchasePrice", "Preço Compra", 120);
                AddText("OriginDocument", "Documento Origem", 150);
                AddText("SaleDocument", "Documento Venda", 150);
                AddText("Warehouse", "Armazém", 130);
                AddText("Location", "Localização", 130);
                AddText("UpdatedAt", "Atualizado em", 150);
                AddEditAction();
                AddDeleteAction();
                break;
            case StockTab.Warehouse:
                AddText("Warehouse", "Armazém", 140);
                AddText("Location", "Localização", 140);
                AddText("Article", "Designação", 220, star: true);
                AddText("SerialNumber", "Número de série", 150);
                AddText("Quantity", "Quantidade", 110);
                AddText("UpdatedAt", "Atualizado em", 150);
                AddEditAction();
                break;
        }

        Grid.ItemsSource = null;
        Grid.ItemsSource = _rows.ToList();
    }

    private void AddText(string binding, string header, double minWidth, bool star = false)
    {
        Grid.Columns.Add(new DataGridTextColumn
        {
            Header = header,
            Binding = new Binding(binding)
            {
                StringFormat = binding is "Quantity" or "Price" or "TotalStock" or "MinimumStock" or "PurchasePrice"
                    ? "{0:N3}"
                    : binding is "Date" or "PurchaseDate"
                        ? "{0:dd/MM/yyyy}"
                        : binding == "UpdatedAt"
                            ? "{0:dd/MM/yyyy HH:mm}"
                            : null
            },
            MinWidth = minWidth,
            MaxWidth = star ? double.PositiveInfinity : Math.Max(minWidth + 80, 280),
            Width = star
                ? new DataGridLength(1, DataGridLengthUnitType.Star)
                : new DataGridLength(1, DataGridLengthUnitType.Auto)
        });
    }

    private void AddEditAction()
    {
        Grid.Columns.Add(ActionColumn(
            "avares://LogicPOS.App/Assets/Images/Documents/botao_editar_b.svg",
            "Editar",
            row =>
            {
                Grid.SelectedItem = row;
                if (_tab == StockTab.Warehouse)
                {
                    OnChangeLocationClick(null, new RoutedEventArgs());
                    return;
                }

                OnEditClick(null, new RoutedEventArgs());
            },
            row => row is StockMovementRow movement ? movement.CanEdit : true));
    }

    private void AddDeleteAction()
    {
        Grid.Columns.Add(ActionColumn(
            "avares://LogicPOS.App/Assets/Images/Documents/botao_eliminar_b.svg",
            "Apagar",
            row =>
            {
                Grid.SelectedItem = row;
                OnDeleteClick(null, new RoutedEventArgs());
            },
            _ => true));
    }

    private static DataGridTemplateColumn ActionColumn(string icon, string tip, Action<object> click, Func<object, bool> enabled)
    {
        return new DataGridTemplateColumn
        {
            Header = string.Empty,
            CanUserSort = false,
            CanUserReorder = false,
            MinWidth = 36,
            MaxWidth = 36,
            Width = new DataGridLength(36),
            CellTemplate = new FuncDataTemplate<object>((row, _) =>
            {
                var button = new Button
                {
                    Classes = { "bo_doc_icon_button" },
                    IsEnabled = enabled(row)
                };
                ToolTip.SetTip(button, tip);
                button.Content = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
                {
                    Classes = { "bo_doc_row_icon" },
                    Path = icon
                };
                button.Click += (_, _) => click(row);
                return button;
            }, true)
        };
    }

    private void AddLabelSelectColumn()
    {
        Grid.Columns.Add(new DataGridTemplateColumn
        {
            Header = string.Empty,
            CanUserSort = false,
            CanUserReorder = false,
            MinWidth = 36,
            MaxWidth = 36,
            Width = new DataGridLength(36),
            CellTemplate = new FuncDataTemplate<object>((row, _) =>
            {
                if (row is not StockHistoryRow history)
                {
                    return new TextBlock();
                }

                var box = new CheckBox
                {
                    IsChecked = _labelIds.Contains(history.Id),
                    Classes = { "bo_entity_input" }
                };
                box.IsCheckedChanged += (_, _) =>
                {
                    if (box.IsChecked == true)
                    {
                        _labelIds.Add(history.Id);
                    }
                    else
                    {
                        _labelIds.Remove(history.Id);
                    }
                };
                return box;
            }, true)
        });
    }

    private void ApplyToolbar()
    {
        if (_simpleOnly)
        {
            TabHeaders.IsVisible = false;
            Grid.IsVisible = false;
            BtnInsert.IsVisible = true;
            BtnEdit.IsVisible = false;
            BtnDelete.IsVisible = false;
            BtnOrigin.IsVisible = false;
            BtnSale.IsVisible = false;
            BtnBarcode.IsVisible = false;
            BtnLocation.IsVisible = false;
            BtnExchange.IsVisible = false;
            StartDateField.IsVisible = false;
            EndDateField.IsVisible = false;
            ArticleFilterField.IsVisible = false;
            CustomerFilterField.IsVisible = false;
            return;
        }

        BtnInsert.IsVisible = _tab is StockTab.Movements or StockTab.Warehouse;
        BtnEdit.IsVisible = _tab is StockTab.Articles or StockTab.Movements or StockTab.History;
        BtnDelete.IsVisible = _tab is StockTab.Movements or StockTab.History;
        BtnOrigin.IsVisible = _tab is StockTab.Movements or StockTab.History;
        BtnSale.IsVisible = _tab is StockTab.Movements or StockTab.History;
        BtnBarcode.IsVisible = _tab == StockTab.History;
        BtnLocation.IsVisible = _tab is StockTab.History or StockTab.Warehouse;
        BtnExchange.IsVisible = _tab == StockTab.History;
        var showDates = _tab is StockTab.Movements or StockTab.History;
        StartDateField.IsVisible = showDates;
        EndDateField.IsVisible = showDates;
        StartDate.IsVisible = showDates;
        EndDate.IsVisible = showDates;
        ArticleFilterField.IsVisible = _tab == StockTab.Movements;
        CustomerFilterField.IsVisible = _tab == StockTab.Movements;
    }

    private async void OnTabClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string tag)
        {
            return;
        }

        _tab = tag switch
        {
            "movements" => StockTab.Movements,
            "history" => StockTab.History,
            "warehouse" => StockTab.Warehouse,
            _ => StockTab.Articles
        };

        TabArticles.Classes.Set("bo_entity_tab_on", _tab == StockTab.Articles);
        TabMovements.Classes.Set("bo_entity_tab_on", _tab == StockTab.Movements);
        TabHistory.Classes.Set("bo_entity_tab_on", _tab == StockTab.History);
        TabWarehouse.Classes.Set("bo_entity_tab_on", _tab == StockTab.Warehouse);
        if (_tab == StockTab.Movements)
        {
            await LoadMovementFiltersAsync();
        }

        await LoadAsync(resetPage: true);
    }

    private void OnFilterClick(object? sender, RoutedEventArgs e) => _ = LoadAsync(resetPage: true);

    private void OnSearchKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _ = LoadAsync(resetPage: true);
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        => SearchClear.IsVisible = string.IsNullOrEmpty(SearchBox.Text) == false;

    private void OnSearchClearClick(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        _ = LoadAsync(resetPage: true);
    }

    private void OnMoreClick(object? sender, RoutedEventArgs e)
    {
        _page++;
        _ = LoadAsync(resetPage: false);
    }

    private async void OnInsertClick(object? sender, RoutedEventArgs e)
    {
        if (_tab == StockTab.Movements)
        {
            await OpenCreateMovementAsync();
            return;
        }

        if (_tab == StockTab.Warehouse)
        {
            await OpenWarehousePositionAsync();
        }
    }

    private async void OnEditClick(object? sender, RoutedEventArgs e)
    {
        switch (_tab)
        {
            case StockTab.Articles when SelectedArticle() is { } article:
                OpenAdjustArticle(article);
                break;
            case StockTab.Movements when SelectedMovement() is { } movement:
                if (movement.CanEdit == false)
                {
                    Notice.Text = "Este movimento está ligado a um documento e não pode ser editado.";
                    return;
                }

                await OpenEditMovementAsync(movement);
                break;
            case StockTab.History when SelectedHistory() is { } history:
                if (string.IsNullOrWhiteSpace(history.SerialNumber))
                {
                    Notice.Text = "Selecione um número de série.";
                    return;
                }

                await OpenEditUniqueAsync(history);
                break;
        }
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_tab == StockTab.Movements && SelectedMovement() is { } movement)
        {
            AskConfirm(
                $"Confirma que deseja eliminar o movimento {movement.DocumentNumber}?",
                async () =>
                {
                    var service = Service;
                    if (service is null)
                    {
                        return;
                    }

                    var result = await service.DeleteMovementAsync(movement.Id);
                    Notice.Text = result.Succeeded ? result.Message ?? "Concluído." : result.Error ?? "Falha.";
                    if (result.Succeeded)
                    {
                        await LoadAsync(resetPage: false);
                    }
                });
            return;
        }

        if (_tab == StockTab.History && SelectedHistory() is { } history)
        {
            if (history.IsSold)
            {
                Notice.Text = "Não é possível apagar um número de série vendido.";
                return;
            }

            AskConfirm(
                $"Confirma que deseja eliminar o número de série {history.SerialNumber}?",
                async () =>
                {
                    var service = Service;
                    if (service is null)
                    {
                        return;
                    }

                    var result = await service.DeleteSerialAsync(history.WarehouseArticleId);
                    Notice.Text = result.Succeeded ? result.Message ?? "Concluído." : result.Error ?? "Falha.";
                    if (result.Succeeded)
                    {
                        await LoadAsync(resetPage: false);
                    }
                });
            return;
        }

        Notice.Text = "Selecione um registo.";
    }

    private void AskConfirm(string message, Func<Task> action, Func<Task>? cancel = null)
    {
        _pendingConfirm = action;
        _pendingCancel = cancel;
        ConfirmMessage.Text = message;
        ConfirmOverlay.IsVisible = true;
    }

    private async void OnConfirmYesClick(object? sender, RoutedEventArgs e)
    {
        ConfirmOverlay.IsVisible = false;
        var action = _pendingConfirm;
        _pendingConfirm = null;
        _pendingCancel = null;
        if (action is not null)
        {
            await action();
        }
    }

    private async void OnConfirmNoClick(object? sender, RoutedEventArgs e)
    {
        ConfirmOverlay.IsVisible = false;
        var action = _pendingCancel;
        _pendingConfirm = null;
        _pendingCancel = null;
        if (action is not null)
        {
            await action();
        }
    }

    private async void OnOriginClick(object? sender, RoutedEventArgs e)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        Guid? movementId = _tab switch
        {
            StockTab.Movements => SelectedMovement()?.Id,
            StockTab.History => SelectedHistory() is { } history && history.InMovementId != Guid.Empty
                ? history.InMovementId
                : null,
            _ => null
        };

        if (movementId is null)
        {
            Notice.Text = "Selecione um registo com documento de origem.";
            return;
        }

        try
        {
            var path = await service.SaveExternalDocumentAsync(movementId.Value);
            if (string.IsNullOrWhiteSpace(path))
            {
                Fail("Este registo não tem documento de origem anexado.");
                return;
            }

            await ShowPdfAsync(path, "Doc. origem");
        }
        catch (Exception exception)
        {
            Fail(exception.Message);
        }
    }

    private async void OnSaleClick(object? sender, RoutedEventArgs e)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        var number = _tab switch
        {
            StockTab.Movements => SelectedMovement()?.SaleDocumentNumber,
            StockTab.History => SelectedHistory()?.SaleDocument,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(number))
        {
            Notice.Text = "Este registo não tem documento de venda.";
            return;
        }

        try
        {
            var path = await service.SaveSaleDocumentPdfAsync(number);
            if (string.IsNullOrWhiteSpace(path))
            {
                Fail("Não foi possível abrir o documento de venda.");
                return;
            }

            await ShowPdfAsync(path, number);
        }
        catch (Exception exception)
        {
            Fail(exception.Message);
        }
    }

    private async void OnBarcodeClick(object? sender, RoutedEventArgs e)
    {
        var service = Service;
        var ids = _labelIds.ToList();
        if (ids.Count == 0 && SelectedHistory() is { } history && string.IsNullOrWhiteSpace(history.SerialNumber) == false)
        {
            ids.Add(history.Id);
        }

        if (service is null || ids.Count == 0)
        {
            Notice.Text = "Selecione um número de série.";
            return;
        }

        try
        {
            var path = await service.GenerateBarcodeLabelsAsync(ids);
            if (string.IsNullOrWhiteSpace(path))
            {
                Fail("Não foi possível gerar a etiqueta.");
                return;
            }

            if (path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                await ShowPdfAsync(path, "Etiqueta");
            }
            else
            {
                Notice.Text = $"Etiqueta gerada: {path}";
            }
        }
        catch (Exception exception)
        {
            Fail(exception.Message);
        }
    }

    private async void OnChangeLocationClick(object? sender, RoutedEventArgs e)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        Guid warehouseArticleId;
        Guid currentLocation;
        decimal quantity;
        if (_tab == StockTab.History && SelectedHistory() is { } history)
        {
            warehouseArticleId = history.WarehouseArticleId;
            currentLocation = history.WarehouseLocationId;
            quantity = 1;
        }
        else if (_tab == StockTab.Warehouse && SelectedWarehouse() is { } warehouse)
        {
            warehouseArticleId = warehouse.Id;
            currentLocation = warehouse.LocationId;
            quantity = warehouse.Quantity;
        }
        else
        {
            Notice.Text = "Selecione um registo.";
            return;
        }

        IReadOnlyList<StockLocationOption> locations;
        try
        {
            locations = await service.GetLocationsAsync();
        }
        catch (Exception exception)
        {
            Fail(exception.Message);
            return;
        }

        if (locations.Count == 0)
        {
            Fail("Não há localizações de armazém.");
            return;
        }

        _editor = EditorMode.ChangeLocation;
        _editId = warehouseArticleId;
        EditorTitle.Text = "Alterar localização do artigo";
        EditorHost.Children.Clear();
        if (_tab == StockTab.History && SelectedHistory() is { } historyRow)
        {
            EditorHost.Children.Add(ReadOnly("Artigo", historyRow.Article));
            EditorHost.Children.Add(ReadOnly("Número de série", historyRow.SerialNumber));
            EditorHost.Children.Add(ReadOnly("Armazém atual", historyRow.Warehouse));
        }
        else if (_tab == StockTab.Warehouse && SelectedWarehouse() is { } warehouseRow)
        {
            EditorHost.Children.Add(ReadOnly("Artigo", warehouseRow.Article));
            EditorHost.Children.Add(ReadOnly("Número de série", warehouseRow.SerialNumber));
            EditorHost.Children.Add(ReadOnly("Armazém atual", warehouseRow.Warehouse));
        }

        _locationBox = new ComboBox
        {
            ItemsSource = locations,
            SelectedItem = locations.FirstOrDefault(item => item.Id == currentLocation) ?? locations.FirstOrDefault()
        };
        _locationBox.Classes.Add("bo_entity_input");
        _quantityBox = Field("Quantidade", quantity.ToString("0.###", CultureInfo.CurrentCulture), out _);
        EditorHost.Children.Add(Label("Nova localização"));
        EditorHost.Children.Add(_locationBox);
        EditorHost.Children.Add(_quantityBox);
        EditorOverlay.IsVisible = true;
    }

    private async void OnExchangeClick(object? sender, RoutedEventArgs e)
    {
        var service = Service;
        if (service is null || SelectedHistory() is not { } history)
        {
            Notice.Text = "Selecione um número de série vendido.";
            return;
        }

        if (history.IsSold == false)
        {
            Notice.Text = "A troca só está disponível para artigos vendidos.";
            return;
        }

        var available = await service.GetAvailableSerialsAsync(history.ArticleId);
        if (available.Count == 0)
        {
            Notice.Text = "Não há números de série disponíveis para troca.";
            return;
        }

        _editor = EditorMode.Exchange;
        _editId = history.WarehouseArticleId;
        EditorTitle.Text = "Trocar artigo único";
        EditorHost.Children.Clear();
        _exchangeBox = new AutoCompleteBox { ItemsSource = available, Classes = { "bo_entity_input" } };
        ListingFilters.EnableLookupSearch(_exchangeBox);
        _exchangeBox.PlaceholderText = "Número de série";
        EditorHost.Children.Add(Label("Artigo para troca"));
        EditorHost.Children.Add(_exchangeBox);
        EditorOverlay.IsVisible = true;
    }

    private async Task OpenCreateMovementAsync()
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        var suppliers = await service.LookupSuppliersAsync();
        var articles = await service.LookupArticlesAsync();
        var locations = await service.GetLocationsAsync();
        if (suppliers.Count == 0)
        {
            Notice.Text = "Não há fornecedores para o movimento.";
            return;
        }

        if (articles.Count == 0)
        {
            Notice.Text = "Não há artigos.";
            return;
        }

        _editor = EditorMode.CreateMovement;
        _editId = null;
        _attachedPdf = null;
        EditorTitle.Text = "Inserir movimento de stock";
        EditorHost.Children.Clear();
        _lineControls.Clear();

        _supplierBox = SupplierSearch(suppliers, null);
        _dateBox = new CalendarDatePicker { SelectedDate = DateTime.Today };
        _dateBox.Classes.Add("bo_doc_date");
        _documentBox = Field("Documento", string.Empty, out var documentField);
        _documentBox.PlaceholderText = "Número do documento do fornecedor";
        _notesBox = Field("Notas", string.Empty, out var notesField);
        _attachLabel = new TextBlock { Text = "Sem anexo", Classes = { "bo_entity_notice" } };
        var attachButton = UploadButton();
        var addLine = new Button { Content = "Adicionar linha", Classes = { "bo_listing_columns" } };
        addLine.Click += (_, _) => AddMovementLine(articles, locations);

        EditorHost.Children.Add(Label("Fornecedor"));
        EditorHost.Children.Add(_supplierBox);
        EditorHost.Children.Add(Label("Data"));
        EditorHost.Children.Add(_dateBox);
        EditorHost.Children.Add(documentField);
        EditorHost.Children.Add(notesField);
        EditorHost.Children.Add(attachButton);
        EditorHost.Children.Add(_attachLabel);
        EditorHost.Children.Add(addLine);
        _linesHost = new StackPanel { Spacing = 8, Classes = { "bo_stock_lines" } };
        EditorHost.Children.Add(_linesHost);
        AddMovementLine(articles, locations);
        EditorOverlay.IsVisible = true;
    }

    private async Task OpenEditMovementAsync(StockMovementRow movement)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        var suppliers = await service.LookupSuppliersAsync();
        _editor = EditorMode.EditMovement;
        _editId = movement.Id;
        _attachedPdf = null;
        EditorTitle.Text = "Editar movimento";
        EditorHost.Children.Clear();
        _supplierBox = SupplierSearch(suppliers, suppliers.FirstOrDefault(item => item.Id == movement.CustomerId));
        _dateBox = new CalendarDatePicker { SelectedDate = movement.Date };
        _dateBox.Classes.Add("bo_doc_date");
        _documentBox = Field("Documento", movement.DocumentNumber, out var documentField);
        _quantityBox = Field("Quantidade", movement.Quantity.ToString("0.###", CultureInfo.CurrentCulture), out var quantityField);
        _priceBox = Field("Preço", movement.Price.ToString("0.###", CultureInfo.CurrentCulture), out var priceField);
        _attachLabel = new TextBlock
        {
            Text = movement.HasExternalDocument ? "Anexo atual mantido (escolha outro para substituir)" : "Sem anexo",
            Classes = { "bo_entity_notice" }
        };
        var attachButton = UploadButton();
        EditorHost.Children.Add(Label("Fornecedor"));
        EditorHost.Children.Add(_supplierBox);
        EditorHost.Children.Add(Label("Data"));
        EditorHost.Children.Add(_dateBox);
        EditorHost.Children.Add(documentField);
        EditorHost.Children.Add(quantityField);
        EditorHost.Children.Add(priceField);
        EditorHost.Children.Add(attachButton);
        EditorHost.Children.Add(_attachLabel);
        EditorHost.Children.Add(Label("Número de série (só leitura)"));
        EditorHost.Children.Add(new TextBox { Text = movement.SerialNumber, IsReadOnly = true, Classes = { "bo_entity_input" } });
        EditorOverlay.IsVisible = true;
    }

    private void OpenAdjustArticle(StockArticleRow article)
    {
        _editor = EditorMode.AdjustArticle;
        _editId = article.Id;
        EditorTitle.Text = $"Stock - {article.Designation}";
        EditorHost.Children.Clear();
        _totalBox = Field("Total stock", article.TotalStock.ToString("0.###", CultureInfo.CurrentCulture), out var totalField);
        _minimumBox = Field("Stock mínimo", article.MinimumStock.ToString("0.###", CultureInfo.CurrentCulture), out var minimumField);
        EditorHost.Children.Add(totalField);
        EditorHost.Children.Add(minimumField);
        EditorOverlay.IsVisible = true;
    }

    private async Task OpenEditUniqueAsync(StockHistoryRow history)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        IReadOnlyList<StockCompositionSlot> slots;
        IReadOnlyList<StockSaleDocumentOption> documents;
        try
        {
            slots = history.IsComposed
                ? await service.GetCompositionSlotsAsync(history.WarehouseArticleId, history.ArticleId)
                : [];
            documents = history.IsSold ? [] : await service.LookupSaleDocumentsAsync();
        }
        catch (Exception exception)
        {
            Fail(exception.Message);
            return;
        }

        _editor = EditorMode.EditUnique;
        _editId = history.WarehouseArticleId;
        _editPrice = history.ArticlePrice;
        _editLocationId = history.WarehouseLocationId;
        _childBoxes.Clear();
        EditorTitle.Text = "Editar Artigo Único";
        EditorHost.Children.Clear();
        EditorHost.Children.Add(ReadOnly("Artigo", history.Article));
        EditorHost.Children.Add(ReadOnly("Estado", history.Status));
        EditorHost.Children.Add(ReadOnly("Armazém", history.Warehouse));
        EditorHost.Children.Add(ReadOnly("Localização", history.Location));
        _serialBox = Field("Número de série", history.SerialNumber, out var serialField);
        _serialBox.IsReadOnly = history.IsSold;
        EditorHost.Children.Add(serialField);
        if (history.IsSold)
        {
            EditorHost.Children.Add(ReadOnly("Documento de venda", history.SaleDocument ?? string.Empty));
            EditorHost.Children.Add(ReadOnly("Data de venda", history.SaleDate?.ToString("yyyy-MM-dd") ?? string.Empty));
            EditorHost.Children.Add(new TextBlock
            {
                Text = "Artigo vendido: o número de série não pode ser alterado.",
                Classes = { "bo_doc_count" }
            });
        }

        foreach (var slot in slots)
        {
            var options = (await service.GetAvailableSerialsAsync(slot.ArticleId)).ToList();
            if (slot.ChildId is Guid childId && options.All(item => item.Id != childId))
            {
                options.Insert(0, new LookupOption
                {
                    Id = childId,
                    Label = string.IsNullOrWhiteSpace(slot.SerialNumber) ? childId.ToString("N") : slot.SerialNumber
                });
            }

            var combo = new AutoCompleteBox
            {
                ItemsSource = options,
                Classes = { "bo_entity_input" }
            };
            ListingFilters.EnableLookupSearch(combo);
            combo.PlaceholderText = "Número de série";
            ListingFilters.SelectLookup(combo, options.FirstOrDefault(item => item.Id == slot.ChildId) ?? options.FirstOrDefault());
            _childBoxes.Add(combo);
            EditorHost.Children.Add(Label(string.IsNullOrWhiteSpace(slot.Article) ? "Artigo filho" : slot.Article));
            EditorHost.Children.Add(combo);
        }

        if (history.IsSold == false)
        {
            _saleDocumentBox = new ComboBox
            {
                ItemsSource = documents,
                SelectedIndex = documents.Count > 0 ? 0 : -1,
                Classes = { "bo_entity_input" }
            };
            _saleDateBox = new CalendarDatePicker { SelectedDate = DateTime.Today, Classes = { "bo_doc_date" } };
            var sell = new Button { Content = "Registar saída", Classes = { "bo_listing_icon_button" } };
            sell.Click += async (_, _) => await RegisterSaleAsync(history);
            EditorHost.Children.Add(Label("Documento de venda"));
            EditorHost.Children.Add(_saleDocumentBox);
            EditorHost.Children.Add(Label("Data de venda"));
            EditorHost.Children.Add(_saleDateBox);
            EditorHost.Children.Add(sell);
        }

        EditorOverlay.IsVisible = true;
    }

    private async Task OpenWarehousePositionAsync()
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        var articles = await service.LookupArticlesAsync();
        var locations = await service.GetLocationsAsync();
        var suppliers = await service.LookupSuppliersAsync();
        if (articles.Count == 0 || locations.Count == 0)
        {
            Notice.Text = "Não há artigos ou localizações para criar a posição.";
            return;
        }

        _editor = EditorMode.WarehousePosition;
        _editId = null;
        _attachedPdf = null;
        EditorTitle.Text = "Gestão de Armazéns";
        EditorHost.Children.Clear();
        _articleBox = new AutoCompleteBox { ItemsSource = articles, Classes = { "bo_entity_input" } };
        ListingFilters.EnableLookupSearch(_articleBox);
        _serialBox = Field("Número de série", string.Empty, out var serialField);
        _locationBox = new ComboBox
        {
            ItemsSource = locations,
            SelectedItem = locations.FirstOrDefault(item => item.IsDefault) ?? locations.FirstOrDefault()
        };
        _locationBox.Classes.Add("bo_entity_input");
        _quantityBox = Field("Quantidade", "1", out var quantityField);
        _supplierBox = SupplierSearch(suppliers, null);
        _dateBox = new CalendarDatePicker { SelectedDate = DateTime.Today };
        _dateBox.Classes.Add("bo_doc_date");
        _documentBox = Field("Documento", string.Empty, out var documentField);
        _documentBox.PlaceholderText = "Número do documento do fornecedor";

        EditorHost.Children.Add(Label("Artigo"));
        EditorHost.Children.Add(_articleBox);
        EditorHost.Children.Add(serialField);
        EditorHost.Children.Add(Label("Localização"));
        EditorHost.Children.Add(_locationBox);
        EditorHost.Children.Add(quantityField);
        if (suppliers.Count > 0)
        {
            EditorHost.Children.Add(Label("Fornecedor"));
            EditorHost.Children.Add(_supplierBox);
        }

        EditorHost.Children.Add(Label("Data"));
        EditorHost.Children.Add(_dateBox);
        EditorHost.Children.Add(documentField);
        EditorOverlay.IsVisible = true;
    }

    private void AddMovementLine(IReadOnlyList<LookupOption> articles, IReadOnlyList<StockLocationOption> locations)
    {
        if (_linesHost is null)
        {
            return;
        }

        var panel = new StackPanel { Spacing = 4, Classes = { "bo_stock_line" } };
        var article = new AutoCompleteBox { ItemsSource = articles, Classes = { "bo_entity_input" } };
        ListingFilters.EnableLookupSearch(article);
        var quantity = new TextBox { Text = "1", Classes = { "bo_entity_input", "bo_numeric" } };
        var price = new TextBox { Text = "0", Classes = { "bo_entity_input", "bo_numeric" } };
        var serial = new TextBox { PlaceholderText = "Nº série", Classes = { "bo_entity_input" } };
        var location = new ComboBox
        {
            ItemsSource = locations,
            SelectedItem = locations.FirstOrDefault(item => item.IsDefault) ?? locations.FirstOrDefault()
        };
        location.Classes.Add("bo_entity_input");
        panel.Children.Add(Label("Artigo"));
        panel.Children.Add(article);
        panel.Children.Add(Label("Quantidade"));
        panel.Children.Add(quantity);
        panel.Children.Add(Label("Preço"));
        panel.Children.Add(price);
        panel.Children.Add(Label("Nº série"));
        panel.Children.Add(serial);
        panel.Children.Add(Label("Localização"));
        panel.Children.Add(location);
        _linesHost.Children.Add(panel);
        TouchFields.Attach(panel);
        _lineControls.Add(new MovementLineControls
        {
            Article = article,
            Quantity = quantity,
            Price = price,
            Serial = serial,
            Location = location
        });
    }

    private async Task PickAttachmentAsync()
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is null)
        {
            return;
        }

        var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Anexar documento",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("PDF") { Patterns = ["*.pdf"] }]
        });
        var file = files.FirstOrDefault();
        if (file is null)
        {
            return;
        }

        await using var stream = await file.OpenReadAsync();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        _attachedPdf = memory.ToArray();
        if (_attachLabel is not null)
        {
            _attachLabel.Text = file.Name;
        }
    }

    private async void OnEditorSaveClick(object? sender, RoutedEventArgs e)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        ListingSaveResult result;
        switch (_editor)
        {
            case EditorMode.CreateMovement:
            {
                var supplier = ListingFilters.SelectedLookup(_supplierBox);
                if (supplier is null || _dateBox?.SelectedDate is null)
                {
                    Notice.Text = "Preencha fornecedor e data.";
                    return;
                }

                var lines = new List<StockMovementLineInput>();
                foreach (var line in _lineControls)
                {
                    var article = ListingFilters.SelectedLookup(line.Article);
                    if (article is null)
                    {
                        if (string.IsNullOrWhiteSpace(line.Article.Text))
                        {
                            continue;
                        }

                        Notice.Text = "Selecione o artigo na lista.";
                        return;
                    }

                    if (decimal.TryParse(line.Quantity.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var quantity) == false || quantity == 0)
                    {
                        Notice.Text = "Quantidade inválida numa das linhas.";
                        return;
                    }

                    decimal.TryParse(line.Price.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price);
                    lines.Add(new StockMovementLineInput
                    {
                        ArticleId = article.Id,
                        Quantity = quantity,
                        Price = price,
                        SerialNumber = string.IsNullOrWhiteSpace(line.Serial.Text) ? null : line.Serial.Text.Trim(),
                        WarehouseLocationId = (line.Location.SelectedItem as StockLocationOption)?.Id
                    });
                }

                result = await service.CreateMovementAsync(new StockMovementCreateRequest
                {
                    SupplierId = supplier.Id,
                    Date = _dateBox.SelectedDate.Value.Date,
                    DocumentNumber = DocumentNumberOrReference(),
                    Notes = _notesBox?.Text,
                    ExternalDocument = _attachedPdf,
                    Items = lines
                });
                if (result.Succeeded)
                {
                    await ShowLabelsAsync(service, lines.Select(item => item.SerialNumber));
                }

                break;
            }
            case EditorMode.EditMovement when _editId is Guid id:
            {
                if (decimal.TryParse(_quantityBox?.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var quantity) == false)
                {
                    Notice.Text = "Quantidade inválida.";
                    return;
                }

                decimal.TryParse(_priceBox?.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price);
                result = await service.UpdateMovementAsync(new StockMovementUpdateRequest
                {
                    Id = id,
                    SupplierId = ListingFilters.SelectedLookup(_supplierBox)?.Id,
                    Date = _dateBox?.SelectedDate?.Date,
                    DocumentNumber = _documentBox?.Text?.Trim(),
                    Quantity = quantity,
                    Price = price,
                    ExternalDocument = _attachedPdf
                });
                break;
            }
            case EditorMode.AdjustArticle when _editId is Guid articleId:
            {
                if (decimal.TryParse(_totalBox?.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var total) == false ||
                    decimal.TryParse(_minimumBox?.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var minimum) == false)
                {
                    Notice.Text = "Valores de stock inválidos.";
                    return;
                }

                var minimumResult = await service.SaveMinimumStockAsync(articleId, minimum);
                if (minimumResult.Succeeded == false)
                {
                    result = minimumResult;
                    break;
                }

                result = await service.AdjustArticleStockAsync(articleId, total);
                break;
            }
            case EditorMode.ChangeLocation when _editId is Guid warehouseArticleId:
            {
                if (_locationBox?.SelectedItem is not StockLocationOption location ||
                    decimal.TryParse(_quantityBox?.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var quantity) == false)
                {
                    Notice.Text = "Localização ou quantidade inválida.";
                    return;
                }

                result = await service.ChangeLocationAsync(warehouseArticleId, location.Id, quantity);
                break;
            }
            case EditorMode.Exchange when _editId is Guid returnedId:
            {
                if (ListingFilters.SelectedLookup(_exchangeBox) is not LookupOption exchange)
                {
                    Notice.Text = "Selecione o artigo de troca.";
                    return;
                }

                result = await service.ExchangeUniqueArticleAsync(returnedId, exchange.Id);
                break;
            }
            case EditorMode.EditUnique when _editId is Guid uniqueId:
            {
                if (string.IsNullOrWhiteSpace(_serialBox?.Text))
                {
                    Notice.Text = "Indique o número de série.";
                    return;
                }

                List<Guid>? children = null;
                if (_childBoxes.Count > 0)
                {
                    children = [];
                    foreach (var box in _childBoxes)
                    {
                        if (ListingFilters.SelectedLookup(box) is not LookupOption child || child.Id == Guid.Empty)
                        {
                            Notice.Text = "Selecione o número de série de cada artigo filho.";
                            return;
                        }

                        children.Add(child.Id);
                    }
                }

                result = await service.UpdateUniqueArticleAsync(uniqueId, _serialBox.Text.Trim(), children);
                break;
            }
            case EditorMode.WarehousePosition:
            {
                if (ListingFilters.SelectedLookup(_articleBox) is not LookupOption article ||
                    _locationBox?.SelectedItem is not StockLocationOption location ||
                    _dateBox?.SelectedDate is null ||
                    decimal.TryParse(_quantityBox?.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var positionQty) == false ||
                    positionQty == 0)
                {
                    Notice.Text = "Preencha artigo, localização, data e quantidade.";
                    return;
                }

                var supplier = ListingFilters.SelectedLookup(_supplierBox);
                if (HasChoices(_supplierBox) && supplier is null)
                {
                    Notice.Text = "Selecione o fornecedor.";
                    return;
                }

                var positionSerial = string.IsNullOrWhiteSpace(_serialBox?.Text) ? null : _serialBox.Text.Trim();
                result = await service.CreateMovementAsync(new StockMovementCreateRequest
                {
                    SupplierId = supplier?.Id ?? Guid.Empty,
                    Date = _dateBox.SelectedDate.Value.Date,
                    DocumentNumber = DocumentNumberOrReference(),
                    Notes = "Posição de armazém",
                    Items =
                    [
                        new StockMovementLineInput
                        {
                            ArticleId = article.Id,
                            Quantity = positionQty,
                            Price = 0,
                            SerialNumber = positionSerial,
                            WarehouseLocationId = location.Id
                        }
                    ]
                });
                if (result.Succeeded)
                {
                    await ShowLabelsAsync(service, [positionSerial]);
                }

                break;
            }
            default:
                return;
        }

        Notice.Text = result.Succeeded ? string.Empty : result.Error ?? "Falha ao guardar.";
        if (result.Succeeded == false)
        {
            Toast.Error(this, Notice.Text);
            return;
        }

        Toast.Success(this, result.Message ?? "Guardado com sucesso.");
        EditorOverlay.IsVisible = false;
        _editor = EditorMode.None;
        await LoadAsync(resetPage: false);
    }

    private void OnEditorCancelClick(object? sender, RoutedEventArgs e)
    {
        EditorOverlay.IsVisible = false;
        _editor = EditorMode.None;
    }

    private StockArticleRow? SelectedArticle() => Grid.SelectedItem as StockArticleRow;

    private StockMovementRow? SelectedMovement() => Grid.SelectedItem as StockMovementRow;

    private StockHistoryRow? SelectedHistory() => Grid.SelectedItem as StockHistoryRow;

    private StockWarehouseRow? SelectedWarehouse() => Grid.SelectedItem as StockWarehouseRow;

    private async Task<bool> EnsureModuleAsync()
    {
        if (_moduleChecked)
        {
            return _simpleOnly == false;
        }

        var service = Service;
        if (service is null)
        {
            _moduleChecked = true;
            return true;
        }

        StockModuleAccess access;
        try
        {
            access = await service.GetModuleAccessAsync();
        }
        catch
        {
            access = new StockModuleAccess { HasModule = true };
        }

        _moduleChecked = true;
        if (access.HasModule)
        {
            return true;
        }

        _simpleOnly = true;
        _printLabels = false;
        ApplyToolbar();
        if (access.ShowAcquireMessage)
        {
            AskConfirm(
                "O módulo de gestão de stocks não está incluído nesta licença. Quer abrir a página para o adquirir?",
                () =>
                {
                    Process.Start(new ProcessStartInfo("https://logic-pos.com/") { UseShellExecute = true });
                    Notice.Text = "Módulo de stocks não incluído na licença.";
                    BtnInsert.IsVisible = false;
                    return Task.CompletedTask;
                },
                OpenCreateMovementAsync);
            return false;
        }

        await OpenCreateMovementAsync();
        return false;
    }

    private async Task LoadMovementFiltersAsync()
    {
        if (_filtersLoaded)
        {
            return;
        }

        var service = Service;
        if (service is null)
        {
            return;
        }

        var articles = await service.LookupArticlesAsync();
        var suppliers = await service.LookupSuppliersAsync();
        FilterArticle.ItemsSource = WithAll(articles);
        FilterCustomer.ItemsSource = WithAll(suppliers);
        ListingFilters.EnableLookupSearch(FilterArticle);
        ListingFilters.SelectLookup(FilterArticle, null);
        FilterCustomer.SelectedIndex = 0;
        _filtersLoaded = true;
    }

    private static List<LookupOption> WithAll(IReadOnlyList<LookupOption> options)
    {
        var items = new List<LookupOption> { new() { Id = Guid.Empty, Label = "Todos" } };
        items.AddRange(options);
        return items;
    }

    private async Task RegisterSaleAsync(StockHistoryRow history)
    {
        var service = Service;
        if (service is null)
        {
            return;
        }

        if (_saleDocumentBox?.SelectedItem is not StockSaleDocumentOption document || _saleDateBox?.SelectedDate is null)
        {
            Fail("Selecione o documento e a data de venda.");
            return;
        }

        var result = await service.CreateMovementAsync(new StockMovementCreateRequest
        {
            SupplierId = document.CustomerId,
            Date = _saleDateBox.SelectedDate.Value.Date,
            DocumentNumber = document.Number,
            Notes = "Saída de artigo único",
            Items =
            [
                new StockMovementLineInput
                {
                    ArticleId = history.ArticleId,
                    Quantity = -1,
                    Price = _editPrice,
                    SerialNumber = string.IsNullOrWhiteSpace(_serialBox?.Text) ? history.SerialNumber : _serialBox.Text.Trim(),
                    WarehouseLocationId = _editLocationId == Guid.Empty ? null : _editLocationId
                }
            ]
        });
        if (result.Succeeded == false)
        {
            Fail(result.Error ?? "Não foi possível registar a saída.");
            return;
        }

        Toast.Success(this, result.Message ?? "Saída registada.");
        EditorOverlay.IsVisible = false;
        _editor = EditorMode.None;
        await LoadAsync(resetPage: false);
    }

    private async Task ShowLabelsAsync(IStockManagementService service, IEnumerable<string?> serials)
    {
        if (_printLabels == false)
        {
            return;
        }

        var values = serials.Where(item => string.IsNullOrWhiteSpace(item) == false).Select(item => item!.Trim()).Distinct().ToList();
        if (values.Count == 0)
        {
            return;
        }

        try
        {
            var path = await service.GenerateBarcodeLabelsBySerialAsync(values);
            if (string.IsNullOrWhiteSpace(path) == false && path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                await ShowPdfAsync(path, "Etiquetas");
            }
        }
        catch (Exception exception)
        {
            Fail(exception.Message);
        }
    }

    private void Fail(string message)
    {
        Notice.Text = message;
        Toast.Error(this, message);
    }

    private async Task ShowPdfAsync(string path, string title)
    {
        if (TopLevel.GetTopLevel(this) is IOfficeSurface office)
        {
            await office.ShowPdfAsync(path, title);
            return;
        }

        Fail("Não foi possível mostrar o documento.");
    }

    private static TextBlock Label(string text) => new()
    {
        Text = text,
        Classes = { "bo_doc_label" }
    };

    private static StackPanel ReadOnly(string label, string value)
    {
        var host = new StackPanel { Spacing = 2 };
        host.Children.Add(Label(label));
        host.Children.Add(new TextBox
        {
            Text = value,
            IsReadOnly = true,
            Classes = { "bo_entity_input" }
        });
        return host;
    }

    private static AutoCompleteBox SupplierSearch(IReadOnlyList<LookupOption> suppliers, LookupOption? selected)
    {
        var box = new AutoCompleteBox
        {
            ItemsSource = suppliers,
            Classes = { "bo_entity_input" }
        };
        ListingFilters.EnableLookupSearch(box);
        box.PlaceholderText = "Nome do fornecedor";
        if (selected is not null)
        {
            ListingFilters.SelectLookup(box, selected);
        }

        return box;
    }

    private static bool HasChoices(AutoCompleteBox? box)
        => box?.ItemsSource is System.Collections.ICollection { Count: > 0 };

    private Button UploadButton()
    {
        var button = new Button
        {
            Classes = { "bo_listing_icon_button" },
            Content = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
            {
                Classes = { "bo_listing_icon" },
                Path = "avares://LogicPOS.App/Assets/Images/Listing/botao_importar_b.svg"
            }
        };
        ToolTip.SetTip(button, "Anexar");
        button.Click += async (_, _) => await PickAttachmentAsync();
        return button;
    }

    private string DocumentNumberOrReference()
    {
        var typed = _documentBox?.Text?.Trim();
        return string.IsNullOrWhiteSpace(typed) ? $"ST{DateTime.Now:yyyyMMddHHmmss}" : typed;
    }

    private static TextBox Field(string label, string value, out StackPanel host)
    {
        var box = new TextBox { Text = value };
        box.Classes.Add("bo_entity_input");
        host = new StackPanel { Spacing = 2 };
        host.Children.Add(Label(label));
        host.Children.Add(box);
        return box;
    }
}

