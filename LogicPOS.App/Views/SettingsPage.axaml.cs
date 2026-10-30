using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class SettingsPage : UserControl
{
    private static readonly string[] CategoryOrder =
    [
        "Empresa", "Geral", "SAFT", "Email", "Impressão", "Backup", "Stock", "Relatórios", "Pastas", "Notificações", "Outros"
    ];

    private readonly List<(Guid Id, string Token, string Order, string Code, string Notes, Func<string> Read)> _fields = new();
    private readonly Dictionary<string, Grid> _pages = new();
    private readonly Dictionary<string, Control> _byToken = new(StringComparer.OrdinalIgnoreCase);
    private List<LookupOption> _countries = [];
    private static List<LookupOption>? _countryCache;
    private string _title = string.Empty;
    private string _tab = string.Empty;

    public SettingsPage()
    {
        InitializeComponent();
    }

    public async Task ShowAsync(string title)
    {
        _title = title;
        TitleText.Text = title;
        Notice.Text = string.Empty;
        _fields.Clear();
        _pages.Clear();
        _byToken.Clear();
        _countries.Clear();
        FieldsHost.Children.Clear();
        TabBar.Children.Clear();
        var service = AppComposition.Services?.GetService<IBackOfficeListingService>();
        if (service is null)
        {
            Notice.Text = "A aplicação ainda não está ligada.";
            return;
        }

        var snapshot = await service.QueryAsync(title);
        var company = title.Contains("Empresa", StringComparison.Ordinal);
        var rows = snapshot.Rows.Where(row => IsCompany(row) == company).ToList();
        if (rows.Count == 0)
        {
            rows = snapshot.Rows.ToList();
        }

        if (rows.Any(row => row["Token"].Equals("COMPANY_COUNTRY", StringComparison.OrdinalIgnoreCase)))
        {
            if (_countryCache is { Count: > 0 })
            {
                _countries = _countryCache;
            }
            else
            {
                _countries = (await service.LookupAsync("País")).ToList();
                if (_countries.Count == 0)
                {
                    Notice.Text = "Não foi possível carregar a lista de países.";
                }
                else
                {
                    _countryCache = _countries;
                }
            }
        }

        var groups = rows.GroupBy(row => Category(row["Token"])).ToDictionary(group => group.Key, group => group.ToList());
        var tabs = CategoryOrder.Where(groups.ContainsKey).ToList();
        foreach (var extra in groups.Keys.Where(key => tabs.Contains(key) == false))
        {
            tabs.Add(extra);
        }

        _tab = tabs.FirstOrDefault() ?? string.Empty;
        foreach (var tab in tabs)
        {
            var page = BuildPage(groups[tab]);
            page.IsHitTestVisible = tab == _tab;
            page.Opacity = tab == _tab ? 1 : 0;
            _pages[tab] = page;
            FieldsHost.Children.Add(page);
        }

        if (_byToken.TryGetValue("COMPANY_COUNTRY", out var countryInput) && countryInput.Tag is CountryPick countryPick)
        {
            ApplyCountryChoice(countryPick.Country);
        }

        BuildTabs(tabs);
        TouchFields.Attach(FieldsHost);
    }

    private Grid BuildPage(IReadOnlyList<ListingRow> rows)
    {
        // Capital Social (left) + Chave Privada AGT-FE (right) on the same row.
        var pem = rows.FirstOrDefault(row => AgtFePrivateKeyBox.IsToken(row["Token"]));
        var capital = rows.FirstOrDefault(row =>
            row["Token"].Equals("COMPANY_STOCK_CAPITAL", StringComparison.OrdinalIgnoreCase));
        var regular = rows
            .Where(row => AgtFePrivateKeyBox.IsToken(row["Token"]) == false
                && row["Token"].Equals("COMPANY_STOCK_CAPITAL", StringComparison.OrdinalIgnoreCase) == false)
            .OrderBy(row => ParseOrder(row["Order"]))
            .ThenBy(row => row["Code"], StringComparer.OrdinalIgnoreCase)
            .ToList();

        var capitalOrder = capital is null ? int.MaxValue : ParseOrder(capital["Order"]);
        var before = regular.Where(row => ParseOrder(row["Order"]) < capitalOrder).ToList();
        var after = regular.Where(row => ParseOrder(row["Order"]) >= capitalOrder).ToList();

        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
        var rowIndex = 0;
        rowIndex = AddTwoColumnBlock(grid, before, rowIndex);

        if (capital is not null || pem is not null)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            if (capital is not null)
            {
                AddField(grid, capital, rowIndex, 0, columnSpan: 1);
            }

            if (pem is not null)
            {
                AddField(grid, pem, rowIndex, capital is null ? 0 : 1, columnSpan: 1);
            }

            rowIndex++;
        }

        AddTwoColumnBlock(grid, after, rowIndex);
        return grid;
    }

    private int AddTwoColumnBlock(Grid grid, IReadOnlyList<ListingRow> rows, int startRow)
    {
        var half = (int)Math.Ceiling(rows.Count / 2d);
        for (var index = 0; index < half; index++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var rowIndex = startRow + index;
            AddField(grid, rows[index], rowIndex, 0, columnSpan: 1);
            if (index + half < rows.Count)
            {
                AddField(grid, rows[index + half], rowIndex, 1, columnSpan: 1);
            }
        }

        return startRow + half;
    }

    private static int ParseOrder(string? text)
        => int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var order)
            ? order
            : int.MaxValue;

    private void AddField(Grid grid, ListingRow row, int rowIndex, int column, int columnSpan)
    {
        var stack = new StackPanel { Margin = new Avalonia.Thickness(8, 6) };
        var translated = row["ResourceStringValue"];
        var label = string.IsNullOrWhiteSpace(translated) || translated.StartsWith("prefparam_", StringComparison.OrdinalIgnoreCase)
            ? PreferenceLabels.Get(row["ResourceString"], row["Token"])
            : translated;
        stack.Children.Add(new TextBlock { Classes = { "bo_doc_label" }, Text = PreferenceLabels.Title(label) });
        var input = CreateInput(row);
        stack.Children.Add(input);
        _byToken[row["Token"]] = input;
        Grid.SetRow(stack, rowIndex);
        Grid.SetColumn(stack, column);
        Grid.SetColumnSpan(stack, columnSpan);
        grid.Children.Add(stack);
        _fields.Add((row.Id, row["Token"], row["Order"], row["Code"], row["Notes"], () => ReadInput(input)));
    }

    private Control CreateInput(ListingRow row)
    {
        var value = row["Value"];
        var token = row["Token"];
        if (token.Equals("COMPANY_COUNTRY", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCountryField(value);
        }

        var kind = row["InputType"];
        if (kind.Contains("FilePicker", StringComparison.OrdinalIgnoreCase))
        {
            return CreateLogoField(value);
        }

        if (kind.Contains("Check", StringComparison.OrdinalIgnoreCase))
        {
            return new CheckBox
            {
                Classes = { "bo_entity_input" },
                IsChecked = value is "1" or "true" or "True" or "Sim"
            };
        }

        if (AgtFePrivateKeyBox.IsToken(token))
        {
            return AgtFePrivateKeyBox.Create(value);
        }

        var multiline = kind.Contains("Multi", StringComparison.OrdinalIgnoreCase);
        var box = new TextBox
        {
            Classes = { multiline ? "bo_entity_multiline" : "bo_entity_input" },
            Text = value,
            AcceptsReturn = multiline,
            IsReadOnly = token.Equals("COMPANY_COUNTRY_CODE2", StringComparison.OrdinalIgnoreCase)
                || token.Equals("COMPANY_COUNTRY_OID", StringComparison.OrdinalIgnoreCase)
        };
        if (kind.Contains("Password", StringComparison.OrdinalIgnoreCase))
        {
            box.PasswordChar = '*';
        }

        return box;
    }

    private Control CreateCountryField(string designation)
    {
        var current = _countries.FirstOrDefault(item =>
            string.Equals(item.Label, designation, StringComparison.CurrentCultureIgnoreCase));
        var box = new TextBox
        {
            Classes = { "bo_entity_input" },
            Text = current?.Label ?? designation,
            Watermark = "Escreva para filtrar o país"
        };
        var list = new ListBox
        {
            Classes = { "bo_entity_input" },
            MaxHeight = 220,
            IsVisible = false,
            ItemTemplate = new FuncDataTemplate<LookupOption>((option, _) =>
                new TextBlock { Text = option?.Label ?? string.Empty, Margin = new Avalonia.Thickness(4, 2) })
        };
        var pick = new CountryPick { Country = current, Box = box, List = list };
        var stack = new StackPanel { Tag = pick };
        stack.Children.Add(box);
        stack.Children.Add(list);

        void Show(bool all)
        {
            var term = TextSearch.Fold(box.Text?.Trim());
            var matches = all || term.Length == 0
                ? _countries
                : _countries.Where(item =>
                    TextSearch.Fold(item.Label).Contains(term, StringComparison.OrdinalIgnoreCase)
                    || TextSearch.Fold(item.Meta).Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
            list.ItemsSource = matches;
            list.IsVisible = matches.Count > 0;
        }

        var writing = false;
        box.GotFocus += (_, _) => Show(string.Equals(box.Text?.Trim(), pick.Country?.Label, StringComparison.CurrentCultureIgnoreCase));
        box.TextChanged += (_, _) =>
        {
            if (writing)
            {
                return;
            }

            pick.Country = _countries.FirstOrDefault(item =>
                string.Equals(item.Label, box.Text?.Trim(), StringComparison.CurrentCultureIgnoreCase));
            ApplyCountryChoice(pick.Country);
            if (box.IsFocused)
            {
                Show(false);
            }
        };
        box.LostFocus += (_, _) => Dispatcher.UIThread.Post(() =>
        {
            if (list.IsPointerOver == false && list.IsFocused == false)
            {
                list.IsVisible = false;
            }
        }, DispatcherPriority.Background);
        list.AddHandler(InputElement.PointerPressedEvent, (_, args) =>
        {
            var source = args.Source as Control;
            var row = source?.FindAncestorOfType<ListBoxItem>();
            if (row?.DataContext is not LookupOption option)
            {
                return;
            }

            writing = true;
            pick.Country = option;
            box.Text = option.Label;
            writing = false;
            ApplyCountryChoice(option);
            list.IsVisible = false;
            args.Handled = true;
        }, RoutingStrategies.Tunnel, true);
        return stack;
    }

    private sealed class CountryPick
    {
        public LookupOption? Country { get; set; }

        public required TextBox Box { get; init; }

        public required ListBox List { get; init; }
    }

    private void ApplyCountryChoice(LookupOption? country)
    {
        if (_byToken.TryGetValue("COMPANY_COUNTRY_CODE2", out var code) && code is TextBox codeBox)
        {
            codeBox.Text = country?.Meta ?? string.Empty;
        }

        if (_byToken.TryGetValue("COMPANY_COUNTRY_OID", out var oid) && oid is TextBox oidBox)
        {
            oidBox.Text = country is null ? string.Empty : country.Id.ToString();
        }
    }

    private static string ReadInput(Control input)
    {
        if (input.Tag is LogoPick logo)
        {
            return logo.Value;
        }

        if (input is CheckBox check)
        {
            return check.IsChecked == true ? "1" : "0";
        }

        if (input.Tag is CountryPick country)
        {
            if (country.Country is not null
                && string.Equals(country.Country.Label, country.Box.Text?.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                return country.Country.Id.ToString();
            }

            return country.Box.Text?.Trim() ?? string.Empty;
        }

        if (input is AutoCompleteBox search)
        {
            return ListingFilters.SelectedLookup(search)?.Id.ToString() ?? string.Empty;
        }

        if (AgtFePrivateKeyBox.TryRead(input, out var pem))
        {
            return pem;
        }

        return input is TextBox box ? box.Text ?? string.Empty : string.Empty;
    }

    private Control CreateLogoField(string value)
    {
        var pick = new LogoPick { Value = value ?? string.Empty };
        var photo = new Image { Classes = { "bo_logo_photo" } };
        var frame = new Border { Classes = { "bo_logo_frame" }, ClipToBounds = true, Child = photo };
        var remove = new Button { Classes = { "bo_image_remove" }, Content = "×", IsVisible = false };
        var wrap = new Grid { Classes = { "bo_logo_photo_wrap" } };
        wrap.Children.Add(frame);
        wrap.Children.Add(remove);
        var browse = new Button { Classes = { "bo_image_browse" }, Content = "Procurar..." };
        var hint = new TextBlock { Classes = { "bo_image_hint" }, Text = "Guardada na API" };
        var row = new StackPanel { Classes = { "bo_image_drop_row" } };
        row.Children.Add(browse);
        row.Children.Add(hint);
        var picker = new StackPanel { Classes = { "bo_logo_picker" }, Tag = pick };
        picker.Children.Add(wrap);
        picker.Children.Add(row);

        void Show(Bitmap? bitmap)
        {
            photo.Source = bitmap;
            remove.IsVisible = string.IsNullOrEmpty(pick.Value) == false;
            hint.Text = string.IsNullOrEmpty(pick.Value) ? "Sem imagem" : "Guardada na API";
        }

        remove.Click += (_, _) =>
        {
            pick.Value = string.Empty;
            Show(null);
        };
        browse.Click += async (_, _) =>
        {
            var file = await PickLogoAsync();
            if (file is not null)
            {
                await LoadLogoAsync(pick, file, Show);
            }
        };
        DragDrop.SetAllowDrop(picker, true);
        picker.AddHandler(DragDrop.DropEvent, async (_, args) =>
        {
            var file = args.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault();
            if (file is not null)
            {
                await LoadLogoAsync(pick, file, Show);
            }
        });
        Show(DecodeLogo(pick.Value));
        return picker;
    }

    private async Task<IStorageFile?> PickLogoAsync()
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is null)
        {
            return null;
        }

        var files = await top.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Logótipo",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Imagens") { Patterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp"] }]
        });
        return files.FirstOrDefault();
    }

    private async Task LoadLogoAsync(LogoPick pick, IStorageFile file, Action<Bitmap?> show)
    {
        var extension = Path.GetExtension(file.Name).TrimStart('.').ToLowerInvariant();
        if (extension is not ("jpg" or "jpeg" or "png" or "bmp"))
        {
            Notice.Text = "O logótipo tem de ser jpg, png ou bmp.";
            return;
        }

        await using var stream = await file.OpenReadAsync();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        if (memory.Length > 2 * 1024 * 1024)
        {
            Notice.Text = "A imagem não pode exceder 2 MB.";
            return;
        }

        pick.Value = Convert.ToBase64String(memory.ToArray());
        memory.Position = 0;
        show(new Bitmap(memory));
        Notice.Text = string.Empty;
    }

    private static Bitmap? DecodeLogo(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return new Bitmap(new MemoryStream(Convert.FromBase64String(value.Trim())));
        }
        catch (FormatException)
        {
            try
            {
                return File.Exists(value) ? new Bitmap(new MemoryStream(File.ReadAllBytes(value))) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        catch (Exception)
        {
            return null;
        }
    }

    private sealed class LogoPick
    {
        public string Value { get; set; } = string.Empty;
    }

    private void BuildTabs(IReadOnlyList<string> tabs)
    {
        TabBar.Children.Clear();
        foreach (var name in tabs)
        {
            var tab = name;
            var button = new Button
            {
                Classes = { "bo_entity_tab" },
                Content = tab
            };
            if (tab == _tab)
            {
                button.Classes.Add("bo_entity_tab_on");
            }

            button.Click += (_, _) =>
            {
                _tab = tab;
                foreach (var page in _pages)
                {
                    var selected = page.Key == tab;
                    page.Value.IsHitTestVisible = selected;
                    page.Value.Opacity = selected ? 1 : 0;
                }

                BuildTabs(tabs);
            };
            TabBar.Children.Add(button);
        }
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        var service = AppComposition.Services?.GetService<IBackOfficeListingService>();
        if (service is null)
        {
            return;
        }

        foreach (var field in _fields.OrderBy(item => item.Token.Equals("COMPANY_COUNTRY", StringComparison.OrdinalIgnoreCase) ? 1 : 0))
        {
            var result = await service.SaveAsync(_title, field.Id, new Dictionary<string, string>
            {
                ["Value"] = field.Read(),
                ["Notes"] = field.Notes,
                ["Order"] = field.Order,
                ["Code"] = field.Code
            });
            if (result.Succeeded == false)
            {
                Notice.Text = result.Error ?? "Não foi possível gravar.";
                Toast.Error(this, Notice.Text);
                return;
            }
        }

        Notice.Text = string.Empty;
        Toast.Success(this, "Parâmetros gravados com sucesso.");
    }

    private static bool IsCompany(ListingRow row)
    {
        var formType = row["FormType"];
        if (formType is "1")
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(formType) == false)
        {
            return false;
        }

        var token = row["Token"];
        return token.StartsWith("COMPANY", StringComparison.OrdinalIgnoreCase);
    }

    private static string Category(string token)
    {
        var key = token.ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(key))
        {
            return "Outros";
        }

        if (key.StartsWith("COMPANY") || key.StartsWith("AGT_FE") || key == "SYSTEM_CURRENCY")
        {
            return "Empresa";
        }

        if (key.Contains("SAFT") || key.StartsWith("SERVICE_AT") || key.StartsWith("AT_"))
        {
            return "SAFT";
        }

        if (key.Contains("MAIL") || key.Contains("SMTP") || key.Contains("EMAIL"))
        {
            return "Email";
        }

        if (key.Contains("BACKUP"))
        {
            return "Backup";
        }

        if (key.StartsWith("TICKET") || key.StartsWith("PRINT") || key.Contains("QRCODE"))
        {
            return "Impressão";
        }

        if (key.Contains("STOCK"))
        {
            return "Stock";
        }

        if (key.StartsWith("REPORT"))
        {
            return "Relatórios";
        }

        if (key.StartsWith("PATH"))
        {
            return "Pastas";
        }

        if (key.StartsWith("NOTIFICATION"))
        {
            return "Notificações";
        }

        if (key.StartsWith("APP") || key == "CULTURE" || key.StartsWith("USE_") || key.StartsWith("SPLIT_"))
        {
            return "Geral";
        }

        return "Outros";
    }
}
