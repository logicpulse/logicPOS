using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LogicPOS.Core.Authentication;

namespace LogicPOS.App.Views;

public sealed class LoginViewModel : INotifyPropertyChanged
{
    private const string PromptPassword = "Introduza a palavra-passe";
    private const string PromptNewPassword = "Introduza a nova palavra-passe";
    private const string PromptConfirmPassword = "Confirme a nova palavra-passe";
    private const string WrongPinText = "Palavra-passe errada!";
    private const string PasswordChangedText = "A palavra-passe foi alterada com sucesso!";
    private const string PinsDoNotMatchText = "As palavras-passe introduzidas não são iguais. Introduza novamente as palavras-passe para continuar!";
    private const string EqualPasswordText = "A palavra-passe introduzida é igual à anterior. Para continuar introduza uma nova palavra-passe!";
    private const string ChangePasswordTitle = "Alterar palavra-passe";

    private readonly ILoginService? _loginService;
    private readonly List<UserOption> _users = new();
    private readonly string? _startupError;
    private UserOption? _selectedUser;
    private string _pin = string.Empty;
    private string _statusMessage = PromptPassword;
    private string _okText = "Entrar";
    private bool _pinIsError;
    private bool _pinVisible;
    private bool _isBusy;
    private bool _alertVisible;
    private string _alertTitle = string.Empty;
    private string _alertMessage = string.Empty;
    private double _alertWidth = 600;
    private double _alertHeight = 400;
    private bool _alertIsQuestion;
    private bool _alertIsError;
    private bool _alertIsInformation;
    private bool _alertShowsCancel;
    private string _alertConfirmText = "Ok";
    private Action? _alertConfirm;
    private int _page;
    private int _pageSize = 7;
    private Guid? _terminalId;
    private Guid? _pendingTerminalId;
    private string? _pendingSessionUser;
    private bool _chooseTerminalAfterLogin;
    private string? _pendingNewPin;
    private string? _previousPin;
    private PinMode _mode = PinMode.Password;
    private string _versionText = string.Empty;

    public LoginViewModel(ILoginService? loginService, string? startupError)
    {
        _loginService = loginService;
        _startupError = startupError;
        UserSlots = new ObservableCollection<UserSlot>();
        _versionText = "Powered by LogicPulse Technologies © Vers. v1.6.0";
        if (string.IsNullOrWhiteSpace(startupError) == false)
        {
            StatusMessage = startupError;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? QuitConfirmed;

    public event EventHandler? TerminalChoiceRequested;

    public event EventHandler<string>? SessionStarted;

    public ObservableCollection<UserSlot> UserSlots { get; }

    public string VersionText
    {
        get => _versionText;
        set => SetField(ref _versionText, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public string OkText
    {
        get => _okText;
        private set => SetField(ref _okText, value);
    }

    public string Pin
    {
        get => _pin;
        set
        {
            if (SetField(ref _pin, value) == false)
            {
                return;
            }

            if (_pinIsError && value != WrongPinText)
            {
                PinIsError = false;
            }

            NotifyCommands();
        }
    }

    public bool PinIsError
    {
        get => _pinIsError;
        private set
        {
            if (SetField(ref _pinIsError, value))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PinMask)));
            }
        }
    }

    public bool PinVisible
    {
        get => _pinVisible;
        private set
        {
            if (SetField(ref _pinVisible, value))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PinMask)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PinVisibilityIcon)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PinVisibilityTip)));
            }
        }
    }

    public void TogglePinVisibility() => PinVisible = !PinVisible;

    public char PinMask => _pinIsError || _pinVisible ? '\0' : '*';

    public string PinVisibilityIcon => _pinVisible
        ? "avares://LogicPOS.App/Assets/Images/Login/botao_password_visivel.svg"
        : "avares://LogicPOS.App/Assets/Images/Login/botao_password_invisivel.svg";

    public string PinVisibilityTip => _pinVisible ? "Ocultar PIN" : "Mostrar PIN";

    public bool AlertVisible
    {
        get => _alertVisible;
        private set => SetField(ref _alertVisible, value);
    }

    public string AlertTitle
    {
        get => _alertTitle;
        private set => SetField(ref _alertTitle, value);
    }

    public string AlertMessage
    {
        get => _alertMessage;
        private set => SetField(ref _alertMessage, value);
    }

    public double AlertWidth
    {
        get => _alertWidth;
        private set => SetField(ref _alertWidth, value);
    }

    public double AlertHeight
    {
        get => _alertHeight;
        private set => SetField(ref _alertHeight, value);
    }

    public bool AlertIsQuestion
    {
        get => _alertIsQuestion;
        private set => SetField(ref _alertIsQuestion, value);
    }

    public bool AlertIsError
    {
        get => _alertIsError;
        private set => SetField(ref _alertIsError, value);
    }

    public bool AlertIsInformation
    {
        get => _alertIsInformation;
        private set => SetField(ref _alertIsInformation, value);
    }

    public bool AlertShowsCancel
    {
        get => _alertShowsCancel;
        private set
        {
            if (SetField(ref _alertShowsCancel, value))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlertIsOkOnly)));
            }
        }
    }

    public bool AlertIsOkOnly => _alertShowsCancel == false;

    public string AlertConfirmText
    {
        get => _alertConfirmText;
        private set => SetField(ref _alertConfirmText, value);
    }

    public bool CanSubmit =>
        _isBusy == false
        && _loginService is not null
        && _selectedUser is not null
        && _pinIsError == false
        && PinLooksValid(_pin);

    public bool CanResetPassword =>
        CanSubmit
        && _mode == PinMode.Password;

    public bool NeedsTerminalChoice { get; private set; }

    public IReadOnlyList<TerminalChoice> TerminalChoices { get; private set; } = [];

    public void SetPageSize(int pageSize)
    {
        _pageSize = pageSize < 1 ? 1 : pageSize;
        _page = 0;
        RebuildSlots();
    }

    public async Task LoadAsync()
    {
        if (_loginService is null)
        {
            return;
        }

        try
        {
            var lookup = await _loginService.LookupTerminalAsync();
            _terminalId = lookup.TerminalId;
            _chooseTerminalAfterLogin = lookup.NeedsSelection && lookup.TerminalId is null;
            NeedsTerminalChoice = false;
            TerminalChoices = lookup.Terminals;
            _pendingTerminalId = null;
            _users.Clear();
            _users.AddRange(await _loginService.GetUsersAsync());
            RebuildSlots();

            if (string.IsNullOrWhiteSpace(_startupError) == false)
            {
                ShowAlert("Erro", _startupError, AlertKind.Error, 600, 400, false, null);
            }
            else if (_chooseTerminalAfterLogin == false && _terminalId is null && lookup.NeedsSelection == false)
            {
                StatusMessage = "Terminal não encontrado";
            }
            else if (_users.Count == 0)
            {
                StatusMessage = "Não há utilizadores activos.";
            }
            else
            {
                SelectUser(_users[0]);
            }
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }

        NotifyCommands();
    }

    public void SelectTerminal(Guid terminalId) => _pendingTerminalId = terminalId;

    public async Task<string?> ConfirmTerminalAsync()
    {
        if (_loginService is null)
        {
            return "A aplicação não está pronta.";
        }

        if (_pendingTerminalId is not Guid terminalId)
        {
            return "Selecione um terminal.";
        }

        var choice = TerminalChoices.FirstOrDefault(item => item.Id == terminalId);
        _loginService.RememberTerminal(terminalId, choice?.Designation ?? "Terminal");
        var claimed = await _loginService.ClaimTerminalAsync(terminalId);
        if (claimed.Succeeded == false)
        {
            return claimed.Message;
        }

        _terminalId = terminalId;
        _chooseTerminalAfterLogin = false;
        NeedsTerminalChoice = false;
        var sessionUser = _pendingSessionUser;
        _pendingSessionUser = null;
        if (string.IsNullOrWhiteSpace(sessionUser) == false)
        {
            SessionStarted?.Invoke(this, sessionUser);
        }
        else
        {
            StatusMessage = PromptPassword;
            if (_users.Count > 0 && _selectedUser is null)
            {
                SelectUser(_users[0]);
            }
        }

        NotifyCommands();
        return null;
    }

    public void SelectSlot(UserSlot slot)
    {
        if (slot.User is null)
        {
            return;
        }

        SelectUser(slot.User);
    }

    public void ShowPreviousPage()
    {
        if (_page == 0)
        {
            return;
        }

        _page--;
        RebuildSlots();
    }

    public void ShowNextPage()
    {
        if ((_page + 1) * _pageSize >= _users.Count)
        {
            return;
        }

        _page++;
        RebuildSlots();
    }

    public void AppendDigit(string digit)
    {
        if (_pinIsError)
        {
            PinIsError = false;
            Pin = digit;
            return;
        }

        Pin += digit;
    }

    public void ClearPin()
    {
        PinIsError = false;
        Pin = string.Empty;
    }

    public void Backspace()
    {
        if (_pinIsError)
        {
            ClearPin();
            return;
        }

        if (_pin.Length == 0)
        {
            return;
        }

        Pin = _pin[..^1];
    }

    public async Task SubmitAsync()
    {
        if (CanSubmit == false || _loginService is null || _selectedUser is null)
        {
            return;
        }

        _isBusy = true;
        NotifyCommands();

        try
        {
            switch (_mode)
            {
                case PinMode.Password:
                    await SignInAsync();
                    break;
                case PinMode.PasswordNew:
                    AcceptNewPin();
                    break;
                case PinMode.PasswordNewConfirm:
                    await ConfirmNewPinAsync();
                    break;
            }
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
        finally
        {
            _isBusy = false;
            NotifyCommands();
        }
    }

    public async Task BeginPasswordResetAsync()
    {
        if (CanResetPassword == false || _loginService is null || _selectedUser is null)
        {
            return;
        }

        var result = await _loginService.SignInAsync(_selectedUser.Id, Pin);
        if (result.Succeeded == false)
        {
            if (result.TerminalUpdateFailed)
            {
                StatusMessage = result.Message;
                return;
            }

            ShowWrongPin();
            return;
        }

        _previousPin = Pin;
        _pendingNewPin = null;
        SetMode(PinMode.PasswordNew);
    }

    public void RequestQuit()
    {
        ShowAlert("Sair", "Sair da Aplicação?", AlertKind.Question, 400, 300, true, () =>
        {
            QuitConfirmed?.Invoke(this, EventArgs.Empty);
        });
    }

    public void ConfirmAlert()
    {
        AlertVisible = false;
        var confirm = _alertConfirm;
        _alertConfirm = null;
        confirm?.Invoke();
    }

    public void DismissAlert()
    {
        AlertVisible = false;
        _alertConfirm = null;
    }

    private void ShowAlert(
        string title,
        string message,
        AlertKind kind,
        double width,
        double height,
        bool yesNo,
        Action? onConfirm)
    {
        AlertTitle = title;
        AlertMessage = message;
        AlertWidth = width;
        AlertHeight = height;
        AlertIsQuestion = kind == AlertKind.Question;
        AlertIsError = kind == AlertKind.Error;
        AlertIsInformation = kind == AlertKind.Information;
        AlertShowsCancel = yesNo;
        AlertConfirmText = yesNo ? "Sim" : "Ok";
        _alertConfirm = onConfirm;
        AlertVisible = true;
    }

    private async Task SignInAsync()
    {
        if (_loginService is null || _selectedUser is null)
        {
            return;
        }

        var result = await _loginService.SignInAsync(_selectedUser.Id, Pin);
        if (result.Succeeded == false)
        {
            if (result.TerminalUpdateFailed)
            {
                StatusMessage = result.Message;
                return;
            }

            ShowWrongPin();
            return;
        }

        Pin = string.Empty;
        StatusMessage = "Entrada autorizada.";
        EnterApplication(result.Message);
    }

    public void ClearSession()
    {
        Pin = string.Empty;
        _pendingNewPin = null;
        SetMode(PinMode.Password);
        StatusMessage = PromptPassword;
    }

    private void AcceptNewPin()
    {
        if (Pin == "0000")
        {
            Pin = string.Empty;
            ShowAlert(ChangePasswordTitle, EqualPasswordText, AlertKind.Error, 600, 400, false, null);
            return;
        }

        _pendingNewPin = Pin;
        SetMode(PinMode.PasswordNewConfirm);
    }

    private async Task ConfirmNewPinAsync()
    {
        if (_loginService is null || _selectedUser is null)
        {
            return;
        }

        if (Pin != _pendingNewPin)
        {
            _pendingNewPin = null;
            SetMode(PinMode.PasswordNew);
            ShowAlert(ChangePasswordTitle, PinsDoNotMatchText, AlertKind.Error, 600, 400, false, null);
            return;
        }

        var result = await _loginService.ReplaceDefaultPinAsync(_selectedUser.Id, Pin, _previousPin);
        if (result.Succeeded == false)
        {
            _pendingNewPin = null;
            SetMode(result.RequiresNormalLogin ? PinMode.Password : PinMode.PasswordNew);
            ShowAlert("Erro", result.Message, AlertKind.Error, 600, 400, false, null);
            return;
        }

        _previousPin = null;

        _selectedUser.RequiresPasswordReset = false;
        _pendingNewPin = null;
        SetMode(PinMode.Password);
        var userName = _selectedUser.Name;
        ShowAlert(ChangePasswordTitle, PasswordChangedText, AlertKind.Information, 600, 400, false, () =>
        {
            EnterApplication(userName);
        });
    }

    private void SelectUser(UserOption user)
    {
        _selectedUser = user;
        _pendingNewPin = null;
        _previousPin = null;
        foreach (var slot in UserSlots)
        {
            slot.IsSelected = slot.User?.Id == user.Id;
        }

        SetMode(user.RequiresPasswordReset ? PinMode.PasswordNew : PinMode.Password);
    }

    private void SetMode(PinMode mode)
    {
        _mode = mode;
        PinIsError = false;
        Pin = string.Empty;
        OkText = mode == PinMode.Password ? "Entrar" : "Ok";
        StatusMessage = mode switch
        {
            PinMode.PasswordNew => PromptNewPassword,
            PinMode.PasswordNewConfirm => PromptConfirmPassword,
            _ => PromptPassword
        };
        NotifyCommands();
    }

    private void EnterApplication(string userName)
    {
        if (_chooseTerminalAfterLogin)
        {
            _pendingSessionUser = userName;
            NeedsTerminalChoice = true;
            TerminalChoiceRequested?.Invoke(this, EventArgs.Empty);
            return;
        }

        SessionStarted?.Invoke(this, userName);
    }

    private void ShowWrongPin()
    {
        PinIsError = true;
        Pin = WrongPinText;
    }

    private void RebuildSlots()
    {
        UserSlots.Clear();
        var pageUsers = _users.Skip(_page * _pageSize).Take(_pageSize).ToList();
        foreach (var user in pageUsers)
        {
            UserSlots.Add(new UserSlot(user)
            {
                IsSelected = _selectedUser is not null && user.Id == _selectedUser.Id
            });
        }

        while (UserSlots.Count < _pageSize)
        {
            UserSlots.Add(new UserSlot(null));
        }
    }

    private void NotifyCommands()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanSubmit)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanResetPassword)));
    }

    private static bool PinLooksValid(string pin) => pin.Length >= 4 && pin.All(char.IsDigit);

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    private enum PinMode
    {
        Password,
        PasswordNew,
        PasswordNewConfirm
    }

    private enum AlertKind
    {
        Question,
        Error,
        Information
    }
}
