using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class FiscalYearWizard : UserControl
{
    private static readonly Regex AcronymPattern = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);

    private static readonly string[] AtKeys =
    [
        "service_at_production_mode_enabled",
        "service_at_production_account_fiscal_number",
        "service_at_production_account_password",
        "service_at_send_documents_waybill",
        "service_at_send_documents",
        "service_at_waybill_agricultural_mode_enabled"
    ];

    private readonly List<CheckBox> _terminals = new();
    private readonly List<CheckBox> _series = new();
    private readonly List<(Guid Id, string Key, string Order, string Code, string Notes, Func<string> Read)> _atFields = new();
    private FiscalYearDraft? _draft;
    private int _step;
    private bool _busy;
    private bool _syncingSeries;

    public FiscalYearWizard()
    {
        InitializeComponent();
        CreateSeries.IsCheckedChanged += (_, _) => SelectAllSeries();
        Communicate.IsCheckedChanged += (_, _) => AtPanel.IsVisible = Communicate.IsChecked == true;
    }

    public event EventHandler<string?>? Finished;

    public async Task ShowAsync()
    {
        Notice.Text = "A carregar...";
        _step = 0;
        SetNavigationEnabled(false);
        var wizard = AppComposition.Services?.GetService<IFiscalYearWizard>();
        if (wizard is null)
        {
            Notice.Text = "O assistente de ano fiscal não está disponível.";
            ShowStep();
            SetNavigationEnabled(true);
            return;
        }

        try
        {
            _draft = await wizard.LoadAsync();
        }
        catch (Exception exception)
        {
            Notice.Text = exception.GetBaseException().Message;
            ShowStep();
            SetNavigationEnabled(true);
            return;
        }

        YearBox.Text = _draft.Year.ToString();
        DesignationBox.Text = _draft.Designation;
        AcronymBox.Text = _draft.Acronym;
        NotesBox.Text = string.Empty;
        CloseRow.IsVisible = _draft.ActiveYearId is not null;
        CloseCurrent.IsChecked = _draft.ActiveYearId is not null;
        CloseCurrent.Content = $"Fechar o ano fiscal activo ({_draft.ActiveYearDesignation})";
        CreateSeries.IsChecked = true;
        PerTerminal.IsChecked = _draft.Portugal;
        Communicate.IsChecked = _draft.Portugal;
        BuildSeries();
        BuildTerminals();
        await LoadAtParametersAsync();
        Notice.Text = string.Empty;
        ShowStep();
        SetNavigationEnabled(true);
    }

    private void SetNavigationEnabled(bool enabled)
    {
        BackButton.IsEnabled = enabled;
        NextButton.IsEnabled = enabled;
        FinishButton.IsEnabled = enabled;
    }

    private void BuildSeries()
    {
        SeriesList.Children.Clear();
        _series.Clear();
        foreach (var type in _draft?.DocumentTypes ?? [])
        {
            var box = new CheckBox
            {
                Classes = { "bo_entity_input" },
                Content = $"{type.Acronym} - {type.Label}",
                Tag = type.Id,
                IsChecked = true
            };
            box.IsCheckedChanged += (_, _) => OnSeriesChanged();
            _series.Add(box);
            SeriesList.Children.Add(box);
        }
    }

    private void SelectAllSeries()
    {
        if (_syncingSeries)
        {
            return;
        }

        _syncingSeries = true;
        foreach (var box in _series)
        {
            box.IsChecked = CreateSeries.IsChecked == true;
        }

        _syncingSeries = false;
    }

    private void OnSeriesChanged()
    {
        if (_syncingSeries)
        {
            return;
        }

        _syncingSeries = true;
        CreateSeries.IsChecked = _series.Count > 0 && _series.All(box => box.IsChecked == true);
        _syncingSeries = false;
    }

    private List<FiscalYearChoice> SelectedSeries()
    {
        var selected = new List<FiscalYearChoice>();
        foreach (var box in _series)
        {
            if (box.IsChecked == true && box.Tag is Guid id)
            {
                var type = _draft?.DocumentTypes.FirstOrDefault(item => item.Id == id);
                if (type is not null)
                {
                    selected.Add(type);
                }
            }
        }

        return selected;
    }

    private void BuildTerminals()
    {
        TerminalList.Children.Clear();
        _terminals.Clear();
        foreach (var terminal in _draft?.Terminals ?? [])
        {
            var box = new CheckBox
            {
                Classes = { "bo_entity_input" },
                Content = terminal.Label,
                Tag = terminal.Id,
                IsChecked = true
            };
            _terminals.Add(box);
            TerminalList.Children.Add(box);
        }
    }

    private IReadOnlyList<string> Steps()
    {
        var steps = new List<string> { "Ano", "Séries", "Terminais" };
        if (_draft?.Portugal == true)
        {
            steps.Add("AT");
        }

        return steps;
    }

    private void ShowStep()
    {
        var steps = Steps();
        if (_step >= steps.Count)
        {
            _step = steps.Count - 1;
        }

        var name = steps[_step];
        YearStep.IsVisible = name == "Ano";
        SeriesStep.IsVisible = name == "Séries";
        TerminalsStep.IsVisible = name == "Terminais";
        AtStep.IsVisible = name == "AT";
        var creating = SelectedSeries().Count > 0;
        var perTerminal = PerTerminal.IsChecked == true;
        TerminalsHint.Text = creating == false
            ? "As séries não vão ser criadas."
            : perTerminal
                ? "Escolha os terminais que ficam com série própria."
                : "As séries ficam disponíveis em todos os terminais.";
        foreach (var box in _terminals)
        {
            box.IsEnabled = creating && perTerminal;
        }

        Communicate.IsEnabled = creating;
        BackButton.IsVisible = _step > 0;
        NextButton.IsVisible = _step < steps.Count - 1;
        FinishButton.IsVisible = _step == steps.Count - 1;
        TabBar.Children.Clear();
        for (var index = 0; index < steps.Count; index++)
        {
            var tab = new Button
            {
                Classes = { "bo_entity_tab" },
                Content = steps[index]
            };
            if (index == _step)
            {
                tab.Classes.Add("bo_entity_tab_on");
            }

            var target = index;
            tab.Click += (_, _) =>
            {
                if (target <= _step)
                {
                    _step = target;
                    ShowStep();
                }
            };
            TabBar.Children.Add(tab);
        }
    }

    private List<FiscalYearChoice> SelectedTerminals()
    {
        var selected = new List<FiscalYearChoice>();
        foreach (var box in _terminals)
        {
            if (box.IsChecked == true && box.Tag is Guid id)
            {
                var terminal = _draft?.Terminals.FirstOrDefault(item => item.Id == id);
                if (terminal is not null)
                {
                    selected.Add(terminal);
                }
            }
        }

        return selected;
    }

    private string? Validate()
    {
        if (_draft is null)
        {
            return "Não foi possível preparar o ano fiscal.";
        }

        if (string.IsNullOrWhiteSpace(DesignationBox.Text))
        {
            return "Indique a designação.";
        }

        var acronym = AcronymBox.Text?.Trim() ?? string.Empty;
        if (AcronymPattern.IsMatch(acronym) == false)
        {
            return "O acrónimo só pode ter letras e números.";
        }

        if (_draft.ActiveYearId is not null && CloseCurrent.IsChecked != true)
        {
            return "Confirme o fecho do ano fiscal activo.";
        }

        if (SelectedSeries().Count > 0 && PerTerminal.IsChecked == true && SelectedTerminals().Count == 0)
        {
            return "Escolha pelo menos um terminal.";
        }

        return null;
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        if (_step > 0)
        {
            _step--;
            Notice.Text = string.Empty;
            ShowStep();
        }
    }

    private void OnNextClick(object? sender, RoutedEventArgs e)
    {
        var error = Validate();
        if (error is not null)
        {
            Notice.Text = error;
            return;
        }

        Notice.Text = string.Empty;
        _step++;
        ShowStep();
    }

    private async void OnFinishClick(object? sender, RoutedEventArgs e)
    {
        if (_busy || _draft is null)
        {
            return;
        }

        var error = Validate();
        if (error is not null)
        {
            Notice.Text = error;
            return;
        }

        var wizard = AppComposition.Services?.GetService<IFiscalYearWizard>();
        if (wizard is null)
        {
            Notice.Text = "O assistente de ano fiscal não está disponível.";
            return;
        }

        _busy = true;
        FinishButton.IsEnabled = false;
        try
        {
            var result = await wizard.CreateAsync(new FiscalYearSetupRequest
            {
                Designation = DesignationBox.Text?.Trim() ?? string.Empty,
                Year = _draft.Year,
                Acronym = AcronymBox.Text?.Trim() ?? string.Empty,
                Notes = NotesBox.Text,
                CloseCurrent = CloseCurrent.IsChecked == true,
                CreateSeries = SelectedSeries().Count > 0,
                SeriesForEachTerminal = PerTerminal.IsChecked == true,
                CommunicateWithAt = _draft.Portugal && Communicate.IsChecked == true && SelectedSeries().Count > 0,
                TerminalIds = SelectedTerminals().Select(item => item.Id).ToList(),
                DocumentTypeIds = SelectedSeries().Select(item => item.Id).ToList()
            });
            if (result.Ok == false)
            {
                Notice.Text = result.Message;
                return;
            }

            Finished?.Invoke(this, result.Message);
        }
        catch (Exception exception)
        {
            Notice.Text = exception.GetBaseException().Message;
        }
        finally
        {
            _busy = false;
            FinishButton.IsEnabled = true;
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Finished?.Invoke(this, null);

    private async Task LoadAtParametersAsync()
    {
        AtFields.Children.Clear();
        _atFields.Clear();
        AtPanel.IsVisible = Communicate.IsChecked == true;
        if (_draft?.Portugal != true)
        {
            return;
        }

        var service = AppComposition.Services?.GetService<IBackOfficeListingService>();
        if (service is null)
        {
            return;
        }

        ListingSnapshot snapshot;
        try
        {
            snapshot = await service.QueryAsync("Parâmetros de Sistema");
        }
        catch (Exception exception)
        {
            Notice.Text = exception.GetBaseException().Message;
            return;
        }

        var rows = snapshot.Rows
            .Select(row => (Row: row, Key: MatchAtKey(row)))
            .Where(item => item.Key is not null)
            .GroupBy(item => item.Key)
            .Select(group => group.First())
            .OrderBy(item => Array.IndexOf(AtKeys, item.Key))
            .ToList();
        foreach (var item in rows)
        {
            AddAtField(item.Row, item.Key!);
        }

        TouchFields.Attach(AtFields);
    }

    private static string? MatchAtKey(ListingRow row)
    {
        var text = (row["Token"] + " " + row["ResourceString"]).ToLowerInvariant();
        foreach (var key in AtKeys)
        {
            if (text.Contains(key, StringComparison.Ordinal))
            {
                return key;
            }
        }

        return null;
    }

    private void AddAtField(ListingRow row, string key)
    {
        var translated = row["ResourceStringValue"];
        var label = string.IsNullOrWhiteSpace(translated) || translated.StartsWith("prefparam_", StringComparison.OrdinalIgnoreCase)
            ? PreferenceLabels.Get(row["ResourceString"], row["Token"])
            : translated;
        AtFields.Children.Add(new TextBlock
        {
            Classes = { "bo_new_doc_caption" },
            Text = PreferenceLabels.Title(label),
            TextWrapping = TextWrapping.Wrap
        });
        var input = CreateAtInput(row, key);
        AtFields.Children.Add(input);
        _atFields.Add((row.Id, key, row["Order"], row["Code"], row["Notes"], () => ReadAtInput(input)));
    }

    private static Control CreateAtInput(ListingRow row, string key)
    {
        var value = row["Value"];
        var kind = row["InputType"];
        var password = key.Contains("password", StringComparison.OrdinalIgnoreCase) || kind.Contains("Password", StringComparison.OrdinalIgnoreCase);
        var check = password == false
            && (kind.Contains("Check", StringComparison.OrdinalIgnoreCase)
                || key.Contains("enabled", StringComparison.OrdinalIgnoreCase)
                || key.Contains("send_documents", StringComparison.OrdinalIgnoreCase));
        if (check)
        {
            return new CheckBox
            {
                Classes = { "bo_entity_input" },
                IsChecked = value is "1" or "true" or "True" or "Sim"
            };
        }

        var box = new TextBox { Classes = { "bo_entity_input" }, Text = value };
        if (password)
        {
            box.PasswordChar = '*';
        }

        return box;
    }

    private static string ReadAtInput(Control input)
    {
        if (input is CheckBox check)
        {
            return check.IsChecked == true ? "1" : "0";
        }

        return input is TextBox box ? box.Text ?? string.Empty : string.Empty;
    }

    private string? ReadAtValue(string keyPart)
    {
        foreach (var field in _atFields)
        {
            if (field.Key.Contains(keyPart, StringComparison.OrdinalIgnoreCase))
            {
                return field.Read();
            }
        }

        return null;
    }

    private async void OnTestAtClick(object? sender, RoutedEventArgs e)
    {
        if (_busy || Communicate.IsChecked != true)
        {
            return;
        }

        var fiscalNumber = ReadAtValue("fiscal_number");
        var password = ReadAtValue("password");
        if ((fiscalNumber is not null && string.IsNullOrWhiteSpace(fiscalNumber))
            || (password is not null && string.IsNullOrWhiteSpace(password)))
        {
            Notice.Text = "Indique o NIF e a palavra-passe da conta da AT.";
            return;
        }

        var service = AppComposition.Services?.GetService<IBackOfficeListingService>();
        var wizard = AppComposition.Services?.GetService<IFiscalYearWizard>();
        if (service is null || wizard is null)
        {
            Notice.Text = "O teste à AT não está disponível.";
            return;
        }

        _busy = true;
        TestAtButton.IsEnabled = false;
        Notice.Text = "A testar a comunicação com a AT...";
        try
        {
            foreach (var field in _atFields)
            {
                var saved = await service.SaveAsync("Parâmetros de Sistema", field.Id, new Dictionary<string, string>
                {
                    ["Value"] = field.Read(),
                    ["Notes"] = field.Notes,
                    ["Order"] = field.Order,
                    ["Code"] = field.Code
                });
                if (saved.Succeeded == false)
                {
                    Notice.Text = saved.Error ?? "Não foi possível gravar os parâmetros.";
                    return;
                }
            }

            var result = await wizard.TestAtAsync();
            Notice.Text = result.Message;
        }
        catch (Exception exception)
        {
            Notice.Text = exception.GetBaseException().Message;
        }
        finally
        {
            _busy = false;
            TestAtButton.IsEnabled = true;
        }
    }
}
