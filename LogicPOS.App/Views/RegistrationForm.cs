using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LogicPOS.App.Views;

public sealed class RegistrationCountryOption
{
    public RegistrationCountryOption(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }

    public string Name { get; }

    public override string ToString() => Name;
}

public sealed class RegistrationForm : INotifyPropertyChanged
{
    private string _hardwareId = string.Empty;
    private string _name = string.Empty;
    private string _company = string.Empty;
    private string _fiscalNumber = string.Empty;
    private string _address = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private string _softwareKey = string.Empty;
    private string _notice = string.Empty;
    private bool _busy;
    private int _contactTab;
    private RegistrationCountryOption? _selectedCountry;

    private string _windowTitle = string.Empty;
    private string _titleText = string.Empty;
    private string _welcomeTitle = string.Empty;
    private string _welcomeText = string.Empty;
    private string _sectionTitle = string.Empty;
    private string _labelName = string.Empty;
    private string _labelCompany = string.Empty;
    private string _labelAddress = string.Empty;
    private string _labelCountry = string.Empty;
    private string _labelEmail = string.Empty;
    private string _labelPhone = string.Empty;
    private string _labelHardwareId = string.Empty;
    private string _labelFiscalNumber = string.Empty;
    private string _labelSoftwareKey = string.Empty;
    private string _contactHeader = string.Empty;
    private string _labelTechnicalAssistance = string.Empty;
    private string _labelContactEmail = string.Empty;
    private string _buttonExit = string.Empty;
    private string _buttonContinue = string.Empty;
    private string _buttonRegister = string.Empty;

    private bool _countriesFromApi;

    public RegistrationForm()
    {
        Countries = [];
        ApplyCulture(CultureInfo.CurrentUICulture);
        EnsureFallbackCountries();
    }

    public ObservableCollection<RegistrationCountryOption> Countries { get; }

    public RegistrationCountryOption? SelectedCountry
    {
        get => _selectedCountry;
        set => Set(ref _selectedCountry, value);
    }

    public int CountryId => SelectedCountry?.Id ?? 168;

    public void SetCountries(IEnumerable<(int Id, string Name)> countries, int preferredId = 168)
    {
        var options = countries
            .Where(country => country.Id > 0 && string.IsNullOrWhiteSpace(country.Name) == false)
            .GroupBy(country => country.Id)
            .Select(group => new RegistrationCountryOption(group.Key, group.First().Name.Trim()))
            .OrderBy(country => country.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        if (options.Count == 0)
        {
            EnsureFallbackCountries(preferredId);
            return;
        }

        var selectedId = SelectedCountry?.Id ?? preferredId;
        Countries.Clear();
        foreach (var option in options)
        {
            Countries.Add(option);
        }

        _countriesFromApi = true;
        SelectedCountry = Countries.FirstOrDefault(country => country.Id == selectedId)
            ?? Countries.FirstOrDefault(country => country.Id == preferredId)
            ?? Countries[0];
    }

    public string HardwareId
    {
        get => _hardwareId;
        set => Set(ref _hardwareId, value);
    }

    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    public string Company
    {
        get => _company;
        set => Set(ref _company, value);
    }

    public string FiscalNumber
    {
        get => _fiscalNumber;
        set => Set(ref _fiscalNumber, value);
    }

    public string Address
    {
        get => _address;
        set => Set(ref _address, value);
    }

    public string Email
    {
        get => _email;
        set => Set(ref _email, value);
    }

    public string Phone
    {
        get => _phone;
        set => Set(ref _phone, value);
    }

    public string SoftwareKey
    {
        get => _softwareKey;
        set => Set(ref _softwareKey, value);
    }

    public string Notice
    {
        get => _notice;
        set => Set(ref _notice, value);
    }

    public bool Busy
    {
        get => _busy;
        set => Set(ref _busy, value);
    }

    public int ContactTab
    {
        get => _contactTab;
        set
        {
            if (Set(ref _contactTab, value))
            {
                OnPropertyChanged(nameof(IsContactPortugal));
                OnPropertyChanged(nameof(IsContactAngola));
                OnPropertyChanged(nameof(IsContactMozambique));
                OnPropertyChanged(nameof(ContactPhone));
                OnPropertyChanged(nameof(ContactEmail));
            }
        }
    }

    public bool IsContactPortugal => ContactTab == 0;

    public bool IsContactAngola => ContactTab == 1;

    public bool IsContactMozambique => ContactTab == 2;

    public string ContactPhone => ContactTab switch
    {
        1 => "+244 999 112 233",
        2 => "+258 84 123 123 5",
        _ => "+351 233 098 692"
    };

    public string ContactEmail => ContactTab switch
    {
        1 => "angola@logicpulse.com",
        2 => "mocambique@logicpulse.com",
        _ => "portugal@logicpulse.com"
    };

    public string WindowTitle
    {
        get => _windowTitle;
        private set => Set(ref _windowTitle, value);
    }

    public string TitleText
    {
        get => _titleText;
        private set => Set(ref _titleText, value);
    }

    public string WelcomeTitle
    {
        get => _welcomeTitle;
        private set => Set(ref _welcomeTitle, value);
    }

    public string WelcomeText
    {
        get => _welcomeText;
        private set => Set(ref _welcomeText, value);
    }

    public string SectionTitle
    {
        get => _sectionTitle;
        private set => Set(ref _sectionTitle, value);
    }

    public string LabelName
    {
        get => _labelName;
        private set => Set(ref _labelName, value);
    }

    public string LabelCompany
    {
        get => _labelCompany;
        private set => Set(ref _labelCompany, value);
    }

    public string LabelAddress
    {
        get => _labelAddress;
        private set => Set(ref _labelAddress, value);
    }

    public string LabelCountry
    {
        get => _labelCountry;
        private set => Set(ref _labelCountry, value);
    }

    public string LabelEmail
    {
        get => _labelEmail;
        private set => Set(ref _labelEmail, value);
    }

    public string LabelPhone
    {
        get => _labelPhone;
        private set => Set(ref _labelPhone, value);
    }

    public string LabelHardwareId
    {
        get => _labelHardwareId;
        private set => Set(ref _labelHardwareId, value);
    }

    public string LabelFiscalNumber
    {
        get => _labelFiscalNumber;
        private set => Set(ref _labelFiscalNumber, value);
    }

    public string LabelSoftwareKey
    {
        get => _labelSoftwareKey;
        private set => Set(ref _labelSoftwareKey, value);
    }

    public string ContactHeader
    {
        get => _contactHeader;
        private set => Set(ref _contactHeader, value);
    }

    public string LabelTechnicalAssistance
    {
        get => _labelTechnicalAssistance;
        private set => Set(ref _labelTechnicalAssistance, value);
    }

    public string LabelContactEmail
    {
        get => _labelContactEmail;
        private set => Set(ref _labelContactEmail, value);
    }

    public string ButtonExit
    {
        get => _buttonExit;
        private set => Set(ref _buttonExit, value);
    }

    public string ButtonContinue
    {
        get => _buttonContinue;
        private set => Set(ref _buttonContinue, value);
    }

    public string ButtonRegister
    {
        get => _buttonRegister;
        private set => Set(ref _buttonRegister, value);
    }

    public void ApplyCulture(CultureInfo culture)
    {
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        WindowTitle = PreferenceLabels.Text("window_title_license", "License registration");
        TitleText = PreferenceLabels.Text("window_title_license", "Licensing mechanism logicpos");
        WelcomeTitle = PreferenceLabels.Text("window_license_label_welcome", "Welcome") + "!";
        WelcomeText = PreferenceLabels.Text(
            "window_license_label_info",
            "To use logicpos you need to register a licence. Without a valid licence, printing and document creation stay inactive.");
        SectionTitle = PreferenceLabels.Text("window_license_label_internet_registration", "Internet registration").ToUpperInvariant();
        LabelName = Label(PreferenceLabels.Text("global_name", "Name"));
        LabelCompany = Label(PreferenceLabels.Text("global_company", "Company"));
        LabelAddress = Label(PreferenceLabels.Text("global_address", "Address"));
        LabelCountry = Label(PreferenceLabels.Text("global_country", "Country"));
        LabelEmail = Label(PreferenceLabels.Text("global_email", "Email"));
        LabelPhone = Label(PreferenceLabels.Text("global_phone", "Phone"));
        LabelHardwareId = Label(PreferenceLabels.Text("global_hardware_id", "Hardware ID"));
        LabelFiscalNumber = Label(PreferenceLabels.Text("global_fiscal_number_acronym", "NIF"));
        LabelSoftwareKey = Label(PreferenceLabels.Text("global_software_key", "Software key"));
        LabelTechnicalAssistance = culture.TwoLetterISOLanguageName switch
        {
            "pt" => "ASSISTÊNCIA TÉCNICA",
            "fr" => "ASSISTANCE TECHNIQUE",
            "es" => "ASISTENCIA TÉCNICA",
            _ => "TECHNICAL ASSISTANCE"
        };
        LabelContactEmail = PreferenceLabels.Text("global_email", "Email").ToUpperInvariant();
        ButtonExit = PreferenceLabels.Text("global_quit", "Exit");
        ButtonContinue = PreferenceLabels.Text("pos_button_label_licence_continue", "Continue");
        ButtonRegister = PreferenceLabels.Text("pos_button_label_licence_register", "Register");

        if (_countriesFromApi == false)
        {
            EnsureFallbackCountries(SelectedCountry?.Id ?? 168);
        }

        ContactHeader = culture.TwoLetterISOLanguageName switch
        {
            "pt" => "Dúvidas no registo?",
            "fr" => "Des questions sur l'enregistrement ?",
            "es" => "¿Dudas sobre el registro?",
            _ => "Questions about registration?"
        };
        TitleText = culture.TwoLetterISOLanguageName switch
        {
            "pt" => "Mecanismo de licenciamento logicpos",
            "fr" => "Mécanisme de licence logicpos",
            "es" => "Mecanismo de licencia logicpos",
            _ => "Licensing mechanism logicpos"
        };
        WindowTitle = culture.TwoLetterISOLanguageName switch
        {
            "pt" => "Registo de licença",
            "fr" => "Enregistrement de licence",
            "es" => "Registro de licencia",
            _ => "License registration"
        };
    }

    public string ValidationName => cultureMsg("Indica o nome.", "Enter the name.", "Indiquez le nom.", "Indica el nombre.");
    public string ValidationCompany => cultureMsg("Indica a empresa.", "Enter the company.", "Indiquez l'entreprise.", "Indica la empresa.");
    public string ValidationAddress => cultureMsg("Indica a morada.", "Enter the address.", "Indiquez l'adresse.", "Indica la dirección.");
    public string ValidationEmail => cultureMsg("Indica um email válido.", "Enter a valid email.", "Indiquez un email valide.", "Indica un email válido.");
    public string ValidationPhone => cultureMsg("Indica o telefone.", "Enter the phone number.", "Indiquez le téléphone.", "Indica el teléfono.");
    public string ValidationFiscal => cultureMsg("Indica o NIF.", "Enter the tax number.", "Indiquez le NIF.", "Indica el NIF.");
    public string RegisteringNotice => cultureMsg(
        "A contactar o servidor de licenças…",
        "Contacting the licence server…",
        "Contact du serveur de licences…",
        "Contactando el servidor de licencias…");
    public string RegisterFailed => cultureMsg(
        "Não foi possível gravar a licença.",
        "Could not save the licence.",
        "Impossible d'enregistrer la licence.",
        "No se pudo guardar la licencia.");

    private static string Label(string text) => text.EndsWith(':') ? text : text + ":";

    private void EnsureFallbackCountries(int preferredId = 168)
    {
        var selectedId = SelectedCountry?.Id ?? preferredId;
        Countries.Clear();
        Countries.Add(new RegistrationCountryOption(168, "Portugal"));
        Countries.Add(new RegistrationCountryOption(6, "Angola"));
        Countries.Add(new RegistrationCountryOption(142, "Moçambique"));
        SelectedCountry = Countries.FirstOrDefault(country => country.Id == selectedId)
            ?? Countries.FirstOrDefault(country => country.Id == preferredId)
            ?? Countries[0];
    }

    private static string cultureMsg(string pt, string en, string fr, string es)
        => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "pt" => pt,
            "fr" => fr,
            "es" => es,
            _ => en
        };

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(name);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}