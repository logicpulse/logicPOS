using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.App.Branding;
using LogicPOS.Core;
using LogicPOS.Core.Licensing;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class RegistrationWindow : Window
{
    private readonly ILicenseModule _license;
    private readonly RegistrationForm _form;

    public RegistrationWindow()
        : this(AppComposition.Services?.GetService<ILicenseModule>() ?? new NullLicenseModule())
    {
    }

    public RegistrationWindow(ILicenseModule license)
    {
        _license = license;
        _form = new RegistrationForm
        {
            HardwareId = license.HardwareId,
            Notice = string.Empty
        };
        InitializeComponent();
        DataContext = _form;
        TouchFields.Attach(this);
        Opened += (_, _) => CoverOwner();
        Opened += OnOpened;
    }

    private void CoverOwner()
    {
        if (Owner is not Window owner)
        {
            return;
        }

        Position = owner.PointToScreen(new Point(0, 0));
        Width = owner.Bounds.Width;
        Height = owner.Bounds.Height;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        try
        {
            RegisterLogo.Source = AppBranding.LoadSimpleLogo();
        }
        catch
        {
            // Keep XAML default logo.
        }

        try
        {
            var countries = await _license.GetCountriesAsync();
            _form.SetCountries(countries.Select(country => (country.Id, country.Name)));
        }
        catch
        {
            // Keep fallback Portugal/Angola/Moçambique already loaded in the form.
        }
    }

    private void OnContactTabClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string tag } && int.TryParse(tag, out var tab))
        {
            _form.ContactTab = tab;
        }
        else if (sender is Button { Tag: int tabInt })
        {
            _form.ContactTab = tabInt;
        }
    }

    private void OnCultureClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string cultureCode } || string.IsNullOrWhiteSpace(cultureCode))
        {
            return;
        }

        try
        {
            _form.ApplyCulture(CultureInfo.GetCultureInfo(cultureCode));
            Title = _form.WindowTitle;
        }
        catch (CultureNotFoundException)
        {
            // Ignore unknown cultures.
        }
    }

    private void OnExitClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
        if (Owner is Window owner)
        {
            owner.Close();
        }
    }

    private void OnSkipClick(object? sender, RoutedEventArgs e) => Close(false);

    private async void OnRegisterClick(object? sender, RoutedEventArgs e)
    {
        if (_form.Busy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_form.Name))
        {
            _form.Notice = _form.ValidationName;
            return;
        }

        if (string.IsNullOrWhiteSpace(_form.Company))
        {
            _form.Notice = _form.ValidationCompany;
            return;
        }

        if (string.IsNullOrWhiteSpace(_form.Address))
        {
            _form.Notice = _form.ValidationAddress;
            return;
        }

        if (string.IsNullOrWhiteSpace(_form.Email) || IsValidEmail(_form.Email) == false)
        {
            _form.Notice = _form.ValidationEmail;
            return;
        }

        if (string.IsNullOrWhiteSpace(_form.Phone))
        {
            _form.Notice = _form.ValidationPhone;
            return;
        }

        if (string.IsNullOrWhiteSpace(_form.FiscalNumber))
        {
            _form.Notice = _form.ValidationFiscal;
            return;
        }

        _form.Busy = true;
        _form.Notice = _form.RegisteringNotice;
        try
        {
            var result = await _license.RegisterAsync(new LicenseRegistrationRequest(
                _form.Name.Trim(),
                _form.Company.Trim(),
                _form.FiscalNumber.Trim(),
                _form.Address.Trim(),
                _form.Email.Trim(),
                _form.Phone.Trim(),
                _form.SoftwareKey.Trim(),
                _form.CountryId));

            if (result.Succeeded)
            {
                Close(true);
                return;
            }

            _form.Notice = result.Error ?? _form.RegisterFailed;
        }
        catch (Exception exception)
        {
            _form.Notice = exception.Message;
        }
        finally
        {
            _form.Busy = false;
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}