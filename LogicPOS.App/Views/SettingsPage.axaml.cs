using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using LogicPOS.App.Themes;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class SettingsPage : UserControl
{
    private static readonly string[] CategoryOrder =
    [
        "Empresa", "Geral", "Aparência", "SAFT", "Email", "Impressão", "Backup", "Stock", "Relatórios", "Pastas", "Notificações", "Outros"
    ];

    private readonly List<(Guid Id, string Token, string Order, string Code, string Notes, Func<string> Read)> _fields = new();
    private readonly Dictionary<string, Control> _pages = new();
    private readonly Dictionary<string, Control> _byToken = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<(Grid Grid, int Columns, List<(Control Block, bool Wide)> Blocks)> _cardGrids = new();
    private List<LookupOption> _countries = [];
    private static List<LookupOption>? _countryCache;
    private string _title = string.Empty;
    private string _tab = string.Empty;
    private bool _portrait;

    public SettingsPage()
    {
        InitializeComponent();
        ListingNotice.BindToast(Notice, this);
        SizeChanged += (_, _) => RelayoutCards();
    }

    public async Task ShowAsync(string title)
    {
        _title = title;
        TitleText.Text = title;
        BackOfficePageIcons.Apply(TitleIcon, title);
        Notice.Text = string.Empty;
        _fields.Clear();
        _pages.Clear();
        _byToken.Clear();
        _cardGrids.Clear();
        _countries.Clear();
        _portrait = IsPortrait();
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
            var page = BuildPage(tab, groups[tab]);
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

    /// <summary>One tab as section cards from <see cref="SettingsLayout"/>: a field grid, then the Sim/Não toggles.</summary>
    private Control BuildPage(string tab, IReadOnlyList<ListingRow> rows)
    {
        // OID interno: sincronizado pelo País; não mostrar GUID na ficha.
        foreach (var hidden in rows.Where(IsHiddenCompanyField))
        {
            RegisterHiddenField(hidden);
        }

        var shown = rows
            .Where(row => IsHiddenCompanyField(row) == false)
            .OrderBy(row => ParseOrder(row["Order"]))
            .ThenBy(row => row["Code"], StringComparer.OrdinalIgnoreCase)
            .ToList();
        var byToken = shown.ToLookup(row => row["Token"], StringComparer.OrdinalIgnoreCase);
        var tokens = shown.Select(row => row["Token"]).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var root = new StackPanel { Classes = { "bo_article_sections" } };
        foreach (var (card, cardTokens) in SettingsLayout.CardsFor(tab, tokens))
        {
            var blocks = new List<(Control Block, bool Wide)>();
            var toggles = new List<(Control Block, bool Wide)>();
            foreach (var row in cardTokens.SelectMany(token => byToken[token]))
            {
                var input = AddField(row);
                if (input is ToggleSwitch toggle)
                {
                    toggles.Add((ToggleRow(LabelOf(row), toggle), false));
                    continue;
                }

                var caption = new TextBlock { Classes = { "bo_new_doc_caption" }, Text = LabelOf(row) };
                var wide = AgtFePrivateKeyBox.IsToken(row["Token"]) || row["InputType"].Contains("Multi", StringComparison.OrdinalIgnoreCase);
                blocks.Add((CompanySetupWindow.Labelled(caption, SettingsLayout.FieldIcon(row["Token"]), input), wide));
            }

            var body = new StackPanel { Classes = { "bo_settings_card_body" } };
            AddCardGrid(body, "bo_entity_columns", card.Columns, blocks);
            AddCardGrid(body, "bo_settings_toggles", Math.Min(2, card.Columns), toggles);
            root.Children.Add(EntityListing.ArticleSection(card.Title, card.Icon, body));
        }

        return root;
    }

    private void AddCardGrid(Panel body, string classes, int columns, List<(Control Block, bool Wide)> blocks)
    {
        if (blocks.Count == 0)
        {
            return;
        }

        var grid = new Grid { Classes = { classes } };
        body.Children.Add(grid);
        _cardGrids.Add((grid, columns, blocks));
        ArrangeCardGrid(grid, columns, blocks);
    }

    /// <summary>Card grid left to right in <paramref name="columns"/> (one in portrait); wide fields take a full row.</summary>
    private void ArrangeCardGrid(Grid grid, int columns, IReadOnlyList<(Control Block, bool Wide)> blocks)
    {
        var count = _portrait ? 1 : columns;
        grid.Children.Clear();
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        for (var index = 0; index < count; index++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        }

        var row = 0;
        var column = 0;
        foreach (var (block, wide) in blocks)
        {
            if (wide && column > 0)
            {
                column = 0;
                row++;
            }

            if (grid.RowDefinitions.Count <= row)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            Grid.SetRow(block, row);
            Grid.SetColumn(block, column);
            Grid.SetColumnSpan(block, wide ? count : 1);
            grid.Children.Add(block);
            column = wide ? count : column + 1;
            if (column >= count)
            {
                column = 0;
                row++;
            }
        }
    }

    private bool IsPortrait()
        => TopLevel.GetTopLevel(this) is { } top && top.ClientSize.Height > top.ClientSize.Width;

    private void RelayoutCards()
    {
        var portrait = IsPortrait();
        if (portrait == _portrait)
        {
            return;
        }

        _portrait = portrait;
        foreach (var (grid, columns, blocks) in _cardGrids)
        {
            ArrangeCardGrid(grid, columns, blocks);
        }
    }

    private static Control ToggleRow(string label, ToggleSwitch toggle)
    {
        var text = new TextBlock { Classes = { "bo_settings_toggle_label" }, Text = label };
        text.Tapped += (_, _) => toggle.IsChecked = toggle.IsChecked != true;
        var row = new Grid { Classes = { "bo_settings_toggle_row" }, ColumnDefinitions = new ColumnDefinitions("*,Auto") };
        Grid.SetColumn(toggle, 1);
        row.Children.Add(text);
        row.Children.Add(toggle);
        return row;
    }

    private static int ParseOrder(string? text)
        => int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var order)
            ? order
            : int.MaxValue;

    private static bool IsHiddenCompanyField(ListingRow row)
        => row["Token"].Equals("COMPANY_COUNTRY_OID", StringComparison.OrdinalIgnoreCase);

    private void RegisterHiddenField(ListingRow row)
    {
        var input = CreateInput(row);
        _byToken[row["Token"]] = input;
        _fields.Add((row.Id, row["Token"], row["Order"], row["Code"], row["Notes"], () => ReadInput(input)));
    }

    private Control AddField(ListingRow row)
    {
        var input = CreateInput(row);
        _byToken[row["Token"]] = input;
        _fields.Add((row.Id, row["Token"], row["Order"], row["Code"], row["Notes"], () => ReadInput(input)));
        return input;
    }

    private static string LabelOf(ListingRow row)
    {
        var translated = row["ResourceStringValue"];
        var label = string.IsNullOrWhiteSpace(translated) || translated.StartsWith("prefparam_", StringComparison.OrdinalIgnoreCase)
            ? PreferenceLabels.Get(row["ResourceString"], row["Token"])
            : translated;
        return PreferenceLabels.Title(label);
    }

    private Control CreateInput(ListingRow row)
    {
        var value = row["Value"];
        var token = row["Token"];
        if (token.Equals("COMPANY_COUNTRY", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCountryField(value);
        }

        if (token.Equals(GlassButtonTheme.PreferenceToken, StringComparison.OrdinalIgnoreCase))
        {
            return CreateGlossField(value);
        }

        if (token.Equals("APP_SCREEN_SIZE", StringComparison.OrdinalIgnoreCase))
        {
            return CreateScreenSizeField(value);
        }

        var kind = row["InputType"];
        if (kind.Contains("FilePicker", StringComparison.OrdinalIgnoreCase))
        {
            return CreateLogoField(value);
        }

        if (kind.Contains("Check", StringComparison.OrdinalIgnoreCase) || LooksBoolean(value))
        {
            return CreateBooleanField(value);
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

        if (input is ToggleSwitch toggle)
        {
            return toggle.IsChecked == true ? "True" : "False";
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

        if (input is ComboBox combo)
        {
            if (combo.SelectedItem is ComboBoxItem item && item.Tag is string tagged)
            {
                return tagged;
            }

            return combo.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        }

        return input is TextBox box ? box.Text ?? string.Empty : string.Empty;
    }

    private static bool LooksBoolean(string? value)
        => value is "True" or "False" or "true" or "false" or "TRUE" or "FALSE";

    private static bool IsTruthy(string? value)
        => value is "1" or "true" or "True" or "TRUE"
            or "Sim" or "Yes" or "Oui" or "Sí" or "Si";

    private static Control CreateBooleanField(string value)
        => new ToggleSwitch
        {
            Classes = { "bo_settings_toggle" },
            IsChecked = IsTruthy(value),
            OnContent = null,
            OffContent = null
        };

    private static Control CreateGlossField(string value)
    {
        var level = GlassButtonTheme.ParseLevel(value);
        var options = new (string Value, string Label)[]
        {
            ("0", "Desligado (0)"),
            ("25", "Suave (25)"),
            ("50", "Padrão (50)"),
            ("75", "Alto (75)"),
            ("100", "Máximo (100)"),
        };

        var box = new ComboBox
        {
            Classes = { "bo_entity_input" },
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
        };
        ComboBoxItem? selected = null;
        foreach (var (optionValue, label) in options)
        {
            var item = new ComboBoxItem { Content = label, Tag = optionValue };
            box.Items.Add(item);
            if (optionValue == level.ToString())
            {
                selected = item;
            }
        }

        if (selected is null)
        {
            selected = new ComboBoxItem { Content = $"Personalizado ({level})", Tag = level.ToString() };
            box.Items.Insert(0, selected);
        }

        box.SelectedItem = selected;
        box.SelectionChanged += (_, _) =>
        {
            if (box.SelectedItem is ComboBoxItem item && item.Tag is string raw)
            {
                GlassButtonTheme.ApplyFromPreference(raw);
            }
        };
        return box;
    }

    private static Control CreateScreenSizeField(string value)
    {
        var current = string.IsNullOrWhiteSpace(value) ? "0,0" : value.Trim();
        var options = new (string Value, string Label)[]
        {
            ("0,0", "Automático"),
            ("1024,768", "1024 × 768"),
            ("1280,800", "1280 × 800"),
            ("1366,768", "1366 × 768"),
            ("1600,900", "1600 × 900"),
            ("1920,1080", "1920 × 1080"),
        };

        var box = new ComboBox
        {
            Classes = { "bo_entity_input" },
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
        };
        ComboBoxItem? selected = null;
        foreach (var (optionValue, label) in options)
        {
            var item = new ComboBoxItem { Content = label, Tag = optionValue };
            box.Items.Add(item);
            if (string.Equals(optionValue, current, StringComparison.OrdinalIgnoreCase))
            {
                selected = item;
            }
        }

        if (selected is null)
        {
            selected = new ComboBoxItem { Content = current, Tag = current };
            box.Items.Insert(0, selected);
        }

        box.SelectedItem = selected;
        return box;
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
                Content = TabContent(tab, tab == _tab)
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

    private static Control TabContent(string tab, bool on)
    {
        var content = new StackPanel { Classes = { "bo_article_tab_content" } };
        content.Children.Add(new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
        {
            Classes = { "bo_article_tab_icon" },
            Path = $"avares://LogicPOS.App/Assets/Images/Articles/icon_{SettingsLayout.TabIcon(tab)}_{(on ? "g" : "b")}.svg"
        });
        content.Children.Add(new TextBlock { Text = tab, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });
        return content;
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
                return;
            }
        }

        if (_byToken.TryGetValue(GlassButtonTheme.PreferenceToken, out var glossInput))
        {
            GlassButtonTheme.ApplyFromPreference(ReadInput(glossInput));
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

        if (key is "APP_SCREEN_SIZE" or "APP_BUTTON_GLOSS")
        {
            return "Aparência";
        }

        if (key.StartsWith("APP") || key == "CULTURE" || key.StartsWith("USE_") || key.StartsWith("SPLIT_"))
        {
            return "Geral";
        }

        return "Outros";
    }
}
