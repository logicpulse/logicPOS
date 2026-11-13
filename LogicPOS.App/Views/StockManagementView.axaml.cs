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
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using LogicPOS.App;
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
    private StockTab _tab = StockTab.History;
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
    private TextBlock? _attachHint;
    private StackPanel? _linesHost;
    private TextBlock? _linesCountBadge;
    private StackPanel? _linesEmpty;
    private readonly List<CommittedMovementLine> _committedLines = [];
    private IReadOnlyList<StockLocationOption> _draftLocations = [];
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
    private bool _syncingSelection;
    private ComboBox? _saleDocumentBox;
    private CalendarDatePicker? _saleDateBox;
    private decimal _editPrice;
    private Guid _editLocationId;
    private bool _moduleChecked;
    private bool _simpleOnly;
    private bool _filtersLoaded;
    private bool _printLabels = true;
    private bool _portrait;
    private UiViewport _viewport;
    private Panel? _stockFiltersHome;

    private sealed class MovementLineControls
    {
        public required AutoCompleteBox Article { get; init; }
        public required TextBox Quantity { get; init; }
        public required TextBox Price { get; init; }
        public required StackPanel SerialHost { get; init; }
        public List<TextBox> Serials { get; } = new();
        /// <summary>Survives quantity edits so typed serials are not wiped while rebuilding boxes.</summary>
        public List<string> SerialValues { get; } = new();
        public required ComboBox Location { get; init; }
    }

    private sealed class CommittedMovementLine
    {
        public required Guid ArticleId { get; init; }
        public required string ArticleLabel { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal Price { get; init; }
        public required IReadOnlyList<string> Serials { get; init; }
        public Guid? WarehouseLocationId { get; init; }
        public required string LocationLabel { get; init; }
    }

    public StockManagementView()
    {
        InitializeComponent();
        Grid.SelectionChanged += OnGridSelectionChanged;
        _load = new ListingLoad(Busy);
        ListingNotice.BindToast(Notice, this, skipProgress: true);
        ListingNotice.BindToast(EditorNotice, this);
        Period.SetRange(
            DateTime.Today.AddMonths(-1),
            DateTime.Today.AddDays(1).AddSeconds(-1));
        Period.PeriodChanged += (_, _) =>
        {
            if (Period.IsVisible)
            {
                _ = LoadAsync(resetPage: true);
            }
        };
        ApplyToolbar();
        TouchFields.Attach(this);
        EditorOverlay.PropertyChanged += (_, args) =>
        {
            if (args.Property == IsVisibleProperty && EditorOverlay.IsVisible)
            {
                TouchFields.Attach(EditorHost);
            }
        };
        // No recycling: cards bake title/Tag from the row; recycled cells would show the wrong item.
        CardList.ItemTemplate = new FuncDataTemplate<object>((row, _) => BuildStockCard(row), false);
        AttachedToVisualTree += (_, _) =>
        {
            RelayoutForViewport();
            LayoutUpdated += OnStockLayoutUpdated;
        };
        DetachedFromVisualTree += (_, _) => LayoutUpdated -= OnStockLayoutUpdated;
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
                    CountLabel.Text = $"{page.TotalCount} artigo(s) — página {page.Page}/{page.TotalPages}";
                    break;
                }
                case StockTab.Movements:
                {
                    var page = await service.GetMovementsAsync(request);
                    _rows.AddRange(page.Items);
                    CountLabel.Text = $"{page.TotalCount} movimento(s) — página {page.Page}/{page.TotalPages}";
                    break;
                }
                case StockTab.History:
                {
                    var page = await service.GetHistoryAsync(request);
                    _rows.AddRange(page.Items);
                    CountLabel.Text = $"{page.TotalCount} série(s) — página {page.Page}/{page.TotalPages}";
                    break;
                }
                case StockTab.Warehouse:
                {
                    var page = await service.GetWarehouseArticlesAsync(request);
                    _rows.AddRange(page.Items);
                    CountLabel.Text = $"{page.TotalCount} posição(ões) — página {page.Page}/{page.TotalPages}";
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
        StartDate = _tab is StockTab.Movements or StockTab.History ? Period.StartDate : null,
        EndDate = _tab is StockTab.Movements or StockTab.History ? Period.EndDate : null,
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
                AddEditArticleAction();
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

        var page = _rows.ToList();
        _syncingSelection = true;
        Grid.ItemsSource = null;
        Grid.ItemsSource = page;
        CardList.ItemsSource = page;
        _syncingSelection = false;
        if (_tab == StockTab.History)
        {
            SyncGridSelectionFromLabels();
        }
    }

    private void AddText(string binding, string header, double minWidth, bool star = false)
    {
        // Header TextBlock: ellipsis when narrow + tooltip with the full title on hover.
        var headerBlock = new TextBlock
        {
            Text = header,
            TextTrimming = TextTrimming.CharacterEllipsis,
            TextWrapping = TextWrapping.NoWrap
        };
        ToolTip.SetTip(headerBlock, header);
        Grid.Columns.Add(new DataGridTextColumn
        {
            Header = headerBlock,
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
            MinWidth = Math.Max(minWidth, EstimateHeaderWidth(header)),
            MaxWidth = star ? double.PositiveInfinity : Math.Max(minWidth + 120, 320),
            Width = star
                ? new DataGridLength(1, DataGridLengthUnitType.Star)
                : new DataGridLength(1, DataGridLengthUnitType.Auto)
        });
    }

    private static double EstimateHeaderWidth(string header)
        => Math.Max(96, (header?.Length ?? 0) * 8 + 28);

    private void AddEditAction()
    {
        Grid.Columns.Add(ActionColumn(
            "avares://LogicPOS.App/Assets/Images/Documents/botao_editar_b.svg",
            "Editar",
            row =>
            {
                SelectRowWithoutLabelSync(row);
                if (_tab == StockTab.Warehouse)
                {
                    OnChangeLocationClick(null, new RoutedEventArgs());
                    return;
                }

                OnEditClick(null, new RoutedEventArgs());
            },
            row => row is StockMovementRow movement ? movement.CanEdit : true));
    }

    private void AddEditArticleAction()
    {
        Grid.Columns.Add(ActionColumn(
            "avares://LogicPOS.App/Assets/Images/Documents/botao_editar_b.svg",
            "Editar artigo",
            row =>
            {
                SelectRowWithoutLabelSync(row);
                if (row is StockHistoryRow history)
                {
                    _ = OpenHistoryArticleAsync(history);
                }
            },
            row => row is StockHistoryRow history && history.ArticleId != Guid.Empty));
    }

    private async Task OpenHistoryArticleAsync(StockHistoryRow history)
    {
        if (history.ArticleId == Guid.Empty)
        {
            Notice.Text = "Esta linha não tem artigo associado.";
            return;
        }

        if (this.FindAncestorOfType<BackOfficeWindow>() is not BackOfficeWindow office)
        {
            Notice.Text = "Não foi possível abrir o artigo.";
            return;
        }

        try
        {
            await office.EditEntityAsync("Artigos", history.ArticleId, returnToStock: true);
        }
        catch (Exception exception)
        {
            Fail(exception.Message);
        }
    }

    private void AddDeleteAction()
    {
        Grid.Columns.Add(ActionColumn(
            "avares://LogicPOS.App/Assets/Images/Documents/botao_eliminar_b.svg",
            "Apagar",
            row =>
            {
                SelectRowWithoutLabelSync(row);
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
            // No recycling: recycled cells would keep the previous row in the click closure.
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
                button.Click += (_, _) =>
                {
                    var current = button.DataContext
                        ?? button.FindAncestorOfType<DataGridRow>()?.DataContext
                        ?? row;
                    if (current is not null)
                    {
                        click(current);
                    }
                };
                return button;
            }, false)
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
            // supportsRecycling must stay false: recycled checkboxes kept the previous row Id in the click handler.
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
                    if (_syncingSelection)
                    {
                        return;
                    }

                    // Prefer live DataContext — row containers can be rebound after scroll.
                    var id = Guid.Empty;
                    if (box.DataContext is StockHistoryRow bound)
                    {
                        id = bound.Id;
                    }
                    else if (box.FindAncestorOfType<DataGridRow>()?.DataContext is StockHistoryRow live)
                    {
                        id = live.Id;
                    }
                    else
                    {
                        id = history.Id;
                    }

                    if (id == Guid.Empty)
                    {
                        return;
                    }

                    if (box.IsChecked == true)
                    {
                        _labelIds.Add(id);
                    }
                    else
                    {
                        _labelIds.Remove(id);
                    }

                    SyncGridSelectionFromLabels();
                };
                return box;
            }, false)
        });
    }

    private void OnGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_syncingSelection || _tab != StockTab.History)
        {
            return;
        }

        SyncLabelsFromGridSelection();
    }

    private void SelectRowWithoutLabelSync(object row)
    {
        _syncingSelection = true;
        try
        {
            Grid.SelectedItem = row;
        }
        finally
        {
            _syncingSelection = false;
        }
    }

    private void SyncLabelsFromGridSelection()
    {
        _syncingSelection = true;
        try
        {
            var visible = _rows.OfType<StockHistoryRow>().Select(row => row.Id).ToHashSet();
            var selected = Grid.SelectedItems?.OfType<StockHistoryRow>().Select(row => row.Id).ToHashSet() ?? [];
            foreach (var id in visible)
            {
                _labelIds.Remove(id);
            }

            foreach (var id in selected)
            {
                _labelIds.Add(id);
            }

            ApplyLabelChecks();
        }
        finally
        {
            _syncingSelection = false;
        }
    }

    private void SyncGridSelectionFromLabels()
    {
        _syncingSelection = true;
        try
        {
            Grid.SelectedItems.Clear();
            foreach (var row in _rows.OfType<StockHistoryRow>())
            {
                if (_labelIds.Contains(row.Id))
                {
                    Grid.SelectedItems.Add(row);
                }
            }
        }
        finally
        {
            _syncingSelection = false;
        }
    }

    private void ApplyLabelChecks()
    {
        foreach (var row in Grid.GetVisualDescendants().OfType<DataGridRow>())
        {
            if (row.DataContext is not StockHistoryRow history)
            {
                continue;
            }

            var box = row.GetVisualDescendants().OfType<CheckBox>().FirstOrDefault();
            if (box is null || box.IsChecked == _labelIds.Contains(history.Id))
            {
                continue;
            }

            box.IsChecked = _labelIds.Contains(history.Id);
        }
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
            Period.IsVisible = false;
            ArticleFilterField.IsVisible = false;
            CustomerFilterField.IsVisible = false;
            return;
        }

        BtnInsert.IsVisible = _tab is StockTab.History or StockTab.Movements or StockTab.Warehouse;
        BtnEdit.IsVisible = _tab is StockTab.Articles or StockTab.Movements or StockTab.History;
        BtnDelete.IsVisible = _tab is StockTab.Movements or StockTab.History;
        BtnOrigin.IsVisible = _tab is StockTab.Movements or StockTab.History;
        BtnSale.IsVisible = _tab is StockTab.Movements or StockTab.History;
        BtnBarcode.IsVisible = _tab == StockTab.History;
        BtnLocation.IsVisible = _tab is StockTab.History or StockTab.Warehouse;
        BtnExchange.IsVisible = _tab == StockTab.History;
        var showDates = _tab is StockTab.Movements or StockTab.History;
        Period.IsVisible = showDates;
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
        if (_tab is StockTab.History or StockTab.Movements)
        {
            await OpenCreateMovementAsync();
            return;
        }

        if (_tab == StockTab.Warehouse)
        {
            await OpenWarehousePositionAsync();
        }
    }

    private void OnGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (e.Source is Visual visual && visual.FindAncestorOfType<Button>() is not null)
        {
            return;
        }

        var row = (e.Source as Visual)?.FindAncestorOfType<DataGridRow>()?.DataContext
            ?? Grid.SelectedItem;
        if (row is null)
        {
            return;
        }

        // Same path as the row pencil: warehouse → location; other tabs → toolbar Edit.
        SelectRowWithoutLabelSync(row);
        if (_tab == StockTab.Warehouse)
        {
            OnChangeLocationClick(null, new RoutedEventArgs());
            return;
        }

        if (_tab is StockTab.Articles or StockTab.Movements or StockTab.History)
        {
            OnEditClick(null, new RoutedEventArgs());
        }
    }

    private async void OnEditClick(object? sender, RoutedEventArgs e)
    {
        switch (_tab)
        {
            case StockTab.Articles when SelectedArticle() is { } article:
                await OpenFullArticleAsync(article);
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
            var documentLabel = string.IsNullOrWhiteSpace(movement.DocumentNumber)
                ? movement.Id.ToString("N")[..8]
                : movement.DocumentNumber;
            AskConfirm(
                $"Confirma que deseja eliminar o movimento {documentLabel}?",
                async () =>
                {
                    try
                    {
                        var service = Service;
                        if (service is null)
                        {
                            Fail("Serviço de stocks indisponível.");
                            return;
                        }

                        var result = await service.DeleteMovementAsync(movement.Id);
                        if (result.Succeeded == false)
                        {
                            Fail(result.Error ?? "Não foi possível apagar o movimento.");
                            return;
                        }

                        Toast.Success(this, result.Message ?? "Movimento apagado.");
                        Notice.Text = string.Empty;
                        await LoadAsync(resetPage: false);
                    }
                    catch (Exception exception)
                    {
                        Fail(exception.Message);
                    }
                });
            return;
        }

        if (_tab == StockTab.History && SelectedHistory() is { } history)
        {
            if (history.IsSold)
            {
                Fail("Não é possível apagar um número de série vendido.");
                return;
            }

            AskConfirm(
                $"Confirma que deseja eliminar o número de série {history.SerialNumber}?",
                async () =>
                {
                    try
                    {
                        var service = Service;
                        if (service is null)
                        {
                            Fail("Serviço de stocks indisponível.");
                            return;
                        }

                        var result = await service.DeleteSerialAsync(history.WarehouseArticleId);
                        if (result.Succeeded == false)
                        {
                            Fail(result.Error ?? "Não foi possível apagar o número de série.");
                            return;
                        }

                        Toast.Success(this, result.Message ?? "Número de série apagado.");
                        Notice.Text = string.Empty;
                        await LoadAsync(resetPage: false);
                    }
                    catch (Exception exception)
                    {
                        Fail(exception.Message);
                    }
                });
            return;
        }

        Fail("Selecione um registo.");
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
            var result = await service.GenerateBarcodeLabelsAsync(ids);
            if (result is null || string.IsNullOrWhiteSpace(result.Path))
            {
                Fail("Não foi possível gerar a etiqueta.");
                return;
            }

            if (result.Path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                await ShowPdfAsync(result.Path, "Etiqueta", result.PreferredPrinter, result);
            }
            else
            {
                Notice.Text = $"Etiqueta gerada: {result.Path}";
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
        DialogTitleIcons.ApplyEdit(EditorTitleIcon);
        ResetEditorShell();
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
        _quantityBox = Field("Quantidade", FormDecimal.Format(quantity), out var quantityField, FormDecimal.QuantityRule);
        EditorNotice.Text = string.Empty;
        EditorHost.Children.Add(Label("Nova localização"));
        EditorHost.Children.Add(_locationBox);
        EditorHost.Children.Add(quantityField);
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
        DialogTitleIcons.ApplyEdit(EditorTitleIcon);
        ResetEditorShell();
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
        DialogTitleIcons.ApplyCreate(EditorTitleIcon);
        ResetEditorShell(split: true);
        _lineControls.Clear();
        _committedLines.Clear();
        _draftLocations = locations;

        _supplierBox = SupplierSearch(suppliers, null);
        _supplierBox.PlaceholderText = "Selecione o fornecedor...";
        _dateBox = new CalendarDatePicker { SelectedDate = DateTime.Today };
        _dateBox.Classes.Add("bo_doc_date");
        _documentBox = Field("Documento", string.Empty, out var documentField);
        _documentBox.PlaceholderText = "Número do documento do fornecedor...";
        _notesBox = new TextBox
        {
            Text = string.Empty,
            PlaceholderText = "Observações do movimento...",
            AcceptsReturn = true,
            MinHeight = 96,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Stretch,
            Classes = { "bo_entity_input" }
        };
        _dateBox.SelectedDate = DateTime.Today;
        EditorNotice.Text = string.Empty;

        var dadosTop = new StackPanel { Classes = { "bo_stock_card_body" } };
        dadosTop.Children.Add(Label("Fornecedor"));
        dadosTop.Children.Add(_supplierBox);
        dadosTop.Children.Add(Label("Data"));
        dadosTop.Children.Add(_dateBox);
        dadosTop.Children.Add(documentField);
        var notesLabel = Label("Notas");
        var notesBlock = new DockPanel { Margin = new Thickness(0, 4, 0, 0) };
        DockPanel.SetDock(notesLabel, Dock.Top);
        notesBlock.Children.Add(notesLabel);
        notesBlock.Children.Add(_notesBox);
        var dadosBody = new DockPanel();
        DockPanel.SetDock(dadosTop, Dock.Top);
        dadosBody.Children.Add(dadosTop);
        dadosBody.Children.Add(notesBlock);

        var dadosCard = StockCard(
            "Dados do movimento",
            "avares://LogicPOS.App/Assets/Images/Dialogs/icon_window_documents.png",
            dadosBody,
            stretch: true);
        var anexosCard = StockCard(
            "Anexos",
            "avares://LogicPOS.App/Assets/Images/Listing/botao_importar_b.svg",
            BuildAttachmentDropzone(stretch: true),
            stretch: true);

        var left = new Avalonia.Controls.Grid
        {
            Classes = { "bo_stock_editor_col" },
            VerticalAlignment = VerticalAlignment.Stretch
        };
        left.RowDefinitions.Add(new RowDefinition(new GridLength(1.35, GridUnitType.Star)));
        left.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        Avalonia.Controls.Grid.SetRow(dadosCard, 0);
        Avalonia.Controls.Grid.SetRow(anexosCard, 1);
        left.Children.Add(dadosCard);
        left.Children.Add(anexosCard);

        var composer = BuildDraftComposer(articles, locations);
        _linesHost = new StackPanel { Classes = { "bo_stock_lines" } };
        _linesCountBadge = new TextBlock { Text = "0 itens", Classes = { "bo_stock_card_badge" } };
        _linesEmpty = new StackPanel { Classes = { "bo_stock_empty" } };
        _linesEmpty.Children.Add(new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
        {
            Classes = { "bo_stock_card_icon" },
            Path = "avares://LogicPOS.App/Assets/Images/BackOffice/Menu/menu_artigos_gestao_stocks.svg",
            Width = 40,
            Height = 40,
            Opacity = 0.45
        });
        _linesEmpty.Children.Add(new TextBlock
        {
            Text = "Ainda não adicionou artigos. Selecione um artigo e clique em Adicionar.",
            Classes = { "bo_stock_empty_text" }
        });
        var linesBody = new DockPanel();
        linesBody.Children.Add(_linesEmpty);
        linesBody.Children.Add(_linesHost);
        var linesCard = StockCard(
            "Linhas do movimento",
            "avares://LogicPOS.App/Assets/Images/BackOffice/Menu/menu_artigos_gestao_stocks.svg",
            linesBody,
            _linesCountBadge,
            stretch: true);

        var right = new Avalonia.Controls.Grid
        {
            Classes = { "bo_stock_editor_col" },
            VerticalAlignment = VerticalAlignment.Stretch
        };
        right.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        right.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        Avalonia.Controls.Grid.SetRow(composer, 0);
        Avalonia.Controls.Grid.SetRow(linesCard, 1);
        right.Children.Add(composer);
        right.Children.Add(linesCard);

        var split = new Avalonia.Controls.Grid
        {
            Classes = { "bo_stock_editor_split_grid" },
            MinHeight = 520
        };
        split.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1.1, GridUnitType.Star)));
        split.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1.6, GridUnitType.Star)));
        Avalonia.Controls.Grid.SetColumn(left, 0);
        Avalonia.Controls.Grid.SetColumn(right, 1);
        split.Children.Add(left);
        split.Children.Add(right);
        EditorHost.Children.Add(split);
        RefreshCommittedLinesUi();
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
        DialogTitleIcons.ApplyEdit(EditorTitleIcon);
        ResetEditorShell();
        _supplierBox = SupplierSearch(suppliers, suppliers.FirstOrDefault(item => item.Id == movement.CustomerId));
        _dateBox = new CalendarDatePicker { SelectedDate = movement.Date };
        _dateBox.Classes.Add("bo_doc_date");
        _documentBox = Field("Documento", movement.DocumentNumber, out var documentField);
        _quantityBox = Field("Quantidade", FormDecimal.Format(movement.Quantity), out var quantityField, FormDecimal.QuantityRule);
        _priceBox = Field("Preço", FormDecimal.Format(movement.Price, FormDecimalKind.Money), out var priceField, FormDecimal.MoneyRule);
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

    private async Task OpenFullArticleAsync(StockArticleRow article)
    {
        if (this.FindAncestorOfType<BackOfficeWindow>() is not BackOfficeWindow office)
        {
            OpenAdjustArticle(article);
            return;
        }

        try
        {
            await office.EditEntityAsync("Artigos", article.Id, returnToStock: true);
        }
        catch (Exception exception)
        {
            Fail(exception.Message);
        }
    }

    private void OpenAdjustArticle(StockArticleRow article)
    {
        _editor = EditorMode.AdjustArticle;
        _editId = article.Id;
        EditorTitle.Text = $"Stock - {article.Designation}";
        DialogTitleIcons.ApplyEdit(EditorTitleIcon);
        ResetEditorShell();
        _totalBox = Field("Total stock", FormDecimal.Format(article.TotalStock), out var totalField, FormDecimal.QuantityRule);
        _minimumBox = Field("Stock mínimo", FormDecimal.Format(article.MinimumStock), out var minimumField, FormDecimal.QuantityRule);
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
        DialogTitleIcons.ApplyEdit(EditorTitleIcon);
        ResetEditorShell();
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
            // Do not pre-select a document: first item is the latest invoice and would bind by accident.
            _saleDocumentBox = new ComboBox
            {
                ItemsSource = documents,
                SelectedIndex = -1,
                PlaceholderText = documents.Count > 0 ? "Selecione o documento de venda…" : "Sem documentos de venda",
                Classes = { "bo_entity_input" }
            };
            _saleDateBox = new CalendarDatePicker { SelectedDate = DateTime.Today, Classes = { "bo_doc_date" } };
            // bo_listing_icon_button is 32x32 (icon-only) and truncates labels to "Reg." — use text action.
            var sell = new Button { Content = "Registar saída", Classes = { "bo_doc_action" } };
            ToolTip.SetTip(sell, "Associa este número de série a um documento de venda e marca-o como vendido.");
            sell.IsEnabled = documents.Count > 0;
            sell.Click += (_, _) =>
            {
                if (_saleDocumentBox?.SelectedItem is not StockSaleDocumentOption document || _saleDateBox?.SelectedDate is null)
                {
                    Fail("Selecione o documento e a data de venda.");
                    return;
                }

                AskConfirm(
                    $"Confirma a saída do artigo único para o documento {document.Number}?",
                    async () => await RegisterSaleAsync(history));
            };
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
        DialogTitleIcons.ApplyCreate(EditorTitleIcon);
        ResetEditorShell();
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

    private Border BuildDraftComposer(IReadOnlyList<LookupOption> articles, IReadOnlyList<StockLocationOption> locations)
    {
        var article = new AutoCompleteBox { ItemsSource = articles, Classes = { "bo_entity_input" } };
        ListingFilters.EnableLookupSearch(article);
        article.PlaceholderText = "Selecione o artigo...";
        var quantity = new TextBox { Text = "1", Classes = { "bo_entity_input", "bo_numeric", "bo_no_touch" }, MinWidth = 72 };
        ToolTip.SetTip(quantity, FormDecimal.QuantityRule);
        var price = new TextBox { Text = "0", Classes = { "bo_entity_input", "bo_numeric" } };
        ToolTip.SetTip(price, FormDecimal.MoneyRule);
        price.PlaceholderText = "0,00";
        var serialHost = new StackPanel { Spacing = 4, Classes = { "bo_stock_serials" } };
        var location = new ComboBox
        {
            ItemsSource = locations,
            SelectedItem = locations.FirstOrDefault(item => item.IsDefault) ?? locations.FirstOrDefault()
        };
        location.Classes.Add("bo_entity_input");

        var qtyRow = new StackPanel { Classes = { "bo_stock_qty_row" } };
        var minus = new Button { Content = "−", Classes = { "bo_stock_qty_step" } };
        var plus = new Button { Content = "+", Classes = { "bo_stock_qty_step" } };
        minus.Click += (_, _) => StepQuantity(quantity, -1);
        plus.Click += (_, _) => StepQuantity(quantity, 1);
        qtyRow.Children.Add(minus);
        qtyRow.Children.Add(quantity);
        qtyRow.Children.Add(plus);

        var body = new StackPanel { Classes = { "bo_stock_card_body" } };
        body.Children.Add(Label("Artigo"));
        body.Children.Add(article);
        body.Children.Add(Label("Quantidade"));
        body.Children.Add(qtyRow);
        body.Children.Add(Label("Preço"));
        body.Children.Add(price);
        body.Children.Add(Label("Nº série"));
        body.Children.Add(serialHost);
        body.Children.Add(Label("Localização"));
        body.Children.Add(location);

        var add = new Button { Classes = { "bo_stock_add_btn" } };
        add.Content = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Children =
            {
                new TextBlock { Text = "+", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center },
                new TextBlock { Text = "Adicionar", VerticalAlignment = VerticalAlignment.Center }
            }
        };
        add.Click += (_, _) => _ = TryCommitDraftLine();
        body.Children.Add(add);
        TouchFields.Attach(body);

        var line = new MovementLineControls
        {
            Article = article,
            Quantity = quantity,
            Price = price,
            SerialHost = serialHost,
            Location = location
        };
        quantity.LostFocus += (_, _) => SyncSerialBoxes(line);
        quantity.KeyUp += (_, args) =>
        {
            if (args.Key == Key.Enter)
            {
                SyncSerialBoxes(line);
            }
        };
        SyncSerialBoxes(line);
        _lineControls.Clear();
        _lineControls.Add(line);

        var header = new StackPanel { Classes = { "bo_stock_card_header" } };
        header.Children.Add(new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
        {
            Classes = { "bo_stock_card_icon" },
            Path = "avares://LogicPOS.App/Assets/Images/Listing/botao_novo_w.svg"
        });
        header.Children.Add(new TextBlock { Text = "Adicionar linha", Classes = { "bo_stock_card_title" } });

        var card = new Border { Classes = { "bo_stock_composer" } };
        var stack = new StackPanel { Classes = { "bo_stock_card_body" } };
        stack.Children.Add(header);
        stack.Children.Add(body);
        card.Child = stack;
        return card;
    }

    private void StepQuantity(TextBox quantity, int delta)
    {
        var current = ParseSerialCount(quantity.Text);
        var next = Math.Max(1, current + delta);
        quantity.Text = next.ToString(CultureInfo.CurrentCulture);
        if (_lineControls.Count > 0)
        {
            SyncSerialBoxes(_lineControls[0]);
        }
    }

    private bool DraftHasArticleInput()
    {
        if (_lineControls.Count == 0)
        {
            return false;
        }

        var line = _lineControls[0];
        return ChosenOption(line.Article) is not null
            || string.IsNullOrWhiteSpace(line.Article.Text) == false;
    }

    /// <summary>
    /// Commits the composer draft into the movement lines list.
    /// When <paramref name="quietIfEmpty"/> is true, an empty draft is ignored (used by Guardar).
    /// </summary>
    private bool TryCommitDraftLine(bool quietIfEmpty = false)
    {
        if (_lineControls.Count == 0)
        {
            return false;
        }

        var line = _lineControls[0];
        var article = ChosenOption(line.Article);
        if (article is null)
        {
            if (quietIfEmpty && string.IsNullOrWhiteSpace(line.Article.Text))
            {
                return false;
            }

            EditorError("Selecione o artigo na lista.");
            return false;
        }

        if (ReadDecimal(line.Quantity.Text, "Quantidade inválida.", out var quantity, FormDecimalKind.Quantity) == false || quantity == 0)
        {
            EditorError("Quantidade inválida.");
            return false;
        }

        var price = 0m;
        if (string.IsNullOrWhiteSpace(line.Price.Text) == false
            && ReadDecimal(line.Price.Text, "Preço inválido.", out price, FormDecimalKind.Money) == false)
        {
            return false;
        }

        CaptureSerialValues(line);
        var expected = ParseSerialCount(line.Quantity.Text);
        var serials = line.SerialValues
            .Take(expected)
            .Select(value => value.Trim())
            .Where(value => string.IsNullOrWhiteSpace(value) == false)
            .ToList();
        if (serials.Count > 0 && serials.Count != expected)
        {
            EditorError($"Indique {expected} número(s) de série para o artigo.");
            return false;
        }

        if (serials.Count != serials.Distinct(StringComparer.OrdinalIgnoreCase).Count())
        {
            EditorError("Há números de série duplicados na linha.");
            return false;
        }

        var location = line.Location.SelectedItem as StockLocationOption;
        _committedLines.Add(new CommittedMovementLine
        {
            ArticleId = article.Id,
            ArticleLabel = article.Label,
            Quantity = quantity,
            Price = price,
            Serials = serials,
            WarehouseLocationId = location?.Id,
            LocationLabel = location?.Label ?? string.Empty
        });

        EditorNotice.Text = string.Empty;
        ResetDraftComposer();
        RefreshCommittedLinesUi();
        return true;
    }

    private void ResetDraftComposer()
    {
        if (_lineControls.Count == 0)
        {
            return;
        }

        var line = _lineControls[0];
        line.Article.SelectedItem = null;
        line.Article.Text = string.Empty;
        line.Quantity.Text = "1";
        line.Price.Text = "0";
        line.SerialValues.Clear();
        line.Location.SelectedItem = _draftLocations.FirstOrDefault(item => item.IsDefault) ?? _draftLocations.FirstOrDefault();
        SyncSerialBoxes(line);
    }

    private void RefreshCommittedLinesUi()
    {
        if (_linesHost is null)
        {
            return;
        }

        _linesHost.Children.Clear();
        if (_linesCountBadge is not null)
        {
            var count = _committedLines.Count;
            _linesCountBadge.Text = count == 1 ? "1 item" : $"{count} itens";
        }

        if (_linesEmpty is not null)
        {
            _linesEmpty.IsVisible = _committedLines.Count == 0;
        }

        foreach (var committed in _committedLines)
        {
            var title = committed.ArticleLabel;
            var detail = $"{FormDecimal.Format(committed.Quantity)} × {FormDecimal.Format(committed.Price, FormDecimalKind.Money)}";
            if (string.IsNullOrWhiteSpace(committed.LocationLabel) == false)
            {
                detail = $"{detail} · {committed.LocationLabel}";
            }

            if (committed.Serials.Count > 0)
            {
                detail = $"{detail} · SN: {string.Join(", ", committed.Serials)}";
            }

            var texts = new StackPanel { Spacing = 2 };
            texts.Children.Add(new TextBlock { Text = title, FontWeight = FontWeight.SemiBold });
            texts.Children.Add(new TextBlock { Text = detail, Classes = { "bo_entity_notice" }, TextWrapping = TextWrapping.Wrap });

            var remove = new Button { Content = "Remover", Classes = { "bo_stock_line_remove" } };
            remove.Click += (_, _) =>
            {
                _committedLines.Remove(committed);
                RefreshCommittedLinesUi();
            };

            var row = new Border { Classes = { "bo_stock_line_row" } };
            var dock = new DockPanel();
            DockPanel.SetDock(remove, Dock.Right);
            dock.Children.Add(remove);
            dock.Children.Add(texts);
            row.Child = dock;
            _linesHost.Children.Add(row);
        }
    }

    private Border BuildAttachmentDropzone(bool stretch = false)
    {
        _attachLabel = new TextBlock { Text = "Sem anexo", Classes = { "bo_stock_dropzone_title" } };
        _attachHint = new TextBlock
        {
            Text = "Arraste um ficheiro PDF para aqui ou clique para selecionar",
            Classes = { "bo_stock_dropzone_hint" }
        };
        var content = new StackPanel { Classes = { "bo_stock_dropzone_content" } };
        content.Children.Add(new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
        {
            Classes = { "bo_stock_card_icon" },
            Path = "avares://LogicPOS.App/Assets/Images/Listing/botao_importar_b.svg",
            Width = 28,
            Height = 28
        });
        content.Children.Add(_attachLabel);
        content.Children.Add(_attachHint);

        var zone = new Border
        {
            Classes = { "bo_stock_dropzone" },
            Child = content,
            VerticalAlignment = stretch ? VerticalAlignment.Stretch : VerticalAlignment.Top,
            MinHeight = stretch ? 140 : 96
        };
        zone.PointerPressed += async (_, _) => await PickAttachmentAsync();
        DragDrop.SetAllowDrop(zone, true);
        zone.AddHandler(DragDrop.DropEvent, async (_, args) =>
        {
            var file = args.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault();
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

            if (_attachHint is not null)
            {
                _attachHint.Text = "Clique para substituir o anexo";
            }
        });
        return zone;
    }

    private static Border StockCard(
        string title,
        string iconPath,
        Control body,
        Control? trailing = null,
        bool stretch = false)
    {
        Control icon = iconPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
            ? new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
            {
                Classes = { "bo_stock_card_icon" },
                Path = iconPath
            }
            : new Image
            {
                Classes = { "bo_stock_card_icon" },
                Source = new Bitmap(Avalonia.Platform.AssetLoader.Open(new Uri(iconPath)))
            };

        var titleText = new TextBlock { Text = title, Classes = { "bo_stock_card_title" } };
        Control header;
        if (trailing is null)
        {
            var row = new StackPanel { Classes = { "bo_stock_card_header" } };
            row.Children.Add(icon);
            row.Children.Add(titleText);
            header = row;
        }
        else
        {
            var row = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };
            DockPanel.SetDock(trailing, Dock.Right);
            row.Children.Add(trailing);
            var left = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                VerticalAlignment = VerticalAlignment.Center
            };
            left.Children.Add(icon);
            left.Children.Add(titleText);
            row.Children.Add(left);
            header = row;
        }

        var shell = new DockPanel();
        DockPanel.SetDock(header, Dock.Top);
        shell.Children.Add(header);
        body.VerticalAlignment = VerticalAlignment.Stretch;
        shell.Children.Add(body);

        return new Border
        {
            Classes = { "bo_stock_card" },
            Child = shell,
            VerticalAlignment = stretch ? VerticalAlignment.Stretch : VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
    }

    private static void CaptureSerialValues(MovementLineControls line)
    {
        for (var index = 0; index < line.Serials.Count; index++)
        {
            var text = line.Serials[index].Text ?? string.Empty;
            if (index < line.SerialValues.Count)
            {
                line.SerialValues[index] = text;
            }
            else
            {
                line.SerialValues.Add(text);
            }
        }
    }

    private static void SyncSerialBoxes(MovementLineControls line)
    {
        CaptureSerialValues(line);
        var count = ParseSerialCount(line.Quantity.Text);
        while (line.SerialValues.Count < count)
        {
            line.SerialValues.Add(string.Empty);
        }

        if (line.Serials.Count == count)
        {
            for (var index = 0; index < count; index++)
            {
                line.Serials[index].PlaceholderText = count == 1 ? "Nº série" : $"Nº série {index + 1}";
            }

            return;
        }

        line.SerialHost.Children.Clear();
        line.Serials.Clear();
        for (var index = 0; index < count; index++)
        {
            var box = new TextBox
            {
                Classes = { "bo_entity_input", "bo_no_touch" },
                PlaceholderText = count == 1 ? "Nº série" : $"Nº série {index + 1}",
                Text = line.SerialValues[index]
            };
            var captured = index;
            box.TextChanged += (_, _) =>
            {
                if (captured < line.SerialValues.Count)
                {
                    line.SerialValues[captured] = box.Text ?? string.Empty;
                }
            };
            box.KeyUp += (_, args) =>
            {
                if (args.Key != Key.Enter)
                {
                    return;
                }

                args.Handled = true;
                FocusNextSerialBox(line, captured);
            };
            line.Serials.Add(box);
            line.SerialHost.Children.Add(box);
        }

        TouchFields.Attach(line.SerialHost);
    }

    private static int ParseSerialCount(string? text)
    {
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var value) == false
            && decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value) == false)
        {
            return 1;
        }

        var count = (int)Math.Round(value, MidpointRounding.AwayFromZero);
        return Math.Clamp(count, 1, 100);
    }

    private static void FocusNextSerialBox(MovementLineControls line, int currentIndex)
    {
        for (var index = currentIndex + 1; index < line.Serials.Count; index++)
        {
            var next = line.Serials[index];
            next.Focus();
            next.SelectAll();
            return;
        }
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

        if (_attachHint is not null)
        {
            _attachHint.Text = "Clique para substituir o anexo";
        }
    }

    private async void OnEditorSaveClick(object? sender, RoutedEventArgs e)
    {
        var service = Service;
        if (service is null)
        {
            EditorError("Serviço de stocks indisponível.");
            return;
        }

        EditorNotice.Text = string.Empty;
        var result = ListingSaveResult.Fail("Falha ao guardar.");
        try
        {
        switch (_editor)
        {
            case EditorMode.CreateMovement:
            {
                var supplier = ChosenOption(_supplierBox);
                var date = ReadEditorDate();
                if (supplier is null || date is null)
                {
                    EditorError("Preencha fornecedor e data.");
                    return;
                }

                // Guardar: se o composer já tem artigo, assume a linha sem clicar em + Adicionar.
                if (DraftHasArticleInput() && TryCommitDraftLine() == false)
                {
                    return;
                }

                if (_committedLines.Count == 0)
                {
                    EditorError("Adicione pelo menos uma linha de artigo.");
                    return;
                }

                var lines = new List<StockMovementLineInput>();
                foreach (var line in _committedLines)
                {
                    if (line.Serials.Count == 0)
                    {
                        lines.Add(new StockMovementLineInput
                        {
                            ArticleId = line.ArticleId,
                            Quantity = line.Quantity,
                            Price = line.Price,
                            SerialNumber = null,
                            WarehouseLocationId = line.WarehouseLocationId
                        });
                        continue;
                    }

                    // Unique SN articles: one stock unit per serial (quantity 1 each).
                    foreach (var serial in line.Serials)
                    {
                        lines.Add(new StockMovementLineInput
                        {
                            ArticleId = line.ArticleId,
                            Quantity = 1,
                            Price = line.Price,
                            SerialNumber = serial,
                            WarehouseLocationId = line.WarehouseLocationId
                        });
                    }
                }

                result = await service.CreateMovementAsync(new StockMovementCreateRequest
                {
                    SupplierId = supplier.Id,
                    Date = date.Value,
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
                if (ReadDecimal(_quantityBox?.Text, "Quantidade inválida.", out var quantity, FormDecimalKind.Quantity) == false)
                {
                    return;
                }

                var price = 0m;
                if (string.IsNullOrWhiteSpace(_priceBox?.Text) == false
                    && ReadDecimal(_priceBox?.Text, "Preço inválido.", out price, FormDecimalKind.Money) == false)
                {
                    return;
                }
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
                if (ReadDecimal(_totalBox?.Text, "Valores de stock inválidos.", out var total, FormDecimalKind.Quantity) == false ||
                    ReadDecimal(_minimumBox?.Text, "Valores de stock inválidos.", out var minimum, FormDecimalKind.Quantity) == false)
                {
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
                if (SelectedLocation() is not StockLocationOption location ||
                    ReadDecimal(_quantityBox?.Text, "Localização ou quantidade inválida.", out var quantity) == false)
                {
                    if (string.IsNullOrWhiteSpace(EditorNotice.Text))
                    {
                        EditorError("Localização ou quantidade inválida.");
                    }

                    return;
                }

                result = await service.ChangeLocationAsync(warehouseArticleId, location.Id, quantity);
                break;
            }
            case EditorMode.Exchange when _editId is Guid returnedId:
            {
                if (ListingFilters.SelectedLookup(_exchangeBox) is not LookupOption exchange)
                {
                    EditorError("Selecione o artigo de troca.");
                    return;
                }

                result = await service.ExchangeUniqueArticleAsync(returnedId, exchange.Id);
                break;
            }
            case EditorMode.EditUnique when _editId is Guid uniqueId:
            {
                if (string.IsNullOrWhiteSpace(_serialBox?.Text))
                {
                    EditorError("Indique o número de série.");
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
                            EditorError("Selecione o número de série de cada artigo filho.");
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
                    ReadDecimal(_quantityBox?.Text, "Preencha artigo, localização, data e quantidade.", out var positionQty) == false ||
                    positionQty == 0)
                {
                    if (string.IsNullOrWhiteSpace(EditorNotice.Text))
                    {
                        EditorError("Preencha artigo, localização, data e quantidade.");
                    }

                    return;
                }

                var supplier = ChosenOption(_supplierBox);
                if (HasChoices(_supplierBox) && supplier is null)
                {
                    EditorError("Selecione o fornecedor.");
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
        }
        catch (Exception)
        {
            EditorError("Não foi possível guardar.");
            return;
        }

        if (result.Succeeded == false)
        {
            EditorError(result.Error ?? "Falha ao guardar.");
            return;
        }

        Notice.Text = string.Empty;
        EditorNotice.Text = string.Empty;

        Toast.Success(this, result.Message ?? "Guardado com sucesso.");
        CloseEditorShell();
        await LoadAsync(resetPage: false);
    }

    private void OnEditorCancelClick(object? sender, RoutedEventArgs e) => CloseEditorShell();

    private void ResetEditorShell(bool split = false)
    {
        EditorFrame.Classes.Set("bo_stock_editor_split", split);
        EditorHost.Children.Clear();
        _committedLines.Clear();
        _linesHost = null;
        _linesCountBadge = null;
        _linesEmpty = null;
        _attachHint = null;
    }

    private void CloseEditorShell()
    {
        EditorOverlay.IsVisible = false;
        EditorFrame.Classes.Set("bo_stock_editor_split", false);
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
        CloseEditorShell();
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
            var result = await service.GenerateBarcodeLabelsBySerialAsync(values);
            if (result is not null
                && string.IsNullOrWhiteSpace(result.Path) == false
                && result.Path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                await ShowPdfAsync(result.Path, "Etiquetas", result.PreferredPrinter, result);
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
    }

    private async Task ShowPdfAsync(
        string path,
        string title,
        string? preferredPrinter = null,
        LogicPOS.Core.BackOffice.BarcodeLabelPrintResult? barcode = null)
    {
        if (OfficeSurface.Find(this) is { } office)
        {
            await office.ShowPdfAsync(path, title, preferredPrinter: preferredPrinter, barcode: barcode);
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

    private void EditorError(string message)
    {
        EditorNotice.Text = message;
        Notice.Text = message;
    }

    private DateTime? ReadEditorDate()
    {
        if (_dateBox?.SelectedDate is DateTime date)
        {
            return date.Date;
        }

        return null;
    }

    private StockLocationOption? SelectedLocation()
        => _locationBox?.SelectedItem as StockLocationOption
            ?? _locationBox?.Items.Cast<object?>().OfType<StockLocationOption>()
                .FirstOrDefault(item => string.Equals(item.Label, _locationBox.SelectionBoxItem?.ToString(), StringComparison.CurrentCulture));

    private static LookupOption? ChosenOption(AutoCompleteBox? box)
        => LookupChoice.Choose(
            ListingFilters.SelectedLookup(box),
            box?.SelectedItem as LookupOption,
            box?.Text,
            box?.ItemsSource as System.Collections.IEnumerable);

    private bool ReadDecimal(string? text, string invalidMessage, out decimal value, FormDecimalKind kind = FormDecimalKind.Quantity)
    {
        if (FormDecimal.TryParse(text, kind, out value, out var issue))
        {
            return true;
        }

        EditorError(issue switch
        {
            FormDecimalIssue.AmbiguousThousands => FormDecimal.MoneyRule,
            FormDecimalIssue.TooManyDecimals => FormDecimal.QuantityRule,
            _ => invalidMessage
        });
        return false;
    }

    private static bool TryDecimal(string? text, out decimal value)
        => FormDecimal.TryParse(text, FormDecimalKind.Quantity, out value, out _);

    private string DocumentNumberOrReference()
    {
        var typed = _documentBox?.Text?.Trim();
        return string.IsNullOrWhiteSpace(typed) ? $"ST{DateTime.Now:yyyyMMddHHmmss}" : typed;
    }

    private static TextBox Field(string label, string value, out StackPanel host, string? hint = null)
    {
        var box = new TextBox { Text = value };
        box.Classes.Add("bo_entity_input");
        if (string.IsNullOrWhiteSpace(hint) == false)
        {
            ToolTip.SetTip(box, hint);
        }

        host = new StackPanel { Spacing = 2 };
        host.Children.Add(Label(label));
        host.Children.Add(box);
        if (string.IsNullOrWhiteSpace(hint) == false)
        {
            host.Children.Add(new TextBlock { Text = hint, TextWrapping = Avalonia.Media.TextWrapping.Wrap, Classes = { "bo_entity_notice" } });
        }

        return box;
    }

    // -- Portrait layout --

    private void OnStockLayoutUpdated(object? sender, EventArgs e) => RelayoutForViewport();

    private void RelayoutForViewport()
    {
        if (PortraitChrome.TryUpdate(this, ref _viewport, out _portrait) == false)
        {
            return;
        }

        MobileBar.IsVisible = _portrait;
        DesktopTools.IsVisible = _portrait == false;
        Grid.IsVisible = _portrait == false;
        CardList.IsVisible = _portrait;
        ActionBar.Classes.Set("bo_page_actions_compact", _portrait);
        var editorMax = EditorFrame.Classes.Contains("bo_stock_editor_split") ? 1040 : 760;
        PortraitChrome.FitToContent(EditorFrame, this, editorMax, minWidth: 420);
        if (_portrait == false)
        {
            FilterSheet.IsVisible = false;
            MoreSheet.IsVisible = false;
            RestoreStockFilters();
        }
    }

    private Border BuildStockCard(object row)
    {
        return _tab switch
        {
            StockTab.Articles => PortraitChrome.Card(
                Prop(row, "Designation"),
                Prop(row, "Code"),
                $"Stock: {Prop(row, "TotalStock")} — Min: {Prop(row, "MinimumStock")}",
                row),
            StockTab.Movements => PortraitChrome.Card(
                Prop(row, "Article"),
                $"{Prop(row, "Movement")} — {Prop(row, "Date")}",
                $"{Prop(row, "Customer")} — Qty {Prop(row, "Quantity")}",
                row),
            StockTab.History => PortraitChrome.Card(
                Prop(row, "Article"),
                Prop(row, "Status"),
                $"{Prop(row, "Warehouse")} — {Prop(row, "Location")}",
                row),
            _ => PortraitChrome.Card(
                Prop(row, "Article"),
                $"{Prop(row, "Warehouse")} — {Prop(row, "Location")}",
                $"Qty {Prop(row, "Quantity")}",
                row)
        };
    }

    private static string Prop(object row, string name)
        => row.GetType().GetProperty(name)?.GetValue(row)?.ToString() ?? string.Empty;

    private void OnSearchClick(object? sender, RoutedEventArgs e)
        => _ = LoadAsync(resetPage: true);

    private void OnMobileSearchChanged(object? sender, TextChangedEventArgs e)
    {
        SearchBox.Text = MobileSearchBox.Text;
        OnSearchTextChanged(sender, e);
    }

    private void OnMobileFilterClick(object? sender, RoutedEventArgs e)
    {
        _stockFiltersHome ??= StockFilters.Parent as Panel;
        if (StockFilters.Parent is Panel parent)
        {
            parent.Children.Remove(StockFilters);
        }

        if (FilterSheetHost.Children.Contains(StockFilters) == false)
        {
            FilterSheetHost.Children.Add(StockFilters);
        }

        FilterSheet.IsVisible = true;
    }

    private void OnMobileFilterApplyClick(object? sender, RoutedEventArgs e)
    {
        OnFilterClick(sender, e);
        FilterSheet.IsVisible = false;
        RestoreStockFilters();
    }

    private void OnMobileFilterCloseClick(object? sender, RoutedEventArgs e)
    {
        FilterSheet.IsVisible = false;
        RestoreStockFilters();
    }

    private void RestoreStockFilters()
    {
        if (StockFilters.Parent != FilterSheetHost)
        {
            return;
        }

        FilterSheetHost.Children.Remove(StockFilters);
        _stockFiltersHome?.Children.Insert(0, StockFilters);
        DockPanel.SetDock(StockFilters, Dock.Left);
    }

    private void OnMobileMoreClick(object? sender, RoutedEventArgs e)
    {
        MoreSheetHost.Children.Clear();
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Inserir", OnInsertClick));
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Editar", OnEditClick));
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Apagar", OnDeleteClick));
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Doc. origem", OnOriginClick));
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Doc. venda", OnSaleClick));
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Cod. barras", OnBarcodeClick));
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Modif. Loc.", OnChangeLocationClick));
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Trocar", OnExchangeClick));
        MoreSheetHost.Children.Add(PortraitChrome.SheetButton("Mais resultados", OnMoreClick));
        MoreSheet.IsVisible = true;
    }

    private void OnMobileMoreCloseClick(object? sender, RoutedEventArgs e)
        => MoreSheet.IsVisible = false;

    private void OnCardTapped(object? sender, TappedEventArgs e)
    {
        object? row = null;
        if (e.Source is Visual visual)
        {
            // Prefer live container DataContext/Content over Tag (Tag is set at template build time).
            row = visual.FindAncestorOfType<ListBoxItem>()?.Content
                ?? visual.FindAncestorOfType<ListBoxItem>()?.DataContext
                ?? visual.FindAncestorOfType<Border>()?.DataContext
                ?? visual.FindAncestorOfType<Border>()?.Tag;
        }

        row ??= CardList.SelectedItem;
        if (row is null)
        {
            return;
        }

        Grid.SelectedItem = row;
        OnEditClick(sender, new RoutedEventArgs());
    }
}

