using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

/// <summary>«Alterar em massa»: row picks on the listing and a dialog that writes only the ticked fields.</summary>
public partial class EntityListing
{
    private readonly HashSet<Guid> _picked = new();
    private readonly List<(string Key, CheckBox Tick, Control Input)> _bulkInputs = new();
    private List<Guid> _bulkIds = new();
    private CheckBox? _pickAll;
    private CheckBox? _bulkConfirm;
    private bool _bulkBusy;

    private static IBulkEditService? BulkEdit => AppComposition.Services?.GetService<IBulkEditService>();

    private bool BulkEditable => BulkEdit?.Supports(_title) == true;

    private IReadOnlyList<ListingRow> FilteredRows => Grid.ItemsSource as IReadOnlyList<ListingRow> ?? [];

    private Button BulkEditButton()
    {
        const string label = "Alterar em massa";
        var button = new Button
        {
            Classes = { "bo_page_action" },
            Content = ActionButtonContent(label, "avares://LogicPOS.App/Assets/Images/Documents/botao_editar_w.svg")
        };
        ToolTip.SetTip(button, "Alterar os artigos selecionados (ou todos os filtrados)");
        button.Click += async (_, _) => await OpenBulkEditAsync();
        return button;
    }

    private DataGridTemplateColumn PickColumn()
    {
        _pickAll = new CheckBox { Classes = { "bo_doc_pick" } };
        ToolTip.SetTip(_pickAll, "Selecionar todos os filtrados");
        _pickAll.Click += (_, _) =>
        {
            var rows = FilteredRows;
            if (rows.Count > 0 && rows.All(row => _picked.Contains(row.Id)))
            {
                _picked.ExceptWith(rows.Select(row => row.Id));
            }
            else
            {
                _picked.UnionWith(rows.Select(row => row.Id));
            }

            ApplyFilter();
        };

        return new DataGridTemplateColumn
        {
            Header = _pickAll,
            CanUserSort = false,
            CanUserReorder = false,
            MinWidth = 42,
            MaxWidth = 42,
            Width = new DataGridLength(42),
            CellTemplate = new FuncDataTemplate<ListingRow>((_, _) =>
            {
                var box = new CheckBox { Classes = { "bo_doc_pick" } };
                var syncing = false;
                // Rows are recycled, so the state follows the DataContext and not the template argument.
                box.DataContextChanged += (_, _) =>
                {
                    syncing = true;
                    box.IsChecked = box.DataContext is ListingRow current && _picked.Contains(current.Id);
                    syncing = false;
                };
                box.IsCheckedChanged += (_, _) =>
                {
                    if (syncing || box.DataContext is not ListingRow current)
                    {
                        return;
                    }

                    if (box.IsChecked == true)
                    {
                        _picked.Add(current.Id);
                    }
                    else
                    {
                        _picked.Remove(current.Id);
                    }

                    RefreshPickAll();
                };
                return box;
            }, false)
        };
    }

    private void RefreshPickAll()
    {
        if (_pickAll is null)
        {
            return;
        }

        var rows = FilteredRows;
        _pickAll.IsChecked = rows.Count > 0 && rows.All(row => _picked.Contains(row.Id));
    }

    private async Task OpenBulkEditAsync()
    {
        var bulk = BulkEdit;
        var service = Service;
        if (bulk is null || service is null)
        {
            return;
        }

        var loaded = _rows.Select(row => row.Id).ToHashSet();
        var picked = _picked.Where(loaded.Contains).ToList();
        _bulkIds = picked.Count > 0 ? picked : FilteredRows.Select(row => row.Id).ToList();
        if (_bulkIds.Count == 0)
        {
            Notice.Text = "Não existem artigos para alterar.";
            return;
        }

        BulkTitle.Text = $"Alterar em massa — {_title}";
        BulkNotice.Text = string.Empty;
        BulkFields.Children.Clear();
        _bulkInputs.Clear();
        _bulkConfirm = null;

        var scope = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Avalonia.Thickness(0, 0, 0, 8) };
        BulkFields.Children.Add(scope);
        if (picked.Count > 0)
        {
            scope.Text = $"Vai alterar {picked.Count} artigo(s) selecionado(s). Só os campos marcados com «Alterar» são gravados.";
        }
        else
        {
            scope.Text = $"Nenhum artigo selecionado. As alterações aplicam-se a todos os {_bulkIds.Count} artigo(s) filtrado(s). Só os campos marcados com «Alterar» são gravados.";
            _bulkConfirm = new CheckBox
            {
                Classes = { "bo_entity_input" },
                Content = $"Confirmo aplicar a todos os {_bulkIds.Count} artigo(s) filtrado(s)"
            };
            BulkFields.Children.Add(_bulkConfirm);
        }

        var fields = bulk.Fields(_title);
        foreach (var group in fields.GroupBy(field => field.Group ?? string.Empty))
        {
            var body = new StackPanel { Spacing = 4 };
            foreach (var field in group)
            {
                body.Children.Add(await BulkFieldAsync(service, field));
            }

            var icon = group.Key switch
            {
                "Impostos" => "coins",
                "Classificação" => "layers",
                _ => "gear"
            };
            BulkFields.Children.Add(ArticleSection(group.Key, icon, body));
        }

        BulkOverlay.IsVisible = true;
    }

    private async Task<Control> BulkFieldAsync(IBackOfficeListingService service, ListingField field)
    {
        var input = await CreateInputAsync(service, field);
        if (field.Kind == "bool" && input is CheckBox check)
        {
            check.Content = field.Label;
        }

        input.IsEnabled = false;
        var tick = new CheckBox { Classes = { "bo_entity_input" }, Content = "Alterar" };
        tick.IsCheckedChanged += (_, _) => input.IsEnabled = tick.IsChecked == true;
        _bulkInputs.Add((field.Key, tick, input));

        var header = new DockPanel();
        DockPanel.SetDock(tick, Dock.Right);
        header.Children.Add(tick);
        header.Children.Add(FieldCaption(field));
        var block = new StackPanel();
        block.Children.Add(header);
        block.Children.Add(input);
        if (field.Key == BulkEditService.ArticleVatKey)
        {
            block.Children.Add(new TextBlock
            {
                Classes = { "bo_new_doc_caption" },
                TextWrapping = TextWrapping.Wrap,
                Text = "Aplica a mesma taxa ao IVA de mesa e ao IVA de venda direta. Com taxa 0% o motivo de isenção é obrigatório; com taxa superior o motivo é limpo."
            });
        }

        return block;
    }

    private async void OnBulkSaveClick(object? sender, RoutedEventArgs e)
    {
        var bulk = BulkEdit;
        if (bulk is null || _bulkBusy)
        {
            return;
        }

        var changes = _bulkInputs
            .Where(item => item.Tick.IsChecked == true)
            .ToDictionary(item => item.Key, item => ReadInput(item.Input));
        if (changes.Count == 0)
        {
            BulkNotice.Text = "Marque «Alterar» em pelo menos um campo.";
            return;
        }

        if (_bulkConfirm is not null && _bulkConfirm.IsChecked != true)
        {
            BulkNotice.Text = $"Confirme que quer aplicar a todos os {_bulkIds.Count} artigo(s) filtrado(s).";
            return;
        }

        _bulkBusy = true;
        BulkSaveButton.IsEnabled = false;
        try
        {
            var result = await bulk.ApplyAsync(_title, _bulkIds, changes);
            BulkNotice.Text = result.Error ?? string.Empty;
            ShowResultToast(result.Succeeded, result.Error, result.Message ?? "Registos atualizados.");
            if (result.Succeeded == false)
            {
                return;
            }

            BulkOverlay.IsVisible = false;
            _picked.Clear();
            ClearCatalogImageCacheIfNeeded();
            await ReloadAsync();
            EntitySaved?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            _bulkBusy = false;
            BulkSaveButton.IsEnabled = true;
        }
    }

    private void OnBulkCancelClick(object? sender, RoutedEventArgs e) => BulkOverlay.IsVisible = false;
}
