using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
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

    private readonly List<(Guid Id, string Order, string Code, string Notes, Func<string> Read)> _fields = new();
    private readonly Dictionary<string, Grid> _pages = new();
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

        BuildTabs(tabs);
        TouchFields.Attach(FieldsHost);
    }

    private Grid BuildPage(IReadOnlyList<ListingRow> rows)
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
        var half = (int)Math.Ceiling(rows.Count / 2d);
        for (var index = 0; index < half; index++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            AddField(grid, rows[index], index, 0);
            if (index + half < rows.Count)
            {
                AddField(grid, rows[index + half], index, 1);
            }
        }

        return grid;
    }

    private void AddField(Grid grid, ListingRow row, int rowIndex, int column)
    {
        var stack = new StackPanel { Margin = new Avalonia.Thickness(8, 6) };
        var translated = row["ResourceStringValue"];
        var label = string.IsNullOrWhiteSpace(translated) || translated.StartsWith("prefparam_", StringComparison.OrdinalIgnoreCase)
            ? PreferenceLabels.Get(row["ResourceString"], row["Token"])
            : translated;
        stack.Children.Add(new TextBlock { Classes = { "bo_doc_label" }, Text = PreferenceLabels.Title(label) });
        var input = CreateInput(row);
        stack.Children.Add(input);
        Grid.SetRow(stack, rowIndex);
        Grid.SetColumn(stack, column);
        grid.Children.Add(stack);
        _fields.Add((row.Id, row["Order"], row["Code"], row["Notes"], () => ReadInput(input)));
    }

    private Control CreateInput(ListingRow row)
    {
        var value = row["Value"];
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

        var box = new TextBox
        {
            Classes = { kind.Contains("Multi", StringComparison.OrdinalIgnoreCase) ? "bo_entity_multiline" : "bo_entity_input" },
            Text = value,
            AcceptsReturn = kind.Contains("Multi", StringComparison.OrdinalIgnoreCase)
        };
        if (kind.Contains("Password", StringComparison.OrdinalIgnoreCase))
        {
            box.PasswordChar = '*';
        }

        return box;
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

        foreach (var field in _fields)
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

        if (key.StartsWith("COMPANY") || key == "SYSTEM_CURRENCY")
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
